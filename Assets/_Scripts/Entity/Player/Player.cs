using Bladiator.Entities;
using Bladiator.Managers;

namespace Bladiator.Entity.Player
{
	public class Player : EntityBase
	{
		private PlayerController m_PlayerController;

		protected override void Awake()
		{
			base.Awake();

			GameManager.Instance.AddPlayer(this);
		}

		public override void Move()
		{
			// Movement.
		}
	}
}