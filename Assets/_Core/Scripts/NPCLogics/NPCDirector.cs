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
			NPC callingNPC = null;
			NPC[] npcs = NPCCommunicator.Instance.GetNPCs(x => x.NPCState == NPC.State.Idle);
			System.Array.Sort(npcs, (a, b) => (int)(a.CalculateLengthPathToTarget(breakable.GetNavMeshOrigin()) - b.CalculateLengthPathToTarget(breakable.GetNavMeshOrigin())));

			if(npcs.Length > 1)
			{
				callingNPC = npcs[Mathf.FloorToInt(npcs.Length / 2f)];
			}
			else if(npcs.Length == 1)
			{
				callingNPC = npcs[0];
			}

			if (callingNPC != null)
			{
				callingNPC.CallNPCToBreakable(breakable);
			}
		}
	}
}
