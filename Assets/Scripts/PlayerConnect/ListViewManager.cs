using UnityEngine;
using Unity.Netcode;
using TMPro; // Nếu bạn sử dụng TextMeshPro

public class ListViewManager : MonoBehaviour
{


    //[SerializeField] private TMP_Text txtPlayers; // TMP_Text để hiển thị danh sách người chơi
    //private string connectedPlayers = ""; // Chuỗi lưu danh sách người chơi

    //private void Start()
    //{
    //    // Đăng ký callback khi client kết nối/ngắt kết nối
    //    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //    NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    //}

    //private void OnDestroy()
    //{
    //    // Hủy đăng ký callback để tránh lỗi khi script bị hủy
    //    if (NetworkManager.Singleton != null)
    //    {
    //        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    //    }
    //}

    //// Khi một client kết nối
    //private void OnClientConnected(ulong clientId)
    //{
    //    Debug.Log($"Client connected: {clientId}");
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        Debug.Log("Server is adding player to list...");
    //        connectedPlayers += $"Player{clientId}\n";

    //        // Chỉ gọi ClientRpc khi đã có ít nhất 1 client
    //        if (NetworkManager.Singleton.ConnectedClients.Count > 0)
    //        {
    //            UpdatePlayerListClientRpc(connectedPlayers);
    //        }
    //    }
    //}

    //// Khi một client ngắt kết nối
    //private void OnClientDisconnected(ulong clientId)
    //{
    //    Debug.LogWarning("Client failed to connect!");
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        // Xóa client khỏi danh sách
    //        connectedPlayers = connectedPlayers.Replace($"Player{clientId}\n", "");
    //        UpdatePlayerListClientRpc(connectedPlayers);
    //    }
    //}

    //// RPC để đồng bộ danh sách thiết bị với tất cả client
    //[ClientRpc]
    //private void UpdatePlayerListClientRpc(string playerList)
    //{
    //    connectedPlayers = playerList;

    //    if (txtPlayers != null)
    //    {
    //        txtPlayers.text = connectedPlayers;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("txtPlayers is not assigned!");
    //    }
    //}

    //// Khi nhấn nút StartHost
    //public void StartHost()
    //{
    //    NetworkManager.Singleton.StartHost();
    //    txtPlayers.text = "Starting Host...\n"; // Hiển thị trạng thái ban đầu
    //}

    //// Khi nhấn nút StartClient
    //public void StartClient()
    //{
    //    NetworkManager.Singleton.StartClient();
    //    txtPlayers.text = "Connecting to Host...\n"; // Hiển thị trạng thái kết nối
    //}
}
