using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class HelloWorldManager : MonoBehaviour
{
    public GameObject cellPrefab_;
    public GameObject teamPrefab_;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            InitializeMapSetup();
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    void InitializeMapSetup()
    {
        if(NetworkManager.Singleton.IsServer)
        if (GUILayout.Button("Start game"))
        {
                NetworkTeam.teamColors.Add(-1, Color.gray);
                NetworkTeam.teamColors.Add(0, Color.blue);
                NetworkTeam.teamColors.Add(1, Color.red);
                NetworkTeam.teamColors.Add(2, Color.green);
                NetworkTeam.teamColors.Add(3, Color.yellow);
                NetworkTeam.teamColors.Add(4, Color.magenta);

                NetworkTeam neutralTeam = CreateTeam(-1);
                NetworkTeam team1 = CreateTeam(1);
                NetworkTeam team2 = CreateTeam(2);

                NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<NetworkPlayer>().Initialize(1, team1);

                CreateCell(new Vector2(0, -3.5f), team1, 1, 10, 50, 1);
                CreateCell(new Vector2(-1, -1.5f), neutralTeam, 2, 10, 50, 1);
                CreateCell(new Vector2(1, -1.5f), neutralTeam, 3, 10, 50, 1);
                CreateCell(new Vector2(-1, 2.5f), team2, 4, 10, 50, 1);
                CreateCell(new Vector2(1, 2.5f), team2, 5, 10, 50, 1);
                CreateCell(new Vector2(-2, 3.5f), team2, 6, 10, 50, 1);
                CreateCell(new Vector2(0, 3.5f), team2, 7, 10, 50, 1);
                CreateCell(new Vector2(2, 3.5f), team2, 8, 10, 50, 1);
            }
    }

    void CreateCell(Vector2 position, NetworkTeam team, int id, int initialHelath, int maxHealth, int pulseValue)
    {
        GameObject cell = GameObject.Instantiate(cellPrefab_);
        cell.transform.position = position;
        team.AddCell(cell);
        cell.GetComponent<NetworkCell>().Initialize(id, initialHelath, maxHealth, pulseValue);
        cell.GetComponent<NetworkObject>().Spawn();
    }

    NetworkTeam CreateTeam(int teamId)
    {
        GameObject teamGO = GameObject.Instantiate(teamPrefab_);
        NetworkTeam team = teamGO.GetComponent<NetworkTeam>();
        team.Initialize(teamId);
        NetworkTeam.teams_.Add(team);
        teamGO.GetComponent<NetworkObject>().Spawn();
        return team;
    }
}
