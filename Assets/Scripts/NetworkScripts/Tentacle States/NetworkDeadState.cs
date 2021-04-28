using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDeadState : INetworkTentacleState
{
    public NetworkDeadState(NetworkTentacle tentacle)
    {
        tentacle.GetComponentInParent<NetworkCell>().RemoveTarget(tentacle.Target_);
    }
    public INetworkTentacleState ChangeState(NetworkTentacle tentacle, GameObject gameObject)
    {


        return null;
    }

    public void DoStateAction(NetworkTentacle tentacle)
    {

    }
}
