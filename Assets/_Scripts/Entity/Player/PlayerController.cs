using System;
using Bladiator.Entities;
using UnityEngine;

namespace Bladiator.Entity.Player
{
	[RequireComponent(typeof(Player))]
	public class PlayerController : MonoBehaviour
	{
		Player m_Player;
		void Awake()
		{
			m_Player = GetComponent<Player>();
		}

		public void Update()
		{
		}
	}
}