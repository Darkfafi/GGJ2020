using System.Collections.Generic;

public class BreakablesListener
{
	public event Breakable.BreakbleStateHandler BreakableStateChangedEvent;

	public static BreakablesListener Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new BreakablesListener();
			}
			return _instance;
		}
	}

	private static BreakablesListener _instance = null;

	private List<Breakable> _registeredBreakables = new List<Breakable>();

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
