using UnityEngine;

[RequireComponent(typeof(RepairTarget), typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
	public RepairTarget RepairTarget
	{
		get; private set;
	}

	public PlayerMovement Movement
	{
		get; private set;
	}

	protected void Awake()
	{
		RepairTarget = gameObject.GetComponent<RepairTarget>();
		Movement = gameObject.GetComponent<PlayerMovement>();
	}
}
