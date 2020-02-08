using System.Collections.Generic;
using UnityEngine;

public class IndicatorCreator : MonoBehaviour
{
	[SerializeField]
	private ScreenIcon _indicatorPrefab = null;

	private EntityFilter _breakablesFilter;
	private Dictionary<Breakable, ScreenIcon> _indicators = new Dictionary<Breakable, ScreenIcon>();

	protected void Awake()
	{
		FilterRules filterRules = FilterRulesBuilder.SetupNoTagsBuilder()
			.AddHasComponentRule<Breakable>(true)
			.Result();
		_breakablesFilter = EntityFilter.Create(filterRules, OnBreakableTracked, OnBreakableUntracked);
	}

	protected void OnDestroy()
	{
		_breakablesFilter.Clean(OnBreakableTracked, OnBreakableUntracked);
		_breakablesFilter = null;
	}

	private void OnBreakableTracked(Entity entity)
	{
		entity.GetComponent<Breakable>().StateChangedEvent += OnBreakableStateChangedEvent;
	}

	private void OnBreakableUntracked(Entity entity)
	{
		entity.GetComponent<Breakable>().StateChangedEvent -= OnBreakableStateChangedEvent;
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		ScreenIcon arrow = null;
		switch (newState)
		{
			case Breakable.State.Broken:
				if (!_indicators.TryGetValue(breakable, out arrow))
				{
					arrow = Instantiate(_indicatorPrefab, transform);
					_indicators[breakable] = arrow;
					arrow.SetIndicationTarget(breakable.transform);
				}

				break;
			default:
				if(_indicators.ContainsKey(breakable))
				{
					arrow = _indicators[breakable];
					Destroy(arrow.gameObject);
					_indicators.Remove(breakable);
				}
				break;
		}
	}
}
