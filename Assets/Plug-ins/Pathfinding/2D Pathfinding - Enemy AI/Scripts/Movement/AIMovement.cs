using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding2D
{
    public class AIMovement : MonoBehaviour
    {
        [SerializeField] private SteeringBehaviourManager behaviourManager;
        [SerializeField] private List<SteeringBehaviour> steeringBehaviours;

        [SerializeField] private float nextWaypointDistance = 0.5f;
        [SerializeField] private bool reachClosestWalkableNode = true;

        [Header("Update Delay")]
        [SerializeField] private float aStarUpdateDelay = 1f;
        [SerializeField] private float steeringUpdateDelay = 0.1f;

        [Header("Smoothing")]
        [SerializeField] private float directionUpdateDelay = 0.05f;
        [SerializeField] private float smoothing = 0.02f;

        [SerializeField] private bool showGizmos = true;

        /// <summary>
        /// Checks whether the current path is valid. 
        /// </summary>
        public bool IsPathValid => (path != null && path.Length > 0 && currentWaypoint < path.Length);

        private Vector3[] path;
        private int currentWaypoint;

        private Vector2 prevDirection = Vector2.zero;
        private Vector2 dirSmoothVelocity;

        private float aStarTimer;
        private float steeringTimer;
        private float directionTimer;

        private bool isProcessingPath = false;

        private void Update()
        {
            aStarTimer -= Time.deltaTime;
            steeringTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Calculates a new shortest path between two points using the default pathfinding instance,
        /// and updates steering behaviours.
        /// </summary>
        /// <param name="from">The starting world position.</param>
        /// <param name="to">The target world position.</param>
        public void CalculateShortestPath(Vector3 from, Vector3 to)
        {
            if (aStarTimer <= 0f)
            {
                aStarTimer = aStarUpdateDelay;
                UpdateAStarPath(from, to);
            }
            if (steeringTimer <= 0f)
            {
                steeringTimer = steeringUpdateDelay;
                UpdateSteeringBehavior();
            }
        }

        /// <summary>
        /// Calculates a new shortest path between two points using the specified pathfinding instance,
        /// and updates steering behaviours.
        /// </summary>
        /// <param name="from">The starting world position.</param>
        /// <param name="to">The target world position.</param>
        /// <param name="pathfinding">AStarPathfinding instance used for calculation of the shortest path.</param>
        public void CalculateShortestPath(Vector3 from, Vector3 to, AStarPathfinding pathfinding)
        {
            UpdateAStarPath(from, to, pathfinding);
            UpdateSteeringBehavior();
        }

        /// <summary>
        /// Returns the movement direction the agent should take, 
        /// based on pathfinding and steering behaviour calculations.
        /// </summary>
        /// <param name="from">The current world position of the agent.</param>
        /// <returns>
        /// A smoothed <see cref="Vector2"/> direction to move in.  
        /// Returns the previous direction if the path is invalid or 
        /// waiting for the next update cycle.
        /// </returns>
        public Vector2 GetDirectionToMove(Vector3 from)
        {
            directionTimer -= Time.deltaTime;

            UpdateWaypoint(from);

            if (!IsPathValid) return Vector2.zero;

            if (directionTimer <= 0f)
            {
                directionTimer = directionUpdateDelay;

                Vector2 desiredDirection = (path[currentWaypoint] - from).normalized;
                Vector3 steeringDir = behaviourManager.GetFinalDirection(steeringBehaviours, desiredDirection);

                prevDirection = Vector2.SmoothDamp(prevDirection, steeringDir, ref dirSmoothVelocity, smoothing);
            }

            return prevDirection;
        }

        public void UpdateAStarPath(Vector3 from, Vector3 to)
        {
            if (isProcessingPath) return;

            isProcessingPath = true;

            PathRequestManager.RequestPath(new PathRequest(from, to, reachClosestWalkableNode, OnPathFound));
        }

        private void UpdateAStarPath(Vector3 from, Vector3 to, AStarPathfinding pathfinding)
        {
            PathRequestManager.RequestPath(new PathRequest(from, to, reachClosestWalkableNode, OnPathFound), pathfinding);
        }

        private void OnPathFound(Vector3[] waypoints, bool foundPath)
        {
            isProcessingPath = false;
            if (!foundPath) return;
            path = waypoints;
            currentWaypoint = 0;
        }

        private void UpdateSteeringBehavior()
        {
            foreach (var steeringBehaviour in steeringBehaviours)
            {
                steeringBehaviour.Detect();
            }
        }

        private void UpdateWaypoint(Vector3 from)
        {
            if (!IsPathValid) return;

            float distance = Vector2.Distance(from, path[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                if (currentWaypoint < path.Length - 1)
                {
                    currentWaypoint++;
                }
                else
                {
                    aStarTimer = 0f;
                    steeringTimer = 0f;
                }
            }
        }

        private void OnDisable()
        {
            prevDirection = Vector2.zero;
            directionTimer = 0f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmos || path == null) return;

            for (int i = currentWaypoint; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                if (i == currentWaypoint)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
#endif
    }
}