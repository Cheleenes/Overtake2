using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    int teamId_;
    List<Player> teamPlayers_;
    [SerializeField] List<GameObject> teamCells;
    public static List<Team> teams = new List<Team>();
    public static Dictionary<int, Color> teamColors = new Dictionary<int, Color>();

    public int TeamId_ { get => teamId_; }

    public void Initialize(int teamId)
    {
        gameObject.name = "Team " + teamId;
        teamId_ = teamId;
        teamPlayers_ = new List<Player>();
        teamCells = new List<GameObject>();
        teams.Add(this);
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

    public bool HasCell_(Cell cell)
    {
        return teamCells.Contains(cell.gameObject);
    }
    public static int GetTeamId(Cell cell)
    {
        foreach(Team team in teams)
        {
            if (team.HasCell_(cell))
                return team.teamId_;
        }
        return -1;
    }
    public static Team GetTeam(Cell cell)
    {
        foreach (Team team in teams)
        {
            if (team.HasCell_(cell))
                return team;
        }
        return null;
    }
    public void AddTeamate(Player player)
    {
        teamPlayers_.Add(player);
    }
    public static void ConvertCell(Cell attacker, Cell prey)
    {
        Team teamAttacker = null;
        Team teamPrey = null;
        foreach (Team team in teams)
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
