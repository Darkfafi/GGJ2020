using System;
using System.Collections.Generic;
using UnityEngine;

public class AlertIconCreator : MonoBehaviour
{
	[SerializeField]
	private ScreenIcon _alertItemPrefab;

	private Dictionary<NPC, ScreenIcon> _icons = new Dictionary<NPC, ScreenIcon>();

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
		ScreenIcon item = null;
		switch (state)
		{
			case NPC.State.MovingToBreakable:
				if (!_icons.TryGetValue(npc, out item))
				{
					item = Instantiate(_alertItemPrefab, transform);
					_icons[npc] = item;
					item.SetIndicationTarget(npc.transform);
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
