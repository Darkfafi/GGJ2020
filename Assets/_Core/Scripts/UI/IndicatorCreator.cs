using System.Collections.Generic;
using UnityEngine;

public class IndicatorCreator : MonoBehaviour
{
	[SerializeField]
	private IndicationArrow _indicatorPrefab;

	private Dictionary<Breakable, IndicationArrow> _indicators = new Dictionary<Breakable, IndicationArrow>();

	protected void Awake()
	{
		BreakablesCommunicator.Instance.BreakableStateChangedEvent += OnBreakableStateChangedEvent;
	}

	protected void OnDestroy()
	{
		BreakablesCommunicator.Instance.BreakableStateChangedEvent -= OnBreakableStateChangedEvent;
	}

	private void OnBreakableStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		IndicationArrow arrow = null;
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
