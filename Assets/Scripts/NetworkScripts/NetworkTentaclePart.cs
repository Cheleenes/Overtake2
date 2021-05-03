using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;

public class NetworkTentaclePart : NetworkBehaviour
{
    int bodyPartId_;
    public GameObject targetGO_;
    NetworkTentaclePart nextPart_;
    //private bool hasPulse_;
    public NetworkVariableBool hasPulse_ = new NetworkVariableBool();
    NetworkTentacle parentTentacle_;
    SpriteRenderer spr_;
    int pulseVal_;

    public bool atDestination { get => transform.position == Vector3.MoveTowards(targetGO_.transform.position, parentTentacle_.transform.position, 0.2f); }
    public GameObject TargetGO { get => targetGO_; set => targetGO_ = value; }
    public bool HasPulse_ { get => hasPulse_.Value; set => hasPulse_.Value = value; }
    public int BodyPartId_ { get => bodyPartId_; }

    

    public void Initialize(int bodyPartId, GameObject targetGO)
    {
        gameObject.name = "Body part " + bodyPartId;
        bodyPartId_ = bodyPartId;
        targetGO_ = targetGO;
        if (bodyPartId != 0)
            nextPart_ = targetGO_.GetComponent<NetworkTentaclePart>();
        parentTentacle_ = GetComponentInParent<NetworkTentacle>();
        hasPulse_.Value = false;
        spr_ = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            spr_ = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            if(hasPulse_.Value)
                spr_.color = Color.blue;
            else
                spr_.color = Color.white;
        }
    }

    public void RecievePulse(int pulse)
    {
        pulseVal_ = pulse;
        hasPulse_.Value = true;
        spr_.color = Color.blue;
    }

    public void SendPulse()
    {
        if (nextPart_)
        {
            nextPart_.RecievePulse(pulseVal_);
        }
        else
        {
            parentTentacle_.Target_.GetComponent<NetworkCell>().RecievePulse(pulseVal_, parentTentacle_);
        }
        pulseVal_ = 0;
        hasPulse_.Value = false;
        spr_.color = Color.white;
    }
}
