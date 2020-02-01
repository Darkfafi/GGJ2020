using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public GameObject theObject;
    public Renderer myRenderer;

    public void MoveTowardsObject(GameObject obj)
    {
        Vector3 diff = obj.transform.position - transform.position;
        myRenderer.material.DOFade(0f, 0.5f).SetEase(Ease.InOutBounce);

		transform.DOMove(transform.position + diff.normalized * 2f, 1f).OnComplete(() =>
        {
            transform.position = obj.transform.position + (transform.position - obj.transform.position).normalized * 4f;
            transform.DOMove(obj.transform.position + diff.normalized * -2.5f, 1f);
            myRenderer.material.DOFade(1f, 0.5f).SetEase(Ease.InOutBounce);
        });
    }
}
