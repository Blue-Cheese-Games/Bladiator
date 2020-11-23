using UnityEngine;

namespace Bladiator.Weapons
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/WeaponBase", order = 0)]
    public class WeaponObject : ScriptableObject
    {
        public WeaponData WeaponData = new WeaponData();
    }

    [System.Serializable]
    public struct WeaponData
    {
        public float Damage;
        public float DragVelocity;
        public float Reach;
    }
}