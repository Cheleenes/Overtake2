using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutState : ITentacleState
{
    public float distanceToParent_;
    public float distanceToTarget_;
    int cutIdenx_;
    Tentacle tentacle_;
    List<GameObject> toTarget_;
    List<GameObject> toParent_;
    float travelTimeParent_;
    float travelTimeTarget_;
    float travelSpeed_;
    public CutState(Tentacle tentacle, GameObject cutOrigin)
    {
        tentacle_ = tentacle;
        cutIdenx_ = tentacle_.BodyParts_.IndexOf(cutOrigin);
        toTarget_ = new List<GameObject>();
        toParent_ = new List<GameObject>();
        for(int i = 0; i <= cutIdenx_; i++)
        {
            toTarget_.Add(tentacle_.BodyParts_[i]);
        }
        for(int i = tentacle_.bodyPartCount_ - 1; i > cutIdenx_; i--)
        {
            toParent_.Add(tentacle_.BodyParts_[i]);
        }
    }
    public ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject)
    {
        if (tentacle.bodyPartCount_ == 0)
        {
            return new DeadState(tentacle);
        }
        return this;
    }

    public void DoStateAction(Tentacle tentacle)
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
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.transform.position, Tentacle.fastTravelSpeed_ * Time.deltaTime);
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
                    bodyPartGO.transform.position = Vector3.MoveTowards(bodyPartGO.transform.position, tentacle_.Target_.transform.position, Tentacle.fastTravelSpeed_ * Time.deltaTime);
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
