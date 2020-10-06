using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().onEndLevel += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOver){
            if(Input.GetKeyDown(KeyCode.Space)){
                SceneManager.LoadScene(0);
            }
            
        }
    }

    void ShowGameWinUI(){
        OnGameOver(gameWinUI);
    }

    void ShowGameLoseUI(){
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI){
        gameOverUI.SetActive(true);
        gameOver = true;
        Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().onEndLevel -= ShowGameWinUI;
    }
}
