using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding2D
{
    /// <summary>
    /// Manages pathfinding requests and ensures they are processed 
    /// asynchronously and returned safely.
    /// </summary>
    public class PathRequestManager : MonoBehaviour
    {
        /// <summary>
        /// Queue holding the results of completed pathfinding requests.
        /// </summary>
        Queue<PathResult> results = new Queue<PathResult>();

        /// <summary>
        /// Singleton instance of the <see cref="PathRequestManager"/>.
        /// Used internally to route requests and results.
        /// </summary>
        static PathRequestManager instance;

        /// <summary>
        /// Reference to the pathfinding algorithm (A* implementation).
        /// </summary>
        AStarPathfinding pathfinding;

        void Awake()
        {
            instance = this;
            pathfinding = GetComponent<AStarPathfinding>();
        }

        void Update()
        {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }
        }

        /// <summary>
        /// Submits a new pathfinding request using the default pathfinding instance.
        /// </summary>
        public static void RequestPath(PathRequest request)
        {
            RequestPath(request, instance.pathfinding);
        }

        /// <summary>
        /// Submits a new pathfinding request using a specified pathfinding instance.
        /// </summary>
        public static void RequestPath(PathRequest request, AStarPathfinding pathfinding)
        {
            pathfinding.FindPath(request, instance.FinishedProcessingPath);
        }

        /// <summary>
        /// Called internally when a pathfinding request has finished processing.  
        /// Enqueues the result for processing on the main thread.
        /// </summary>
        /// <param name="result">The result of the pathfinding operation.</param>
        private void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }
    }
}