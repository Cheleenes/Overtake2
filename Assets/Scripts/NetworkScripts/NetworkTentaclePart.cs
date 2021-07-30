using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;

public class NetworkTentaclePart : NetworkBehaviour
{
    static int Ids_ = 0;
    //int bodyPartId_;
    public NetworkVariableInt bodyPartId_ = new NetworkVariableInt();
    public GameObject targetGO_;
    NetworkTentaclePart nextPart_;
    //private bool hasPulse_;
    public NetworkVariableBool hasPulse_ = new NetworkVariableBool();
    public NetworkTentacle parentTentacle_;
    SpriteRenderer spr_;
    int pulseVal_;
    bool isHead_;

    public bool atDestination { get
        {
            if (IsHead_)
            {
                Vector3 targetPos = Vector3.MoveTowards(targetGO_.transform.position, parentTentacle_.transform.position, parentTentacle_.Target_.GetComponent<CircleCollider2D>().radius);
                return transform.position == targetPos;
            }
            else
            {
                return transform.position == Vector3.MoveTowards(targetGO_.transform.position, parentTentacle_.transform.position, 0.2f);
            }
        }
    }
    public GameObject TargetGO { get => targetGO_; set => targetGO_ = value; }
    public bool HasPulse_ { get => hasPulse_.Value; set => hasPulse_.Value = value; }
    public int BodyPartId_ { get => bodyPartId_.Value; }
    public bool IsHead_ { get => isHead_; }

    public void Initialize(int bodyPartId, GameObject targetGO, bool isHead)
    {
        gameObject.name = "Body part " + bodyPartId;
        bodyPartId_.Value = Ids_++;//bodyPartId;
        targetGO_ = targetGO;
        if (bodyPartId != 0)
            nextPart_ = targetGO_.GetComponent<NetworkTentaclePart>();
        parentTentacle_ = GetComponentInParent<NetworkTentacle>();
        hasPulse_.Value = false;
        spr_ = GetComponent<SpriteRenderer>();
        isHead_ = isHead;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            spr_ = GetComponent<SpriteRenderer>();
            parentTentacle_ = GetComponentInParent<NetworkTentacle>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            if (hasPulse_.Value)
                spr_.color = parentTentacle_.ParentCell_.myColor_.Value;
            else
                spr_.color = Color.white;
        }
    }

    public void RecievePulse(int pulse)
    {
        pulseVal_ = pulse;
        hasPulse_.Value = true;
        spr_.color = parentTentacle_.ParentCell_.myColor_.Value;
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
