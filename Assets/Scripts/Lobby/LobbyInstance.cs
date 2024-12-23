using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyInstance : MonoBehaviour
{
    public static LobbyInstance Instance { get; private set; }

    public string LobbyCode { get; set; }

    public string LobbyID { get; set; }

    public string PlayerLobbyID { get; set; }

    public string PlayerName { get; set; }

    public string Message { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
