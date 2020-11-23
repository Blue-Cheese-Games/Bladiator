using Bladiator.Entities;
using Bladiator.Managers;
using UnityEngine;

namespace Bladiator.Weapons
{
    public class WeaponBase : MonoBehaviour
    {
        private MouseManager m_MouseManager = null;
        
        protected Sprite SetSprite(Weapon weapon)
            => weapon.WeaponObject.WeaponAestheticData.Sprite;
        
        protected float GetDamage(Weapon weapon)
            => weapon.WeaponObject.WeaponCoreData.Damage;

        protected void DealDamage(Weapon weapon, EntityBase entity)
        {
            entity.Damage((int)weapon.WeaponObject.WeaponCoreData.Damage);
        }
        
        /// <summary>
        /// Set the position of the weapon object equal to the mouse
        /// </summary>
        /// <param name="weapon"> Weapon object </param>
        protected void SetPosition(Weapon weapon)
        {
            Vector3 position = new Vector3()
            {   
                x = m_MouseManager.GetMouseAxisAsVector().x,
                y = 0,
                z = m_MouseManager.GetMouseAxisAsVector().y
            };

            weapon.transform.Translate(position * 
                                       (weapon.WeaponObject.WeaponCoreData.acceleration * Time.deltaTime));
        }

        private void Awake()
        {
            m_MouseManager = MouseManager.Instance;
        }
    }
}
