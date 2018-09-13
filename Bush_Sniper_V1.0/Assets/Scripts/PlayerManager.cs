using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// this class keeps track of all the players connected
public class PlayerManager : MonoBehaviour
{
    public MatchSettings matchSettings;

    private static PlayerManager instance;
    public Dictionary<string, Player> ConnectedPlayers { get; set; }

    public int NumberConnectedPlayers { get; private set; }

    public string PlayerID { get; private set; }

    private PlayerManager()
    {
        if (instance != null)
        {
            return;
        }

        ConnectedPlayers = new Dictionary<string, Player>();
        NumberConnectedPlayers = 0;

        instance = this;
    }

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerManager();
            }

            return instance;
        }
    }

    public void AddPlayerToConnectedPlayers(string _playerID, Player _playerObject)
    {
        if (!ConnectedPlayers.ContainsKey(_playerID))
        {
            ConnectedPlayers.Add(_playerID, _playerObject);
            NumberConnectedPlayers++;
        }
    }

    public void RemovePlayerFromConnectedPlayers(string _playerID)
    {
        if (ConnectedPlayers.ContainsKey(_playerID))
        {
            ConnectedPlayers.Remove(_playerID);
            NumberConnectedPlayers--;
        }
    }

    public Player[] GetConnectedPlayers()
    {
        return ConnectedPlayers.Values.ToArray();
    }

    public void SetLocalPlayerID(string _playerID)
    {
        PlayerID = _playerID;
    }

    public Player GetPlayerFromConnectedPlayers(string _playerID)
    {
        if (ConnectedPlayers.ContainsKey(_playerID))
        {
            return ConnectedPlayers[_playerID];
        }

        return null;
    }
}