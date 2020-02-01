using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField]
	private NPCDirector _npcDirector;

	protected void Start()
	{
		_npcDirector.SetDirectorState(NPCDirector.State.Active);
	}
}
