using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTargetState : ITentacleState
{
    public float headDistance_;
    public TentaclePart headPart_;
    public float nextPartDistance_;
    public GameObject target_;
    public Tentacle tentacle_;
    public float bodyPartRadius_;
    float travelTime_;
    float travelSpeed_;
    GameObject middleGO_;
    Cell targetCell_;
    public ToTargetState(GameObject target, Tentacle tentacle)
    {
        tentacle_ = tentacle;
        target_ = target;
        headDistance_ = Vector3.Distance(tentacle.Head_.transform.position, target_.transform.position);
        bodyPartRadius_ = 0.2f;
        nextPartDistance_ = headDistance_ - bodyPartRadius_;
        headPart_ = tentacle.Head_.GetComponent<TentaclePart>();
        targetCell_ = target_.GetComponent<Cell>();
        if (Cell.AreAlly(tentacle_.ParentCell_, targetCell_))
        {
            foreach(Tentacle targetTentacle in targetCell_.Tentacles_)
            {
                if(targetTentacle.Target_ == tentacle_.ParentCell_.gameObject)
                {
                    targetTentacle.Retreat();
                }
            }
        }
    }
    public ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject)
    {
        if(tentacle.ParentCellCurrentHealth <= 0 || tentacle.gotCut_)//TODO: isDefending is not enough to know wether the tentacle should expant or contract, need a better solution
        {
            return new ToParentState(tentacle.Target_, tentacle);
        }
        if(headPart_.atDestination)
        {
            return new AtTargetState(tentacle.Target_, tentacle_);
        }
        if (tentacle.IsDefending_())
        {
            float distanceFromHeadToParent = Vector3.Distance(tentacle_.Head_.transform.position, tentacle_.transform.position);
            Vector3 middlePoint = (tentacle_.transform.position + tentacle_.Target_.transform.position) / 2;
            float distanceFromMiddleToParent = Vector3.Distance(middlePoint, tentacle_.transform.position);
            if (distanceFromHeadToParent >= distanceFromMiddleToParent)
            {
                return new ToParentState(tentacle.Target_, tentacle);
            }
        }


        return this;
    }

    public void DoStateAction(Tentacle tentacle)
    {
        headDistance_ = Vector3.Distance(tentacle_.Head_.transform.position, target_.transform.position);
        if(headDistance_ <= nextPartDistance_)
        {
            nextPartDistance_ = headDistance_ - bodyPartRadius_;
            tentacle_.CreateBodyPart();
        }
        HandleExtensionMovement();
    }

    void HandleExtensionMovement()
    {
        travelTime_ += Time.deltaTime;
        if (travelTime_ > travelSpeed_)
        {
            foreach (GameObject bodyPart in tentacle_.BodyParts_)
            {
                TentaclePart part = bodyPart.GetComponent<TentaclePart>();
                if (!part.atDestination)
                {
                    if (part.BodyPartId_ == 0 && tentacle_.IsDefending_())
                    {                  
                        if (middleGO_ == null)
                        {
                            Vector3 midlePos = Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(tentacle_.transform.position, part.targetGO_.transform.position) / 2);
                            midlePos = Vector3.MoveTowards(midlePos, tentacle_.Target_.transform.position, 0.1f);
                            middleGO_ = new GameObject();
                            middleGO_.transform.position = midlePos;
                            part.targetGO_ = middleGO_;
                        }
                        part.transform.position = Vector3.MoveTowards(part.transform.position, part.targetGO_.transform.position, Tentacle.travelSpeed_ * Time.deltaTime);
                    }
                    else
                    {
                        if(part.BodyPartId_ == 0 &&  middleGO_ != null)
                        {
                            part.targetGO_ = tentacle_.Target_;
                        }
                        Vector3 variable = Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, 0.2f);
                        float distance = Vector3.Distance(part.transform.position, variable);
                        float distance2D = Vector2.Distance(part.transform.position, variable);
                        part.transform.position = Vector3.MoveTowards(part.transform.position, Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, 0.2f), Tentacle.travelSpeed_ * Time.deltaTime);
                    }
                }
            }
            travelTime_ = 0;
        }
    }
}
