using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image[] exclamationMarks;
    public Image[] wrenches;

	[Header("Requirements")]
	[SerializeField]
	private AlertIconCreator _alertIconCreator;

	[Header("UI")]
	[SerializeField]
	private RepairTarget _repairTargetSource;

	[SerializeField]
	private Canvas _gameCanvas;

	[SerializeField]
	private FlyingIcon _alertIconPrefab;

	[SerializeField]
	private FlyingIcon _wrenchIconPrefab;

	[Header("Popups")]
	[SerializeField]
    private Image _endScreen;

	[SerializeField]
    private Image _winScreen;

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

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _endScreen.gameObject.SetActive(false);
        _winScreen.gameObject.SetActive(false);
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent += OnShock;
		_repairTargetSource.EndedRepairingBreakableEvent += OnEndedRepairingBreakableEvent;
		CurrentExcelation = 0;
		CurrentWrench = 0;
    }

    private void OnDestroy()
	{
		_repairTargetSource.EndedRepairingBreakableEvent -= OnEndedRepairingBreakableEvent;
		NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent -= OnShock;
    }

	private void OnEndedRepairingBreakableEvent(Breakable breakable)
	{
		if (breakable.BreakState == Breakable.State.Unbroken)
		{
			WrenchActivate(breakable);
		}
	}

	private void OnShock(NPC npc, Breakable breakableSeen)
    {
        ExclamationActivate(npc);
    }

	private void ExclamationActivate(NPC source)
	{
		if (exclamationMarks != null)
		{
			FlyingIcon icon = Instantiate(_alertIconPrefab, _gameCanvas.transform);
			if (!icon.PositionOverWorldPosition(source.transform.position))
			{
				if(_alertIconCreator.TryGetScreenIconFor(source, out ScreenIcon screenIcon))
				{
					icon.transform.position = screenIcon.transform.position;
				}
			}

			icon.FlyIconTo(exclamationMarks[CurrentExcelation].rectTransform.position, 1f, (x) =>
			{
				Destroy(x.gameObject);
				exclamationMarks[CurrentExcelation].gameObject.SetActive(true);
				CurrentExcelation++;
				if (CurrentExcelation > exclamationMarks.Length - 1)
				{
					exclamationMarks = null;
					GameOverScreen();
					CurrentExcelation = 0;
				}
			});
        }
    }

    private void WrenchActivate(Breakable source)
    {
        if (wrenches != null)
        {
			FlyingIcon icon = Instantiate(_wrenchIconPrefab, _gameCanvas.transform);
			icon.PositionOverWorldPosition(source.transform.position);
			icon.FlyIconTo(wrenches[CurrentWrench].rectTransform.position, 1f, (x) =>
			{
				Destroy(x.gameObject);
				wrenches[CurrentWrench].gameObject.SetActive(true);
				CurrentWrench++;
				if (CurrentWrench > wrenches.Length - 1)
				{
					wrenches = null;
					CurrentWrench = 0;
					WinScreen();
				}
			});
        }
    }

    public void GameOverScreen()
    {
        _endScreen.gameObject.SetActive(true);
        _audio.PlayOneShot(LoseSFX);
        Time.timeScale = 0f;
    }

    public void WinScreen()
    {
        _winScreen.gameObject.SetActive(true);
        _audio.PlayOneShot(winSFX);
        Time.timeScale = 0f;
    }
}
