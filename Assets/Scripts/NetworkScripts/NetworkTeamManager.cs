using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable.Collections;

public class NetworkTeamManager : NetworkBehaviour
{
    public NetworkList<NetworkTeam> teams_ = new NetworkList<NetworkTeam>();
    public Dictionary<int, Color> teamColors = new Dictionary<int, Color>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int GetTeamId(NetworkCell cell)
    {
        foreach (NetworkTeam team in teams_)
        {
            if (team.HasCell_(cell))
                return team.TeamId_;
        }
        return -1;
    }
    public NetworkTeam GetTeam(NetworkCell cell)
    {
        foreach (NetworkTeam team in teams_)
        {
            if (team.HasCell_(cell))
                return team;
        }
        return null;
    }
    public void ConvertCell(NetworkCell attacker, NetworkCell prey)
    {
        NetworkTeam teamAttacker = null;
        NetworkTeam teamPrey = null;
        foreach (NetworkTeam team in teams_)
        {
            if (team.HasCell_(attacker))
                teamAttacker = team;
            if (team.HasCell_(prey))
                teamPrey = team;
        }
        teamPrey.RemoveCell(prey.gameObject);
        teamAttacker.AddCell(prey.gameObject);
        prey.RetreatAllTentacles();
    }
}
