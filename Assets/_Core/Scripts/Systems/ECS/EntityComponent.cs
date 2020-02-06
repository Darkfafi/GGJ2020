using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class EntityComponent : MonoBehaviour, IComponentLifecycle
{
	public event Action<EntityComponent> AddedComponentEvent;
	public event Action<EntityComponent> RemovedComponentEvent;
	public event Action<EntityComponent> EnabledComponentEvent;
	public event Action<EntityComponent> DisabledComponentEvent;

	public Entity Parent
	{
		get; private set;
	}

	protected void Awake()
	{
		Parent = GetComponent<Entity>();
		Parent.RegisterComponent(this);
	}

	protected void Start()
	{
		if(AddedComponentEvent != null)
		{
			AddedComponentEvent(this);
		}
	}

	protected void OnEnable()
	{
		if (EnabledComponentEvent != null)
		{
			EnabledComponentEvent(this);
		}
	}

	protected void OnDisable()
	{
		if (DisabledComponentEvent != null)
		{
			DisabledComponentEvent(this);
		}
	}

	protected void OnDestroy()
	{
		if (RemovedComponentEvent != null)
		{
			RemovedComponentEvent(this);
		}

		Parent.UnregisterComponent(this);
		Parent = null;
	}
}


public interface IComponentLifecycle
{
	event Action<EntityComponent> AddedComponentEvent;
	event Action<EntityComponent> RemovedComponentEvent;
	event Action<EntityComponent> EnabledComponentEvent;
	event Action<EntityComponent> DisabledComponentEvent;
}