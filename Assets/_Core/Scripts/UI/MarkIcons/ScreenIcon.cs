using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenIcon : CanvasItem
{
	[SerializeField]
	private GameObject _iconContainer = null;

	[SerializeField]
	private bool _inScreenIndication = true;

	[SerializeField]
	private Vector2 _offsetInScreenIndication = Vector2.zero;

	private Transform _relativeOrigin;
	private Transform _currentTarget;

	protected override void Awake()
	{
		base.Awake();
		_relativeOrigin = FindObjectOfType<PlayerMovement>().transform;
	}

	protected void Update()
	{
		if (_currentTarget != null)
		{
			if(IsInViewport(_currentTarget.transform.position, out Vector3 viewportPos))
			{
				if(_inScreenIndication)
				{
					RectTransform.anchoredPosition = ViewportToCanvasPosition(viewportPos) + _offsetInScreenIndication;
					return;
				}
				else
				{
					RectTransform.anchoredPosition = Vector2.zero;
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
			Vector2 screenPos = new Vector2(CanvasSize.x * relativePosDir.x, CanvasSize.y * relativePosDir.z);
			screenPos.x = screenPos.x.Map(-CanvasSize.x, CanvasSize.x, -CanvasSize.x / 2, CanvasSize.x / 2);
			screenPos.y = screenPos.y.Map(-CanvasSize.y, CanvasSize.y, -CanvasSize.y / 2, CanvasSize.y / 2);
			screenPos.x = Mathf.Lerp(RectTransform.anchoredPosition.x, screenPos.x, Time.deltaTime * 5f);
			screenPos.y = Mathf.Lerp(RectTransform.anchoredPosition.y, screenPos.y, Time.deltaTime * 5f);
			RectTransform.anchoredPosition = screenPos;
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
