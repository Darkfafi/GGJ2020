﻿using System;
using UnityEngine;

public class NPCDirector : MonoBehaviour
{
	public enum State
	{
		Active,
		Deactive
	}

	[SerializeField]
	private int _npcAmount = 5;

	[SerializeField]
	private NPC _npcPrefab = null;

	private EntityFilter _breakablesFilter;
	private EntityFilter _npcsFilter;

	public State DirectorState
	{
		get; private set;
	}

	protected void Awake()
	{
		SetDirectorState(State.Deactive);
	}

	protected void Start()
	{
		for (int i = 0; i < _npcAmount; i++)
		{
			Checkpoint nextOpenCheckpoint = CheckpointCommunicator.Instance.GetRandomUnusedCheckpoint();
			if (nextOpenCheckpoint == null)
			{
				Debug.LogErrorFormat("No checkpoints left to spawn NPCs on! Asked for {0} npcs, while only having {1} checkpoints!", _npcAmount, CheckpointCommunicator.Instance.GetAllCheckpoint().Length);
				break;
			}

			NPC npc = Instantiate(_npcPrefab);
			npc.AssignToCheckpoint(nextOpenCheckpoint);
			npc.UnassignFromCheckpoint();
			npc.StopNPCCallToBreakable();
		}
	}

	protected void OnDestroy()
	{
		SetDirectorState(State.Deactive);
	}

	public void SetDirectorState(State state)
	{
		if(DirectorState == state)
		{
			return;
		}

		DirectorState = state;

		if (_breakablesFilter != null)
		{
			_breakablesFilter.Clean(OnTrackedBreakable, OnUntrackedBreakable);
			_breakablesFilter = null;
		}

		if(_npcsFilter != null)
		{
			_npcsFilter.Clean(null, null);
			_npcsFilter = null;
		}

		if(DirectorState == State.Active)
		{
			_npcsFilter = EntityFilter.Create(FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<NPC>(true).Result(), null, null);
			_breakablesFilter = EntityFilter.Create(FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<Breakable>(true).Result(), OnTrackedBreakable, OnUntrackedBreakable);
		}
	}

	private void OnTrackedBreakable(Entity entity)
	{
		entity.GetEntityComponent<Breakable>().StateChangedEvent += OnBreakableStateChangedEvent;
	}

	private void OnUntrackedBreakable(Entity entity)
	{
		entity.GetEntityComponent<Breakable>().StateChangedEvent -= OnBreakableStateChangedEvent;
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		if (newState == Breakable.State.Broken)
		{
			Entity callingNPCEntity = null;
			Entity[] npcEntities = _npcsFilter.GetAll
				(x => x.GetEntityComponent<NPC>().NPCState == NPC.State.Idle, 
				(a, b) =>
				{
					NPC aNPC = a.GetEntityComponent<NPC>();
					NPC bNPC = b.GetEntityComponent<NPC>();
					return (int)(aNPC.CalculateLengthPathToTarget(breakable.GetNavMeshOrigin()) - bNPC.CalculateLengthPathToTarget(breakable.GetNavMeshOrigin()));
				}
			);

			if(npcEntities.Length > 1)
			{
				callingNPCEntity = npcEntities[Mathf.FloorToInt(npcEntities.Length / 2f)];
			}

			else if(npcEntities.Length == 1)
			{
				callingNPCEntity = npcEntities[0];
			}

			if (callingNPCEntity != null)
			{
				callingNPCEntity.GetEntityComponent<NPC>().CallNPCToBreakable(breakable);
			}
		}
	}
}
