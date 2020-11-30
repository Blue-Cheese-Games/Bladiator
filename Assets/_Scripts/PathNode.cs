using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladiator.Collisions;

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

        public PathNode GetContinuedNodeClosestToGoal(PathNode goal, ref List<PathNode> excludes, ref Stack<BackTrack> backTrack, ref List<PathNode> visitedNodes)
        {
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

            closestNodeFound = FindClosestNodeFromList(goal, continuedNodesFiltered, visitedNodes, ref backTrack);

            if(closestNodeFound != null)
            {
                // A node was found without using the "backtrack" or "excludes".

                if(backTrack.Count <= 1)
                {
                    return closestNodeFound;
                }

                // Check if this node is currently backtracking.
                if (visitedNodes.Contains(this))
                {
                    // Backtracking.

                    //exclude and pop this node.
                    excludes.Add(this);
                    backTrack.Pop();

                    // Check if the backtracking node before this one, is closer to the chosen node.
                    if (backTrack.Peek().choiceNode.GetDistanceToTarget(closestNodeFound.transform.position) <= GetDistanceToTarget(closestNodeFound.transform.position))
                    {
                        // The previous backtracking node is closer to the chosen node then this node.
                        
                        // Check if there is a clear line-of-sight between the previous backtracking node and the chosenNode.
                        if (CollisionCheck.CheckForCollision(backTrack.Peek().choiceNode.transform.position, closestNodeFound.transform.position, PathingManager.Instance.GetIgnoreLayers()));
                        {
                            // choose the backtracking node before this one.
                            closestNodeFound = backTrack.Peek().choiceNode;
                        }
                    }
                }

                visitedNodes.Add(this);
                return closestNodeFound;
            }
            else
            {
                // No node could be found, backtrack to the previous node, and exclude this node.

                excludes.Add(this);

                visitedNodes.Add(this);
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

        private PathNode FindClosestNodeFromList(PathNode goal, List<PathNode> nodesToCheck, List<PathNode> visitedNodes, ref Stack<BackTrack> backTrack)
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

            if(m_ContinuedNodes.Count > 1 && !visitedNodes.Contains(this) && closestNode != null)
            {
                BackTrack newBackTrack = new BackTrack()
                {
                    choiceNode = this,
                    passedNodesSinceChoice = new Stack<PathNode>()
                };

                backTrack.Push(newBackTrack);
            }

            if(closestNode == null && visitedNodes.Contains(this))
            {
                backTrack.Pop();
            }

            return closestNode;
        }
    }
}
