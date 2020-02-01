using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
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

	[SerializeField]
	private InteractonType _checkpointInteractionType = InteractonType.Speech;

	public void SetState(State state)
	{
		CheckpointState = state;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.one );
	}
}
