using System.Collections;
using System.Collections.Generic;
using Bladiator.EnemyAttacks;
using UnityEngine;

public class AttackForwarder : MonoBehaviour
{
	public void DoAttack()
	{
		transform.parent.GetComponentInChildren<EnemyMeleeAttack>().DoAttack();
	}
}
