using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtTargetState : ITentacleState
{
    public GameObject target_;
    public Tentacle tentacle_;
    public float bodyPartRadius_;
    float pulseTime_;
    float pulseHoldTime_;
    bool atCell_;
    Cell targetCell_;
    TentaclePart tentacleHead_;
    public AtTargetState(GameObject target, Tentacle tentacle)
    {
        tentacle_ = tentacle;
        target_ = target;
        bodyPartRadius_ = 0.2f;
        pulseTime_ = 0;
        pulseHoldTime_ = 0.03f;
        tentacleHead_ = tentacle.Head_.GetComponent<TentaclePart>();
        atCell_ = tentacleHead_.targetGO_.GetComponent<Cell>() != null;
        targetCell_ = target.GetComponent<Cell>();
    }
    public ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject)
    {
        if(tentacle.gotCut_)
        {
            return new CutState(tentacle, tentacle_.cutOrigin_);
        }
        if (tentacle.IsDefending_() && atCell_)
        {
            return new ToParentState(tentacle.Target_, tentacle);
        }
        if (!targetCell_.Targets_.Contains(tentacle_.ParentCell_.gameObject) && tentacleHead_.TargetGO.tag != "Cell")//If target cell stopped defending
        {
            tentacleHead_.targetGO_ = tentacle_.Target_;
            return new ToTargetState(tentacle.Target_, tentacle);
        }

        return this;
    }

    public void DoStateAction(Tentacle tentacle)
    {
        HandlePulse();
    }

    void HandlePulse()
    {
        pulseTime_ += Time.deltaTime;
        if (pulseTime_ > pulseHoldTime_)
        {
            foreach(GameObject bodyPartGO in tentacle_.BodyParts_)
            {
                TentaclePart bodyPart = bodyPartGO.GetComponent<TentaclePart>();
                if (bodyPart.HasPulse_)
                {
                    bodyPart.SendPulse();
                }
            }            
            pulseTime_ = 0;
        }
    }
}
