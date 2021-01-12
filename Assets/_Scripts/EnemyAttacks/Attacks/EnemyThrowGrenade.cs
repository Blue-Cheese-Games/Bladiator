using Bladiator.Collisions;
using Bladiator.Entities.Enemies;
using Bladiator.Entities.Players;
using Bladiator.Managers.EnemyManager;
using Bladiator.Pathing;
using Bladiator.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{ 
    [RequireComponent(typeof(Rigidbody), typeof(GrenadeLobber))]
    public class EnemyThrowGrenade : EnemyAttackBase
    {
        [Header("Objects")]
        [SerializeField] private GameObject m_GrenadePrefab;

        [Tooltip("The range of the attack in which entities will be affected.")]
        [SerializeField] private float m_GrenadeAoERange = 3;
        
        [Tooltip("After how long will the grenade be able to explode?")]
        [SerializeField] private float m_GrenadeArmDelay = 0.5f;

        [Tooltip("The delay for the grenade to explode and the enemy to notice it has exploded (when the enemy notices the grenade exploding, it will move towards the player again)")]
        [SerializeField] private float m_DelayInOnExploded = 1;

        protected override void Activate(Enemy enemy, Player target)
        {
            GrenadeLobber grenadeLobber = (GrenadeLobber)enemy;

            Vector3 spawnPos = grenadeLobber.transform.position + Vector3.up * ((grenadeLobber.GetComponent<Collider>().bounds.extents.y) + 0.1f);

            Grenade grenade = Instantiate(m_GrenadePrefab, spawnPos, Quaternion.identity).GetComponent<Grenade>();

            grenade.Initialize(GetStats().Damage, GetStats().Knockback, GetStats().KnockbackDuration, m_GrenadeAoERange, m_GrenadeArmDelay);

            grenade.SubscribeToOnExplode(() => {
                if(grenadeLobber != null)
                {
                    StartCoroutine(OnGrenadeExploded(grenadeLobber, m_DelayInOnExploded)); 
                }
            });
            grenadeLobber.SetThrownGrenade(grenade);

            Launch(grenade.GetComponent<Rigidbody>(), target.transform.position - Vector3.down * 0.3f, 100);
        }

        private IEnumerator OnGrenadeExploded(GrenadeLobber grenadeLobber, float delayInActivation)
        {
            yield return new WaitForSeconds(delayInActivation);

            grenadeLobber?.SetExtraState(GrenadeLobberExtraState.MOVE_TOWARDS_PLAYER);
            grenadeLobber?.SetThrownGrenade(null);
        }

        // Method for getting an arch from position to target from:
        // https://gamedev.stackexchange.com/questions/114522/how-can-i-launch-a-gameobject-at-a-target-if-i-am-given-everything-except-for-it
        // By: DMGregory - https://gamedev.stackexchange.com/users/39518/dmgregory
        private void Launch(Rigidbody objectToLaunch, Vector3 target, float speed)
        {
            Vector3 toTarget = target - transform.position;

            // Set up the terms we need to solve the quadratic equations.
            float gSquared = Physics.gravity.sqrMagnitude;
            float b = speed * speed + Vector3.Dot(toTarget, Physics.gravity);
            float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

            // Check whether the target is reachable at max speed or less.
            if (discriminant < 0)
            {
                // Target is too far away to hit at this speed.
                // Abort, or fire at max speed in its general direction?
            }

            float discRoot = Mathf.Sqrt(discriminant);

            // Highest shot with the given max speed:
            float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

            // Most direct shot with the given max speed:
            float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

            // Lowest-speed arc available:
            float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

            // choose T_max, T_min, or some T in-between like T_lowEnergy
            float T = T_lowEnergy;

            // Convert from time-to-hit to a launch velocity:
            Vector3 velocity = toTarget / T - Physics.gravity * T / 2f;

            // Apply the calculated velocity (do not use force, acceleration, or impulse modes)
            objectToLaunch.AddForce(velocity, ForceMode.VelocityChange);
        }
    }
}