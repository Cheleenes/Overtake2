using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using UnityEngine.UI;

public class HelloWorldManager : NetworkBehaviour
{
    
    
    
    

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 600, 600));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            //StartButtons();
        }
        else
        {
            StatusLabels();

            //InitializeMapSetup();
        }
        
        //OnClientConnectedCallback.Invoke()
        GUILayout.EndArea();
    }

    
    static void StartButtons()
    {       
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
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

    
}
