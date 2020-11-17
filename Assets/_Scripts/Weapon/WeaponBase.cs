using System;
using System.Collections;
using Bladiator.Entities;
using Bladiator.Entity;
using Bladiator.Entity.Player;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class WeaponBase : MonoBehaviour
    {
        public static event Action<Weapon> OnEntityAttack = null;

        [SerializeField] private Player m_WeaponHolder = null;
        [SerializeField] private Weapon m_Weapon = new Weapon();

        private bool m_CanAttack = true;
        private bool m_AttackTimerIsRunning = false;
        
        /// <summary>
        /// Attack an entity
        /// </summary>
        public void Fire()
        {
            if (!m_CanAttack)
                return;

            try
            {
                // Damage the entity
                //IDamageable damageable = entity.GetComponent<IDamageable>();
                //damageable.Damage((int)m_Weapon.Damage);

                OnEntityAttack?.Invoke(m_Weapon);
                m_CanAttack = false;
                m_AttackTimerIsRunning = false;
            }
            catch (Exception exception)
            {
                Debug.Log($"Did not deal damage: {exception}");
            }
        }

        /// <summary>
        /// Countdown the 'UseCooldown' timer on a weapon
        /// </summary>
        private IEnumerator AttackTimer()
        {
            float cooldown = m_Weapon.UseCooldown;
            yield return new WaitForSeconds(cooldown);
            m_CanAttack = true;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                Fire();
#endif

            if (!m_CanAttack && !m_AttackTimerIsRunning)
            {
                m_AttackTimerIsRunning = true;
                StartCoroutine(AttackTimer());
            }
        }
        
        /// <summary>
        /// Set the weapon sprite onto the player
        /// </summary>
        /// <param name="playerWeaponSpriteRenderer"> Weapon holder sprite on the player </param>
        private void SetWeaponSprite(SpriteRenderer playerWeaponSpriteRenderer)
        {
            playerWeaponSpriteRenderer.sprite = m_Weapon.SpriteAsset;
        }
    }

    [Serializable]
    public struct Weapon
    {
        public Sprite SpriteAsset;
        public float Damage;
        public float MaxHitDistance;

        /// <summary>
        /// The time it takes before you can use the weapon again
        /// </summary>
        public float UseCooldown;
    }
}
