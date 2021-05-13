using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class NetworkPlayer : NetworkBehaviour
{
    public enum PlayerState
    {
        Empty,
        FromSelected,
        Cutting
    }
    int playerId_;
    public LayerMask justMe_;
    public GameObject fromCellGO_;
    public GameObject toCellGO_;
    NetworkTeamManager teamManager_;
    public PlayerState state_;
    public NetworkVariable<NetworkTeam> myTeam_ = new NetworkVariable<NetworkTeam>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public PlayerState State_ { get => state_; }

    // Start is called before the first frame update
    public void Initialize(int playerId, NetworkTeam myTeam)
    {
        playerId_ = playerId;
        state_ = PlayerState.Empty;
        myTeam_.Value = myTeam;
        Debug.Log("Server player initialized, my team id is: " + myTeam_.Value.TeamId_);
    }

    public void InitializeClient(int playreId, NetworkTeam myTeam)
    {      
        state_ = PlayerState.Empty;
        myTeam_.Value = myTeam;
        Debug.Log("Client player initialized, my team id is: " + myTeam_.Value.TeamId_);
    }
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].ClientId == gameObject.GetComponent<NetworkObject>().OwnerClientId)
        {
            teamManager_ = GameObject.FindGameObjectWithTag("TeamManager").GetComponent<NetworkTeamManager>();
            InitializeClient((int)NetworkManager.Singleton.LocalClientId, teamManager_.teams_[2]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].ClientId == gameObject.GetComponent<NetworkObject>().OwnerClientId)
        {
            //TouchControllers();
            MouseControllers();
        }
    }

    void TouchControllers()
    {
        switch (state_)
        {
            case PlayerState.Empty:
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        justMe_ = LayerMask.GetMask("Cell Layer");
                        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero, justMe_);
                        if (hitInfo)
                        {
                            if (hitInfo.collider.gameObject.tag == "Cell" && myTeam_.Value.HasCell_(hitInfo.collider.gameObject.GetComponent<NetworkCell>()))
                            {
                                state_ = PlayerState.FromSelected;
                                fromCellGO_ = hitInfo.collider.gameObject;
                            }
                        }
                        else
                        {
                            state_ = PlayerState.Cutting;
                        }
                    }
                }
                else
                {
                    fromCellGO_ = null;
                    toCellGO_ = null;
                    state_ = PlayerState.Empty;
                }
                break;

            case PlayerState.FromSelected:
                if (Input.touchCount > 0)
                {
                    Vector3 ray1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hitInfo1 = Physics2D.Raycast(ray1, Vector2.zero);
                    if (hitInfo1 && hitInfo1.collider.gameObject.tag == "Cell" && hitInfo1.collider.gameObject != fromCellGO_)
                    {
                        toCellGO_ = hitInfo1.collider.gameObject;
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                    {
                        if (toCellGO_ != null)
                        {
                            if (NetworkManager.Singleton.IsServer)
                                fromCellGO_.GetComponent<NetworkCell>().AttackCell(toCellGO_);
                            else
                                fromCellGO_.GetComponent<NetworkCell>().AttackCellServerRpc(toCellGO_.GetComponent<NetworkCell>().CellId_);
                        }
                        state_ = PlayerState.Empty;
                        fromCellGO_ = null;
                        toCellGO_ = null;
                    }
                }
                break;

            case PlayerState.Cutting:
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        justMe_ = LayerMask.GetMask("Tentacle body Layer");
                        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero, justMe_);
                        if (hitInfo)
                        {
                            if (hitInfo.collider.gameObject.tag == "BodyPart" && myTeam_.Value.HasCell_(hitInfo.collider.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<NetworkCell>()))
                            {
                                if(NetworkManager.Singleton.IsServer)
                                    hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().GetCut(hitInfo.collider.gameObject);
                                else
                                    hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().GetCutServerRpc(hitInfo.collider.gameObject.GetComponent<NetworkTentaclePart>().BodyPartId_);
                            }
                        }
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                    {
                        state_ = PlayerState.Empty;
                    }
                }
                else
                {
                    fromCellGO_ = null;
                    toCellGO_ = null;
                    state_ = PlayerState.Empty;
                }
                break;
        }
    }
    void MouseControllers()
    {
        switch (state_)
        {
            case PlayerState.Empty:
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    justMe_ = LayerMask.GetMask("Cell Layer");
                    Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero, justMe_);
                    if (hitInfo)
                    {
                        if (hitInfo.collider.gameObject.tag == "Cell" && myTeam_.Value.HasCell_(hitInfo.collider.gameObject.GetComponent<NetworkCell>()))
                        {
                            state_ = PlayerState.FromSelected;
                            fromCellGO_ = hitInfo.collider.gameObject;
                        }
                    }
                    else
                    {
                        state_ = PlayerState.Cutting;
                    }
                }
                else
                {
                    fromCellGO_ = null;
                    toCellGO_ = null;
                    state_ = PlayerState.Empty;
                }
                break;

            case PlayerState.FromSelected:
                Vector3 ray1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInfo1 = Physics2D.Raycast(ray1, Vector2.zero);
                if (hitInfo1 && hitInfo1.collider.gameObject.tag == "Cell" && hitInfo1.collider.gameObject != fromCellGO_)
                {
                    toCellGO_ = hitInfo1.collider.gameObject;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (toCellGO_ != null)
                    {
                        if (NetworkManager.Singleton.IsServer)
                            fromCellGO_.GetComponent<NetworkCell>().AttackCell(toCellGO_);
                        else
                            fromCellGO_.GetComponent<NetworkCell>().AttackCellServerRpc(toCellGO_.GetComponent<NetworkCell>().CellId_);
                    }
                    state_ = PlayerState.Empty;
                    fromCellGO_ = null;
                    toCellGO_ = null;
                }            
                break;

            case PlayerState.Cutting:
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
                    justMe_ = LayerMask.GetMask("Tentacle body Layer");
                    Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero, justMe_);
                    if (hitInfo)
                    {
                        if (hitInfo.collider.gameObject.tag == "BodyPart" && myTeam_.Value.HasCell_(hitInfo.collider.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<NetworkCell>()))
                        {
                            if (NetworkManager.Singleton.IsServer)
                                hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().GetCut(hitInfo.collider.gameObject);
                            else
                                hitInfo.collider.gameObject.GetComponentInParent<NetworkTentacle>().GetCutServerRpc(hitInfo.collider.gameObject.GetComponent<NetworkTentaclePart>().BodyPartId_);
                        }
                    }
                }
                else 
                {
                    fromCellGO_ = null;
                    toCellGO_ = null;
                    state_ = PlayerState.Empty;
                }
                break;
        }
    }

    public void SetTeam(NetworkTeam team)
    {
        myTeam_.Value = team;
    }
}
