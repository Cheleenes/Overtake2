using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;
using MLAPI.NetworkVariable;
using MLAPI.Transports;
using System.IO;
using MLAPI;

public class NetworkTeam : INetworkSerializable
{
    int teamId_;
    List<NetworkPlayer> teamPlayers_;
    List<int> teamCells_;
    int[] teamCellsArray_;
    public int TeamId_ { get => teamId_; }

    public ushort RemoteTick => throw new System.NotImplementedException();

    public int[] TeamCellsArray_ { get => teamCellsArray_; set => teamCellsArray_ = value; }

    public NetworkTeam(int teamId)
    {
        teamId_ = teamId;
        teamPlayers_ = new List<NetworkPlayer>();
        teamCells_ = new List<int>();
        teamCellsArray_ = teamCells_.ToArray();
    }
    public NetworkTeam()
    {
        teamId_ = 1;
        teamPlayers_ = new List<NetworkPlayer>();
        teamCells_ = new List<int>();
        teamCellsArray_ = teamCells_.ToArray();
    }

    public void AddCell(GameObject cellGO)
    {
        teamCells_.Add(cellGO.GetComponent<NetworkCell>().CellId_);
        teamCellsArray_ = teamCells_.ToArray();
    }

    public void RemoveCell(GameObject cellGO)
    {
        teamCells_.Remove(cellGO.GetComponent<NetworkCell>().CellId_);
        teamCellsArray_ = teamCells_.ToArray();
    }

    public bool HasCell_(NetworkCell cell)
    {
        for(int i = 0; i < teamCellsArray_.Length; i++)
        {
            if (teamCellsArray_[i] == cell.CellId_)
                return true;
        }
        return false;
    }

    public void AddTeamate(NetworkPlayer player)
    {
        teamPlayers_.Add(player);
    }
    

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref teamId_);
        serializer.Serialize(ref teamCellsArray_);
    }
}
