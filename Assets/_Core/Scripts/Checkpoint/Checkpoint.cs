using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
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

	[SerializeField]
	private InteractonType _checkpointInteractionType = InteractonType.Speech;

	protected void Awake()
	{
		CheckpointCommunicator.Instance.RegisterCheckpoint(this);
	}

	protected void OnDestroy()
	{
		CheckpointCommunicator.Instance.UnregisterCheckpoint(this);
	}

	public void SetState(State state)
	{
		CheckpointState = state;
		if(CheckpointStateChangedEvent != null)
		{
			CheckpointStateChangedEvent(this, CheckpointState);
		}
	}
}
