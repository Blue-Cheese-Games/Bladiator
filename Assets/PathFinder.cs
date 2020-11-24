using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladiator.Pathing
{
    public class PathFinder : MonoBehaviour
    {
        private void Start()
        {
            FindPath(transform.position, FindObjectOfType<Bladiator.Entities.Players.Player>().transform.position); // DEBUG ----------
        }

        public void FindPath(Vector3 from, Vector3 to)
        {
            // Stack of "BackTrack"'s which will make the head backtrack when hitting a deadend.
            Stack<BackTrack> backTracks = new Stack<BackTrack>();

            // List to keep track of paths that lead to a deadend.
            List<PathNode> excludedNodes = new List<PathNode>();


            PathNode start = PathingManager.Instance.FindNearestNodeTo(from, true);
            PathNode goal = PathingManager.Instance.FindNearestNodeTo(to, true);

            // Check if both a start & goal node could be found.
            if(start == null) { Debug.Log($"No PathNode could be found as a start for the path of \"{name}\""); return; }
            if(goal == null) { Debug.Log($"No PathNode could be found as a goal for the path of \"{name}\""); return; }

            PathNode current = start;

            // DEBUG >>>>>>>>>>>>>>>>>>>>>>
            print("start: " + start.name);
            print("goal: " + goal.name);

            int counter = 0; 
            // <<<<<<<<<<<<<<<<<<<<<<<<<<

            while(current != goal)
            {
                // DEBUG >>>
                if(counter >= 100) { print("broke"); break; }
                counter += 1;
                // <<<

                print(current.gameObject.name);

                PathNode nextNode = current.GetContinuedNodeClosestToGoal(goal, ref excludedNodes, ref backTracks);
                
                if(backTracks.Count > 0)
                {
                    backTracks.Peek().passedNodesSinceChoice.Push(current);
                }
                
                current = nextNode;
            }

            print($"reached goal: {goal.gameObject.name}");

            // The final path to be returned.
            Stack<PathNode> path = new Stack<PathNode>();

            //while (backTrackToLastChoiceNode.Count > 0)
            //{
            //    path.Push(backTrackToLastChoiceNode.Pop());
            //}

            string result = "Final Path: ";

            foreach (PathNode node in path)
            {
                result += $" {node.gameObject.name} ";
            }

            result += " ----- ChoiceNodes: ";

            foreach (BackTrack backTrack in backTracks)
            {
                result += $" node: {backTrack.choiceNode} ";
                foreach (PathNode node in backTrack.passedNodesSinceChoice)
                {
                    result += $" {node.gameObject.name} ";
                }
            }

            print(result);
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
