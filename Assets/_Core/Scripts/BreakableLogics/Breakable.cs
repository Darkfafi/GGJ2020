using UnityEngine;

public class Breakable : MonoBehaviour, INavMeshTarget
{
	public delegate void BreakbleStateHandler(Breakable breakable, State newState);
	public event BreakbleStateHandler StateChangedEvent;

	public enum State
	{
		Unbroken,
		Broken,
		PermanentlyBroken
	}

	[SerializeField]
	private Transform _targetTransform;

	[SerializeField]
	private MeshRenderer _fixedMesh;

	[SerializeField]
	private MeshRenderer _brokenMesh;

	[SerializeField]
	private GameObject _brokenParticles;

	public State BreakState
	{
		get; private set;
	}

	protected void Awake()
	{
		BreakablesCommunicator.Instance.RegisterBreakable(this);
	}

	protected void Start()
	{
		SetState(State.Unbroken);
	}

	protected void OnDestroy()
	{
		BreakablesCommunicator.Instance.UnregisterBreakable(this);
	}

	public Vector3 GetNavMeshOrigin()
	{
		return _targetTransform != null ? _targetTransform.position : transform.position;
	}

	public void Break()
	{
		SetState(State.Broken);
	}

	public void PermanentlyBreak()
	{
		SetState(State.PermanentlyBroken);
	}

	public void Repair()
	{
		SetState(State.Unbroken);
	}

	private void SetState(State state)
	{
		if (BreakState == State.PermanentlyBroken)
		{
			Debug.LogErrorFormat("Can't go to state {0} for it is permanently broken", state);
			return;
		}

		bool isBroken = state != State.Unbroken;
		if(_fixedMesh != null)
		{
			_fixedMesh.enabled = !isBroken;
		}

		if(_brokenMesh != null)
		{
			_brokenMesh.enabled = isBroken;
		}

		if (_brokenParticles != null)
		{
			_brokenParticles.SetActive(isBroken);
		}

		BreakState = state;
		FireStateChangedEvent();
	}

	private void FireStateChangedEvent()
	{
		if (StateChangedEvent != null)
		{
			StateChangedEvent(this, BreakState);
		}
	}
}
