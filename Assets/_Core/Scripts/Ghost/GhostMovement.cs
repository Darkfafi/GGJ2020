using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public GameObject theObject;
    public Renderer[] myRenderer;

	[SerializeField]
	private Behaviour _haloComp;

	[SerializeField]
	private Vector3 _offset = Vector3.zero;

    public void MoveTowardsObject(GameObject obj)
    {
        Vector3 diff = obj.transform.position - transform.position;

        foreach (Renderer r in myRenderer)
        {
            r.material.DOFade(0f, 0.5f).SetEase(Ease.InOutBounce);
        }

		_haloComp.enabled = false;

		transform.DOLookAt(obj.transform.position, 0.5f, AxisConstraint.Y, Vector3.up);
        transform.DOMove(transform.position + diff.normalized * 2f, 1f).OnComplete(() =>
		{
			_haloComp.enabled = true;
			Vector3 targetMovementPos = obj.transform.position + _offset;
            transform.position = targetMovementPos + (transform.position - targetMovementPos).normalized * 4f;
            transform.DOMove(targetMovementPos + diff.normalized * -2.5f, 1f);
            foreach (Renderer r in myRenderer)
            {
                r.material.DOFade(0.75f, 0.5f).SetEase(Ease.InOutBounce);
            }
        });
    }
}
