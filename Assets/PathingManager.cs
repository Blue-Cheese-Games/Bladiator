using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Pathing
{
    public class PathingManager : MonoBehaviour
    {
        public static PathingManager Instance;

        [SerializeField] private LayerMask m_IgnoredCollisionCheckLayers;

        private List<PathNode> m_Nodes = new List<PathNode>();

        private bool m_setup;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            for (int selected = 0; selected < m_Nodes.Count; selected++)
            {
                for (int target = 0; target < m_Nodes.Count; target++)
                {
                    if (selected == target) { continue; }

                    if(!CheckForCollision(m_Nodes[selected].transform.position, m_Nodes[target].transform.position))
                    {
                        // "selected" has a clear sight to "target".

                        m_Nodes[selected].AddContinuedNode(m_Nodes[target]);
                    }
                }
            }
        }

        #region "CheckForCollision" check if there is a clear line-of-sight from "from" to "to".
        /// <summary>
        /// Check if the line of sight "self" to "to" doesn't collide with anything (true = collision, false = no collision).
        /// </summary>
        /// <returns>true = collision, false = no collision</returns>
        private bool CheckForCollision(Vector3 self, Vector3 to)
        {
            RaycastHit[] hits;

            Vector3 direction = to - self;

            float distanceBetweenToAndFrom = Vector3.Distance(to, self);

            hits = Physics.RaycastAll(self, direction, distanceBetweenToAndFrom, ~m_IgnoredCollisionCheckLayers);

            foreach (RaycastHit hit in hits)
            {
                // Check if the collision is the "from" or "to", if so, skip the collision check, as it is intended.
                if(hit.transform.position == self || hit.transform.position == to) { continue; }

                if (hit.collider != null)
                {
                    return true;
                }
            }
            
            return false;
        }
        #endregion

        /// <summary>
        /// Finds the closest node to "target" and returns it (can also check for a clear line-of-sight).
        /// </summary>
        /// <returns>The closest node to "target"</returns>
        public PathNode FindNearestNodeTo(Vector3 target, bool checkCollisions)
        {
            float closestDistance = float.MaxValue;
            PathNode closestNode = null;

            float activeDistance;

            foreach (PathNode node in m_Nodes)
            {
                activeDistance = Vector3.Distance(node.transform.position, target);

                switch (checkCollisions)
                {
                    case true:
                        if (!CheckForCollision(node.transform.position, target))
                        {
                            if (activeDistance < closestDistance)
                            {
                                closestDistance = activeDistance;
                                closestNode = node;
                            }
                        }
                        break;

                    case false:
                        if (activeDistance < closestDistance)
                        {
                            closestDistance = activeDistance;
                            closestNode = node;
                        }
                        break;
                }
            }

            return closestNode;
        }

        public void AddNode(PathNode nodeToAdd)
        {
            m_Nodes.Add(nodeToAdd);
        }

        public bool IsSetup()
        {
            return m_setup;
        }
    }
}
