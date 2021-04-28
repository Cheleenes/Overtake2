using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCutState : INetworkTentacleState
{
    public float distanceToParent_;
    public float distanceToTarget_;
    int cutIdenx_;
    NetworkTentacle tentacle_;
    List<GameObject> toTarget_;
    List<GameObject> toParent_;
    float travelTimeParent_;
    float travelTimeTarget_;
    float travelSpeed_;
    public NetworkCutState(NetworkTentacle tentacle, GameObject cutOrigin)
    {
        tentacle_ = tentacle;
        cutIdenx_ = tentacle_.BodyParts_.IndexOf(cutOrigin);
        toTarget_ = new List<GameObject>();
        toParent_ = new List<GameObject>();
        for (int i = 0; i <= cutIdenx_; i++)
        {
            toTarget_.Add(tentacle_.BodyParts_[i]);
        }
        for (int i = tentacle_.bodyPartCount_ - 1; i > cutIdenx_; i--)
        {
            toParent_.Add(tentacle_.BodyParts_[i]);
        }
    }
    public INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject)
    {
        if (tentacle.bodyPartCount_ == 0)
        {
            return new NetworkDeadState(tentacle);
        }
        return this;
    }

    public void DoStateAction(NetworkTentacle tentacle)
    {
        TravelToParent();
        TravelToTarget();
    }

    void TravelToParent()
    {
        travelTimeParent_ += Time.deltaTime;
        if (travelTimeParent_ > travelSpeed_)
        {
            GameObject toRemove = null;
            foreach (GameObject bodyPartGO in toParent_)
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
            {
                toParent_.Remove(toRemove);
                tentacle_.RemoveBodyPart(toRemove);
            }
            travelTimeParent_ = 0;
        }
    }
    void TravelToTarget()
    {
        travelTimeTarget_ += Time.deltaTime;
        if (travelTimeTarget_ > travelSpeed_)
        {
            GameObject toRemove = null;
            foreach (GameObject bodyPartGO in toTarget_)
            {

                //TentaclePart bodyPart = bodyPartGO.GetComponent<TentaclePart>();
                if (bodyPartGO.transform.position != tentacle_.Target_.transform.position)
                {
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.Target_.transform.position, NetworkTentacle.fastTravelSpeed_ * Time.deltaTime);
                }
                if (bodyPartGO.transform.position == tentacle_.Target_.transform.position)
                {
                    toRemove = bodyPartGO;
                }
            }
            if (toRemove != null)
            {
                toTarget_.Remove(toRemove);
                tentacle_.InjectPart(toRemove);
            }
            travelTimeTarget_ = 0;
        }
    }
}
