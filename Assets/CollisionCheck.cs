using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Collisions
{
    public static class CollisionCheck
    {
        #region "CheckForCollision" check if there is a clear line-of-sight from "from" to "to".
        /// <summary>
        /// Check if the line of sight "self" to "to" doesn't collide with anything (true = collision, false = no collision).
        /// </summary>
        /// <returns>true = collision, false = no collision</returns>
        public static bool CheckForCollision(Vector3 self, Vector3 to, LayerMask ignoredCollisionCheckLayers)
        {
            RaycastHit[] hits;

            Vector3 direction = to - self;

            float distanceBetweenToAndFrom = Vector3.Distance(to, self);

            hits = Physics.RaycastAll(self, direction, distanceBetweenToAndFrom, ~ignoredCollisionCheckLayers);

            foreach (RaycastHit hit in hits)
            {
                // Check if the collision is the "from" or "to", if so, skip the collision check, as it is intended.
                if (hit.transform.position == self || hit.transform.position == to) { continue; }

                if (hit.collider != null)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
