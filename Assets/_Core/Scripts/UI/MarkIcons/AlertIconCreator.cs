using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertIconCreator : MonoBehaviour
{
	[SerializeField]
	private ScreenIcon _alertItemPrefab = null;

	private Dictionary<NPC, ScreenIcon> _icons = new Dictionary<NPC, ScreenIcon>();

	protected void Awake()
	{
		NPCCommunicator.Instance.NPCStateSetEvent += OnNPCStateSetEvent;
	}

	protected void OnDestroy()
	{
		NPCCommunicator.Instance.NPCStateSetEvent -= OnNPCStateSetEvent;
	}

	public bool TryGetScreenIconFor(NPC npc, out ScreenIcon screenIcon)
	{
		return _icons.TryGetValue(npc, out screenIcon);
	}

	private void OnNPCStateSetEvent(NPC npc, NPC.State state)
	{
		switch (state)
		{
			case NPC.State.MovingToBreakable:
				StartCoroutine(CreateIconEndOfFrame(npc));
				break;
			default:
				StartCoroutine(DestroyIconEndOfFrame(npc));
				break;
		}
	}

	private IEnumerator CreateIconEndOfFrame(NPC npc)
	{
		yield return new WaitForEndOfFrame();
		if (!_icons.TryGetValue(npc, out ScreenIcon item))
		{
			item = Instantiate(_alertItemPrefab, transform);
			_icons[npc] = item;
			item.SetIndicationTarget(npc.transform);
		}
	}

	private IEnumerator DestroyIconEndOfFrame(NPC npc)
	{
		yield return new WaitForEndOfFrame();
		ScreenIcon item = null;
		if (_icons.ContainsKey(npc))
		{
			item = _icons[npc];
			Destroy(item.gameObject);
			_icons.Remove(npc);
		}
	}
}
