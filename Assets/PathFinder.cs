using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Pathing
{
    public class PathFinder : MonoBehaviour
    {
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
            PathNode goal = PathingManager.Instance.FindNearestNodeTo(to, true);

            // Check if both a start & goal node could be found.
            if(start == null) { Debug.Log($"No PathNode could be found as a start for the path of \"{name}\""); return new Stack<PathNode>(); }
            if(goal == null) { Debug.Log($"No PathNode could be found as a goal for the path of \"{name}\""); return new Stack<PathNode>(); }

            PathNode head = start;

            // DEBUG >>>>>>>>>>>>>>>>>>>>>>
            print("start: " + start.name);
            print("goal: " + goal.name);

            int counter = 0; 
            // <<<<<<<<<<<<<<<<<<<<<<<<<<

            while(head != goal)
            {
                // DEBUG >>>
                if(counter >= 100) 
                {
                    print("broke");
                    break;
                }
                counter += 1;

                // <<<

                PathNode nextNode = head.GetContinuedNodeClosestToGoal(goal, ref excludedNodes, ref backTracks, ref visitedNodes);
                
                if(backTracks.Count > 0)
                {
                    backTracks.Peek().passedNodesSinceChoice.Push(head);
                }
                
                head = nextNode;
            }

            print($"reached goal: {goal.gameObject.name}");

            // The final path to be returned.
            Stack<PathNode> path = new Stack<PathNode>();

            path.Push(goal);

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

            string result = "Final Path: ";

            foreach (PathNode node in path)
            {
                result += $" {node.gameObject.name} ";
            }

            print(result);
            return path;
        }
        #endregion
    }

    public struct BackTrack
    {
        // The node that made a choice.
        public PathNode choiceNode;


        // The nodes that the head passed since passing "choiceNode".
        public Stack<PathNode> passedNodesSinceChoice;
    }
}
