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

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    private Allocation relayAllocation; // Thông tin server Relay cho Host
    private JoinAllocation joinAllocation; // Thông tin kết nối Relay cho Client

    private Dictionary<string, ulong> playerIdToClientId = new Dictionary<string, ulong>();


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

            string message = ("Lobby created with ID: " + lobby.Id + " and join code: " + lobby.LobbyCode);
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
            LobbyInstance.Instance.PlayerLobbyID = AuthenticationService.Instance.PlayerId;

            MapPlayerIdToClientId(AuthenticationService.Instance.PlayerId, NetworkManager.Singleton.LocalClientId);
            Debug.Log("Netcode Host started.");

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
                LobbyInstance.Instance.PlayerLobbyID = AuthenticationService.Instance.PlayerId;
                MapPlayerIdToClientId(AuthenticationService.Instance.PlayerId, NetworkManager.Singleton.LocalClientId);
                Debug.Log("Netcode Client started.");
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
            return clientId;
        }

        Debug.LogError($"No ClientId found for PlayerId: {playerId}");
        return 0; // Trả về giá trị mặc định, cần xử lý tốt hơn tùy trường hợp.
    }

    public void MapPlayerIdToClientId(string playerId, ulong clientId)
    {
        try
        {
            if (!playerIdToClientId.ContainsKey(playerId))
            {
                playerIdToClientId[playerId] = clientId;
            }
            Debug.Log("Player " + playerId + " mapped for clientID " + clientId);
        }catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    #endregion


    #region Other


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
