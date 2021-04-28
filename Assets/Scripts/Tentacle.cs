using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public static float travelSpeed_ = 2f;
    public static float fastTravelSpeed_ = 10f;
    ITentacleState state_;
    int tentacleId_;
    List<GameObject> bodyParts_;
    public GameObject bodyPartPrefab_;
    GameObject target_;
    Cell parentCell_;
    public bool gotCut_; //todo: define how tis var will change to true
    public GameObject cutOrigin_;
    public int bodyPartCount_ { get => bodyParts_.Count; }
    public GameObject Target_ { get => target_; }
    public GameObject Head_ { get => bodyParts_[0]; }
    public ITentacleState State { get => state_; }
    public int ParentCellCurrentHealth { get => parentCell_.CurrentHealth_; }
    public List<GameObject> BodyParts_ { get => bodyParts_; set => bodyParts_ = value; }
    public bool IsDefending_()
    {
        foreach(GameObject cellGO in parentCell_.Targets_)
        {
            Cell cell = cellGO.GetComponent<Cell>();
            if (cell.Targets_.Contains(parentCell_.gameObject))
            {
                return true;
            }
        }
        return false;
    }
    public Cell ParentCell_ { get => parentCell_; }

    public void Initialize(int tentacleId, GameObject target)
    {
        gameObject.name = "Tentacle " + tentacleId;
        tentacleId_ = tentacleId;
        target_ = target;
        bodyParts_ = new List<GameObject>();
        GameObject bodyPartGO = GameObject.Instantiate(bodyPartPrefab_, transform);
        bodyPartGO.GetComponent<TentaclePart>().Initialize(bodyPartCount_, target_);
        bodyParts_.Add(bodyPartGO);
        parentCell_ = GetComponentInParent<Cell>();
        state_ = new ToTargetState(target_, this);        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ITentacleState state = state_.ChangeState(this, target_);
        if(state != null)
        {
            if(state.GetType() != state_.GetType())
            {
                Debug.Log(parentCell_.gameObject.name + ", Tentacle " + tentacleId_ + " has changed its state from " + state_.GetType() + " to " + state.GetType());
                if (state is DeadState)
                {
                    parentCell_.RemoveTentacle(this);
                }
            }            
            state_ = null;
            state_ = state;
        }
        state_.DoStateAction(this);
    }

    public void SendPulse(int pulse)
    {
        if (state_ is AtTargetState)
        {
            bodyParts_[bodyPartCount_ - 1].GetComponent<TentaclePart>().RecievePulse(pulse);
        }
    }

    public void CreateBodyPart()
    {
        parentCell_.CurrentHealth_ -= 1;
        GameObject bodyPart = GameObject.Instantiate(bodyPartPrefab_, transform);
        if (bodyPartCount_ == 0)
        {
            bodyPart.GetComponent<TentaclePart>().Initialize(bodyPartCount_ + 1, target_);
        }
        else
        {
            bodyPart.GetComponent<TentaclePart>().Initialize(bodyPartCount_, bodyParts_[bodyPartCount_ - 1]);
        }
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
    public void InjectPart(GameObject bodyPartGO)
    {
        bodyParts_.Remove(bodyPartGO);
        Target_.GetComponent<Cell>().RecievePulse(1, this);
        GameObject.Destroy(bodyPartGO);
    }
    public void Retreat()
    {
        gotCut_ = true;
        state_ = new ToParentState(target_, this);
    }
}
