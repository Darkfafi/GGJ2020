﻿using System;
using UnityEngine;

public class Checkpoint : EntityComponent, INavMeshTarget
{
	public event Action<Checkpoint, State> CheckpointStateChangedEvent;

	public enum InteractonType
	{
		Speech,
	}

	public enum State
	{
		UnOccupied,
		Occupied
	}

	public State CheckpointState
	{
		get; private set;
	}

	public InteractonType CheckpointInteractionType
	{
		get
		{
			return _checkpointInteractionType;
		}
	}

	public Vector3 GetNavMeshOrigin()
	{
		return transform.position;
	}

	[SerializeField]
	private InteractonType _checkpointInteractionType = InteractonType.Speech;

	public void SetState(State state)
	{
		CheckpointState = state;
		if(CheckpointStateChangedEvent != null)
		{
			CheckpointStateChangedEvent(this, CheckpointState);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.one );
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.DrawWireSphere(transform.position + transform.forward, 0.25f);
	}
}
