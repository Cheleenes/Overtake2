using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class NetworkTeam : NetworkBehaviour
{
    int teamId_;
    List<NetworkPlayer> teamPlayers_;
    [SerializeField] List<GameObject> teamCells;
    public static List<NetworkTeam> teams_ = new List<NetworkTeam>();
    public static Dictionary<int, Color> teamColors = new Dictionary<int, Color>();

    public int TeamId_ { get => teamId_; }

    public void Initialize(int teamId)
    {
        gameObject.name = "Team " + teamId;
        teamId_ = teamId;
        teamPlayers_ = new List<NetworkPlayer>();
        teamCells = new List<GameObject>();
        teams_.Add(this);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCell(GameObject cellGO)
    {
        teamCells.Add(cellGO);
    }

    public void RemoveCell(GameObject cellGO)
    {
        teamCells.Remove(cellGO);
    }

    public bool HasCell_(NetworkCell cell)
    {
        return teamCells.Contains(cell.gameObject);
    }
    public static int GetTeamId(NetworkCell cell)
    {
        foreach (NetworkTeam team in teams_)
        {
            if (team.HasCell_(cell))
                return team.teamId_;
        }
        return -1;
    }
    public static NetworkTeam GetTeam(NetworkCell cell)
    {
        foreach (NetworkTeam team in teams_)
        {
            if (team.HasCell_(cell))
                return team;
        }
        return null;
    }
    public void AddTeamate(NetworkPlayer player)
    {
        teamPlayers_.Add(player);
    }
    public static void ConvertCell(NetworkCell attacker, NetworkCell prey)
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
