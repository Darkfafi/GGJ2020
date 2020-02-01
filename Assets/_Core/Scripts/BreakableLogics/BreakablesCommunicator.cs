using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

	public Breakable[] GetBreakables(System.Predicate<Breakable> filter)
	{
		List<Breakable> breakablesToReturn = new List<Breakable>();
		for(int i = 0; i < _registeredBreakables.Count; i++)
		{
			if(filter(_registeredBreakables[i]))
			{
				breakablesToReturn.Add(_registeredBreakables[i]);
			}
		}
		return breakablesToReturn.ToArray();
	}

	public Breakable GetClosestBrokenBreakableToLocation(Vector3 location)
	{
		float dist = 0f;
		Breakable breakable = null;
		for (int i = 0; i < _registeredBreakables.Count; i++)
		{
			float breakableDist = Vector3.Distance(location, _registeredBreakables[i].transform.position);
			if (breakable == null || breakableDist < dist)
			{
				breakable = _registeredBreakables[i];
				dist = breakableDist;
			}
		}
		return breakable;
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
