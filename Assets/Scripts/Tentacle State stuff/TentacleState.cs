using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITentacleState
{
    ITentacleState ChangeState(Tentacle tentacle, GameObject gameObject);
    void DoStateAction(Tentacle tentacle);
}
