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
	private NPC _npcPrefab;

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

			Instantiate(_npcPrefab).AssignToCheckpoint(nextOpenCheckpoint);
		}
	}

	public void SetDirectorState(State state)
	{
		if(DirectorState == state)
		{
			return;
		}

		DirectorState = state;
		switch(DirectorState)
		{
			case State.Active:
				BreakablesCommunicator.Instance.BreakableStateChangedEvent += OnBreakableStateChangedEvent;
				break;
			case State.Deactive:
				BreakablesCommunicator.Instance.BreakableStateChangedEvent -= OnBreakableStateChangedEvent;
				break;
		}
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		if (newState == Breakable.State.Broken)
		{
			float dist = 0f;
			NPC callingNPC = null;

			NPCCommunicator.Instance.Loop(npc =>
			{
				float distToBreakable = npc.CalculateLengthPathToTarget(breakable.GetNavMeshOrigin());
				if ((callingNPC == null || distToBreakable < dist) && npc.NPCState == NPC.State.Idle && distToBreakable > npc.ViewDistance)
				{
					callingNPC = npc;
					dist = distToBreakable;
				}
			});

			if (callingNPC != null)
			{
				callingNPC.CallNPCToBreakable(breakable);
			}
		}
	}
}
