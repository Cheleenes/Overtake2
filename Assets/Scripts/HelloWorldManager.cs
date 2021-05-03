using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;

public class HelloWorldManager : NetworkBehaviour
{
    public GameObject cellPrefab_;
    public GameObject teamPrefab_;
    public GameObject teamManagerPrefab_;
    NetworkTeamManager TM_;
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
        
        //OnClientConnectedCallback.Invoke()
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
        if (NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start game"))
            {
                GameObject teamManagerGO = Instantiate(teamManagerPrefab_);
                teamManagerGO.GetComponent<NetworkObject>().Spawn();
                TM_ = teamManagerGO.GetComponent<NetworkTeamManager>();

                TM_.teamColors.Add(-1, Color.gray);
                TM_.teamColors.Add(0, Color.blue);
                TM_.teamColors.Add(1, Color.red);
                TM_.teamColors.Add(2, Color.green);
                TM_.teamColors.Add(3, Color.yellow);
                TM_.teamColors.Add(4, Color.magenta);

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
    }

    void CreateCell(Vector2 position, NetworkTeam team, int id, int initialHelath, int maxHealth, int pulseValue)
    {
        GameObject cell = GameObject.Instantiate(cellPrefab_);
        cell.transform.position = position;
        team.AddCell(cell);
        cell.GetComponent<NetworkCell>().InitializeServer(id, initialHelath, maxHealth, pulseValue);
        cell.GetComponent<NetworkObject>().Spawn();
    }

    NetworkTeam CreateTeam(int teamId)
    {
        //GameObject teamGO = GameObject.Instantiate(teamPrefab_);
        NetworkTeam team = new NetworkTeam(teamId);//teamGO.GetComponent<NetworkTeam>();
        //team.Initialize(teamId);
        //teamGO.GetComponent<NetworkObject>().Spawn();
        TM_.teams_.Add(team);
        return team;
    }
}
