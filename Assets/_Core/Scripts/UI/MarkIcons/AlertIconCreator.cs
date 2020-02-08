using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertIconCreator : MonoBehaviour
{
	[SerializeField]
	private ScreenIcon _alertItemPrefab = null;

	private Dictionary<NPC, ScreenIcon> _icons = new Dictionary<NPC, ScreenIcon>();
	private EntityFilter _npcFilter;

	protected void Awake()
	{
		_npcFilter = EntityFilter.Create(FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<NPC>(true).Result(), OnNPCTracked, OnNPCUntracked);
	}

	protected void OnDestroy()
	{
		_npcFilter.Clean(OnNPCTracked, OnNPCUntracked);
		_npcFilter = null;
	}

	public bool TryGetScreenIconFor(NPC npc, out ScreenIcon screenIcon)
	{
		return _icons.TryGetValue(npc, out screenIcon);
	}

	private void OnNPCTracked(Entity entity)
	{
		entity.GetEntityComponent<NPC>().NPCStateSetEvent += OnNPCStateSetEvent;
	}

	private void OnNPCUntracked(Entity entity)
	{
		entity.GetEntityComponent<NPC>().NPCStateSetEvent -= OnNPCStateSetEvent;
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
