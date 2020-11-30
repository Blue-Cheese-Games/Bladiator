using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Pathing
{
    public class PathFinder : MonoBehaviour
    {
        private PathNode m_Goal = null;

        #region "FindPath"
        public Stack<PathNode> FindPath(Vector3 from, Vector3 to)
        {
            // Stack of "BackTrack"'s which will make the head backtrack when hitting a deadend.
            Stack<BackTrack> backTracks = new Stack<BackTrack>();

            // List to keep track of paths that lead to a deadend.
            List<PathNode> excludedNodes = new List<PathNode>();

            // A list containg all the nodes that have been visited.
            List<PathNode> visitedNodes = new List<PathNode>();

            PathNode start = PathingManager.Instance.FindNearestNodeTo(from, true);
            m_Goal = PathingManager.Instance.FindNearestNodeTo(to, true);

            // Check if both a start & goal node could be found.
            if(start == null) { return new Stack<PathNode>(); }
            if(m_Goal == null) { return new Stack<PathNode>(); }

            PathNode head = start;

            // Counter just to be sure the game doesnt crash if a path couldn't be found.
            int counter = 0; 
            while(head != m_Goal)
            {
                if(counter >= 100) 
                {
                    print("broke");
                    break;
                }
                counter += 1;

                PathNode nextNode = head.GetContinuedNodeClosestToGoal(m_Goal, ref excludedNodes, ref backTracks, ref visitedNodes);
                
                if(backTracks.Count > 0)
                {
                    backTracks.Peek().passedNodesSinceChoice.Push(head);
                }
                
                head = nextNode;
            }

            // The final path to be returned.
            Stack<PathNode> path = new Stack<PathNode>();

            path.Push(m_Goal);

            while (backTracks.Count > 0)
            {
                while(backTracks.Peek().passedNodesSinceChoice.Count > 0)
                {
                    if (!excludedNodes.Contains(backTracks.Peek().passedNodesSinceChoice.Peek()))
                    {
                        path.Push(backTracks.Peek().passedNodesSinceChoice.Pop());
                    }
                    else
                    {
                        backTracks.Peek().passedNodesSinceChoice.Pop();
                    }
                }
                backTracks.Pop();
            }

            path.Push(start);

            return path;
        }
        #endregion

        public void RerouteToGoal(Vector3 newTarget)
        {
            PathNode newGoal = PathingManager.Instance.FindNearestNodeTo(newTarget, true);

            if (newGoal != null)
            {
                m_Goal = newGoal;
            }
        }
    }

    public struct BackTrack
    {
        // The node that made a choice.
        public PathNode choiceNode;


        // The nodes that the head passed since passing "choiceNode".
        public Stack<PathNode> passedNodesSinceChoice;
    }
}
