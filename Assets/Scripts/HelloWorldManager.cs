using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using UnityEngine.UI;

public class HelloWorldManager : NetworkBehaviour
{
    public GameObject cellPrefab_;
    public GameObject teamPrefab_;
    public GameObject teamManagerPrefab_;
    public GameObject AllPanels_;
    public GameObject InGamePanel_;
    public Sprite Image1, Image2, Image3;
    Image InGamePanelImage_;
    Vector3 AllPanelsStartPos_;
    int mapSelected_;
    NetworkTeamManager TM_;
    static GUILayoutOption[] buttonOptions = {
                GUILayout.Width(600),
                GUILayout.Height(100)               
            };
    static GUIStyle buttonStyle;

    private void Start()
    {
        mapSelected_ = 1;
        AllPanelsStartPos_ = AllPanels_.transform.position;
        InGamePanelImage_ = InGamePanel_.GetComponent<Image>();
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 600, 600));
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 34;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            //StartButtons();
        }
        else
        {
            StatusLabels();

            InitializeMapSetup();
        }
        
        //OnClientConnectedCallback.Invoke()
        GUILayout.EndArea();
    }

    public void TutorialButton()
    {
        AllPanels_.transform.position = new Vector3(AllPanels_.transform.position.x + 8.254054069519043f, AllPanels_.transform.position.y, 0);
    }

    public void ReturnButton()
    {
        AllPanels_.transform.position = AllPanelsStartPos_;
    }

    public void HostButton()
    {
        AllPanels_.transform.position = new Vector3(AllPanels_.transform.position.x - 8.254054069519043f, AllPanels_.transform.position.y, 0);
    }

    public void StartGameButton()
    {
        SetIngamePanel();
        NetworkManager.Singleton.StartHost();
    }

    public void JoinGameButton()
    {
        SetIngamePanel();
        NetworkManager.Singleton.StartClient();
    }

    public void DisconnectButton()
    {
        if (NetworkManager.Singleton.IsHost)
            NetworkManager.StopHost();
        if (NetworkManager.Singleton.IsClient)
            NetworkManager.StopClient();
        ReturnButton();
    }

    public void SetIngamePanel()
    {
        AllPanels_.transform.position = AllPanelsStartPos_;
        AllPanels_.transform.position = new Vector3(AllPanels_.transform.position.x - (8.254054069519043f*2), AllPanels_.transform.position.y, 0);
    }

    public void SelectMap(int mapId)
    {
        mapSelected_ = mapId;
        switch (mapId)
        {
            case 1:
                InGamePanelImage_.sprite = Image1;
                break;
            case 2:
                InGamePanelImage_.sprite = Image2;
                break;
            case 3:
                InGamePanelImage_.sprite = Image3;
                break;
        }
    }
    static void StartButtons()
    {       
        if (GUILayout.Button("Host", buttonStyle, buttonOptions)) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client", buttonStyle, buttonOptions)) NetworkManager.Singleton.StartClient();
        //if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
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
            if (!TM_)
            {
                if (GUILayout.Button("Start game", buttonStyle, buttonOptions))
                {
                    GameObject teamManagerGO = Instantiate(teamManagerPrefab_);
                    teamManagerGO.GetComponent<NetworkObject>().Spawn();
                    TM_ = teamManagerGO.GetComponent<NetworkTeamManager>();

                    TM_.teamColors.Add(-1, Color.white);
                    TM_.teamColors.Add(0, Color.magenta);
                    TM_.teamColors.Add(1, Color.red);
                    TM_.teamColors.Add(2, Color.green);
                    TM_.teamColors.Add(3, Color.yellow);
                    TM_.teamColors.Add(4, Color.cyan);

                    NetworkTeam neutralTeam = CreateTeam(-1);
                    NetworkTeam team1 = CreateTeam(0);
                    NetworkTeam team2 = CreateTeam(1);

                    NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<NetworkPlayer>().Initialize(1, team1);
                    int cellMaxHealth = 30;
                    switch (mapSelected_) {
                        case 1:
                            CreateCell(new Vector2(0, -6), team1, 1, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0, 6), team2, 2, 10, cellMaxHealth, 1);

                            CreateCell(new Vector2(-1.5f, -4), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(1.5f, -4), neutralTeam, 4, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-2.5f, -2), neutralTeam, 5, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0, -2), neutralTeam, 6, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(2.5f, -2), neutralTeam, 7, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-1.5f, 4), neutralTeam, 8, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(1.5f, 4), neutralTeam, 9, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-2.5f, 2), neutralTeam, 10, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0, 2), neutralTeam, 11, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(2.5f, 2), neutralTeam, 12, 10, cellMaxHealth, 1);

                            break;

                        case 2:
                            CreateCell(new Vector2(3.0547359f, -3.03451753f), team1, 1, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-3.03999996f, 3.03999996f), team2, 2, 10, cellMaxHealth, 1);

                            CreateCell(new Vector2(0.579999983f, -2.68000007f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(2.7021470f, -0.559507132f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-2.69237232f, 0.564286232f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-0.565979958f, 2.68052292f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0, 0), neutralTeam, 3, 10, cellMaxHealth, 1);

                            break;

                        case 3:
                            CreateCell(new Vector2(0, -6), team1, 1, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0, 6), team2, 2, 10, cellMaxHealth, 1);

                            CreateCell(new Vector2(-2.20000005f, -1.10000002f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(0.769999981f, -1.10000002f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(-0.710000038f, 1.10000002f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            CreateCell(new Vector2(2.3499999f, 1.10000002f), neutralTeam, 3, 10, cellMaxHealth, 1);
                            break;
                    }

                }
            }
        }
    }

    void CreateCell(Vector2 position, NetworkTeam team, int id, int initialHelath, int maxHealth, int pulseValue)
    {
        GameObject cell = GameObject.Instantiate(cellPrefab_);
        cell.transform.position = position;
        cell.GetComponent<NetworkCell>().InitializeServer(id, initialHelath, maxHealth, pulseValue, team);
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
