using Bladiator.Entities;
using Bladiator.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class Enemy : EntityBase
    {
        private Player m_TargetPlayer = null; // Could be changed to Player type later.
        private Rigidbody m_RigidBody = null;

        protected override void Awake()
        {
            base.Awake();

            m_RigidBody = GetComponent<Rigidbody>();


            // Find the nearest player and set it as "m_TargetPlayer".
            List<Player> players = GameManager.Instance.GetPlayers();

            float shortestDistance = 1000f;
            float currentDistance = 0;

            foreach (Player player in players)
            {
                currentDistance = Vector3.Distance(player.transform.position, transform.position);
                if (currentDistance > shortestDistance)
                {
                    m_TargetPlayer = player;
                    shortestDistance = currentDistance;
                }
            }
        }

        public override void Move()
        {
            // Move towards target player.

            Vector3 direction = m_TargetPlayer.transform.position - transform.position;

            m_RigidBody.MovePosition(transform.position + (direction * Time.deltaTime * m_Movespeed));
        }
    }
}
