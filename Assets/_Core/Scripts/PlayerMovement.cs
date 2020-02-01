using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(RepairTarget))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Camera _playerCamera = null;

	[Header("Options")]
	[SerializeField]
	private float _movementSpeed = 5f;

	private NavMeshAgent _navMeshAgent;
	private RepairTarget _repairTarget;
	private bool _canMove = true;

	protected void Awake()
	{
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		_repairTarget = gameObject.GetComponent<RepairTarget>();

		_repairTarget.StartedRepairingBreakableEvent += OnStartedRepairingBreakableEvent;
		_repairTarget.EndedRepairingBreakableEvent += OnEndedRepairingBreakableEvent;

		if (_playerCamera == null)
		{
			_playerCamera = Camera.main;
		}
	}

    protected void Update()
    {
		if (_canMove)
		{
			float hInput = Input.GetAxis("Horizontal");
			float vInput = Input.GetAxis("Vertical");

			if (!Mathf.Approximately(Mathf.Abs(hInput) + Mathf.Abs(vInput), 0f))
			{
				Vector3 xDelta = hInput * Time.deltaTime * _playerCamera.transform.right;
				Vector3 zDelta = vInput * Time.deltaTime * _playerCamera.transform.forward;
				Vector3 finalDelta = (xDelta + zDelta) * _movementSpeed;
				finalDelta.y = 0f;
				_navMeshAgent.Move(finalDelta);
				Quaternion tRot = Quaternion.LookRotation(finalDelta, transform.up);
				transform.rotation = Quaternion.Lerp(transform.rotation, tRot, 5f * Time.deltaTime);
			}
		}
    }

	protected void OnDestroy()
	{
		_repairTarget.StartedRepairingBreakableEvent -= OnStartedRepairingBreakableEvent;
		_repairTarget.EndedRepairingBreakableEvent -= OnEndedRepairingBreakableEvent;
	}

	private void OnStartedRepairingBreakableEvent(Breakable target)
	{
		_canMove = false;
	}

	private void OnEndedRepairingBreakableEvent(Breakable target)
	{
		_canMove = true;
	}
}
