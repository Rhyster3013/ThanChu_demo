using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPickManager : NetworkBehaviour
{
    [Header("References")]
    public GameObject playerInfoPrefab;
    public Transform playerInfoContainer;

    public Button btnReady;

    private List<int> availableOrders = new List<int>();
    private LobbyManager lobbyManager;

    private NetworkList<PlayerInfo> playerList;

    private void Awake()
    {
        playerList = new NetworkList<PlayerInfo>();
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Khởi tạo thứ tự chơi khi có đủ người chơi
            InitializePlayOrder();
            SpawnPlayerLobbyInfoObjects();
        }
        //lobbyManager = GetComponent<LobbyManager>();
        //playerList.Add(await lobbyManager.GetPlayerInfo(NetworkManager.Singleton.LocalClientId));

        //btnReady.onClick.AddListener(ShowList);
    }

    private void InitializePlayOrder()
    {
        // Xóa danh sách cũ nếu có
        availableOrders.Clear();

        // Thêm thứ tự từ 1 đến n (n = số người chơi trong phòng)
        int totalPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
        for (int i = 1; i <= totalPlayers; i++)
        {
            availableOrders.Add(i);
        }

        // Xáo trộn danh sách để tạo thứ tự ngẫu nhiên
        ShuffleList(availableOrders);
    }

    private void SpawnPlayerLobbyInfoObjects()
    {
        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log($"Spawning player for Client ID: {client.ClientId}, PlayOrder: {availableOrders[index]}");

            // Tạo GameObject cho từng người chơi
            GameObject playerInfoGO = Instantiate(playerInfoPrefab, playerInfoContainer);
            playerInfoGO.name = "Player" + availableOrders[index];

            var networkObject = playerInfoGO.GetComponent<NetworkObject>();
            //networkObject.SpawnWithOwnership(client.ClientId);

            // Gửi lệnh để gắn GameObject vào playerInfoContainer trên các client
            SetParentClientRpc(playerInfoGO.GetComponent<NetworkObject>().NetworkObjectId);

            //var playerLobbyInfo = playerInfoGO.GetComponent<PlayerReady>();
            //if (playerLobbyInfo != null)
            //{
            //    playerLobbyInfo.SetPlayerInfoServerRpc(LobbyInstance.Instance.PlayerName, false, availableOrders[index]);
            //    Debug.Log($"Player Info Set for Client {client.ClientId}");
            //}
            //else
            //{
            //    Debug.LogError("PlayerReady script is missing on prefab!");
            //}

            index++;
        }
    }

    [ClientRpc]
    private void SetParentClientRpc(ulong networkObjectId)
    {
        // Tìm NetworkObject bằng ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            // Gắn vào playerInfoContainer
            networkObject.transform.SetParent(playerInfoContainer, false);
        }
        else
        {
            Debug.LogError($"NetworkObject with ID {networkObjectId} not found.");
        }
    }


    public void ShowList()
    {
        Debug.Log(playerList);
    }

    private void ShuffleList(List<int> list)
    {
        // Xáo trộn danh sách bằng thuật toán Fisher-Yates
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

}
