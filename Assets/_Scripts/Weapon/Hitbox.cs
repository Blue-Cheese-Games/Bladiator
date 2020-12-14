using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public Action<Collider> OnHit;

	private void OnTriggerEnter(Collider other)
	{
		OnHit?.Invoke(other);
	}
}
