using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;



public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;
    private Image pauseMenuUI;
    private Image endScreen;
    private Image winScreen;

    private void Awake()
    {
        pauseMenuUI = GameObject.Find("PauseMenu").GetComponent<Image>();
        endScreen = GameObject.Find("EndScreen").GetComponent<Image>();
        winScreen = GameObject.Find("WinScreen").GetComponent<Image>();
        pauseMenuUI.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (pauseMenuUI.gameObject.activeSelf || endScreen.gameObject.activeSelf || winScreen.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        pauseMenuUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Reset()
    {
        SceneManager.LoadScene("Game-Office");
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
