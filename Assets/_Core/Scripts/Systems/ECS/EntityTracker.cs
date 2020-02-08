using System;

public sealed class EntityTracker : EntitiesHolder, IComponentLifecycle, IEntityLifecycle
{
	public event TrackHandler TrackedEvent;
	public event TrackHandler UntrackedEvent;

	// Entity
	public event Action<Entity, string> TagAddedEvent;
	public event Action<Entity, string> TagRemovedEvent;
	public event Action<Entity> EntityCreatedEvent;
	public event Action<Entity> EntityDestroyEvent;

	// Components
	public event Action<EntityComponent> AddedComponentEvent;
	public event Action<EntityComponent> RemovedComponentEvent;
	public event Action<EntityComponent> EnabledComponentEvent;
	public event Action<EntityComponent> DisabledComponentEvent;

	public static EntityTracker Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new EntityTracker();
			}

			return _instance;
		}
	}

	private static EntityTracker _instance = null;

	private EntityTracker()
	{
		ListenToTrack(FireTrackedEvent, FireUntrackedEvent);
	}

	public void RegisterEntity(Entity entity)
	{
		if (IsCleaned)
		{
			return;
		}

		if (Track(entity))
		{
			entity.AddedComponentEvent += AddedComponentEvent;
			entity.RemovedComponentEvent += RemovedComponentEvent;
			entity.EnabledComponentEvent += EnabledComponentEvent;
			entity.DisabledComponentEvent += DisabledComponentEvent;

			entity.TagAddedEvent += TagAddedEvent;
			entity.TagRemovedEvent += TagRemovedEvent;
			entity.EntityCreatedEvent += EntityCreatedEvent;
			entity.EntityDestroyEvent += EntityDestroyEvent;
		}
	}

	public void UnregisterEntity(Entity entity)
	{
		if(IsCleaned)
		{
			return;
		}

		if(Untrack(entity))
		{
			entity.AddedComponentEvent -= AddedComponentEvent;
			entity.RemovedComponentEvent -= RemovedComponentEvent;
			entity.EnabledComponentEvent -= EnabledComponentEvent;
			entity.DisabledComponentEvent -= DisabledComponentEvent;

			entity.TagAddedEvent -= TagAddedEvent;
			entity.TagRemovedEvent -= TagRemovedEvent;
			entity.EntityCreatedEvent -= EntityCreatedEvent;
			entity.EntityDestroyEvent -= EntityDestroyEvent;
			UnityEngine.Object.Destroy(entity.gameObject);
		}
	}

	protected override void Clean()
	{
		base.Clean();
		UnlistenFromTrack(FireTrackedEvent, FireUntrackedEvent);
		TrackedEvent = null;
		UntrackedEvent = null;
		_instance = null;
	}

	private void FireTrackedEvent(Entity entity)
	{
		if (TrackedEvent != null)
		{
			TrackedEvent(entity);
		}
	}

	private void FireUntrackedEvent(Entity entity)
	{
		if (UntrackedEvent != null)
		{
			UntrackedEvent(entity);
		}
	}
}
