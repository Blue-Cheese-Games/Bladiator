using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladiator.Collisions;

namespace Bladiator.Pathing
{
    public class PathingManager : MonoBehaviour
    {
        public static PathingManager Instance;

        [SerializeField] private LayerMask m_IgnoredCollisionCheckLayers;

        private List<PathNode> m_Nodes = new List<PathNode>();

        private bool m_Setup;

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

                    if(!CollisionCheck.CheckForCollision(m_Nodes[selected].transform.position, m_Nodes[target].transform.position, m_IgnoredCollisionCheckLayers))
                    {
                        // "selected" has a clear sight to "target".

                        m_Nodes[selected].AddContinuedNode(m_Nodes[target]);
                    }
                }
            }
        }

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
                        if (!CollisionCheck.CheckForCollision(node.transform.position, target, m_IgnoredCollisionCheckLayers))
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
            return m_Setup;
        }

        public LayerMask GetIgnoreLayers()
        {
            return m_IgnoredCollisionCheckLayers;
        }
    }
}
