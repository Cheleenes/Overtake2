using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : ITentacleState
{
    public DeadState(Tentacle tentacle)
    {
        tentacle.GetComponentInParent<Cell>().RemoveTarget(tentacle.Target_);
    }
    public ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject)
    {


        return null;
    }

    public void DoStateAction(Tentacle tentacle)
    {
        
    }
}
