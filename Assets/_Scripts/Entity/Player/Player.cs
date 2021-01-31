using Bladiator.Entities;
using Bladiator.Managers;
using Bladiator.UI;
using System.Collections;
using UnityEngine;

namespace Bladiator.Entities.Players
{
	[RequireComponent(typeof(PlayerController))]
	public class Player : EntityBase
	{
		private PlayerController m_PlayerController;

		protected override void Awake()
		{
			base.Awake();

			GameManager.Instance.AddPlayer(this);
			m_PlayerController = GetComponent<PlayerController>();
			OnDeath += m_PlayerController.OnDeath;
		}

		protected override void Start()
		{
			base.Start();
			GameManager.Instance.ResetEvent += ResetEvent;
		}

		private void ResetEvent()
		{
			GameManager.Instance.AddPlayer(this);
			m_Health = m_Maxhealth;
			gameObject.SetActive(true);
		}

		protected override void Update()
		{
			m_PlayerController.MoveHandle();
		}

		public override void Knockback(Vector3 knockback, float knockbackDuration)
		{
			m_PlayerController.Knockback(knockback, knockbackDuration);
			StartCoroutine(ResetAllowedToMove(knockbackDuration));
		}

		protected override IEnumerator ResetAllowedToMove(float delay)
		{
			yield return new WaitForSeconds(delay);
			m_PlayerController.UnlockMovement();
		}
	}
}
