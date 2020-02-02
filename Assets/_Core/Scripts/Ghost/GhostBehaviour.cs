using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public Breakable[] breakableObjects;
	public float waitTimeStart;
    public int waitTimeBetweenBreakingMin;
    public int waitTimeBetweenBreakingMax;

    private int num;
    private Breakable theObject;
    private GhostMovement ghostMovement;

    private Animator _anim;
    public string BreakAnimString = "Breaking";

    private void Awake()
    {
        ghostMovement = GetComponent<GhostMovement>();
        _anim = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
		yield return new WaitForSeconds(waitTimeStart);
        FindRandomObject();
    }

	protected void OnDestroy()
	{
		transform.DOKill();
		SetCurrentBreakingObject(null);
	}

    public void FindRandomObject()
    {
		transform.DOKill();
		breakableObjects =  BreakablesCommunicator.Instance.GetBreakables(x => x.BreakState == Breakable.State.Unbroken);
        if (breakableObjects.Length > 0)
        {
            num = UnityEngine.Random.Range(0, breakableObjects.Length);
			SetCurrentBreakingObject(breakableObjects[num]);
            ghostMovement.MoveTowardsObject(theObject.gameObject);
            StartCoroutine(BreakObject());
        }   
    }

	private void SetCurrentBreakingObject(Breakable obj)
	{
		if(theObject != null)
		{
			theObject.StateChangedEvent -= OnStateChangedEvent;
		}

		theObject = obj;

		if(theObject != null)
		{
			theObject.StateChangedEvent += OnStateChangedEvent;
		}
	}

	private void OnStateChangedEvent(Breakable breakable, Breakable.State newState)
	{
		if (newState != Breakable.State.Broken)
		{
			StopAllCoroutines();
			_anim.SetBool(BreakAnimString, false);
			transform.DOKill();
			transform.DOShakeScale(1f, 0.5f, 5).SetDelay(0.5f).OnComplete(() => 
			{
				FindRandomObject();
			});
		}
	}

	private IEnumerator BreakObject()
    {
        yield return new WaitForSeconds(1f);
        _anim.SetBool(BreakAnimString, true);
        yield return new WaitForSeconds(3f);
		if (theObject != null)
		{
			theObject.Break();
		}
        _anim.SetBool(BreakAnimString, false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(waitTimeBetweenBreakingMin, waitTimeBetweenBreakingMax));
        FindRandomObject();
    }
}
