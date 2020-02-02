using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenIcon : MonoBehaviour
{
	[SerializeField]
	private GameObject _iconContainer;

	[SerializeField]
	private bool _inScreenIndication = true;

	[SerializeField]
	private Vector2 _offsetInScreenIndication = Vector2.zero;

	private Camera _camera;
	private Canvas _canvas;
	private RectTransform _canvasRect;
	private RectTransform _rectTransform;
	private Transform _relativeOrigin;

	private Transform _currentTarget;

	protected void Awake()
	{
		_camera = Camera.main;
		_canvas = FindObjectOfType<Canvas>();
		_canvasRect = _canvas.GetComponent<RectTransform>();
		_relativeOrigin = FindObjectOfType<PlayerMovement>().transform;

		_rectTransform = GetComponent<RectTransform>();
	}

	protected void Update()
	{
		if (_currentTarget != null)
		{
			Vector2 viewportPos = _camera.WorldToViewportPoint(_currentTarget.transform.position);
			if((viewportPos.x >= 0 && viewportPos.x <= 1) && (viewportPos.y >= 0 && viewportPos.y <= 1))
			{
				if(_inScreenIndication)
				{
					Vector2 finalScreenPos = new Vector2((viewportPos.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
							(viewportPos.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)) + _offsetInScreenIndication;
					_rectTransform.anchoredPosition = finalScreenPos;
					return;
				}
				else
				{
					_rectTransform.anchoredPosition = Vector2.zero;
					_iconContainer.SetActive(false);
					return;
				}
			}

			_iconContainer.SetActive(true);
			Vector3 relativePosDir = (_relativeOrigin.transform.position - _currentTarget.transform.position).normalized;
			bool xLock = Mathf.Abs(relativePosDir.x) >= Mathf.Abs(relativePosDir.z);
			relativePosDir.x = xLock ? relativePosDir.x / Mathf.Abs(relativePosDir.x) : relativePosDir.x;
			relativePosDir.z = !xLock ? relativePosDir.z / Mathf.Abs(relativePosDir.z) : relativePosDir.z;
			relativePosDir.x = Mathf.Clamp(relativePosDir.x, -0.92f, 0.92f);
			relativePosDir.z = Mathf.Clamp(relativePosDir.z, -0.88f, 0.88f);
			Vector2 screenPos = new Vector2(_canvasRect.sizeDelta.x * relativePosDir.x, _canvasRect.sizeDelta.y * relativePosDir.z);
			screenPos.x = screenPos.x.Map(-_canvasRect.sizeDelta.x, _canvasRect.sizeDelta.x, -_canvasRect.sizeDelta.x / 2, _canvasRect.sizeDelta.x / 2);
			screenPos.y = screenPos.y.Map(-_canvasRect.sizeDelta.y, _canvasRect.sizeDelta.y, -_canvasRect.sizeDelta.y / 2, _canvasRect.sizeDelta.y / 2);
			screenPos.x = Mathf.Lerp(_rectTransform.anchoredPosition.x, screenPos.x, Time.deltaTime * 5f);
			screenPos.y = Mathf.Lerp(_rectTransform.anchoredPosition.y, screenPos.y, Time.deltaTime * 5f);
			_rectTransform.anchoredPosition = screenPos;
		}
	}

	protected void OnDestroy()
	{
		SetIndicationTarget(null);
	}

	public void SetIndicationTarget(Transform target)
	{
		_currentTarget = target;
	}
}
