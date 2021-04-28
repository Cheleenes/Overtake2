using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapSetup : MonoBehaviour
{
    public GameObject cellPrefab_;
    public GameObject teamPrefab_;
    public GameObject playerPrefab_;
    public GameObject textGO_;
    public static List<Team> teams_ = new List<Team>();
    Cell cellAux1_;
    Cell cellAux2_;
    Player playerAux1_;
    Text textAux1_;
    bool boolAux_;
    // Start is called before the first frame update
    void Start()
    {
        //Read mapfile, instantiate and initialize objects and teams

        //hardcoded for testing
        /*Team.teamColors.Add(-1, Color.gray);
        Team.teamColors.Add(0, Color.blue);
        Team.teamColors.Add(1, Color.red);
        Team.teamColors.Add(2, Color.green);
        Team.teamColors.Add(3, Color.yellow);
        Team.teamColors.Add(4, Color.magenta);

        Team neutralTeam = CreateTeam(-1);
        Team team1 = CreateTeam(1);
        Team team2 = CreateTeam(2);

        GameObject player1 = GameObject.Instantiate(playerPrefab_);
        player1.GetComponent<Player>().Initialize(1, team1);
        team1.AddTeamate(player1.GetComponent<Player>());
        playerAux1_ = player1.GetComponent<Player>();
        textAux1_ = textGO_.GetComponent<Text>();

        CreateCell(new Vector2(0, -3.5f), team1, 1, 10, 50, 1);
        CreateCell(new Vector2(-1, -1.5f), neutralTeam, 2, 10, 50, 1);
        CreateCell(new Vector2(1, -1.5f), neutralTeam, 3, 10, 50, 1);
        CreateCell(new Vector2(-1, 2.5f), team2, 4, 10, 50, 1);
        CreateCell(new Vector2(1, 2.5f), team2, 5, 10, 50, 1);
        CreateCell(new Vector2(-2, 3.5f), team2, 6, 10, 50, 1);
        CreateCell(new Vector2(0, 3.5f), team2, 7, 10, 50, 1);
        CreateCell(new Vector2(2, 3.5f), team2, 8, 10, 50, 1);*/

        /*cell1.GetComponent<Cell>().AttackCell(cell3);
        cell2.GetComponent<Cell>().AttackCell(cell1);

        cellAux1_ = cell1.GetComponent<Cell>();
        cellAux2_ = cell2.GetComponent<Cell>();*/
        //boolAux_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        //textAux1_.text = "FPS: " + (int)(1f / Time.unscaledDeltaTime) + "    Player State: " + playerAux1_.State_.ToString();
        /*if (!boolAux_ && cellAux2_.Tentacles_[0].State is AtTargetState && cellAux2_.CurrentHealth_ > 22)
        {
            cellAux2_.Tentacles_[0].GetCut(cellAux2_.Tentacles_[0].BodyParts_[30]);
            boolAux_ = true;
        }*/
    }

    void CreateCell(Vector2 position, Team team, int id, int initialHelath, int maxHealth, int pulseValue)
    {
        GameObject cell = GameObject.Instantiate(cellPrefab_);
        cell.transform.position = position;
        team.AddCell(cell);
        cell.GetComponent<Cell>().Initialize(id, initialHelath, maxHealth, pulseValue);
    }

    Team CreateTeam(int teamId)
    {
        GameObject teamGO = GameObject.Instantiate(teamPrefab_);
        Team team = teamGO.GetComponent<Team>();
        team.Initialize(teamId);
        teams_.Add(team);
        return team;
    }
}
