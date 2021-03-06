using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class NetworkCell : NetworkBehaviour
{
    static List<NetworkCell> cells_ = new List<NetworkCell>();
    //int cellId_;
    NetworkVariableInt cellId_ = new NetworkVariableInt();
    //int currentHealth_;
    private NetworkVariableInt currentHealth_ = new NetworkVariableInt();
    int maxHealth_;
    int healthToConvet_;
    static float regenInterval_ = 3.5f;
    float regenTime_;
    int pulse_;
    int currentPulseFraction_;
    int pulseFraction_;
    float pulseTime_;
    float pulseInterval_;
    List<NetworkTentacle> tentacles_;
    NetworkTeamManager teamManager_;
    NetworkTeam capturingTeam_;
    NetworkTeam myTeam_;
    public List<GameObject> targets_;
    public GameObject textPrefab_;
    public GameObject canvasPrefab_;
    public GameObject tentaclePrefab_;
    CircleCollider2D cc2d_;
    public int CurrentHealth_ { get => currentHealth_.Value; set => currentHealth_.Value = value; }
    public List<GameObject> Targets_ { get => targets_; }
    public List<NetworkTentacle> Tentacles_ { get => tentacles_; }
    public NetworkTeam MyTeam_ { get => myTeam_; }
    public int CellId_ { get => cellId_.Value; }
    public NetworkTeamManager TeamManager_ { get => teamManager_; }
    public float MyRadius_ { get => cc2d_.radius; }
    //public bool isNeutral_;
    public NetworkVariableBool isNeutral_ = new NetworkVariableBool();
    SpriteRenderer spr_;
    public NetworkVariableColor myColor_ = new NetworkVariableColor();


    public void InitializeServer(int cellId, int initialHealth, int maxHealth, int pulse, NetworkTeam myTeam)
    {
        cells_.Add(this);
        myTeam_ = myTeam;
        gameObject.name = "Cell " + cellId;
        cellId_.Value = cellId;
        myTeam_.AddCell(gameObject);
        currentHealth_.Value = initialHealth;
        maxHealth_ = maxHealth;
        healthToConvet_ = 6;
        pulse_ = pulse;
        tentacles_ = new List<NetworkTentacle>();
        regenTime_ = 0;
        pulseTime_ = 0;
        pulseInterval_ = 1f;
        currentPulseFraction_ = 0;
        pulseFraction_ = 2;
        targets_ = new List<GameObject>();
        spr_ = GetComponent<SpriteRenderer>();
        teamManager_ = GameObject.FindGameObjectWithTag("TeamManager").GetComponent<NetworkTeamManager>();
        isNeutral_.Value = false;
        cc2d_ = GetComponent<CircleCollider2D>();

        if(myTeam_.TeamId_ == -1)
        {
            isNeutral_.Value = true;
            currentHealth_.Value = 0;
        }

        myColor_.Value = teamManager_.teamColors[myTeam_.TeamId_];
        if (!isNeutral_.Value)
            CreateHealthUI();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            spr_ = GetComponent<SpriteRenderer>();
            spr_.color = myColor_.Value;
            if(!isNeutral_.Value)
                CreateHealthUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNeutral_.Value && NetworkManager.Singleton.IsServer)
        {
            Pulse();
            RegenHealth();
        }
        spr_.color = myColor_.Value;
    }
    void CreateHealthUI()
    {
        GameObject canvas = Instantiate(canvasPrefab_, transform);
        GameObject textGO = Instantiate(textPrefab_, canvas.transform);
        textGO.GetComponent<NetworkHealthText>().Initialize(this, transform);
    }
    [ClientRpc]
    void CreateHealthUIClientRpc()
    {
        if(!NetworkManager.Singleton.IsServer)
            CreateHealthUI();
    }
    private void Pulse()
    {
        pulseTime_ += Time.deltaTime;
        if (pulseTime_ >= pulseInterval_)
        {
            foreach (NetworkTentacle tentacle in tentacles_)
            {
                tentacle.SendPulse(pulse_);
            }
            pulseTime_ = 0;
        }
        pulseInterval_ = 2f - ((float)currentHealth_.Value * 0.016f); //(float)currentHealth_.Value / maxHealth_;
    }

    private void PulseNow()
    {
        foreach (NetworkTentacle tentacle in tentacles_)
        {
            tentacle.SendPulse(pulse_);
        }
    }

    private void RegenHealth()
    {
        regenTime_ += Time.deltaTime;
        if (currentHealth_.Value < maxHealth_ && regenTime_ >= regenInterval_)
        {
            regenTime_ = 0;
            currentHealth_.Value += 1;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AttackCellServerRpc(int targetCellId)
    {
        AttackCell(GetCell(targetCellId).gameObject);
    }

    static NetworkCell GetCell(int cellId)
    {
        foreach(NetworkCell cell in cells_)
        {
            if (cell.CellId_ == cellId)
                return cell;
        }
        return null;
    }
    public void AttackCell(GameObject target)
    {
        if (!targets_.Contains(target) && IsTentacleReady())
        {
            Debug.Log("Cell " + CellId_ + " is now attacking " + target.name);
            SpawnTentacle(target);
            targets_.Add(target);
        }
    }
    private void SpawnTentacle(GameObject target)
    {
        GameObject tentacleGO = GameObject.Instantiate(tentaclePrefab_, transform);
        tentacleGO.transform.position = Vector2.MoveTowards(transform.position, target.transform.position, cc2d_.radius);
        NetworkTentacle tentacle = tentacleGO.GetComponent<NetworkTentacle>();
        tentacleGO.GetComponent<NetworkObject>().Spawn();
        tentacles_.Add(tentacle);
        tentacle.Initialize(tentacles_.Count, target);
    }
    public void RemoveTentacle(NetworkTentacle tentacle)
    {
        tentacles_.Remove(tentacle);
        GameObject.Destroy(tentacle.gameObject);
    }
    public void RemoveTarget(GameObject target)
    {
        targets_.Remove(target);
    }
    public void RecievePulse(int pulseVal, NetworkTentacle tentacle)
    {
        if (!isNeutral_.Value)
        {
            if (AreAlly(this, tentacle.ParentCell_, teamManager_))
            {
                if (currentHealth_.Value < maxHealth_)
                    currentHealth_.Value += pulseVal;
                else
                {
                    currentPulseFraction_++;
                    if (currentPulseFraction_ >= pulseFraction_)
                    {
                        PulseNow();
                        currentPulseFraction_ = 0;
                    }
                    currentHealth_.Value = maxHealth_;
                }
            }
            else
            {
                currentHealth_.Value -= pulseVal;
                if (currentHealth_.Value <= 0)
                {
                    teamManager_.ConvertCell(tentacle.ParentCell_, this);
                    myColor_.Value = teamManager_.teamColors[teamManager_.GetTeamId(this)];
                }
            }
        }

        if (isNeutral_.Value)
        {
            if (capturingTeam_ == null)
            {
                capturingTeam_ = teamManager_.GetTeam(tentacle.ParentCell_);
                currentHealth_.Value += pulseVal;
            }
            else if (capturingTeam_ == tentacle.ParentCell_.MyTeam_)
            {
                currentHealth_.Value += pulseVal;
                if (currentHealth_.Value == healthToConvet_)
                {
                    //convert to normal cell
                    teamManager_.ConvertCell(tentacle.ParentCell_, this);
                    myColor_.Value = teamManager_.teamColors[teamManager_.GetTeamId(this)];
                    isNeutral_.Value = false;
                    myTeam_ = tentacle.ParentCell_.MyTeam_;
                    CreateHealthUI();
                    CreateHealthUIClientRpc();
                }
            }
            else
            {
                currentHealth_.Value -= pulseVal;
                if (currentHealth_.Value <= 0)
                {
                    capturingTeam_ = tentacle.ParentCell_.MyTeam_;
                }
            }

            myColor_.Value = Color.Lerp(Color.gray, teamManager_.teamColors[capturingTeam_.TeamId_], (float)currentHealth_.Value / healthToConvet_);
        }
    }

    public static bool AreAlly(NetworkCell cell1, NetworkCell cell2, NetworkTeamManager teamManager)
    {
        foreach (NetworkTeam team in teamManager.teams_)
        {
            if (team.HasCell_(cell1) && team.HasCell_(cell2))
            {
                return true;
            }
        }

        return false;
    }

    public void RetreatAllTentacles()
    {
        foreach (NetworkTentacle tentacle in tentacles_)
        {
            tentacle.Retreat();
        }
    }

    bool IsTentacleReady()
    {
        if (currentHealth_.Value < 15 && tentacles_.Count < 1)
            return true;
        if (currentHealth_.Value >= 15 && currentHealth_.Value < 50 && tentacles_.Count < 2)
            return true;
        if (currentHealth_.Value >= 50 && tentacles_.Count < 3)
            return true;
        return false;
    }
}
