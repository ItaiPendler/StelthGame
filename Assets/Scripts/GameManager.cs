using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject GameWin;
    public GameObject GameLose;
    
    public static bool gameOver;

    private GameObject[] guards;
    private GameObject player;
    void Start()
    {
        Guard.OnGuardSpottedPlayer += ShowGameLose;
        Player.OnReachedWinPlace += ShowGameWin;
        guards = GameObject.FindGameObjectsWithTag("Guard");
        player = GameObject.FindWithTag("Player");
        
    }

    void Update()
    {
        //is the not game over?
        if (!gameOver)
        {
            //are there no guards left?
            if (Guard.CurrentGuardCount< 1)
            {
                //there are no more guards, we win!
                ShowGameWin();
            }
        }
        else
        {
            //the game is over, did we press R?
            if (Input.GetKey(KeyCode.R))
            {
                //re restart the game.
                ReloadScene();
            }
        }
    }

    private void ReloadScene()
    {
        gameOver = false;
        SceneManager.LoadScene(0);

    }

    private void ShowGameWin()
    {
        OnGameOver(GameWin);
    }

    private void ShowGameLose()
    {
        OnGameOver(GameLose);
    }

    
    void OnGameOver(GameObject gameOverUiElement)
    {
        gameOverUiElement.SetActive(true);
        gameOver = true;
        
        //releasing all the events. 
        Guard.OnGuardSpottedPlayer -= ShowGameLose;
        Player.OnReachedWinPlace -= ShowGameWin;
    }
}
