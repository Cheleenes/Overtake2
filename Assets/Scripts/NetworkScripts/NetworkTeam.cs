using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Serialization;

public class NetworkTeam : INetworkSerializable
{
    int teamId_;
    List<NetworkPlayer> teamPlayers_;
    [SerializeField] List<GameObject> teamCells;
    
    public int TeamId_ { get => teamId_; }

    public NetworkTeam(int teamId)
    {
        //gameObject.name = "Team " + teamId;
        teamId_ = teamId;
        teamPlayers_ = new List<NetworkPlayer>();
        teamCells = new List<GameObject>();       
    }
    public NetworkTeam()
    {
        //gameObject.name = "Team " + teamId;
        teamId_ = 1;
        teamPlayers_ = new List<NetworkPlayer>();
        teamCells = new List<GameObject>();
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

    public void AddTeamate(NetworkPlayer player)
    {
        teamPlayers_.Add(player);
    }
    

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref teamId_);
    }
}
