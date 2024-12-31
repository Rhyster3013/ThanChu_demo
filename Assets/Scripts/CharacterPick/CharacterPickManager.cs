using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPickManager : NetworkBehaviour
{
    [Header("References")]
    public GameObject playerInfoPrefab;
    public Transform playerInfoContainer;

    public Button btnReady;
    public Button btnSend;
    public TextMeshProUGUI playerListText;

    private List<int> availableOrders = new List<int>();
    private LobbyManager lobbyManager;


    public override void OnNetworkSpawn()
    {
        lobbyManager = gameObject.GetComponent<LobbyManager>();
        btnSend.onClick.AddListener(UpdatePlayerListServerRpc);

        if (IsServer)
        {
            lobbyManager.AssignOrder();
            UpdatePlayerListServerRpc();
        }

        if (IsHost)
        {
            btnReady.gameObject.SetActive(true);
        }
        else if (IsClient)
        {
            btnReady.gameObject.SetActive(false);
            btnSend.gameObject.SetActive(false);
        }
        //lobbyManager = GetComponent<LobbyManager>();
        //playerList.Add(await lobbyManager.GetPlayerInfo(NetworkManager.Singleton.LocalClientId));

        btnReady.onClick.AddListener(StartGame);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerListServerRpc()
    {
        Debug.Log("Host is refreshing");
        UpdatePlayerListClientRpc(lobbyManager.UpdatePlayerListUI(), RoomSizeInstance.Instance.roomSize);
    }

    [ClientRpc]
    public void UpdatePlayerListClientRpc(string lobby, int roomSize)
    {
        Debug.Log("Client update");
        playerListText.text = lobby;
        RoomSizeInstance.Instance.roomSize = roomSize;
    }


    #region Game Ready


    public void StartGame()
    {
        try
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                LobbySceneManager.Instance.StartGameServerRpc();
            }
            else
            {
                Debug.LogWarning("Only the Host can start the game!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error starting game: " + e.Message);
        }
    }

    #endregion
}
