using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class IndicationArrow : MonoBehaviour
{

	[SerializeField]
	private GameObject _iconContainer;

	private Camera _camera;
	private Canvas _canvas;
	private RectTransform _canvasRect;
	private RectTransform _rectTransform;
	private Transform _relativeOrigin;

	private Transform _currentTarget;
	private Vector2 _uiOffset;

	protected void Awake()
	{
		_camera = Camera.main;
		_canvas = FindObjectOfType<Canvas>();
		_canvasRect = _canvas.GetComponent<RectTransform>();
		_relativeOrigin = FindObjectOfType<PlayerMovement>().transform;

		_rectTransform = GetComponent<RectTransform>();
		_uiOffset = new Vector2(_canvasRect.sizeDelta.x / 2f, _canvasRect.sizeDelta.y / 2f);
	}

	protected void Update()
	{
		if (_currentTarget != null)
		{
			Vector2 viewportPos = _camera.WorldToViewportPoint(_currentTarget.transform.position);
			if ((viewportPos.x >= 0 && viewportPos.x <= 1) && (viewportPos.y >= 0 && viewportPos.y <= 1))
			{
				_rectTransform.anchoredPosition = Vector2.zero;
				_iconContainer.SetActive(false);
				return;
			}
			else
			{
				_iconContainer.SetActive(true);
			}

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
