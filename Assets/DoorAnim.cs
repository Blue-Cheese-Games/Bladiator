using System.Collections;
using System.Collections.Generic;
using Bladiator;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
	private Animator anim;
	public void Start()
	{
		anim = GetComponent<Animator>();
		WaveSystem.Instance.OnSpawnStarted += OnSpawnStarted;
		WaveSystem.Instance.OnSpawnDone += OnSpawnDone;
	}

	private void OnSpawnDone()
	{
		anim.Play("CloseDoor");
	}

	private void OnSpawnStarted()
	{
		anim.Play("OpenDoor");
	}
}
