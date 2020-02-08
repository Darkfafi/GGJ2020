using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class NPC : EntityComponent
{
    public delegate void NPCBreakableHandler(NPC npc, Breakable breakableSeen);
	public delegate void NPCStateHandler(NPC npc, State state);
    public event NPCBreakableHandler NPCSeenBrokenBreakableEvent;
	public event NPCStateHandler NPCStateSetEvent;

    public enum State
	{
		Idle,
		MovingToBreakable,
		Shock,
		MovingToCheckpoint,
	}

	public State NPCState
	{
		get; private set;
	}

	public NavMeshAgent Agent
	{
		get
		{
			return _navMeshAgent;
		}
	}

	public INavMeshTarget CurrentTarget
	{
		get
		{
			return _targetBreakable;
		}
	}

	[SerializeField]
	private float _viewDistance = 5f;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _noticeSFX = null;
	[SerializeField]
	private AudioClip _shockSFX = null;
	[SerializeField]
	private AudioClip _confusedSFX = null;

	private Breakable _targetBreakable = null;
	private Checkpoint _currentCheckpoint = null;
	private NavMeshAgent _navMeshAgent = null;
	private Coroutine _seeTargetCoroutine = null;
	private Coroutine _returnToCheckpointRoutine = null;
	private AudioSource _audioSource = null;
	private EntityFilter _checkpointsFilter;

    private Animator myAnim;

	public float ViewDistance
	{
		get
		{
			return _viewDistance;
		}
	}

	protected override void Awake()
	{
        myAnim = gameObject.GetComponent<Animator>();
		_audioSource = gameObject.GetComponent<AudioSource>();
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		_checkpointsFilter = EntityFilter.Create(FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<Checkpoint>(true).Result(), null, null);
		base.Awake();
	}

	protected override void OnDestroy()
	{
		_checkpointsFilter.Clean(null, null);
		_checkpointsFilter = null;
		base.OnDestroy();
	}

	public void AssignToCheckpoint(Checkpoint checkpoint, bool setToCheckpointPos = true)
	{
		UnassignFromCheckpoint();
		_currentCheckpoint = checkpoint;
		_currentCheckpoint.SetState(Checkpoint.State.Occupied);
		if(setToCheckpointPos)
		{
			transform.position = checkpoint.transform.position;
			Vector3 rot = transform.eulerAngles;
			rot.y = checkpoint.transform.eulerAngles.y;
			transform.eulerAngles = rot;
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
		_audioSource.PlayOneShot(_noticeSFX);
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
			Entity closestCheckpointEntity = _checkpointsFilter.GetFirst(x => x.GetEntityComponent<Checkpoint>().CheckpointState == Checkpoint.State.UnOccupied, 
				(a, b) => 
				{
					return Mathf.RoundToInt(
						CalculateLengthPathToTarget(a.GetEntityComponent<Checkpoint>().GetNavMeshOrigin()) - 
						CalculateLengthPathToTarget(b.GetEntityComponent<Checkpoint>().GetNavMeshOrigin())
					);
				}
			);

			if(closestCheckpointEntity != null)
			{
				_returnToCheckpointRoutine = StartCoroutine(ReturnToCheckpointRoutine(closestCheckpointEntity.GetEntityComponent<Checkpoint>()));
			}
		}
	}

	private void DoCheckpointAction()
	{
		if(_currentCheckpoint != null)
		{
			//Debug.Log("TODO: Do Action " + _currentCheckpoint.CheckpointInteractionType);
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
			case State.Shock:
				myAnim.SetTrigger("Shock");
				_audioSource.PlayOneShot(_shockSFX);
				break;
            case State.Idle:
                myAnim.SetBool("IsWalking", false);
                // Play Idle Animation <-- You are the sweetest my Faf
                break;
        }

		if(NPCStateSetEvent != null)
		{
			NPCStateSetEvent(this, NPCState);
		}
    }
	
	private IEnumerator SeeBreakableRoutine()
	{
		while(_targetBreakable != null)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (_targetBreakable.transform.position - transform.position).normalized, out hit, _viewDistance, ~(1 << 9)))
			{
				if (hit.collider.gameObject.GetComponent<Breakable>() == _targetBreakable)
                {
                    _navMeshAgent.isStopped = true;
                    if (_targetBreakable.BreakState == Breakable.State.Broken)
                    {
						SetState(State.Shock);
                        _targetBreakable.PermanentlyBreak();
                        if (NPCSeenBrokenBreakableEvent != null)
                        {
                            NPCSeenBrokenBreakableEvent(this, _targetBreakable);
                        }

                        yield return new WaitForSeconds(0.8f);
                    }
                    else
                    {
						_audioSource.PlayOneShot(_confusedSFX);
                        myAnim.SetTrigger("IsConfused");
                        yield return new WaitForSeconds(3.417f);
                    }
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
		Vector3 rot = transform.eulerAngles;
		rot.y = checkpoint.transform.eulerAngles.y;
		transform.eulerAngles = rot;
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
