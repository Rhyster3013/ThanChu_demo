using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   FunctionController functionController;
    FunctOnlineController functOnlineController;
    public Button BtnEnemyAction;

    public UnityEngine.UI.Button btnStartGame;

    void Start()
    {
        btnStartGame.onClick.AddListener(GameStart);

        if (!RoomSizeInstance.Instance.online)
        {
            BtnEnemyAction.gameObject.SetActive(true);
            functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();

            //functionController.roomSize = RoomSizeInstance.Instance.roomSize;
            functionController.MatchInitialize();
    }
        else
        {
            BtnEnemyAction.gameObject.SetActive(false);
            functOnlineController = GameObject.Find("GameManager").GetComponent<FunctOnlineController>();

            functOnlineController.roomSize = RoomSizeInstance.Instance.roomSize;
            functOnlineController.MatchInitialize();
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
            functOnlineController.GameStart();
            functOnlineController.ResetDeck();
        }
    }
}
