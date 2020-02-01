using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
    public delegate void NPCBreakableHandler(NPC npc, Breakable breakableSeen);
    public event NPCBreakableHandler NPCSeenBrokenBreakableEvent;

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

    private Animator myAnim;

	public float ViewDistance
	{
		get
		{
			return _viewDistance;
		}
	}

	protected void Awake()
	{
        myAnim = gameObject.GetComponent<Animator>();
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
        SetState(State.MovingToBreakable);
		_targetBreakable = targetBreakable;
		_navMeshAgent.SetDestination(targetBreakable.GetNavMeshOrigin());
		_navMeshAgent.isStopped = false;
		_seeTargetCoroutine = StartCoroutine(SeeBreakableRoutine());
	}

	public float CalculateLengthPathToTarget(Vector3 target)
	{
		return _navMeshAgent.CalculateLengthPathToTarget(target);
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
        SetState(State.Idle);

        if (_currentCheckpoint == null)
		{
			Checkpoint returningCheckpoint = CheckpointCommunicator.Instance.GetClosestUnsusedCheckpointToAgent(_navMeshAgent);
			_returnToCheckpointRoutine = StartCoroutine(ReturnToCheckpointRoutine(returningCheckpoint));
		}
	}

	private void DoCheckpointAction()
	{
		if(_currentCheckpoint != null)
		{
			Debug.Log("TODO: Do Action " + _currentCheckpoint.CheckpointInteractionType);
		}
	}

    private void SetState(State state)
    {
        NPCState = state;
        switch(NPCState)
        {
            case State.MovingToBreakable:
            case State.MovingToCheckpoint:

                myAnim.SetBool("IsWalking", true);
                // Play Walk Animation <-- I am so proud of you Faf.. <3
                break;

            case State.Idle:
                myAnim.SetBool("IsWalking", false);
                // Play Idle Animation <-- You are the sweetest my Faf
                break;
        }
    }
	
	private IEnumerator SeeBreakableRoutine()
	{
		while(_targetBreakable != null)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (_targetBreakable.transform.position - transform.position).normalized, out hit, _viewDistance))
			{
				if (hit.collider.gameObject.GetComponent<Breakable>() == _targetBreakable)
				{
                    myAnim.SetTrigger("Shock");
                    _navMeshAgent.isStopped = true;
                    
                    if(NPCSeenBrokenBreakableEvent != null)
                    {
                        NPCSeenBrokenBreakableEvent(this, _targetBreakable);
                    }

                    yield return new WaitForSeconds(0.8f);
                    StopNPCCallToBreakable();
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		_seeTargetCoroutine = null;
	}

	private IEnumerator ReturnToCheckpointRoutine(Checkpoint checkpoint)
    {
        SetState(State.MovingToCheckpoint);
		_navMeshAgent.isStopped = false;
		_navMeshAgent.velocity = Vector3.zero;
		_navMeshAgent.SetDestination(checkpoint.transform.position);
		AssignToCheckpoint(checkpoint, false);
		while (Vector3.Distance(new Vector2(_navMeshAgent.destination.x, _navMeshAgent.destination.z), new Vector2(transform.position.x, transform.position.z)) > Mathf.Max(0.2f, _navMeshAgent.stoppingDistance))
        {
            yield return null;
		}
		transform.position = new Vector3(checkpoint.transform.position.x, transform.position.y, checkpoint.transform.position.z);
		DoCheckpointAction();
        SetState(State.Idle);
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
