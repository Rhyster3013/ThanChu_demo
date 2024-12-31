using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    FunctionController functionController;
    FunctOnlineController functOnlineController;
    public Button BtnEnemyAction;

    public Button btnStartGame;
    public Button btnInit;

    public override void OnNetworkSpawn()
    {

        if (!RoomSizeInstance.Instance.online)
        {
            btnStartGame.gameObject.SetActive(true);
            btnStartGame.onClick.AddListener(GameStart);

            BtnEnemyAction.gameObject.SetActive(true);
            functionController = GetComponent<FunctionController>();

            //functionController.roomSize = RoomSizeInstance.Instance.roomSize;
            functionController.MatchInitialize();
        }
        else
        {
            functOnlineController = GetComponent<FunctOnlineController>();
            if (IsHost)
            {
                btnStartGame.gameObject.SetActive(true);
                btnStartGame.onClick.AddListener(GameStart);

                btnInit.gameObject.SetActive(true);
                btnInit.onClick.AddListener(GameInit);
            }

            BtnEnemyAction.gameObject.SetActive(false);

            functOnlineController.roomSize = RoomSizeInstance.Instance.roomSize;


            if (IsServer)
            {
                Debug.Log("FunctOnlineController spawned on server.");
            }
            else if (IsClient)
            {
                Debug.Log("FunctOnlineController spawned on client.");
            }
        }
    }

    private void GameStart()
    {
        btnStartGame.gameObject.SetActive(false);

        if (!RoomSizeInstance.Instance.online)
        {
            functionController.GameStart(); 
            functionController.ResetDeck();
        }
        else
        {
            functOnlineController.GameStartServerRpc();
            //functOnlineController.ResetDeck();
        }
    }

    private void GameInit()
    {
        btnInit.gameObject.SetActive(false);

        functOnlineController.MatchInitializeServerRpc();
    }
}
