using System;
using System.Collections.Generic;

public class NPCCommunicator
{
    public event NPC.NPCBreakableHandler NPCSeenBrokenBreakableEvent;
	public event NPC.NPCStateHandler NPCStateSetEvent;

	public static NPCCommunicator Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new NPCCommunicator();
			}

			return _instance;
		}
	}

	private static NPCCommunicator _instance = null;

	private List<NPC> _npcCollection = new List<NPC>();

	public void Loop(Action<NPC> method)
	{
		for(int i = 0; i < _npcCollection.Count; i++)
		{
			method(_npcCollection[i]);
		}
	}

	public NPC[] GetNPCs()
	{
		return _npcCollection.ToArray();
	}

	public NPC[] GetNPCs(Predicate<NPC> filter)
	{
		List<NPC> returnValue = new List<NPC>();
		for (int i = 0; i < _npcCollection.Count; i++)
		{
			if (filter(_npcCollection[i]))
			{
				returnValue.Add(_npcCollection[i]);
			}
		}
		return returnValue.ToArray();
	}

	public void RegisterNPC(NPC npc)
	{
		if(!_npcCollection.Contains(npc))
		{
			_npcCollection.Add(npc);
            npc.NPCSeenBrokenBreakableEvent += OnNPCSeenBrokenBreakableEvent;
			npc.NPCStateSetEvent += OnNPCStateSetEvent;
        }
	}

	public void UnregisterNPC(NPC npc)
	{
		if(_npcCollection.Contains(npc))
		{
			_npcCollection.Remove(npc);
            npc.NPCSeenBrokenBreakableEvent -= OnNPCSeenBrokenBreakableEvent;
			npc.NPCStateSetEvent -= OnNPCStateSetEvent;
		}
	}

	private void OnNPCStateSetEvent(NPC npc, NPC.State state)
	{
		if(NPCStateSetEvent != null)
		{
			NPCStateSetEvent(npc, state);
		}
	}

	private void OnNPCSeenBrokenBreakableEvent(NPC npc, Breakable breakableSeen)
    {
        if(NPCSeenBrokenBreakableEvent != null)
        {
            NPCSeenBrokenBreakableEvent(npc, breakableSeen);
        }
    }
}
