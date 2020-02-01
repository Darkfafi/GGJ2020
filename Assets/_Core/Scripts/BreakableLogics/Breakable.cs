using UnityEngine;

public class Breakable : MonoBehaviour, INavMeshTarget
{
	public delegate void BreakbleStateHandler(Breakable breakable, State newState);
	public event BreakbleStateHandler StateChangedEvent;

	public enum State
	{
		Unbroken,
		Broken
	}

	[SerializeField]
	private Transform _targetTransform;

	public State BreakState
	{
		get; private set;
	}

	protected void Awake()
	{
		BreakablesCommunicator.Instance.RegisterBreakable(this);
	}

	protected void OnDestroy()
	{
		BreakablesCommunicator.Instance.UnregisterBreakable(this);
	}

	public Vector3 GetNavMeshOrigin()
	{
		return _targetTransform != null ? _targetTransform.position : transform.position;
	}

	public void Break()
	{
		// TODO: Set Art to Broken
		BreakState = State.Broken;
		FireStateChangedEvent();
	}

	public void Repair()
	{
		// TODO: Set Art to Repaired
		BreakState = State.Unbroken;
		FireStateChangedEvent();
	}


	private void FireStateChangedEvent()
	{
		if(StateChangedEvent != null)
		{
			StateChangedEvent(this, BreakState);
		}
	}
}
