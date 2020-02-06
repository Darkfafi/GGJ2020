using DG.Tweening;
using System;
using UnityEngine;

public class RepairTarget : EntityComponent
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
	private Color _highlightColor = Color.white;

	private Breakable _closestBreakable;
	private bool _setColor = false;

	protected void Update()
	{
		if (!_miniGame.IsMinigameActive)
		{
			RegisterClosestBreakable();

			if (IsValidBreakableInRange(_closestBreakable))
			{
				if (!_setColor)
				{
					_closestBreakable.GetComponent<BreakableEntity>().OutlineAnimator.SetOutlineColor(_highlightColor);
					_setColor = true;
				}
				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (StartedRepairingBreakableEvent != null)
					{
						StartedRepairingBreakableEvent(_closestBreakable);
					}

					transform.DOKill();
					transform.DOLookAt(_closestBreakable.transform.position, 0.5f, AxisConstraint.Y, transform.up);

					_miniGame.StartMiniGame((success) =>
					{
						if (success && _closestBreakable != null)
						{
							_closestBreakable.Repair();
						}

						if (EndedRepairingBreakableEvent != null)
						{
							EndedRepairingBreakableEvent(_closestBreakable);
						}
					});
				}
			}
			else if (_setColor)
			{
				_closestBreakable.GetComponent<BreakableEntity>().OutlineAnimator.ResetOutlineColor();
				_setColor = false;
			}
		}
		else if(_closestBreakable != null && _closestBreakable.BreakState == Breakable.State.PermanentlyBroken)
		{
			_miniGame.StopMiniGame(false);
		}
	}

	protected override void OnDestroy()
	{
		transform.DOKill();
		ClearClosestBreakableEntry();
		base.OnDestroy();
	}

	private void RegisterClosestBreakable()
	{
		Breakable breakable = BreakablesCommunicator.Instance.GetClosestBrokenBreakableToLocation(transform.position);
		if(breakable != _closestBreakable && IsValidBreakableInRange(breakable))
		{
			ClearClosestBreakableEntry();
			_closestBreakable = breakable;
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
			_closestBreakable.GetComponent<BreakableEntity>().OutlineAnimator.ResetOutlineColor();
			_setColor = false;
			_closestBreakable = null;
		}
	}
}
