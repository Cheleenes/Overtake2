using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkToTargetState : INetworkTentacleState
{
    public float headDistance_;
    public NetworkTentaclePart headPart_;
    public float nextPartDistance_;
    public GameObject target_;
    public NetworkTentacle tentacle_;
    public float bodyPartRadius_;
    public float targetRadius_;
    float travelTime_;
    float travelSpeed_;
    GameObject middleGO_;
    NetworkCell targetCell_;
    public NetworkToTargetState(GameObject target, NetworkTentacle tentacle)
    {
        tentacle_ = tentacle;
        target_ = target;
        targetRadius_ = tentacle.Target_.GetComponent<CircleCollider2D>().radius;
        headPart_ = tentacle.Head_.GetComponent<NetworkTentaclePart>();
        bodyPartRadius_ = headPart_.GetComponent<CircleCollider2D>().radius * headPart_.transform.localScale.x;
        headDistance_ = 0;
        nextPartDistance_ = bodyPartRadius_*2;
        targetCell_ = target_.GetComponent<NetworkCell>();
        if (NetworkCell.AreAlly(tentacle_.ParentCell_, targetCell_, tentacle_.ParentCell_.TeamManager_))
        {
            foreach (NetworkTentacle targetTentacle in targetCell_.Tentacles_)
            {
                if (targetTentacle.Target_ == tentacle_.ParentCell_.gameObject)
                {
                    targetTentacle.Retreat();
                }
            }
        }
    }
    public INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject)
    {
        if (tentacle.ParentCellCurrentHealth <= 0 || tentacle.gotCut_)
        {
            return new NetworkToParentState(tentacle.Target_, tentacle);
        }
        Vector3 auxPox = Vector3.MoveTowards(headPart_.targetGO_.transform.position, headPart_.parentTentacle_.transform.position, targetRadius_);
        if (headPart_.transform.position == auxPox || headPart_.transform.position == headPart_.targetGO_.transform.position) //headPart_.atDestination)
        {
            return new NetworkAtTargetState(tentacle.Target_, tentacle_);
        }
        if (tentacle.IsDefending_())
        {
            float distanceFromHeadToParent = Vector3.Distance(tentacle_.Head_.transform.position, tentacle_.transform.position);
            Vector3 middlePoint = Vector3.MoveTowards(tentacle_.Target_.transform.position, tentacle_.transform.position, Vector3.Distance(tentacle_.transform.position, tentacle_.Target_.transform.position) / 2);
            float distanceFromMiddleToParent = Vector3.Distance(middlePoint, tentacle_.transform.position);
            if (distanceFromHeadToParent > distanceFromMiddleToParent)
            {
                return new NetworkToParentState(tentacle.Target_, tentacle);
            }
        }


        return this;
    }

    public void DoStateAction(NetworkTentacle tentacle)
    {
        headDistance_ = Vector3.Distance(tentacle.transform.position, tentacle_.Head_.transform.position);
        if (headDistance_ >= nextPartDistance_)
        {
            nextPartDistance_ = headDistance_ + (bodyPartRadius_*2);
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
                NetworkTentaclePart part = bodyPart.GetComponent<NetworkTentaclePart>();
                if (!part.atDestination)
                {
                    if (part.IsHead_ && tentacle_.IsDefending_())
                    {
                        if (middleGO_ == null)
                        {
                            Vector3 midlePos = Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, Vector3.Distance(tentacle_.transform.position, part.targetGO_.transform.position) / 2);
                            midlePos = Vector3.MoveTowards(midlePos, tentacle_.transform.position, 0.1f);
                            middleGO_ = new GameObject();
                            middleGO_.transform.position = midlePos;
                            Debug.Log("Middle position: " + midlePos);
                            part.targetGO_ = middleGO_;
                        }
                        part.transform.position = Vector3.MoveTowards(part.transform.position, part.targetGO_.transform.position, NetworkTentacle.travelSpeed_ * Time.deltaTime);
                    }
                    else
                    {
                        if (part.IsHead_)
                        {
                            if (middleGO_ != null)
                                part.targetGO_ = tentacle_.Target_;
                            part.transform.position = Vector3.MoveTowards(part.transform.position, Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, targetRadius_), NetworkTentacle.travelSpeed_ * Time.deltaTime);
                        }
                        else
                        {
                            part.transform.position = Vector3.MoveTowards(part.transform.position, Vector3.MoveTowards(part.targetGO_.transform.position, tentacle_.transform.position, bodyPartRadius_), NetworkTentacle.travelSpeed_ * Time.deltaTime);
                        }
                    }
                }
            }
            travelTime_ = 0;
        }
    }
}
