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
            functionController.MatchInitialize();
    }
        else
        {
            BtnEnemyAction.gameObject.SetActive(false);
            functOnlineController = GameObject.Find("GameManager").GetComponent<FunctOnlineController>();
            functOnlineController.MatchInitialize();
        }
    }

    private void GameStart()
    {
        btnStartGame.gameObject.SetActive(false);
        functionController.GameStart();
    }
}
