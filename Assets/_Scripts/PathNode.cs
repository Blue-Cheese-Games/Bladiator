using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Pathing
{
    public class PathNode : MonoBehaviour
    {
        [SerializeField] List<PathNode> m_ContinuedNodes = new List<PathNode>();

        private void Awake()
        {
            PathingManager.Instance.AddNode(this);
        }

        public void AddContinuedNode(PathNode newContinuedNode)
        {
            m_ContinuedNodes.Add(newContinuedNode);
        }

        public float GetDistanceToTarget(Vector3 target)
        {
            return Vector3.Distance(transform.position, target);
        }

        public PathNode GetContinuedNodeClosestToGoal(PathNode goal, ref List<PathNode> excludes, ref Stack<PathNode> backTrack)
        {
            // DEBUG >>>>>>>>>>>
            if (name.Contains("6"))
            {

            }
            // <<<<<<<<<<<<

            PathNode closestNodeFound = null;

            // Filter the nodes! (excludes backtracks). <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            List<PathNode> continuedNodesFiltered = FilterNodeList(backTrack.ToArray(), excludes.ToArray());

            closestNodeFound = FindClosestNodeFromList(goal, continuedNodesFiltered);

            if(closestNodeFound != null)
            {
                // A node was found without using the "backtrack" or "excludes".
                return closestNodeFound;
            }
            else
            {
                // No node could be found, backtrack to the previous list, and exclude this node.
                excludes.Add(this);
                return backTrack.Pop();

            }
        }

        private List<PathNode> FilterNodeList(params PathNode[][] filterLists)
        {
            List<PathNode> filteredNodes = new List<PathNode>(m_ContinuedNodes);

            foreach (PathNode[] filterList in filterLists)
            {
                foreach (PathNode nodeToFilter in filterList)
                {
                    filteredNodes.Remove(nodeToFilter);
                }
            }

            return filteredNodes;
        }

        private PathNode FindClosestNodeFromList(PathNode goal, List<PathNode> nodesToCheck)
        {
            float closestToGoalDistance = float.MaxValue;
            PathNode closestNode = null;

            float activeDistance;

            foreach (PathNode node in nodesToCheck)
            {
                activeDistance = Vector3.Distance(node.transform.position, goal.transform.position);
                if (activeDistance < closestToGoalDistance)
                {
                    closestToGoalDistance = activeDistance;
                    closestNode = node;
                }
            }

            return closestNode;
        }
    }
}
