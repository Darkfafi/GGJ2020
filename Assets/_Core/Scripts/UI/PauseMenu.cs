using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public BGMController bgmController;
    
    private bool GameIsPaused = false;

	[SerializeField]
	private Image _pauseMenu = null;

	[SerializeField]
	private Image _endScreen = null;

	[SerializeField]
	private Image _winScreen = null;


    private void Awake()
    {
		_pauseMenu.gameObject.SetActive(false);
	}

    void Update()
    {
		if(!_endScreen.gameObject.activeSelf && !_winScreen.gameObject.activeSelf)
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
		}

        if (_pauseMenu.gameObject.activeSelf || _endScreen.gameObject.activeSelf || _winScreen.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        }
    }

    public void Resume()
    {
        bgmController.Alerted = true;
		_pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        bgmController.Alerted = false;
		_pauseMenu.gameObject.SetActive(true);
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
