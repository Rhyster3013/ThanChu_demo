using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : NetworkBehaviour
{
    public static LobbySceneManager Instance;

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

    // Hàm Host gọi khi nhấn StartGame
    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc()
    {
        Debug.Log("Host is loading next scene...");
        LoadPickCharacterSceneClientRpc();
    }

    // ClientRpc gửi lệnh chuyển scene tới tất cả các client
    [ClientRpc]
    private void LoadPickCharacterSceneClientRpc()
    {
        Debug.Log("Loading PickCharacter scene...");
        NetworkManager.Singleton.SceneManager.LoadScene("PickCharacter", LoadSceneMode.Single);
    }

    // Hàm Host gọi khi nhấn StartGame
    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        Debug.Log("Host is starting the game...");
        LoadGameStartClientRpc();
    }

    // ClientRpc gửi lệnh chuyển scene tới tất cả các client
    [ClientRpc]
    private void LoadGameStartClientRpc()
    {
        Debug.Log("Loading GameMatch scene...");
        NetworkManager.Singleton.SceneManager.LoadScene("GameMatch", LoadSceneMode.Single);
    }
}
