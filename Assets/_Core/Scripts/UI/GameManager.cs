using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private RepairGameMode _gameMode = null;

	[Header("Popups")]
	[SerializeField]
    private Image _endScreen = null;

	[SerializeField]
    private Image _winScreen = null;

    private AudioSource _audio;
    public AudioClip winSFX;
    public AudioClip LoseSFX;

	public int CurrentExcelation
	{
		get; private set;
	}

	public int CurrentWrench
	{
		get; private set;
	}

    protected void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _endScreen.gameObject.SetActive(false);
        _winScreen.gameObject.SetActive(false);
    }

	protected void Start()
	{
		_gameMode.GameModeEndedEvent += OnGameModeEndedEvent;
		_gameMode.StartGameMode(new RepairModeSettings(5, 3));
	}

	protected void OnDestroy()
	{
		_gameMode.GameModeEndedEvent -= OnGameModeEndedEvent;
		_gameMode.StopGameMode();
	}

	private void OnGameModeEndedEvent(bool win)
	{
		if(win)
		{
			_winScreen.gameObject.SetActive(true);
			_audio.PlayOneShot(winSFX);
			Time.timeScale = 0f;
		}
		else
		{
			_endScreen.gameObject.SetActive(true);
			_audio.PlayOneShot(LoseSFX);
			Time.timeScale = 0f;
		}

		_gameMode.StopGameMode();
	}
}
