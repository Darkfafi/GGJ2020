using UnityEngine;

public class Breakable : MonoBehaviour
{
	public delegate void BreakbleStateHandler(Breakable breakable, State newState);
	public event BreakbleStateHandler StateChangedEvent;

	public enum State
	{
		Unbroken,
		Broken
	}

	public State BreakState
	{
		get; private set;
	}

	protected void Awake()
	{
		BreakablesCommunicator.Instance.RegisterBreakable(this);
	}

	protected void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Break();
		}
	}

	protected void OnDestroy()
	{
		BreakablesCommunicator.Instance.UnregisterBreakable(this);
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
