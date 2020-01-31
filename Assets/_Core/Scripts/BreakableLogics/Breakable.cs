using System;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	public event Action<State> StateChangedEvent;

	public enum State
	{
		Unbroken,
		Broken
	}

	public State BreakState
	{
		get; private set;
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
			StateChangedEvent(BreakState);
		}
	}
}
