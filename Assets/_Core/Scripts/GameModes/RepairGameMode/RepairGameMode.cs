using System;
using UnityEngine;

public class RepairGameMode : BaseGameMode<RepairModeSettings>
{
	public Action<int, Breakable> RepairIncreasedEvent;
	public Action<int, NPC> ShockIncreasedEvent;

	[SerializeField]
	private NPCDirector _npcDirector = null;

	[SerializeField]
	private Player _player = null;

	public int RepairCount
	{
		get; private set;
	}

	public int ShockCount
	{
		get; private set;
	}

	protected override void StartMode(RepairModeSettings settings)
	{
		// Setup
		RepairCount = 0;
		ShockCount = 0;

		NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent += OnNPCSeenBrokenBreakableEvent;
		_player.RepairTarget.EndedRepairingBreakableEvent += OnEndedRepairingBreakableEvent;

		// Start
		_npcDirector.SetDirectorState(NPCDirector.State.Active);
	}

	protected override void StopMode()
	{
		_player.RepairTarget.EndedRepairingBreakableEvent -= OnEndedRepairingBreakableEvent;
		NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent -= OnNPCSeenBrokenBreakableEvent;
		_npcDirector.SetDirectorState(NPCDirector.State.Deactive);
	}

	private void OnEndedRepairingBreakableEvent(Breakable breakable)
	{
		if (breakable.BreakState == Breakable.State.Unbroken)
		{
			SetRepairCount(RepairCount + 1);

			if (RepairIncreasedEvent != null)
			{
				RepairIncreasedEvent(RepairCount, breakable);
			}
		}
	}

	private void OnNPCSeenBrokenBreakableEvent(NPC npc, Breakable breakableSeen)
	{
		SetShockCount(ShockCount + 1);

		if (ShockIncreasedEvent != null)
		{
			ShockIncreasedEvent(ShockCount, npc);
		}
	}

	private void SetShockCount(int amount)
	{
		ShockCount = Mathf.Clamp(amount, 0, CurrentSetting.ShockLimitAmount);

		if (ShockCount == CurrentSetting.ShockLimitAmount)
		{
			CallLoseCondition();
		}
	}

	private void SetRepairCount(int amount)
	{
		RepairCount = Mathf.Clamp(amount, 0, CurrentSetting.RepairGoalAmount);

		if (RepairCount == CurrentSetting.RepairGoalAmount)
		{
			CallWinCondition();
		}
	}
}


public struct RepairModeSettings : IGameModeSettings
{
	public int RepairGoalAmount
	{
		get; private set;
	}

	public int ShockLimitAmount
	{
		get; private set;
	}

	public RepairModeSettings(int repairGoalAmount, int shockLimitAmount)
	{
		RepairGoalAmount = repairGoalAmount;
		ShockLimitAmount = shockLimitAmount;
	}
}