using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline3D))]
public class OutlineAnimator : MonoBehaviour
{
	[SerializeField]
	private float _minSizeOutline = 2f;

	[SerializeField]
	private float _maxSizeOutline = 4f;

	private Outline3D _outline3D;
	private Coroutine _animationRoutine;
	private Color _originalColor;

	public bool IsAnimating
	{
		get
		{
			return _animationRoutine != null;
		}
	}

    protected void Awake()
	{
		_outline3D = gameObject.GetComponent<Outline3D>();
		_originalColor = _outline3D.OutlineColor;
		StopOutlineAnimation();
	}

	public void StartOutlineAnimation()
	{
		StopOutlineAnimation();
		_animationRoutine = StartCoroutine(OutlineAnimationRoutine());
	}

	public void SetOutlineColor(Color color)
	{
		_outline3D.OutlineColor = color;
	}

	public void ResetOutlineColor()
	{
		_outline3D.OutlineColor = _originalColor;
	}

	public void StopOutlineAnimation()
	{
		if (_animationRoutine != null)
		{
			StopCoroutine(_animationRoutine);
		}
		_outline3D.OutlineWidth = 0f;
	}

	private IEnumerator OutlineAnimationRoutine()
	{
		while(true)
		{
			_outline3D.OutlineWidth = _minSizeOutline + Mathf.Abs(Mathf.Sin(Time.time)) * _maxSizeOutline;
			yield return null;
		}
	}
}
