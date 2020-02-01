using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Camera _playerCamera = null;

	[Header("Options")]
	[SerializeField]
	private float _movementSpeed = 5f;

	private NavMeshAgent _navMeshAgent;

	protected void Awake()
	{
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

		if(_playerCamera == null)
		{
			_playerCamera = Camera.main;
		}
	}

    protected void Update()
    {
		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");

		if(!Mathf.Approximately(hInput + vInput, 0f))
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
