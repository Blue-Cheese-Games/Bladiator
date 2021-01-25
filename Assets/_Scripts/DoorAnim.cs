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
		StartCoroutine(CloseDoor());
	}

	IEnumerator CloseDoor()
	{
		yield return new WaitForSeconds(1.5f);
		anim.Play("CloseDoor");
	}

	private void OnSpawnStarted()
	{
		anim.Play("OpenDoor");
	}
}
