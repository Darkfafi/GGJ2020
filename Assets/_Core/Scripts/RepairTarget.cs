using UnityEngine;
using System;

public class RepairTarget : MonoBehaviour
{
	public Action<Breakable> StartedRepairingBreakableEvent;
	public Action<Breakable> EndedRepairingBreakableEvent;

	[Header("Requirements")]
	[SerializeField]
	private MiniGame _miniGame = null;

	[Header("Options")]
	[SerializeField]
	private float _repairDistance = 2f;

	[SerializeField]
	private Color _highlightColor;

	private Color _originalClosestBreakableColor;
	private Breakable _closestBreakable;
	private bool _setColor = false;

	protected void Update()
	{
		RegisterClosestBreakable();

		if(IsValidBreakableInRange(_closestBreakable) && !_miniGame.IsMinigameActive)
		{
			if(!_setColor)
			{
				_closestBreakable.GetComponent<Outline3D>().OutlineColor = _highlightColor;
				_setColor = true;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if(StartedRepairingBreakableEvent != null)
				{
					StartedRepairingBreakableEvent(_closestBreakable);
				}

				_miniGame.StartMiniGame((success)=> 
				{
					if (success && _closestBreakable != null)
					{
						_closestBreakable.Repair();
					}

					if(EndedRepairingBreakableEvent != null)
					{
						EndedRepairingBreakableEvent(_closestBreakable);
					}
				});
			}
		}
		else if(_setColor)
		{
			_closestBreakable.GetComponent<Outline3D>().OutlineColor = _originalClosestBreakableColor;
			_setColor = false;
		}
	}

	protected void OnDestroy()
	{
		ClearClosestBreakableEntry();
	}

	private void RegisterClosestBreakable()
	{
		Breakable breakable = BreakablesCommunicator.Instance.GetClosestBrokenBreakableToLocation(transform.position);
		if(breakable != _closestBreakable && IsValidBreakableInRange(breakable))
		{
			ClearClosestBreakableEntry();
			_closestBreakable = breakable;
			_originalClosestBreakableColor = _closestBreakable.GetComponent<Outline3D>().OutlineColor;
		}
	}

	private bool IsValidBreakableInRange(Breakable breakable)
	{
		return breakable != null && breakable.BreakState == Breakable.State.Broken && Vector3.Distance(breakable.transform.position, transform.position) <= _repairDistance;
	}

	private void ClearClosestBreakableEntry()
	{
		if (_closestBreakable != null)
		{
			_closestBreakable.GetComponent<Outline3D>().OutlineColor = _originalClosestBreakableColor;
			_setColor = false;
			_closestBreakable = null;
		}
	}
}
