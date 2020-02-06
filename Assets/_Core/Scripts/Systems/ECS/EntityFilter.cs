using System.Collections.Generic;

public class EntityFilter : EntitiesHolder
{
	public FilterRules FilterRules
	{
		get; private set;
	}

	#region Static Construction

	private static List<EntityFilter> _cachedFilters = new List<EntityFilter>();
	private static Dictionary<EntityFilter, int> _cachedFiltersReferenceCounter = new Dictionary<EntityFilter, int>();

	public static EntityFilter Create()
	{
		return Create(FilterRules.CreateNoTagsFilter());
	}

	public static EntityFilter Create(FilterRules filterRules)
	{
		for (int i = _cachedFilters.Count - 1; i >= 0; i--)
		{
			if (_cachedFilters[i].FilterRules.Equals(filterRules))
			{
				AddReference(_cachedFilters[i]);
				return _cachedFilters[i];
			}
		}

		EntityFilter self = new EntityFilter(filterRules);
		AddReference(self);
		_cachedFilters.Add(self);
		return self;
	}

	private static void AddReference(EntityFilter instance)
	{
		if (HasReferences(instance))
		{
			_cachedFiltersReferenceCounter[instance]++;
		}
		else
		{
			_cachedFiltersReferenceCounter.Add(instance, 1);
		}
	}

	private static bool HasReferences(EntityFilter instance)
	{
		return _cachedFiltersReferenceCounter.ContainsKey(instance);
	}

	private static void RemoveReference(EntityFilter instance)
	{
		bool remove = false;
		if (HasReferences(instance))
		{
			_cachedFiltersReferenceCounter[instance]--;
			if (_cachedFiltersReferenceCounter[instance] == 0)
			{
				_cachedFiltersReferenceCounter.Remove(instance);
				remove = true;
			}
		}
		else
		{
			remove = true;
		}

		if (remove)
		{
			_cachedFilters.Remove(instance);
		}
	}

	#endregion

	private EntityFilter(FilterRules filter)
	{
		FilterRules = filter;
		EntityTracker.Instance.TagAddedEvent += OnEntityAddedTagEvent;
		EntityTracker.Instance.TagRemovedEvent += OnEntityRemovedTagEvent;
		EntityTracker.Instance.AddedComponentEvent += OnEntityAddedComponentEvent;
		EntityTracker.Instance.RemovedComponentEvent += OnEntityRemovedComponentEvent;
		EntityTracker.Instance.EnabledComponentEvent += OnEntityChangedEnabledStateOfComponentEvent;
		EntityTracker.Instance.DisabledComponentEvent += OnEntityChangedEnabledStateOfComponentEvent;
		EntityTracker.Instance.TrackedEvent += OnTrackedEvent;
		EntityTracker.Instance.UntrackedEvent += OnEntityUntrackedEvent;
		FillWithAlreadyExistingMatches();
	}

	public override void Clean()
	{
		RemoveReference(this);
		if (!HasReferences(this))
		{
			EntityTracker.Instance.TagAddedEvent -= OnEntityAddedTagEvent;
			EntityTracker.Instance.TagRemovedEvent -= OnEntityRemovedTagEvent;
			EntityTracker.Instance.AddedComponentEvent -= OnEntityAddedComponentEvent;
			EntityTracker.Instance.RemovedComponentEvent -= OnEntityRemovedComponentEvent;
			EntityTracker.Instance.EnabledComponentEvent -= OnEntityChangedEnabledStateOfComponentEvent;
			EntityTracker.Instance.DisabledComponentEvent -= OnEntityChangedEnabledStateOfComponentEvent;
			EntityTracker.Instance.TrackedEvent -= OnTrackedEvent;
			EntityTracker.Instance.UntrackedEvent -= OnEntityUntrackedEvent;
			base.Clean();
		}
	}

	public bool Equals(EntityFilter filter)
	{
		return Equals(filter.FilterRules);
	}

	private void OnTrackedEvent(Entity entity)
	{
		if (entity != null && FilterRules.HasFilterPermission(entity))
		{
			Track(entity);
		}
	}

	private void OnEntityAddedComponentEvent(EntityComponent component)
	{
		TrackLogics(component.Parent);
	}

	private void OnEntityRemovedComponentEvent(EntityComponent component)
	{
		TrackLogics(component.Parent);
	}

	private void OnEntityChangedEnabledStateOfComponentEvent(EntityComponent component)
	{
		TrackLogics(component.Parent);
	}

	private void OnEntityAddedTagEvent(Entity entity, string tag)
	{
		TrackLogics(entity);
	}

	private void OnEntityRemovedTagEvent(Entity entity, string tag)
	{
		TrackLogics(entity);
	}

	private void OnEntityUntrackedEvent(Entity entity)
	{
		if (entity != null)
		{
			Untrack(entity);
		}
	}

	private void FillWithAlreadyExistingMatches()
	{
		Entity[] t = EntityTracker.Instance.GetAll(FilterRules.HasFilterPermission);
		for (int i = 0; i < t.Length; i++)
		{
			Track(t[i]);
		}
	}

	private void TrackLogics(Entity entity)
	{
		if (entity != null)
		{
			if (FilterRules.HasFilterPermission(entity))
			{
				Track(entity);
			}
			else
			{
				Untrack(entity);
			}
		}
	}
}