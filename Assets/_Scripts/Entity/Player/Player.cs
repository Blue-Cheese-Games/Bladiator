using Bladiator.Entities;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Entity.Player
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
		}

		public override void Move()
		{
			m_PlayerController.MoveHandle();
		}
	}
}