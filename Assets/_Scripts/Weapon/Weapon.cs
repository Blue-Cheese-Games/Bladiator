using System;
using Bladiator.Entities;
using UnityEngine;

namespace Bladiator.Weapons
{
    [RequireComponent(typeof(WeaponBase))]
    public class Weapon : WeaponBase
    {
        public WeaponObject WeaponObject = null;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player")) return;
            
            try
            {
                DealDamage(this, other.GetComponent<EntityBase>());
            }
            catch (Exception exception)
            {
                Debug.Log("No entity object attached.", other.gameObject);
            }
        }

        private void Update()
        {
            SetPosition(this);
        }
    }
}
