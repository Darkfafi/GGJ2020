using System.Collections.Generic;
using UnityEngine;

public class NPCPathLineCreator : MonoBehaviour
{
	[SerializeField]
	private PathLine _pathLinePrefab;

	private Dictionary<NPC, PathLine> _icons = new Dictionary<NPC, PathLine>();

	protected void Awake()
	{
		NPCCommunicator.Instance.NPCStateSetEvent += OnNPCStateSetEvent;
	}

	protected void OnDestroy()
	{
		NPCCommunicator.Instance.NPCStateSetEvent -= OnNPCStateSetEvent;
	}

	private void OnNPCStateSetEvent(NPC npc, NPC.State state)
	{
		PathLine item = null;
		switch (state)
		{
			case NPC.State.MovingToBreakable:
				if (!_icons.TryGetValue(npc, out item))
				{
					item = Instantiate(_pathLinePrefab, transform);
					_icons[npc] = item;
					item.SetAgent(npc);
				}

				break;
			default:
				if (_icons.ContainsKey(npc))
				{
					item = _icons[npc];
					Destroy(item.gameObject);
					_icons.Remove(npc);
				}
				break;
		}
	}
}
