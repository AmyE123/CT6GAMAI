namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MovementRange : MonoBehaviour
    {
        [SerializeField] private GridSelector gridSelector;

        public List<Node> ReachableNodes;
        public List<Node> Nodes;

        /// <summary>
        /// Uses Dijkstra's Algorithm to calculate the range that the unit can move
        /// </summary>
        /// <param name="start">The starting node</param>
        /// <param name="movementPoints">How much the unit can move</param>
        /// <returns></returns>
        public List<Node> CalculateMovementRange(Node start, int movementPoints)
        {
            foreach (NodeManager nodeManager in gridSelector.Nodes)
            {
                nodeManager.Node.Visited = false;
                nodeManager.Node.Distance = int.MaxValue;
            }

            // Initialize the starting node's distance to 0
            start.Distance = 0;

            // Priority queue to select the node with the smallest distance
            var priorityQueue = new PriorityQueue<Node>();
            priorityQueue.Enqueue(start, start.Distance);

            if (!Nodes.Contains(start))
            {
                Nodes.Add(start);
            }

            // A list to hold all nodes within movement range
            var reachableNodes = new List<Node>();

            while (!priorityQueue.IsEmpty())
            {
                // Get the node with the smallest distance
                Node current = priorityQueue.Dequeue();

                if (current.Visited)
                {
                    continue;
                }

                current.Visited = true;

                // If the current node is within movement points, add to reachable nodes
                if (current.Distance <= movementPoints)
                {
                    reachableNodes.Add(current);

                    if (!Nodes.Contains(current))
                    {
                        Nodes.Add(current);
                    }
                }

                // Loop through each neighbor of the current node
                foreach (Node neighbor in current.Neighbors)
                {
                    if (neighbor.Visited) continue; // Skip already visited neighbors

                    // Calculate the tentative distance to the neighbor
                    int tentativeDistance = current.Distance + neighbor.Cost;

                    // If the tentative distance is less than the neighbor's recorded distance
                    if (tentativeDistance < neighbor.Distance)
                    {
                        // Update the neighbor's distance
                        neighbor.Distance = tentativeDistance;

                        // Sets the predecessor for pathfinding
                        neighbor.Predecessor = current;

                        // If the neighbor has not been visited or the tentative distance is better, enqueue it
                        if (!neighbor.Visited)
                        {
                            priorityQueue.Enqueue(neighbor, tentativeDistance);
                        }
                    }
                }
            }

            // Reset visited and distance for all nodes for the next calculation
            foreach (NodeManager nodeManager in gridSelector.Nodes)
            {
                nodeManager.Node.Visited = false;
                nodeManager.Node.Distance = int.MaxValue;
            }

            return Nodes;
        }

        public List<Node> ReconstructPath(Node start, Node target)
        {
            List<Node> path = new List<Node>();
            Node current = target;

            while (current != null && current != start)
            {
                path.Add(current);
                current = current.Predecessor;
            }

            path.Add(start); // Add the start node
            path.Reverse(); // Reverse the list to get the path from start to target

            return path;
        }

        public void ResetNodes()
        {
            Nodes.Clear();
            ReachableNodes.Clear();
        }
    }
}