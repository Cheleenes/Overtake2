using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    int cellId_;
    int currentHealth_;
    int maxHealth_;
    int healthToConvet_;
    static float regenInterval_ = 2f;
    float regenTime_;
    int pulse_;
    float pulseTime_;
    float pulseInterval_;
    List<Tentacle> tentacles_;
    Team capturingTeam_;
    Team myTeam_;
    public List<GameObject> targets_;
    public GameObject textPrefab_;
    public GameObject canvasPrefab_;
    public GameObject tentaclePrefab_;
    public int CurrentHealth_ { get => currentHealth_; set => currentHealth_ = value; }
    public List<GameObject> Targets_ { get => targets_; }
    public List<Tentacle> Tentacles_ { get => tentacles_; }
    public Team MyTeam_ { get => myTeam_; }

    public bool isNeutral_;
    SpriteRenderer spr_;

    public void Initialize(int cellId, int initialHealth, int maxHealth, int pulse)
    {
        gameObject.name = "Cell " + cellId;
        cellId_ = cellId;
        currentHealth_ = initialHealth;
        maxHealth_ = maxHealth;
        healthToConvet_ = maxHealth / 3;
        pulse_ = pulse;
        tentacles_ = new List<Tentacle>();
        regenTime_ = 0;
        pulseTime_ = 0;
        pulseInterval_ = 1f;
        targets_ = new List<GameObject>();
        spr_ = GetComponent<SpriteRenderer>();
        isNeutral_ = false;
        if (Team.GetTeamId(this) != -1)
        {
            //spr_.color = Team.teamColors[Team.GetTeamId(this)];
            myTeam_ = Team.GetTeam(this);
        }
        else
        {
            isNeutral_ = true;
            currentHealth_ = 0;
        }
        spr_.color = Team.teamColors[Team.GetTeamId(this)];
        if (!isNeutral_)
            CreateHealthUI();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNeutral_)
        {
            Pulse();
            RegenHealth();
        }
    }
    void CreateHealthUI()
    {
        GameObject canvas = Instantiate(canvasPrefab_, transform);
        GameObject textGO = Instantiate(textPrefab_, canvas.transform);
        textGO.GetComponent<HealthText>().Initialize(this, transform);
    }
    private void Pulse()
    {
        pulseTime_ += Time.deltaTime;
        if (pulseTime_ >= pulseInterval_)
        {
            foreach (Tentacle tentacle in tentacles_)
            {
                    tentacle.SendPulse(pulse_);
            }
            pulseTime_ = 0;
        }
        pulseInterval_ = 1.2f - (float)currentHealth_/maxHealth_;
    }

    private void PulseNow()
    {
        foreach (Tentacle tentacle in tentacles_)
        {
            tentacle.SendPulse(pulse_);
        }
    }

    private void RegenHealth()
    {
        regenTime_ += Time.deltaTime;
        if (currentHealth_ < maxHealth_ && regenTime_ >= regenInterval_)
        {
            regenTime_ = 0;
            currentHealth_ += 1;
        }
    }


    public void AttackCell(GameObject target)
    {
        if (!targets_.Contains(target) && IsTentacleReady())
        {
            Debug.Log("Cell " + cellId_ + " is now attacking " + target.name);
            SpawnTentacle(target);
            targets_.Add(target);
        }
    }
    private void SpawnTentacle(GameObject target)
    {
        GameObject tentacleGO = GameObject.Instantiate(tentaclePrefab_, transform);
        Tentacle tentacle = tentacleGO.GetComponent<Tentacle>();
        tentacles_.Add(tentacle);
        tentacle.Initialize(tentacles_.Count, target);
    }
    public void RemoveTentacle(Tentacle tentacle)
    {
        tentacles_.Remove(tentacle);
        GameObject.Destroy(tentacle.gameObject);       
    }
    public void RemoveTarget(GameObject target)
    {
        targets_.Remove(target);
    }
    public void RecievePulse(int pulseVal, Tentacle tentacle)
    {
        if (!isNeutral_)
        {
            if (AreAlly(this, tentacle.ParentCell_))
            {
                if (currentHealth_ < maxHealth_)
                    currentHealth_ += pulseVal;
                else
                {
                    PulseNow();
                    currentHealth_ = maxHealth_;
                }
            }
            else
            {
                currentHealth_ -= pulseVal;
                if (currentHealth_ == 0)
                {
                    Team.ConvertCell(tentacle.ParentCell_, this);
                    spr_.color = Team.teamColors[Team.GetTeamId(this)];
                }
            }
        }

        if (isNeutral_)
        {
            if (capturingTeam_ == null)
            {
                capturingTeam_ = Team.GetTeam(tentacle.ParentCell_);
                currentHealth_ += pulseVal;
            }else if(capturingTeam_ == tentacle.ParentCell_.MyTeam_)
            {
                currentHealth_ += pulseVal;
                if(currentHealth_ == healthToConvet_)
                {
                    //convert to normal cell
                    Team.ConvertCell(tentacle.ParentCell_, this);
                    spr_.color = Team.teamColors[Team.GetTeamId(this)];
                    isNeutral_ = false;
                    myTeam_ = tentacle.ParentCell_.MyTeam_;
                    CreateHealthUI();
                }
            }else
            {
                currentHealth_ -= pulseVal;
                if (currentHealth_ == 0)
                {
                    capturingTeam_ = tentacle.ParentCell_.MyTeam_;
                }
            }

            spr_.color = Color.Lerp(Color.gray, Team.teamColors[capturingTeam_.TeamId_], (float)currentHealth_/healthToConvet_);
        }
    }

    public static bool AreAlly(Cell cell1, Cell cell2)
    {
        foreach(Team team in MapSetup.teams_)
        {
            if(team.HasCell_(cell1) && team.HasCell_(cell2))
            {
                return true;
            }
        }

        return false;
    }

    public void RetreatAllTentacles()
    {
        foreach(Tentacle tentacle in tentacles_)
        {
            tentacle.Retreat();
        }
    }

    bool IsTentacleReady()
    {
        if (currentHealth_ < 15 && tentacles_.Count < 1)
            return true;
        if (currentHealth_ >= 15 && currentHealth_ < 50 && tentacles_.Count < 2)
            return true;
        if (currentHealth_ >= 50 && tentacles_.Count < 3)
            return true;
        return false;
    }
}
