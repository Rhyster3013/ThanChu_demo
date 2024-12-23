using UnityEngine;

public class GameManager : MonoBehaviour
{
   FunctionController functionController;

    public UnityEngine.UI.Button btnStartGame;

    void Start()
    {
        btnStartGame.onClick.AddListener(GameStart);

        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();
        functionController.MatchInitialize();
    }

    private void GameStart()
    {
        btnStartGame.gameObject.SetActive(false);
        functionController.GameStart();
    }
}
