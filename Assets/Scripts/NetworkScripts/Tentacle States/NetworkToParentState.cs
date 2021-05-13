using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkToParentSubState
{
    totalReturn,
    partialReturn
}
public class NetworkToParentState : INetworkTentacleState
{
    public GameObject target_;
    public NetworkTentacle tentacle_;
    public NetworkTentaclePart headPart_;
    float travelTime_;
    float travelSpeed_;
    NetworkToParentSubState state_;
    GameObject middleGO_;
    public NetworkToParentState(GameObject target, NetworkTentacle tentacle)
    {
        target_ = target;
        tentacle_ = tentacle;
        travelSpeed_ = 0.001f;
        headPart_ = tentacle.Head_.GetComponent<NetworkTentaclePart>();
        if (tentacle_.ParentCellCurrentHealth == 0 || tentacle_.gotCut_)
        {
            state_ = NetworkToParentSubState.totalReturn;
            tentacle_.ParentCell_.RemoveTarget(tentacle_.Target_);
        }
        else
        {
            state_ = NetworkToParentSubState.partialReturn;
        }
    }
    public INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject)
    {
        Vector3 auxPox = Vector3.MoveTowards(headPart_.targetGO_.transform.position, headPart_.parentTentacle_.transform.position, 0.2f);
        if (state_ == NetworkToParentSubState.partialReturn && (headPart_.transform.position == auxPox || headPart_.transform.position == headPart_.targetGO_.transform.position))
        {
            return new NetworkAtTargetState(tentacle.Target_, tentacle);
        }
        if (state_ == NetworkToParentSubState.partialReturn && !tentacle.IsDefending_())
        {
            return new NetworkToTargetState(tentacle.Target_, tentacle);
        }
        if (tentacle.bodyPartCount_ == 0)
        {
            return new NetworkDeadState(tentacle);
        }
        return this;
    }

    public void DoStateAction(NetworkTentacle tentacle)
    {
        HanldeRetractionMovement();
    }
    void HanldeRetractionMovement()
    {
        switch (state_)
        {
            case NetworkToParentSubState.partialReturn:
                PartialReturn();
                break;

            case NetworkToParentSubState.totalReturn:
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
            NetworkTentaclePart headPart = null;
            foreach (GameObject bodyPartGO in tentacle_.BodyParts_)
            {
                NetworkTentaclePart bodyPart = bodyPartGO.GetComponent<NetworkTentaclePart>();
                if (middleGO_ == null && bodyPart.IsHead_)
                {
                    Vector3 midlePos = Vector3.MoveTowards(bodyPart.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(tentacle_.transform.position, bodyPart.targetGO_.transform.position) / 2);
                    midlePos = Vector3.MoveTowards(midlePos, tentacle_.transform.position, 0.1f);
                    middleGO_ = new GameObject();
                    middleGO_.transform.position = midlePos;
                    bodyPart.targetGO_ = middleGO_;
                    headPart = bodyPart;
                }
                if (bodyPart.IsHead_ && !bodyPart.atDestination)
                {
                    /*Vector3 middlePoint = Vector3.MoveTowards(bodyPart.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(bodyPart.targetGO_.transform.position, tentacle_.transform.position) / 2);
                    bodyPart.transform.position = Vector3.MoveTowards(bodyPart.transform.position, middlePoint, 0.01f);*/

                    bodyPart.transform.position = Vector3.MoveTowards(bodyPart.transform.position, bodyPart.targetGO_.transform.position, NetworkTentacle.travelSpeed_ * Time.deltaTime);

                }
                else
                {
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.transform.position, NetworkTentacle.travelSpeed_ * Time.deltaTime);
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
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.transform.position, NetworkTentacle.fastTravelSpeed_ * Time.deltaTime);
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
}
