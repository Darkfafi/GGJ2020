using System.Collections.Generic;
using UnityEngine;

public class CheckpointCommunicator
{
	public static CheckpointCommunicator Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new CheckpointCommunicator();
			}
			return _instance;
		}
	}

	private static CheckpointCommunicator _instance = null;

	private List<Checkpoint> _closedCheckpoints = new List<Checkpoint>();
	private List<Checkpoint> _openCheckpoints = new List<Checkpoint>();
	
	public Checkpoint[] GetAllCheckpoint()
	{
		List<Checkpoint> cp = new List<Checkpoint>(_openCheckpoints);
		cp.AddRange(_closedCheckpoints);
		return cp.ToArray();
	}

	public Checkpoint GetRandomUnusedCheckpoint()
	{
		if (_openCheckpoints.Count == 0)
			return null;

		return _openCheckpoints[UnityEngine.Random.Range(0, _openCheckpoints.Count)];
	}

	public Checkpoint GetClosestUnsusedCheckpointToNPC(NPC npc)
	{
		float dist = 0f;
		Checkpoint checkpoint = null;
		for(int i = 0; i < _openCheckpoints.Count; i++)
		{
			float checkpointDist = npc.CalculateLengthPathToTarget(_openCheckpoints[i].transform.position);
			if (checkpoint == null || checkpointDist < dist)
			{
				checkpoint = _openCheckpoints[i];
				dist = checkpointDist;
			}
		}
		return checkpoint;
	}

	public void RegisterCheckpoint(Checkpoint checkpoint)
	{
		AddCheckpointToList(checkpoint);
	}

	public void UnregisterCheckpoint(Checkpoint checkpoint)
	{
		RemoveCheckpointFromList(checkpoint);
	}

	private void OnCheckpointStateChangedEvent(Checkpoint checkpoint, Checkpoint.State state)
	{
		AddCheckpointToList(checkpoint);
	}

	private void AddCheckpointToList(Checkpoint checkpoint)
	{
		RemoveCheckpointFromList(checkpoint);
		switch(checkpoint.CheckpointState)
		{
			case Checkpoint.State.UnOccupied:
				_openCheckpoints.Add(checkpoint);
				break;
			case Checkpoint.State.Occupied:
				_closedCheckpoints.Add(checkpoint);
				break;
			default:
				Debug.LogError("UNKNOWN STATE: " + checkpoint.CheckpointState);
				break;
		}
		checkpoint.CheckpointStateChangedEvent += OnCheckpointStateChangedEvent;
	}

	private void RemoveCheckpointFromList(Checkpoint checkpoint)
	{
		if(HasCheckpointInList(checkpoint))
		{
			_closedCheckpoints.Remove(checkpoint);
			_openCheckpoints.Remove(checkpoint);
			checkpoint.CheckpointStateChangedEvent -= OnCheckpointStateChangedEvent;
		}
	}

	private bool HasCheckpointInList(Checkpoint checkpoint)
	{
		return _closedCheckpoints.Contains(checkpoint) || _openCheckpoints.Contains(checkpoint);
	}
}
