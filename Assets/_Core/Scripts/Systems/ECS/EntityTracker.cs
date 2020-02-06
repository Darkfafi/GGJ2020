using System;

public class EntityTracker : EntitiesHolder, IComponentLifecycle, IEntityLifecycle
{
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

	public void RegisterEntity(Entity entity)
	{
		if(Track(entity))
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
}
