using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;



public class PauseMenu : MonoBehaviour
{ 
    private bool GameIsPaused = false;
    public Image pauseMenuUI;

    private void Awake()
    {
        pauseMenuUI = GameObject.Find("PauseMenu").GetComponent<Image>();
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
        if (pauseMenuUI.gameObject.activeSelf)
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
