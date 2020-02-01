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
}
