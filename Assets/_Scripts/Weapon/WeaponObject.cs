using UnityEngine;

namespace Bladiator.Weapons
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/WeaponBase", order = 0)]
    public class WeaponObject : ScriptableObject
    {
        public WeaponCoreData WeaponCoreData;
        public WeaponAestheticData WeaponAestheticData;
    }
    
    [System.Serializable]
    public struct WeaponCoreData
    {
        public float Damage;
        public float acceleration;
    }
    
    [System.Serializable]
    public struct WeaponAestheticData
    {
        public Sprite Sprite;
        public ParticleSystem HitParticle;
        public AudioClip HitAudio;
    }
}