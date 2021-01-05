using Bladiator.Entities.Enemies;
using Bladiator.Entities.Players;
using Bladiator.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.EnemyAttacks
{ 
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyThrowGrenade : EnemyAttackBase
    {
        [Header("Objects")]
        [SerializeField] private GameObject m_GrenadePrefab;

        [Tooltip("The range of the attack in which entities will be affected.")]
        [SerializeField] private float m_GrenadeAoERange = 3;
        
        [Tooltip("After how long will the grenade be able to explode?")]
        [SerializeField] private float m_GrenadeArmDelay = 0.5f;

        protected override void Activate(Enemy enemy, Player target)
        {
            Vector3 spawnPos = enemy.transform.position + Vector3.up * ((enemy.GetComponent<Collider>().bounds.extents.y) + 0.1f);

            Grenade grenade = Instantiate(m_GrenadePrefab, spawnPos, Quaternion.identity).GetComponent<Grenade>();

            grenade.Initialize(GetStats().Damage, GetStats().Knockback, GetStats().KnockbackDuration, m_GrenadeAoERange, m_GrenadeArmDelay);

            Launch(grenade.GetComponent<Rigidbody>(), target.transform.position - Vector3.down * 0.3f, 100);
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

            float T = T_lowEnergy;// choose T_max, T_min, or some T in-between like T_lowEnergy

            // Convert from time-to-hit to a launch velocity:
            Vector3 velocity = toTarget / T - Physics.gravity * T / 2f;

            // Apply the calculated velocity (do not use force, acceleration, or impulse modes)
            objectToLaunch.AddForce(velocity, ForceMode.VelocityChange);
        }
    }
}