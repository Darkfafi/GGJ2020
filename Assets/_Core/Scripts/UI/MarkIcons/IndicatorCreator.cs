using System.Collections.Generic;
using UnityEngine;

public class IndicatorCreator : MonoBehaviour
{
	[SerializeField]
	private ScreenIcon _indicatorPrefab = null;

	private Dictionary<Breakable, ScreenIcon> _indicators = new Dictionary<Breakable, ScreenIcon>();

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
