using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasItem : MonoBehaviour
{
	protected Canvas ParentCanvas
	{
		get; private set;
	}

	protected Vector2 CanvasSize
	{
		get
		{
			if(CanvasRectTransform == null)
			{
				return Vector2.zero;
			}

			return CanvasRectTransform.sizeDelta;
		}
	}

	protected RectTransform CanvasRectTransform
	{
		get; private set;
	}

	protected RectTransform RectTransform
	{
		get; private set;
	}

	protected Camera Camera
	{
		get; private set;
	}

	protected virtual void Awake()
	{
		Camera = Camera.main;
		RectTransform = GetComponent<RectTransform>();
		Transform currentCheck = transform.parent;
		while(currentCheck != null && ParentCanvas == null)
		{
			ParentCanvas = currentCheck.GetComponent<Canvas>();
			currentCheck = currentCheck.parent;
		}

		if(ParentCanvas != null)
		{
			CanvasRectTransform = ParentCanvas.GetComponent<RectTransform>();
		}
	}

	public bool PositionOverWorldPosition(Vector3 worldPos)
	{
		return PositionOverWorldPosition(worldPos, Vector2.zero);
	}

	public bool PositionOverWorldPosition(Vector3 worldPos, Vector2 iconOffset)
	{
		if(IsInViewport(worldPos, out Vector3 viewportPos))
		{
			RectTransform.anchoredPosition = ViewportToCanvasPosition(viewportPos);
			return true;
		}

		return false;
	}

	protected bool IsInViewport(Vector3 worldPosition, out Vector3 viewPortPos)
	{
		viewPortPos = Camera.WorldToViewportPoint(worldPosition);
		return viewPortPos.x >= 0 && viewPortPos.x <= 1 && viewPortPos.y >= 0 && viewPortPos.y <= 1;
	}

	protected Vector2 ViewportToCanvasPosition(Vector3 viewportPos)
	{
		return new Vector2((viewportPos.x * CanvasSize.x) - (CanvasSize.x * 0.5f), (viewportPos.y * CanvasSize.y) - (CanvasSize.y * 0.5f));
	}
}
