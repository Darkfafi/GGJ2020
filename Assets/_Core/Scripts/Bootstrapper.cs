using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField]
	private NPCDirector _npcDirector;

	[SerializeField]
	private bool _activateNPCsOnStart = true;

	protected void Start()
	{
		if (_activateNPCsOnStart)
		{
			_npcDirector.SetDirectorState(NPCDirector.State.Active);
		}
	}
}
