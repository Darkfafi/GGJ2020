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

	[SerializeField]
	private Vector2 _offsetOutScreenIndication = Vector2.zero;

	private Transform _currentTarget;

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

			if (viewportPos.z < 0f)
			{
				viewportPos *= -1f;
			}

			Vector2 finalPos = ViewportToCanvasPosition(viewportPos);

			float canvasX = (CanvasSize.x / 2f) - _offsetOutScreenIndication.x;
			float canvasY = (CanvasSize.y / 2f) - _offsetOutScreenIndication.y;

			finalPos.x = Mathf.Clamp(finalPos.x, -canvasX, canvasX);
			finalPos.y = Mathf.Clamp(finalPos.y, -canvasY, canvasY);

			finalPos.x = Mathf.Lerp(RectTransform.anchoredPosition.x, finalPos.x, Time.deltaTime * 5f);
			finalPos.y = Mathf.Lerp(RectTransform.anchoredPosition.y, finalPos.y, Time.deltaTime * 5f);

			RectTransform.anchoredPosition = finalPos;
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
