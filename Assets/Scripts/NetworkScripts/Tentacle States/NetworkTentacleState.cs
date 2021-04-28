using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkTentacleState
{
    INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject);
    void DoStateAction(NetworkTentacle tentacle);
}
