using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
public class NetworkTentacle : NetworkBehaviour
{
    public static float travelSpeed_ = 2f;
    public static float fastTravelSpeed_ = 10f;
    INetworkTentacleState state_;
    int tentacleId_;
    List<GameObject> bodyParts_;
    public GameObject bodyPartPrefab_;
    GameObject target_;
    NetworkCell parentCell_;
    public bool gotCut_;
    public GameObject cutOrigin_;
    public int bodyPartCount_ { get => bodyParts_.Count; }
    public GameObject Target_ { get => target_; }
    public GameObject Head_ { get => bodyParts_[0]; }
    public INetworkTentacleState State { get => state_; }
    public int ParentCellCurrentHealth { get => parentCell_.CurrentHealth_; }
    public List<GameObject> BodyParts_ { get => bodyParts_; set => bodyParts_ = value; }
    public bool IsDefending_()
    {
        foreach (GameObject cellGO in parentCell_.Targets_)
        {
            NetworkCell cell = cellGO.GetComponent<NetworkCell>();
            if (cell.Targets_.Contains(parentCell_.gameObject) && target_ == cellGO)
            {
                return true;
            }
        }
        return false;
    }
    public NetworkCell ParentCell_ { get => parentCell_; }

    public void Initialize(int tentacleId, GameObject target)
    {
        gameObject.name = "Tentacle " + tentacleId;
        tentacleId_ = tentacleId;
        target_ = target;
        bodyParts_ = new List<GameObject>();
        GameObject bodyPartGO = GameObject.Instantiate(bodyPartPrefab_, transform);
        bodyPartGO.GetComponent<NetworkTentaclePart>().Initialize(bodyPartCount_, target_, true);
        bodyPartGO.GetComponent<NetworkObject>().Spawn();
        bodyParts_.Add(bodyPartGO);
        parentCell_ = GetComponentInParent<NetworkCell>();
        state_ = new NetworkToTargetState(target_, this);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            parentCell_ = GetComponentInParent<NetworkCell>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            INetworkTentacleState state = state_.ChangeState(this, target_);
            if (state != null)
            {
                if (state.GetType() != state_.GetType())
                {
                    Debug.Log(parentCell_.gameObject.name + ", Tentacle " + tentacleId_ + " has changed its state from " + state_.GetType() + " to " + state.GetType());
                    if (state is NetworkDeadState)
                    {
                        parentCell_.RemoveTentacle(this);
                    }
                }
                state_ = null;
                state_ = state;
            }
            state_.DoStateAction(this);
        }
    }

    public void SendPulse(int pulse)
    {
        if (state_ is NetworkAtTargetState)
        {
            bodyParts_[bodyPartCount_ - 1].GetComponent<NetworkTentaclePart>().RecievePulse(pulse);
        }
    }

    public void CreateBodyPart()
    {
        parentCell_.CurrentHealth_ -= 1;
        GameObject bodyPart = GameObject.Instantiate(bodyPartPrefab_, transform);
        if (bodyPartCount_ == 0)
        {
            bodyPart.GetComponent<NetworkTentaclePart>().Initialize(bodyPartCount_ + 1, target_, true);
        }
        else
        {
            bodyPart.GetComponent<NetworkTentaclePart>().Initialize(bodyPartCount_, bodyParts_[bodyPartCount_ - 1], false);
        }
        bodyPart.GetComponent<NetworkObject>().Spawn();
        bodyParts_.Add(bodyPart);
    }
    public void RemoveBodyPart(GameObject bodyPartGO)
    {
        bodyParts_.Remove(bodyPartGO);
        parentCell_.RecievePulse(1, this);
        GameObject.Destroy(bodyPartGO);

    }
    public void GetCut(GameObject cutOrigin)
    {
        gotCut_ = true;
        cutOrigin_ = cutOrigin;
    }
    [ServerRpc(RequireOwnership = false)]
    public void GetCutServerRpc(int cutOriginId)
    {
        GetCutFromClient(cutOriginId);
    }
    public void GetCutFromClient(int cutOriginId)
    {
        gotCut_ = true;
        cutOrigin_ = getBodyPartGO(cutOriginId);
    }
    public GameObject getBodyPartGO(int bodyPartId)
    {
        NetworkTentaclePart[] parts = GetComponentsInChildren<NetworkTentaclePart>();
        for (int i = 0; i < parts.Length; i++)
        {
            if (bodyPartId == parts[i].BodyPartId_)
                return parts[i].gameObject;
        }
        return null;
    }
    public void InjectPart(GameObject bodyPartGO)
    {
        bodyParts_.Remove(bodyPartGO);
        Target_.GetComponent<NetworkCell>().RecievePulse(1, this);
        GameObject.Destroy(bodyPartGO);
    }
    public void Retreat()
    {
        gotCut_ = true;
        state_ = new NetworkToParentState(target_, this);
    }
}
