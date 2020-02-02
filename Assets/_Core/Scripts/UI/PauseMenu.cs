using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

	[SerializeField]
	private Image _pauseMenu;

	[SerializeField]
	private Image _endScreen;

	[SerializeField]
	private Image _winScreen;

    private AudioSource _audio;
    public AudioClip winSFX;
    public AudioClip LoseSFX;

    private void Awake()
    {
		_pauseMenu.gameObject.SetActive(false);
        _audio = GetComponent<AudioSource>();

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
        if (_pauseMenu.gameObject.activeSelf || _endScreen.gameObject.activeSelf || _winScreen.gameObject.activeSelf)
        {
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
            if (_endScreen.gameObject.activeSelf)
            {
                _audio.PlayOneShot(LoseSFX);
            }
            if (_winScreen.gameObject.activeSelf)
            {
                _audio.PlayOneShot(winSFX);
            }
        }
    }

    public void Resume()
    {
		_pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
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
