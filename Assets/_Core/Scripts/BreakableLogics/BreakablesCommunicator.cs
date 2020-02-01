using System;
using System.Collections.Generic;

public class BreakablesCommunicator
{
	public event Breakable.BreakbleStateHandler BreakableStateChangedEvent;

	public static BreakablesCommunicator Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new BreakablesCommunicator();
			}
			return _instance;
		}
	}

	private static BreakablesCommunicator _instance = null;

	private List<Breakable> _registeredBreakables = new List<Breakable>();

	public Breakable[] GetBreakables()
	{
		return _registeredBreakables.ToArray();
	}

	public void RegisterBreakable(Breakable breakable)
	{
		if(!_registeredBreakables.Contains(breakable))
		{
			_registeredBreakables.Add(breakable);
			breakable.StateChangedEvent += OnBreakableStateChangedEvent;
		}
	}

	public void UnregisterBreakable(Breakable breakable)
	{
		if (_registeredBreakables.Contains(breakable))
		{
			_registeredBreakables.Remove(breakable);
			breakable.StateChangedEvent -= OnBreakableStateChangedEvent;
		}
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		if(BreakableStateChangedEvent != null)
		{
			BreakableStateChangedEvent(breakable, newState);
		}
	}
}
