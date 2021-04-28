using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToParentSubState
{
    totalReturn,
    partialReturn
}
public class ToParentState : ITentacleState
{
    public GameObject target_;
    public Tentacle tentacle_;
    float travelTime_;
    float travelSpeed_;
    ToParentSubState state_;
    GameObject middleGO_;
    public ToParentState(GameObject target, Tentacle tentacle)
    {
        target_ = target;
        tentacle_ = tentacle;
        travelSpeed_ = 0.001f;
        if (tentacle_.ParentCellCurrentHealth == 0 || tentacle_.gotCut_)
        {
            state_ = ToParentSubState.totalReturn;
            tentacle_.ParentCell_.RemoveTarget(tentacle_.Target_);
        }
        else
        {
            state_ = ToParentSubState.partialReturn;
        }
    }
    public ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject)
    {
        if(state_ == ToParentSubState.partialReturn && tentacle_.Head_.GetComponent<TentaclePart>().atDestination)
        {
            return new AtTargetState(tentacle.Target_, tentacle);
        }
        if (state_ == ToParentSubState.partialReturn && !tentacle.IsDefending_())
        {
            return new ToTargetState(tentacle.Target_, tentacle);
        }
        if(tentacle.bodyPartCount_ == 0)
        {
            return new DeadState(tentacle);
        }
        return this;
    }

    public void DoStateAction(Tentacle tentacle)
    {
        HanldeRetractionMovement();
    }
   void HanldeRetractionMovement()
   {
        switch (state_)
        {
            case ToParentSubState.partialReturn:
                PartialReturn();
                break;

            case ToParentSubState.totalReturn:
                TotalReturn();
                break;
        }
                
   }

    void PartialReturn()
    {
        travelTime_ += Time.deltaTime;
        if (travelTime_ > travelSpeed_)
        {
            GameObject toRemove = null;
            TentaclePart headPart = null;
            foreach (GameObject bodyPartGO in tentacle_.BodyParts_)
            {
                TentaclePart bodyPart = bodyPartGO.GetComponent<TentaclePart>();
                if (middleGO_ == null && bodyPart.BodyPartId_ == 0)
                {
                    Vector3 midlePos = Vector3.MoveTowards(bodyPart.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(tentacle_.transform.position, bodyPart.targetGO_.transform.position) / 2);
                    midlePos = Vector3.MoveTowards(midlePos, tentacle_.Target_.transform.position, -0.3f);
                    middleGO_ = new GameObject();
                    middleGO_.transform.position = midlePos;
                    bodyPart.targetGO_ = middleGO_;
                    headPart = bodyPart;
                }
                if (bodyPart.BodyPartId_ == 0 && !bodyPart.atDestination)
                {
                    /*Vector3 middlePoint = Vector3.MoveTowards(bodyPart.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(bodyPart.targetGO_.transform.position, tentacle_.transform.position) / 2);
                    bodyPart.transform.position = Vector3.MoveTowards(bodyPart.transform.position, middlePoint, 0.01f);*/
                  
                    bodyPart.transform.position = Vector3.MoveTowards(bodyPart.transform.position, bodyPart.targetGO_.transform.position, Tentacle.travelSpeed_ * Time.deltaTime);
                    
                }
                else
                {
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.transform.position, Tentacle.travelSpeed_ * Time.deltaTime);
                }
                if (bodyPartGO.transform.position == tentacle_.transform.position)
                {
                    toRemove = bodyPartGO;
                }
            }
            if (toRemove != null)
                tentacle_.RemoveBodyPart(toRemove);
            travelTime_ = 0;
        }
    }

    void TotalReturn()
    {
        travelTime_ += Time.deltaTime;
        if (travelTime_ > travelSpeed_)
        {
            GameObject toRemove = null;
            foreach (GameObject bodyPartGO in tentacle_.BodyParts_)
            {
                
                //TentaclePart bodyPart = bodyPartGO.GetComponent<TentaclePart>();
                if (bodyPartGO.transform.position != tentacle_.transform.position)
                {
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.transform.position, Tentacle.fastTravelSpeed_ * Time.deltaTime);
                }
                if (bodyPartGO.transform.position == tentacle_.transform.position)
                {
                    toRemove = bodyPartGO;
                }
            }
            if(toRemove != null)
                tentacle_.RemoveBodyPart(toRemove);
            travelTime_ = 0;
        }
    }
}
