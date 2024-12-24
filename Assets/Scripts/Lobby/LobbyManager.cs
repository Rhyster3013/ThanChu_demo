using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Authentication;
using System;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Threading.Tasks;


public class LobbyManager : MonoBehaviour
{
    // Tham chiếu tới thông tin lobbyManager
    private Lobby hostLobby;
    private Lobby joinedLobby;

    private List<PlayerInfo> playerList = new List<PlayerInfo>();
    private float hbTimer;

    #region Keep the server up

    private void Update()
    {
        HandleLobbyHB();
        //HandlePoolForLobby();
        //HandlRefresh();
    }

    private async void HandlRefresh()
    {
        try
        {
            if (!string.IsNullOrEmpty(LobbyInstance.Instance.LobbyID))
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(LobbyInstance.Instance.LobbyID);
                joinedLobby = lobby;
                Debug.Log("Lobby refreshed successfully. Id is: " + joinedLobby.Id);
            }
            else
            {
                Debug.LogError("Lobby ID is null or empty!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error refreshing lobby: {e.Message}");
        }
    }

    //private async void HandlRefresh()
    //{
    //    rfTimer -= Time.deltaTime;
    //    if (rfTimer < 0f)
    //    {
    //        float hbMax = 5f;
    //        rfTimer = hbMax;

    //        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(LobbyInstance.Instance.LobbyID);
    //        joinedLobby = lobby;
    //    }
    //}

    private async void HandleLobbyHB()
    {
        if (hostLobby != null)
        {
            hbTimer -= Time.deltaTime;
            if (hbTimer < 0f)
            {
                float hbMax = 15;
                hbTimer = hbMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    
    //private async void HandlePoolForLobby()
    //{
    //    if (joinedLobby != null)
    //    {
    //        updateTimer -= Time.deltaTime;
    //        if (updateTimer < 0f)
    //        {
    //            float hbMax = 1.1f;
    //            updateTimer = hbMax;

    //            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(LobbyInstance.Instance.CurrentLobby.Id);
    //            LobbyInstance.Instance.CurrentLobby = lobby;
    //        }
    //    }
    //}

    #endregion


    #region Create Lobby
    // Đảm bảo Unity Services đã được khởi tạo
    async void Start()
    {
        try
        {
            // Khởi tạo Unity Services
            await UnityServices.InitializeAsync();

            // Kiểm tra trạng thái đăng nhập trước khi đăng nhập lại
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // Đăng nhập nếu chưa đăng nhập
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in successfully.");
            }
            else
            {
                Debug.Log("Already signed in.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Authentication Error: {e.Message}");
        }
    }

    // Tạo lobbyManager mới
    public async void CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            Debug.Log("Lobby requested: " + lobbyName + " and maximum " + maxPlayers + " players");

            string lobbyJoinCode = await RelayManager.Instance.CreateLobbyWithRelay(lobbyName, maxPlayers); // Giới hạn 4 người chơi
            if (!string.IsNullOrEmpty(lobbyJoinCode))
            {
                string lobbyId = LobbyInstance.Instance.LobbyID;
                LobbyInstance.Instance.LobbyCode = lobbyJoinCode;

                LobbyInstance.Instance.Message = ("Lobby created with ID: " + lobbyId + " and join code: " + LobbyInstance.Instance.LobbyCode);
                Debug.Log("Lobby created with ID: " + lobbyId + " and join code: " + LobbyInstance.Instance.LobbyCode);

                SceneManager.LoadScene("JoinRoom");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating lobbyManager: " + e.Message);
        }
    }
    
    //public async void CreateLobby(string lobbyName, int maxPlayers)
    //{
    //    Debug.Log("Lobby requested: " + lobbyName + " and " + maxPlayers + " max players");
    //    try
    //    {
    //        CreateLobbyOptions options = new CreateLobbyOptions()
    //        {
    //            Player = GetPlayer("Rhyster")
    //        };
    //        // Tạo lobbyManager với tên và số người chơi tối đa
    //        var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
    //        hostLobby = lobby;
    //        joinedLobby = hostLobby;

    //        string joinCode = lobby.LobbyCode;

    //        LobbyInstance.Instance.LobbyCode = joinCode;
    //        LobbyInstance.Instance.LobbyID = lobby.Id;
    //        LobbyInstance.Instance.CurrentLobby = lobby;

    //        LobbyInstance.Instance.Message = ("Lobby created with ID: " + lobby.Id + " and join code: " + LobbyInstance.Instance.LobbyCode);
    //        Debug.Log("Lobby created with ID: " + lobby.Id + " and join code: " + LobbyInstance.Instance.LobbyCode);

    //        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
    //        {
    //            NetworkManager.Singleton.StartHost();
    //            Debug.Log("Netcode Host started!");
    //        }

    //        SceneManager.LoadScene("JoinRoom");
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError("Error creating lobbyManager: " + e.Message);
    //    }
    //}

    public async void DeleteLobby(string lobbyId)
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            Debug.Log("Lobby deleted successfully.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to delete lobby: {e.Message}");
        }
    }

    #endregion


    #region Join Lobby

    public async void JoinLobby(string joinCode, string playerName)
    {
        try
        {
            if (!string.IsNullOrEmpty(joinCode))
            {
                await RelayManager.Instance.JoinLobbyAndRelay(joinCode, playerName);

                string lobbyId = LobbyInstance.Instance.LobbyID;

                Debug.Log($"Joined Lobby");
                LobbyInstance.Instance.Message = ($"Joined Lobby: {lobbyId} with name: {playerName}");
            }
            else
            {
                Debug.LogError("Lobby Join Code is empty!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error joining lobbyManager: {e.Message}");
        }
    }

    #endregion


    #region Display lobby

    public string PrintPlayers()
    {
        HandlRefresh();
        if (joinedLobby != null)
        {
            string players = "Players in Room " + joinedLobby.Name + " :\n";
            foreach (Player player in joinedLobby.Players)
            {
                players += player.Data["PlayerName"].Value + "\n";
            }

            return players;
        }
        else
            return "Lobby is null";
    }

    //public string PrintPlayers()
    //{
    //    try
    //    {
    //        if (joinedLobby != null)
    //            return PrintPlayers(joinedLobby);
    //        else
    //            return "Lobby is null";
    //    }
    //    catch (System.Exception e)
    //    {
    //        return ($"Error joining lobbyManager: {e.Message}");
    //    }
    //}

    #endregion


    #region Start Game

    public async void StartGame()
    {
        try
        {
            //HandlePoolForLobby();
            //hostLobby = LobbyInstance.Instance.CurrentLobby;

            HandlRefresh();
            hostLobby = joinedLobby;
            Debug.Log("Lobby refreshed before starting");

            if (hostLobby == null)
            {
                Debug.Log("Lobby is null!");
                return;
            }

            // Gán số thứ tự ngẫu nhiên từ 1 đến n cho từng người chơi
            List<int> orders = GenerateRandomOrder(hostLobby.Players.Count);
            int index = 0;
            Debug.Log("Player orders: " + orders.Count);

            //foreach (Player player in hostLobby.Players)
            //{
            //    await LobbyService.Instance.UpdatePlayerAsync(hostLobby.Id, player.Id, new UpdatePlayerOptions
            //    {
            //        Data = new Dictionary<string, PlayerDataObject>
            //        {
            //            { "Order", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, orders[index].ToString()) }
            //        }
            //    });

            //    index++;
            //}

            foreach (Player player in hostLobby.Players)
            {
                if (!playerIdToClientId.ContainsKey(player.Id))
                {
                    Debug.LogError($"No ClientId mapped for PlayerId: {player.Id}");
                    continue;
                }

                ulong clientId = GetClientIdFromPlayerId(player.Id);

                var clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { clientId }
                    }
                };

                UpdatePlayerOrderClientRpc(orders[index], clientRpcParams);
                index++;
            }

            Debug.Log("Assigned orders to all players."); 
            Debug.Log("IsServer: " + NetworkManager.Singleton.IsServer);
            Debug.Log("IsHost: " + NetworkManager.Singleton.IsHost);

            // Chuyển tất cả người chơi sang Scene PickCharacter
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

    private List<int> GenerateRandomOrder(int count)
    {
        List<int> orders = new List<int>();
        for (int i = 1; i <= count; i++) orders.Add(i);
        ShuffleList(orders);
        return orders;
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    #endregion

    #region

    private Dictionary<string, ulong> playerIdToClientId = new Dictionary<string, ulong>();

    public void MapPlayerIdToClientId(string playerId, ulong clientId)
    {
        if (!playerIdToClientId.ContainsKey(playerId))
        {
            playerIdToClientId[playerId] = clientId;
        }
    }

    public ulong GetClientIdFromPlayerId(string playerId)
    {
        if (playerIdToClientId.TryGetValue(playerId, out ulong clientId))
        {
            return clientId;
        }

        Debug.LogError($"No ClientId found for PlayerId: {playerId}");
        return 0; // Trả về giá trị mặc định, cần xử lý tốt hơn tùy trường hợp.
    }

    [ClientRpc]
    public void UpdatePlayerOrderClientRpc(int order, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"Received Order: {order}");

        // Cập nhật thứ tự của người chơi hiện tại
        if (!string.IsNullOrEmpty(LobbyInstance.Instance.PlayerLobbyID))
        {
            LobbyService.Instance.UpdatePlayerAsync(LobbyInstance.Instance.LobbyID, LobbyInstance.Instance.PlayerLobbyID, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                { "Order", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, order.ToString()) }
            }
            }).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"Successfully updated order to: {order}");
                }
                else
                {
                    Debug.LogError($"Failed to update order: {task.Exception?.Message}");
                }
            });
        }
    }

    #endregion


    #region Others

    public async Task<string> GetLobbyIdFromJoinCode(string joinCode)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinCode);

            Debug.Log($"Lobby ID for Join Code {joinCode}: {lobby.Id}");
            return lobby.Id;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Error fetching Lobby ID: {e.Message}");
            return null;
        }
    }

    public void AddPlayerToLobby(ulong clientId, string playerName)
    {
        PlayerInfo newPlayer = new PlayerInfo(clientId, playerName, 0);
        playerList.Add(newPlayer);
    }

    public async Task<PlayerInfo> GetPlayerInfo(ulong clientId)
    {
        HandlRefresh();
        if (joinedLobby == null || string.IsNullOrEmpty(joinedLobby.Id))
        {
            Debug.LogError("JoinedLobby is null or invalid.");
        }
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        Debug.Log(lobby.Players);

        PlayerInfo info = new PlayerInfo();
        foreach (var player in lobby.Players)
        {
            if (player.Id == LobbyInstance.Instance.PlayerLobbyID)
            {
                Debug.Log("Found Player: " + player);
                info.ClientId = clientId;
                info.PlayerName = player.Data["PlayerName"].Value;
                info.Order = int.Parse(player.Data["Order"].Value);
                Debug.Log("Id: " + info.ClientId + ", Name: " + info.PlayerName + ", Order: " + info.Order);

                break;
            }
        }

        return info;
    }


    #endregion
}


//public async string PrintPlayer(string code)
//{
//    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(LobbyInstance.Instance.LobbyID);
//    Debug.Log("join lobby: " + joinedLobby);
//    if (joinedLobby != null)
//        return PrintPlayers(joinedLobby);
//    else
//        return "Lobby is null";
//}

//public Transform playerListContainer; // GameObject chứa danh sách
//public GameObject playerPrefab; // Prefab cho từng người chơi

//private IEnumerator StartPolling(string lobbyId, Transform playerListContainer, GameObject playerPrefab, float interval = 5f)
//{
//    while (true)
//    {
//        FetchAndDisplayPlayers(lobbyId, playerListContainer, playerPrefab);
//        yield return new WaitForSeconds(interval);
//    }
//}

//public async void FetchAndDisplayPlayers(string lobbyId, Transform playerListContainer, GameObject playerPrefab)
//{
//    try
//    {
//        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

//        if (lobby.Data.ContainsKey("Players"))
//        {
//            string[] players = lobby.Data["Players"].Value.Split(',');

//            // Xóa danh sách cũ
//            foreach (Transform child in playerListContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            // Tạo danh sách mới
//            foreach (string playerName in players)
//            {
//                GameObject playerObj = Instantiate(playerPrefab, playerListContainer);
//                TMP_Text playerNameText = playerObj.GetComponent<TMP_Text>();
//                playerNameText.text = playerName;
//            }
//        }
//    }
//    catch (Exception e)
//    {
//        Debug.LogError($"Error fetching players: {e.Message}");
//    }
//}

//public async void AddPlayerToLobbyData(string lobbyId, string playerName)
//{
//    try
//    {
//        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

//        string currentPlayers = lobby.Data.ContainsKey("Players")
//            ? lobby.Data["Players"].Value
//            : "";

//        string updatedPlayers = string.IsNullOrEmpty(currentPlayers)
//            ? playerName
//            : $"{currentPlayers},{playerName}";

//        await LobbyService.Instance.UpdateLobbyAsync(
//            lobbyId,
//            new UpdateLobbyOptions
//            {
//                Data = new Dictionary<string, DataObject>
//                {
//                { "Players", new DataObject(DataObject.VisibilityOptions.Public, updatedPlayers) }
//                }
//            }
//        );
//    }
//    catch (Exception e)
//    {
//        Debug.LogError($"Error updating lobby data: {e.Message}");
//    }
//}

//public async void RemovePlayerFromLobbyData(string lobbyId, string playerName)
//{
//    try
//    {
//        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

//        if (lobby.Data.ContainsKey("Players"))
//        {
//            string currentPlayers = lobby.Data["Players"].Value;
//            string updatedPlayers = string.Join(",", currentPlayers.Split(',').Where(p => p != playerName));

//            await LobbyService.Instance.UpdateLobbyAsync(
//                lobbyId,
//                new UpdateLobbyOptions
//                {
//                    Data = new Dictionary<string, DataObject>
//                    {
//                    { "Players", new DataObject(DataObject.VisibilityOptions.Public, updatedPlayers) }
//                    }
//                }
//            );
//        }
//    }
//    catch (Exception e)
//    {
//        Debug.LogError($"Error removing player: {e.Message}");
//    }
//}
