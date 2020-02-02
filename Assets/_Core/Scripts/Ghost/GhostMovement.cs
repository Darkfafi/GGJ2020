using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public GameObject theObject;
    public Renderer[] myRenderer;
    public void MoveTowardsObject(GameObject obj)
    {
        Vector3 diff = obj.transform.position - transform.position;

        foreach (Renderer r in myRenderer)
        {
            r.material.DOFade(0f, 0.5f).SetEase(Ease.InOutBounce);
        }

        transform.DOLookAt(obj.transform.position, 0.5f, AxisConstraint.Y, Vector3.up);
        transform.DOMove(transform.position + diff.normalized * 2f, 1f).OnComplete(() =>
        {
            transform.position = obj.transform.position + (transform.position - obj.transform.position).normalized * 4f;
            transform.DOMove(obj.transform.position + diff.normalized * -2.5f, 1f);
            foreach (Renderer r in myRenderer)
            {
                r.material.DOFade(0.75f, 0.5f).SetEase(Ease.InOutBounce);
            }
        });
    }
}
