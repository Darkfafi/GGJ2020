using UnityEngine;

[RequireComponent(typeof(Breakable), typeof(OutlineAnimator))]
public class BreakableEntity : MonoBehaviour
{
	private Breakable _breakable;
	private OutlineAnimator _outlineAnimator;

	protected void Awake()
	{
		_breakable = gameObject.GetComponent<Breakable>();
		_outlineAnimator = gameObject.GetComponent<OutlineAnimator>();
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