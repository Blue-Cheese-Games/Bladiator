﻿using Bladiator.Entities;
using Bladiator.Managers;
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
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
		}

		private void OnGameStateChange(GameState state)
		{
			gameObject.SetActive(state != GameState.MainMenu);
		}

		private void ResetEvent()
		{
			GameManager.Instance.AddPlayer(this);
			m_Health = m_Maxhealth;
			gameObject.SetActive(false);
		}

		protected override void Update()
		{
			m_PlayerController.MoveHandle();
		}
	}
}
