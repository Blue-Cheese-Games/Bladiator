using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Baldiator.Entities
{

    public class EntityBase : MonoBehaviour
    {
        [SerializeField] protected float m_Maxhealth = 10f;
        [SerializeField] protected float m_Health = 10f;

        [SerializeField] protected float m_Movespeed = 5f;

        private void Update()
        {
            Move();
        }

        public virtual void Move()
        {
            Debug.Log("The method \"Move\" of this entity has not been overridden, make sure it is.");
        }
    }
}
