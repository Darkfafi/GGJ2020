using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
	public float waitTimeStart;
    public int waitTimeBetweenBreakingMin;
    public int waitTimeBetweenBreakingMax;

    private Breakable theObject;
    private GhostMovement ghostMovement;
	private EntityFilter _breakablesFilter;

    private Animator _anim;
    private AudioSource _audio;

    public AudioClip movingSFX;
    public AudioClip upsetSFX;
    public AudioClip breakingSFX;
    public string BreakAnimString = "Breaking";

    private void Awake()
    {
        ghostMovement = GetComponent<GhostMovement>();
        _audio = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
		_breakablesFilter = EntityFilter.Create(FilterRulesBuilder.SetupNoTagsBuilder().AddHasComponentRule<Breakable>(true).Result(), null, null);
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
		_breakablesFilter.Clean(null, null);
		_breakablesFilter = null;
	}

    public void FindRandomObject()
    {
		transform.DOKill();
		Entity potentialTarget = _breakablesFilter.GetRandom(x => x.GetEntityComponent<Breakable>().BreakState == Breakable.State.Unbroken);

		if (potentialTarget != null)
        {
			SetCurrentBreakingObject(potentialTarget.GetEntityComponent<Breakable>());
            ghostMovement.MoveTowardsObject(theObject.gameObject);
            _audio.PlayOneShot(movingSFX);
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
			transform.DOShakeScale(1f, 0.5f, 5).OnStart(()=>
            {
                _audio.PlayOneShot(upsetSFX);
            }).SetDelay(0.5f).OnComplete(() => 
			{
				FindRandomObject();
			});
		}
	}

	private IEnumerator BreakObject()
    {
        yield return new WaitForSeconds(1f);
        _anim.SetBool(BreakAnimString, true);
        _audio.PlayOneShot(breakingSFX);
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
