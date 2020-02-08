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

	private Transform _currentTarget;

	protected void Update()
	{
		if (Camera != null && _currentTarget != null)
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
			Vector3 relativePosDir3D = (Camera.transform.position - _currentTarget.transform.position).normalized;
			Vector2 relativePosDir2D = new Vector2(relativePosDir3D.x, relativePosDir3D.z);

			Vector3 forward = Camera.transform.forward;
			forward = Vector3.Normalize(forward);
			Vector3 right = Quaternion.Euler(new Vector3(0f, 90f, 0f)) * forward;
			Vector3 relativeCameraRotation = (right + forward).normalized;

			float angle = 45f + (Mathf.Atan2(relativeCameraRotation.z, -relativeCameraRotation.x) + Mathf.Atan2(relativePosDir2D.y, relativePosDir2D.x)) * Mathf.Rad2Deg;

			relativePosDir2D.x = Mathf.Cos(angle * Mathf.Deg2Rad);
			relativePosDir2D.y = Mathf.Sin(angle * Mathf.Deg2Rad);

			bool xLock = Mathf.Abs(relativePosDir2D.x) >= Mathf.Abs(relativePosDir2D.y);
			relativePosDir2D.x = xLock ? relativePosDir2D.x / Mathf.Abs(relativePosDir2D.x) : relativePosDir2D.x;
			relativePosDir2D.y = !xLock ? relativePosDir2D.y / Mathf.Abs(relativePosDir2D.y) : relativePosDir2D.y;
			relativePosDir2D.x = Mathf.Clamp(relativePosDir2D.x, -0.92f, 0.92f);
			relativePosDir2D.y = Mathf.Clamp(relativePosDir2D.y, -0.88f, 0.88f);
			Vector2 screenPos = new Vector2(CanvasSize.x * relativePosDir2D.x, CanvasSize.y * relativePosDir2D.y);
			screenPos.x = screenPos.x.Map(-CanvasSize.x, CanvasSize.x, -CanvasSize.x / 2, CanvasSize.x / 2);
			screenPos.y = screenPos.y.Map(-CanvasSize.y, CanvasSize.y, -CanvasSize.y / 2, CanvasSize.y / 2);
			screenPos.x = Mathf.Lerp(RectTransform.anchoredPosition.x, screenPos.x, Time.deltaTime * 5f);
			screenPos.y = Mathf.Lerp(RectTransform.anchoredPosition.y, screenPos.y, Time.deltaTime * 5f);
			RectTransform.anchoredPosition = screenPos;
		}
		else
		{
			_iconContainer.SetActive(false);
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
