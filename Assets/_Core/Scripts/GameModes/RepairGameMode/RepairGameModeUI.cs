using UnityEngine;
using UnityEngine.UI;

public class RepairGameModeUI : MonoBehaviour
{
	[Header("HUD")]
	[SerializeField]
	private Image[] _exclamationHUDIcons = null;

	[SerializeField]
	private Image[] _wrenchesHUDIcons = null;

	[Header("Requirements")]
	[SerializeField]
	private RepairGameMode _gameMode = null;

	[SerializeField]
	private AlertIconCreator _alertIconCreator = null;

	[SerializeField]
	private Canvas _gameCanvas = null;

	[Header("Resources")]
	[SerializeField]
	private FlyingIcon _alertIconPrefab = null;

	[SerializeField]
	private FlyingIcon _wrenchIconPrefab = null;

	protected void Awake()
	{
		_gameMode.RepairIncreasedEvent += OnRepairIncreasedEvent;
		_gameMode.ShockIncreasedEvent += OnShockIncreeasedEvent;
		_gameMode.GameModeEndedEvent += OnGameModeEndedEvent;
	}

	protected void OnDestroy()
	{
		DeInit();
	}

	private void DeInit()
	{
		_gameMode.RepairIncreasedEvent -= OnRepairIncreasedEvent;
		_gameMode.ShockIncreasedEvent -= OnShockIncreeasedEvent;
		_gameMode.GameModeEndedEvent -= OnGameModeEndedEvent;
	}

	private void OnGameModeEndedEvent(bool win)
	{
		DeInit();
	}

	private void OnShockIncreeasedEvent(int count, NPC source)
	{
		if (_exclamationHUDIcons != null)
		{
			for (int i = 0; i < count; i++)
			{
				if (i >= _exclamationHUDIcons.Length)
					break;

				Image image = _exclamationHUDIcons[i];
				if (image.gameObject.activeSelf)
				{
					continue;
				}

				FlyingIcon icon = Instantiate(_alertIconPrefab, _gameCanvas.transform);
				if (!icon.PositionOverWorldPosition(source.transform.position))
				{
					if (_alertIconCreator.TryGetScreenIconFor(source, out ScreenIcon screenIcon))
					{
						icon.transform.position = screenIcon.transform.position;
					}
				}

				icon.FlyIconTo(image.rectTransform.position, 1f, (x) =>
				{
					Destroy(x.gameObject);
					image.gameObject.SetActive(true);
				});
			}
		}
	}

	private void OnRepairIncreasedEvent(int count, Breakable source)
	{
		if (_wrenchesHUDIcons != null)
		{
			for(int i = 0; i < count; i++)
			{
				if (i >= _wrenchesHUDIcons.Length)
					break;

				Image image = _wrenchesHUDIcons[i];
				if (image.gameObject.activeSelf)
				{
					continue;
				}

				FlyingIcon icon = Instantiate(_wrenchIconPrefab, _gameCanvas.transform);
				icon.PositionOverWorldPosition(source.transform.position);
				icon.FlyIconTo(image.rectTransform.position, 1f, (x) =>
				{
					Destroy(x.gameObject);
					image.gameObject.SetActive(true);
				});
			}
		}
	}
}
