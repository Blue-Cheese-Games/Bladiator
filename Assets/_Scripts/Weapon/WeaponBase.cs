using System;
using Bladiator.Entities;
using Bladiator.Entity;
using UnityEngine;

namespace Bladiator.Weapons
{
    [RequireComponent(typeof(Animator))]
    public class WeaponBase : MonoBehaviour
    {
        public static event Action<Weapon> OnEntityAttack = null;

        [SerializeField] private Weapon m_Weapon = new Weapon();
        private bool m_CanAttack = false;
        
        /// <summary>
        /// Attack an entity
        /// </summary>
        /// <param name="entity"> Entity to attack </param>
        public void AttackEntity(EntityBase entity)
        {
            if (!m_CanAttack)
                return;

            try
            {
                if (ChargeTimer())
                {
                    // Damage the entity
                    IDestructable damageable = entity.GetComponent<IDestructable>();
                    damageable.Damage((int)m_Weapon.Damage);

                    OnEntityAttack?.Invoke(m_Weapon);
                    m_CanAttack = false;
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"Did not deal damage: {exception}");
            }
        }
        
        /// <summary>
        /// Countdown the 'ChangeTimer' on a weapon
        /// </summary>
        /// <returns> True when the countdown is over </returns>
        private bool ChargeTimer()
        {
            while (m_Weapon.ChargeTimer > 0)
                m_Weapon.ChargeTimer -= 1 * Time.deltaTime;

            return true;
        }

        private void Update()
        {
            if (!m_CanAttack)
                AttackTimer();
        }

        /// <summary>
        /// Countdown the 'UseCooldown' timer on a weapon
        /// </summary>
        private void AttackTimer()
        {
            if (!m_CanAttack)
            {
                while (m_Weapon.UseCooldown > 0)
                    m_Weapon.UseCooldown -= 1 * Time.deltaTime;

                m_CanAttack = true;
            }
        }
    }

    [Serializable]
    public struct Weapon
    {
        public string Name;
        public Sprite SpriteAsset;
        public float Damage;
        public Animator Animator;

        /// <summary>
        /// The time it takes before hitting the entity
        /// </summary>
        public float ChargeTimer;

        /// <summary>
        /// The time it takes before you can use the weapon again
        /// </summary>
        public float UseCooldown;
    }
}
