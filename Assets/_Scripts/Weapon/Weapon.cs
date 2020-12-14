using UnityEngine;

namespace Bladiator.Weapons
{
    public class Weapon : WeaponBase
    {
        [Header("Weapon")]
        public WeaponObject WeaponObject = null;
        
        public GameObject Player = null;
        
        public ParticleSystem HitParticle = null;
    }
}
