using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public Breakable[] breakableObjects;
    public int waitTimeBetweenBreakingMin;
    public int waitTimeBetweenBreakingMax;

    private int num;
    [SerializeField]
    private Breakable theObject;
    private GhostMovement ghostMovement;

    private Animator _anim;
    public string BreakAnimString = "Breaking";

    private void Awake()
    {
        ghostMovement = GetComponent<GhostMovement>();
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        FindRandomObject();
    }

    public void FindRandomObject()
    {
        breakableObjects =  BreakablesCommunicator.Instance.GetBreakables(x => x.BreakState == Breakable.State.Unbroken);
        if (breakableObjects.Length > 0)
        {
            num = Random.Range(0, breakableObjects.Length);
            theObject = breakableObjects[num];
            ghostMovement.MoveTowardsObject(theObject.gameObject);
            StartCoroutine(BreakObject());
        }   
    }
    private IEnumerator BreakObject()
    {
        _anim.SetBool(BreakAnimString, true);
        yield return new WaitForSeconds(3f);
        
        theObject.Break();
        
        yield return new WaitForSeconds(Random.Range(waitTimeBetweenBreakingMin, waitTimeBetweenBreakingMax));
        _anim.SetBool(BreakAnimString, false);
        FindRandomObject();
        
    }
   
 
}
