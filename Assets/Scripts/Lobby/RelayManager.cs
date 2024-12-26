using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;
using System;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    private Allocation relayAllocation; // Thông tin server Relay cho Host
    private JoinAllocation joinAllocation; // Thông tin kết nối Relay cho Client

    public Dictionary<string, ulong> playerIdToClientId = new Dictionary<string, ulong>();


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


    #region Create and Join

    public async Task<string> CreateLobbyWithRelay(string lobbyName, int maxPlayers)
    {
        try
        {
            // Create Lobby
            CreateLobbyOptions playerName = new CreateLobbyOptions()
            {
                Player = GetPlayer("Rhyster")
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, playerName);
            LobbyInstance.Instance.LobbyID = lobby.Id;
            LobbyInstance.Instance.PlayerName = "Rhyster";

            string message = ("Lobby ID: " + lobby.Id + ", JoinCode: " + lobby.LobbyCode);
            Debug.Log(message);
            LobbyInstance.Instance.Message = message;

            // Create Relay Server
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Relay Join Code: {relayJoinCode}");

            // Save Relay Joincode into Lobby.Data
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
            });

            // Format Unity Transport and start Host
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started");
            StartServerOrHost();
            LobbyInstance.Instance.PlayerLobbyID = AuthenticationService.Instance.PlayerId;

            return lobby.LobbyCode; // Trả về Lobby Join Code
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating lobby and relay: {e.Message}");
            return null;
        }
    }

    public async Task JoinLobbyAndRelay(string lobbyJoinCode, string playerName)
    {
        try
        {
            string rawJoinCode = lobbyJoinCode;
            string sanitizedJoinCode = SanitizeJoinCode(rawJoinCode);

            string name = playerName;
            name ??= "Player00";

            //if (!await IsLobbyFullAsync(lobbyJoinCode))
            //{
            // Join Lobby
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
                {
                    Player = GetPlayer(name)
                };
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(sanitizedJoinCode, options);
                LobbyInstance.Instance.LobbyID = lobby.Id;
            LobbyInstance.Instance.PlayerName = name;

            Debug.Log(sanitizedJoinCode);
            string message = ($"Found Lobby: {lobby.Name} with id: {LobbyInstance.Instance.LobbyID}");
                Debug.Log(message);
                LobbyInstance.Instance.Message = message;

                // Get Relay Join Code from Lobby.Data
                if (lobby.Data.TryGetValue("RelayJoinCode", out DataObject relayData))
                {
                    string relayJoinCode = SanitizeJoinCode(relayData.Value);
                    Debug.Log($"Relay Join Code retrieved: {relayJoinCode}");

                    if (string.IsNullOrEmpty(relayJoinCode))
                    {
                        Debug.LogError("Relay Join Code is invalid or empty.");
                        return;
                    }

                // Connect to Relay Server
                joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                Debug.Log("Successfully joined relay.");

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                    // Start Netcode Client
                    NetworkManager.Singleton.StartClient();
                StartServerOrHost();

                if (!NetworkManager.Singleton.IsClient)
                {
                    Debug.LogError("Client failed to connect to the server.");
                }

                LobbyInstance.Instance.PlayerLobbyID = AuthenticationService.Instance.PlayerId;

                }
                else
                {
                    Debug.LogError("Relay Join Code not found in lobby.");
                }
            //}
            //else
            //{
            //    Debug.Log($"Lobby is full");
            //    LobbyInstance.Instance.Message = ("Lobby is full");
            //}
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error joining lobby and relay: {e.Message}");
        }
    }

    #endregion


    #region Netcode Rpc

    public ulong GetClientIdFromPlayerId(string playerId)
    {
        if (playerIdToClientId.TryGetValue(playerId, out ulong clientId))
        {
            Debug.Log($"{clientId} mapped from PlayerId: {playerId}");
            return clientId;
        }
        else
        {
            Debug.LogError($"No ClientId found for PlayerId: {playerId}");
            return ulong.MaxValue; // Trả về giá trị mặc định, cần xử lý tốt hơn tùy trường hợp.
        }
    }

    public void MapPlayerIdToClientId(string playerId, ulong clientId)
    {
        try
        {
            if (!playerIdToClientId.ContainsKey(playerId))
            {
                playerIdToClientId[playerId] = clientId;
                Debug.Log("Player " + playerId + " mapped for clientID " + clientId);
            }
            else
            {
                Debug.LogWarning($"ClientId not found for PlayerId: {playerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    #endregion


    #region Call back

    public void StartServerOrHost()
    {
        if (NetworkManager.Singleton != null)
        {
            // Bắt đầu host/server
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started.");
            }
            else if (NetworkManager.Singleton.StartServer())
            {
                Debug.Log("Server started.");
            }

            // Đăng ký callback
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private async void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        if (NetworkManager.Singleton.IsHost)
        {
            await AddClientIdToPlayer(AuthenticationService.Instance.PlayerId, "0");
        }
        else
        {
            await AddClientIdToPlayer(AuthenticationService.Instance.PlayerId, clientId.ToString());
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    #endregion


    #region Other


    public async Task AddClientIdToPlayer(string playerId, string clientId)
    {
        try
        {
            if (LobbyInstance.Instance.LobbyID == null)
            {
                Debug.LogError("Lobby is null. Cannot update player data.");
                return;
            }

            await LobbyService.Instance.UpdatePlayerAsync(
                LobbyInstance.Instance.LobbyID,
                playerId,
                new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                    { "ClientId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, clientId) }
                    }
                }
            );

            Debug.Log($"Successfully added ClientId for Player {playerId}: {clientId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add ClientId: {e.Message}");
        }
    }

    // Kiểm tra xem lobbyManager đã đầy chưa
    public async Task<bool> IsLobbyFullAsync(string joinCode)
    {
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinCode);
        if (lobby != null)
        {
            return lobby.MaxPlayers == lobby.Players.Count;
        }
        return false;
    }

    public Player GetPlayer(string playerName)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }

    private string SanitizeJoinCode(string joinCode)
    {
        return joinCode.Trim().Replace("\u200B", ""); // Xóa Zero Width Space
    }

    #endregion


    #region Unused code

    //// 1. Tạo Relay server và bắt đầu Netcode Host
    //public async Task<string> CreateRelayServer(int maxPlayers)
    //{
    //    try
    //    {
    //        // Tạo server Relay với số người chơi tối đa
    //        relayAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
    //        string joinCode = await RelayService.Instance.GetJoinCodeAsync(relayAllocation.AllocationId);
    //        Debug.Log($"Relay server created with Join Code: {joinCode}");

    //        // Cấu hình Unity Transport với Relay Server
    //        RelayServerData relayServerData = new RelayServerData(relayAllocation, "dtls");
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

    //        // Khởi động Netcode Host
    //        NetworkManager.Singleton.StartHost();
    //        Debug.Log("Netcode Host started!");

    //        return joinCode; // Trả về Join Code cho UI
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Error creating relay server: {e.Message}");
    //        return null;
    //    }
    //}

    //// 2. Kết nối Client tới Relay server và bắt đầu Netcode Client
    //public async Task ConnectToRelayServer(string joinCode)
    //{
    //    try
    //    {
    //        // Kết nối tới Relay server thông qua Join Code
    //        joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
    //        Debug.Log("Connected to Relay server!");

    //        // Cấu hình Unity Transport với Relay Server
    //        RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

    //        // Khởi động Netcode Client
    //        NetworkManager.Singleton.StartClient();
    //        Debug.Log("Netcode Client started!");
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Error connecting to relay server: {e.Message}");
    //    }
    //}

    #endregion
}
