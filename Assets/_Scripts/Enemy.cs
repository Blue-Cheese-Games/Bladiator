using Baldiator.Entities;
using Bladiator.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class Enemy : EntityBase
    {
        private EntityBase m_TargetPlayer = null; // Could be changed to Player type later.
        private Rigidbody m_RigidBody = null;

        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();

            List<EntityBase> players = GameManager.Instance.GetPlayers();

            float shortestDistance = 1000f;
            float currentDistance = 0;

            foreach (EntityBase player in players)
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
