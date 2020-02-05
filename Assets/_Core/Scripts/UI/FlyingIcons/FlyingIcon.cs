using System;
using System.Collections;
using UnityEngine;

public class FlyingIcon : CanvasItem
{
	[SerializeField]
	private AnimationCurve _speedCurve = null;

	[SerializeField]
	private AnimationCurve _travelCurve = null;

	private Coroutine _flyAnimationRoutine;

	public void FlyIconTo(Vector2 targetOnScreen, float timeInSeconds, Action<FlyingIcon> onArrivalCallback = null)
	{
		if (_flyAnimationRoutine != null)
		{
			StopCoroutine(_flyAnimationRoutine);
			_flyAnimationRoutine = null;
		}

		if(timeInSeconds <= 0f)
		{
			RectTransform.position = targetOnScreen;
		}
		else
		{
			_flyAnimationRoutine = StartCoroutine(AnimationRoutine(targetOnScreen, timeInSeconds, onArrivalCallback));
		}
	}

	private IEnumerator AnimationRoutine(Vector2 targetOnCanvas, float timeInSeconds, Action<FlyingIcon> onArrivalCallback)
	{
		float timePassed = 0f;
		Vector2 startPos = RectTransform.position;
		Vector2 delta = targetOnCanvas - new Vector2(RectTransform.position.x, RectTransform.position.y);
		bool curveX = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
		while(timePassed < timeInSeconds)
		{
			timePassed = Mathf.Clamp(timePassed + Time.deltaTime, 0f, timeInSeconds);
			float t = timePassed / timeInSeconds;
			RectTransform.position = startPos + (new Vector2(curveX ? _travelCurve.Evaluate(t) * delta.x : delta.x, curveX ? delta.y : _travelCurve.Evaluate(t) * delta.y) * _speedCurve.Evaluate(t));
			yield return null;
		}

		if(onArrivalCallback != null)
		{
			onArrivalCallback(this);
		}

		_flyAnimationRoutine = null;
	}

}
