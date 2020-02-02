using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PathLine : MonoBehaviour
{
	private NPC _npc;
	private LineRenderer _lineRenderer;

	protected void Awake()
	{
		_lineRenderer = gameObject.GetComponent<LineRenderer>();
	}
	
	protected void Update()
	{
		if(_npc != null && _npc.CurrentTarget != null)
		{
			_lineRenderer.SetPosition(0, _npc.transform.position);
			_lineRenderer.SetPosition(1, _npc.CurrentTarget.GetNavMeshOrigin());
		}
	}

	public void SetAgent(NPC agent)
	{
		_npc = agent;
	}
}
