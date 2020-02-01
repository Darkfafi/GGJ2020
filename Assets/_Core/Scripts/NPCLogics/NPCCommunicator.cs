using System.Collections.Generic;

public class NPCCommunicator
{
    public event NPC.NPCBreakableHandler NPCSeenBrokenBreakableEvent;

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

	public void Loop(System.Action<NPC> method)
	{
		for(int i = 0; i < _npcCollection.Count; i++)
		{
			method(_npcCollection[i]);
		}
	}

	public void RegisterNPC(NPC npc)
	{
		if(!_npcCollection.Contains(npc))
		{
			_npcCollection.Add(npc);
            npc.NPCSeenBrokenBreakableEvent += OnNPCSeenBrokenBreakableEvent;

        }
	}

	public void UnregisterNPC(NPC npc)
	{
		if(_npcCollection.Contains(npc))
		{
			_npcCollection.Remove(npc);
            npc.NPCSeenBrokenBreakableEvent -= OnNPCSeenBrokenBreakableEvent;
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
