using System;
using UnityEngine;

public class RepairGameMode : BaseGameMode<RepairModeSettings>
{
	public Action<int, Breakable> RepairIncreasedEvent;
	public Action<int, NPC> ShockIncreasedEvent;

	[SerializeField]
	private NPCDirector _npcDirector = null;

	private EntityFilter _repairersEntityFilter;
	private EntityFilter _npcEntityFilter;

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
		FilterRules repairerFilterRules = FilterRulesBuilder.SetupHasTagBuilder("Repairer")
									.AddHasComponentRule<RepairTarget>(true)
									.Result();

		_repairersEntityFilter = EntityFilter.Create(repairerFilterRules, OnRepairerTracked, OnRepairerUntracked);

		FilterRules npcFilterRules = FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<NPC>(true).Result();
		_npcEntityFilter = EntityFilter.Create(npcFilterRules, OnNPCTracked, OnNPCUntracked);

		// Setup
		RepairCount = 0;
		ShockCount = 0;

		// Start
		_npcDirector.SetDirectorState(NPCDirector.State.Active);
	}

	protected override void StopMode()
	{
		_repairersEntityFilter.Clean(OnRepairerTracked, OnRepairerUntracked);
		_repairersEntityFilter = null;

		_npcEntityFilter.Clean(OnNPCTracked, OnNPCUntracked);
		_npcEntityFilter = null;

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

	// Tracking

	private void OnRepairerTracked(Entity entity)
	{
		entity.GetEntityComponent<RepairTarget>().EndedRepairingBreakableEvent += OnEndedRepairingBreakableEvent;
	}

	private void OnRepairerUntracked(Entity entity)
	{
		entity.GetEntityComponent<RepairTarget>().EndedRepairingBreakableEvent -= OnEndedRepairingBreakableEvent;
	}

	private void OnNPCTracked(Entity entity)
	{
		entity.GetEntityComponent<NPC>().NPCSeenBrokenBreakableEvent += OnNPCSeenBrokenBreakableEvent;
	}

	private void OnNPCUntracked(Entity entity)
	{
		entity.GetEntityComponent<NPC>().NPCSeenBrokenBreakableEvent -= OnNPCSeenBrokenBreakableEvent;
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