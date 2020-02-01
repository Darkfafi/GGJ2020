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

    private Animator _myAnim;

	protected void Awake()
	{
        _myAnim = gameObject.GetComponent<Animator>();
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
        bool isWalking = false;
        if (_canMove)
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            isWalking = !Mathf.Approximately(Mathf.Abs(hInput) + Mathf.Abs(vInput), 0f);
            if (isWalking)
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
        _myAnim.SetBool("IsWalking", isWalking);
    }

	protected void OnDestroy()
	{
		_repairTarget.StartedRepairingBreakableEvent -= OnStartedRepairingBreakableEvent;
		_repairTarget.EndedRepairingBreakableEvent -= OnEndedRepairingBreakableEvent;
	}

	private void OnStartedRepairingBreakableEvent(Breakable target)
	{
		_canMove = false;
        _myAnim.SetBool("IsRepairing", true);
	}

	private void OnEndedRepairingBreakableEvent(Breakable target)
	{
		_canMove = true;
        _myAnim.SetBool("IsRepairing", false);
    }
}
