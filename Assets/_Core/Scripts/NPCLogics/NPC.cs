using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
	public enum State
	{
		Idle,
		MovingToBreakable,
		MovingToCheckpoint,
	}

	public State NPCState
	{
		get; private set;
	}

	[SerializeField]
	private float _viewDistance = 5f;

	private Breakable _targetBreakable = null;
	private Checkpoint _currentCheckpoint = null;
	private NavMeshAgent _navMeshAgent = null;
	private Coroutine _seeTargetCoroutine = null;
	private Coroutine _returnToCheckpointRoutine = null;

	public float ViewDistance
	{
		get
		{
			return _viewDistance;
		}
	}

	protected void Awake()
	{
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		NPCCommunicator.Instance.RegisterNPC(this);
	}

	protected void OnDestroy()
	{
		NPCCommunicator.Instance.UnregisterNPC(this);
	}

	public void AssignToCheckpoint(Checkpoint checkpoint, bool setToCheckpointPos = true)
	{
		UnassignFromCheckpoint();
		_currentCheckpoint = checkpoint;
		_currentCheckpoint.SetState(Checkpoint.State.Occupied);
		if(setToCheckpointPos)
		{
			transform.position = _currentCheckpoint.transform.position;
			DoCheckpointAction();
		}
	}

	public void UnassignFromCheckpoint()
	{
		if(_currentCheckpoint != null)
		{
			_currentCheckpoint.SetState(Checkpoint.State.UnOccupied);
			_currentCheckpoint = null;
		}
	}

	public void CallNPCToBreakable(Breakable targetBreakable)
	{
		StopNPCCallToBreakable();
		UnassignFromCheckpoint();
		NPCState = State.MovingToBreakable;
		_targetBreakable = targetBreakable;
		_navMeshAgent.SetDestination(targetBreakable.transform.position);
		_navMeshAgent.isStopped = false;
		_seeTargetCoroutine = StartCoroutine(SeeBreakableRoutine());
	}

	public void StopNPCCallToBreakable()
	{
		if(_seeTargetCoroutine != null)
		{
			StopCoroutine(_seeTargetCoroutine);
		}

		if(_returnToCheckpointRoutine != null)
		{
			StopCoroutine(_returnToCheckpointRoutine);
		}

		_targetBreakable = null;
		_navMeshAgent.isStopped = true;
		NPCState = State.Idle;

		if (_currentCheckpoint == null)
		{
			Checkpoint returningCheckpoint = CheckpointCommunicator.Instance.GetClosestUnsusedCheckpointToNPC(this);
			_returnToCheckpointRoutine = StartCoroutine(ReturnToCheckpointRoutine(returningCheckpoint));
		}
	}

	public float CalculateLengthPathToTarget(Vector3 target)
	{
		NavMeshPath path = CalculatePathToTarget(target);
		float lengthPath = 0f;

		if(path.corners.Length > 0)
		{
			lengthPath += Vector3.Distance(transform.position, path.corners[0]);
			lengthPath += Vector3.Distance(path.corners[0], target);
		}

		if (path.corners.Length > 1 && path.status != NavMeshPathStatus.PathInvalid)
		{
			for (int i = 0; i < path.corners.Length - 2; i++)
			{
				Vector3 currentCorner = path.corners[i];
				Vector3 nextCorner = path.corners[i + 1];
				lengthPath += Vector3.Distance(currentCorner, nextCorner);
			}
		}
		return lengthPath;
	}

	private void DoCheckpointAction()
	{
		if(_currentCheckpoint != null)
		{
			Debug.Log("TODO: Do Action " + _currentCheckpoint.CheckpointInteractionType);
		}
	}

	private NavMeshPath CalculatePathToTarget(Vector3 target)
	{
		NavMeshPath path = new NavMeshPath();
		_navMeshAgent.CalculatePath(target, path);
		return path;
	}
	
	private IEnumerator SeeBreakableRoutine()
	{
		while(_targetBreakable != null)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (_targetBreakable.transform.position - transform.position).normalized, out hit, _viewDistance))
			{
				Debug.Log(hit.collider.gameObject.name);
				if (hit.collider.gameObject.GetComponent<Breakable>() == _targetBreakable)
				{
					StopNPCCallToBreakable();
					Debug.Log("SHOCK!");
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		_seeTargetCoroutine = null;
	}

	private IEnumerator ReturnToCheckpointRoutine(Checkpoint checkpoint)
	{
		NPCState = State.MovingToCheckpoint;
		_navMeshAgent.isStopped = false;
		_navMeshAgent.velocity = Vector3.zero;
		_navMeshAgent.SetDestination(checkpoint.transform.position);
		AssignToCheckpoint(checkpoint, false);
		while (Vector3.Distance(_navMeshAgent.destination, transform.position) > Mathf.Max(0.5f, _navMeshAgent.stoppingDistance))
		{
			yield return null;
		}
		transform.position = new Vector3(checkpoint.transform.position.x, transform.position.y, checkpoint.transform.position.z);
		DoCheckpointAction();
		NPCState = State.Idle;
		_navMeshAgent.isStopped = true;
		_returnToCheckpointRoutine = null;
	}

	private void OnDrawGizmos()
	{
		if (_targetBreakable != null)
		{
			Gizmos.DrawLine(transform.position, transform.position + (_targetBreakable.transform.position - transform.position).normalized * _viewDistance);
		}
		else
		{
			Gizmos.DrawLine(transform.position, transform.position + transform.forward.normalized * _viewDistance);
		}
	}
}
