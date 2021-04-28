using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentaclePart : MonoBehaviour
{
    int bodyPartId_;
    public GameObject targetGO_;
    TentaclePart nextPart_;
    private bool hasPulse_;
    Tentacle parentTentacle_;
    SpriteRenderer spr_;
    int pulseVal_;
    
    public bool atDestination { get => transform.position == Vector3.MoveTowards(targetGO_.transform.position, parentTentacle_.transform.position, 0.2f); }
    public GameObject TargetGO { get => targetGO_; set => targetGO_ = value; }
    public bool HasPulse_ { get => hasPulse_; set => hasPulse_ = value; }
    public int BodyPartId_ { get => bodyPartId_; }

    public void Initialize(int bodyPartId, GameObject targetGO)
    {
        gameObject.name = "Body part " + bodyPartId;
        bodyPartId_ = bodyPartId;
        targetGO_ = targetGO;
        if(bodyPartId != 0)
            nextPart_ = targetGO_.GetComponent<TentaclePart>();       
        parentTentacle_ = GetComponentInParent<Tentacle>();
        hasPulse_ = false;
        spr_ = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RecievePulse(int pulse)
    {
        pulseVal_ = pulse;
        hasPulse_ = true;
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
            parentTentacle_.Target_.GetComponent<Cell>().RecievePulse(pulseVal_, parentTentacle_);
        }
        pulseVal_ = 0;
        hasPulse_ = false;
        spr_.color = Color.white;
    }
}
