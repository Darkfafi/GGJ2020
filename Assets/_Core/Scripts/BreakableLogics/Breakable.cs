using UnityEngine;

public class Breakable : MonoBehaviour, INavMeshTarget
{
	public delegate void BreakbleStateHandler(Breakable breakable, State newState);
	public event BreakbleStateHandler StateChangedEvent;

	public enum State
	{
		Unbroken,
		Broken,
		PermanentlyBroken
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
		if(BreakState == State.PermanentlyBroken)
		{
			Debug.Log("Can't break item for it is permanently broken");
			return;
		}

		// TODO: Set Art to Broken
		BreakState = State.Broken;
		FireStateChangedEvent();
	}

	public void PermanentlyBreak()
	{
		BreakState = State.PermanentlyBroken;
		FireStateChangedEvent();
	}

	public void Repair()
	{
		if (BreakState == State.PermanentlyBroken)
		{
			Debug.Log("Can't repair item for it is permanently broken");
			return;
		}

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
