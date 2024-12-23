using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerConnect : MonoBehaviour
{
    public Button btnHost;
    public Button btnConnect;
    public Button btnExit;
    public Button btnStartGame;
    public Button btnRefresh;

    public TextMeshProUGUI lobbyID;
    public TextMeshProUGUI playerName;

    public TextMeshProUGUI txtMessage;
    public TextMeshProUGUI txtLobbyCode;
    public GameObject GOLobbyCode;

    public TextMeshProUGUI txtLobbyList;


    public LobbyManager lobbyManager;

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        btnHost.onClick.AddListener(() => {
            //NetworkManager.Singleton.StartHost();
            SceneManager.LoadScene("CreateRoom");
        });
        btnConnect.onClick.AddListener(ConnectLobby);

        btnExit.onClick.AddListener(ExitLobby);
        btnRefresh.onClick.AddListener(Refresh);
        btnStartGame.onClick.AddListener(lobbyManager.StartGame);

        HasHost(false, "");
        UpdateMessage("");

        string lobbyCode = LobbyInstance.Instance.LobbyCode;
        if (lobbyCode == null)
        {
            UpdateMessage("Lobby code is null");
        }
        else
        {
            Debug.Log($"Lobby Code in PlayerConnect: {LobbyInstance.Instance.LobbyCode}");
            HasHost(true, lobbyCode);
        }
    }

    #endregion


    #region Buttons

    void ConnectLobby()
    {
        if (string.IsNullOrEmpty(lobbyID.text))
        {
            Debug.LogError("Join Code is invalid or empty!");
            return;
        }

        try
        {
            lobbyManager.JoinLobby(lobbyID.text, playerName.text);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }

    }

    void ExitLobby()
    {
        lobbyManager.DeleteLobby(LobbyInstance.Instance.LobbyID);
        LobbyInstance.Instance.LobbyCode = null;
        LobbyInstance.Instance.Message = null;

        txtMessage.text = "Lobby deleted";
        HasHost(false, "");
    }

    private void Refresh()
    {
        UpdateLobbyList(lobbyManager.PrintPlayers());
        UpdateMessage(LobbyInstance.Instance.Message);
    }

    public void UpdateLobbyList(string message)
    {
        if (message != null)
        {
            txtLobbyList.text = message;
        }
        else
        {
            txtLobbyList.text = "Lobby unknown";
        }
    }

    public void UpdateMessage(string message)
    {
        if (message != null)
        {
            txtMessage.text = message;
        }
        else
        {
            txtMessage.text = "Message is unknown";
        }
    }

    #endregion


    #region Confirm

    void HasHost(bool hasHost, string lobbyCode)
    {
        if (hasHost)
        {
            Debug.Log($"Lobby Code to save: {lobbyCode}");
            GOLobbyCode.SetActive(true);
            txtLobbyCode.text = lobbyCode;

            UpdateLobbyList(lobbyManager.PrintPlayers());

            btnStartGame.gameObject.SetActive(true);
            btnExit.gameObject.SetActive(true);

            btnConnect.gameObject.SetActive(false);
            lobbyID.gameObject.SetActive(false);
            playerName.gameObject.SetActive(false);

            UpdateMessage(LobbyInstance.Instance.Message);
        }
        else
        {
            GOLobbyCode.SetActive(false);
            UpdateLobbyList("");

            btnStartGame.gameObject.SetActive(false);
            btnExit.gameObject.SetActive(false);

            btnConnect.gameObject.SetActive(true);
            lobbyID.gameObject.SetActive(true);
            playerName.gameObject.SetActive(true);
        }
    }

    #endregion
}



//var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
//Debug.Log("IP before " + transport.ConnectionData.Address + " and Port: " + transport.ConnectionData.Port);
//transport.ConnectionData.Address = "192.168.1.7";
//transport.ConnectionData.Port = 7777;

//if (NetworkManager.Singleton.StartClient())
//{
//    Debug.Log("Client started successfully.");
//}
//else
//{
//    Debug.LogError("Failed to start Client.");
//}
//Debug.Log("IP after " + transport.ConnectionData.Address + " and Port: " + transport.ConnectionData.Port);