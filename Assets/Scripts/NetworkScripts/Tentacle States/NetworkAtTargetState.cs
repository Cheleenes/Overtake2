using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAtTargetState : INetworkTentacleState
{
    public GameObject target_;
    public NetworkTentacle tentacle_;
    public float bodyPartRadius_;
    float pulseTime_;
    float pulseHoldTime_;
    bool atCell_;
    NetworkCell targetCell_;
    NetworkTentaclePart tentacleHead_;
    public NetworkAtTargetState(GameObject target, NetworkTentacle tentacle)
    {
        tentacle_ = tentacle;
        target_ = target;
        bodyPartRadius_ = 0.2f;
        pulseTime_ = 0;
        pulseHoldTime_ = 0.03f;
        tentacleHead_ = tentacle.Head_.GetComponent<NetworkTentaclePart>();
        atCell_ = tentacleHead_.targetGO_.GetComponent<NetworkCell>() != null;
        targetCell_ = target.GetComponent<NetworkCell>();
    }
    public INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject)
    {
        if (tentacle.gotCut_)
        {
            return new NetworkCutState(tentacle, tentacle_.cutOrigin_);
        }
        if (tentacle.IsDefending_() && atCell_)
        {
            return new NetworkToParentState(tentacle.Target_, tentacle);
        }
        if (!targetCell_.Targets_.Contains(tentacle_.ParentCell_.gameObject) && tentacleHead_.TargetGO.tag != "Cell")//If target cell stopped defending
        {
            tentacleHead_.targetGO_ = tentacle_.Target_;
            return new NetworkToTargetState(tentacle.Target_, tentacle);
        }

        return this;
    }

    public void DoStateAction(NetworkTentacle tentacle)
    {
        HandlePulse();
    }

    void HandlePulse()
    {
        pulseTime_ += Time.deltaTime;
        if (pulseTime_ > pulseHoldTime_)
        {
            foreach (GameObject bodyPartGO in tentacle_.BodyParts_)
            {
                NetworkTentaclePart bodyPart = bodyPartGO.GetComponent<NetworkTentaclePart>();
                if (bodyPart.HasPulse_)
                {
                    bodyPart.SendPulse();
                }
            }
            pulseTime_ = 0;
        }
    }
}
