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

        public PathNode GetContinuedNodeClosestToGoal(PathNode goal, ref List<PathNode> excludes, ref Stack<BackTrack> backTrack)
        {
            // DEBUG >>>>>>>>>>>
            if (name.Contains("6"))
            {

            }
            // <<<<<<<<<<<<

            PathNode closestNodeFound = null;

            List<PathNode> continuedNodesFiltered = new List<PathNode>(m_ContinuedNodes);

            // Filter the nodes.
            continuedNodesFiltered = FilterNodeList(continuedNodesFiltered, excludes.ToArray());

            if (backTrack.Count > 0)
            {
                foreach (BackTrack b in backTrack)
                {
                    continuedNodesFiltered = FilterNodeList(continuedNodesFiltered, b.passedNodesSinceChoice.ToArray());
                }
            }

            closestNodeFound = FindClosestNodeFromList(goal, continuedNodesFiltered, ref backTrack);

            if(closestNodeFound != null)
            {
                // A node was found without using the "backtrack" or "excludes".
                return closestNodeFound;
            }
            else
            {
                // No node could be found, backtrack to the previous node, and exclude this node.
                excludes.Add(this);
                return backTrack.Peek().choiceNode;
            }
        }

        private List<PathNode> FilterNodeList(List<PathNode> filteredNodes, params PathNode[][] filterLists)
        {
            foreach (PathNode[] filterList in filterLists)
            {
                foreach (PathNode nodeToFilter in filterList)
                {
                    filteredNodes.Remove(nodeToFilter);
                }
            }

            return filteredNodes;
        }

        private PathNode FindClosestNodeFromList(PathNode goal, List<PathNode> nodesToCheck, ref Stack<BackTrack> backTrack)
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

            if(m_ContinuedNodes.Count > 1)
            {
                BackTrack newBackTrack = new BackTrack()
                {
                    choiceNode = this,
                    passedNodesSinceChoice = new Stack<PathNode>()
                };

                backTrack.Push(newBackTrack);
            }

            return closestNode;
        }
    }
}
