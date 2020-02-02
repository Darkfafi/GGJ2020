using UnityEngine;

[RequireComponent(typeof(Breakable))]
public class BreakableEntity : MonoBehaviour
{
	[SerializeField]
	private OutlineAnimator _outlineAnimator;

	private Breakable _breakable;

	public OutlineAnimator OutlineAnimator
	{
		get
		{
			return _outlineAnimator;
		}
	}

	protected void Awake()
	{
		_breakable = gameObject.GetComponent<Breakable>();

		if (_outlineAnimator == null)
		{
			_outlineAnimator = gameObject.GetComponent<OutlineAnimator>();
		}
        _breakable.StateChangedEvent += OnBreakableStateChangedEvent;
    }

	protected void OnDestroy()
	{
		_breakable.StateChangedEvent -= OnBreakableStateChangedEvent;
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State state)
	{
		switch(state)
		{
			case Breakable.State.Broken:
				_outlineAnimator.StartOutlineAnimation();
				break;
			default:
				_outlineAnimator.StopOutlineAnimation();
				break;
		}
	}
}