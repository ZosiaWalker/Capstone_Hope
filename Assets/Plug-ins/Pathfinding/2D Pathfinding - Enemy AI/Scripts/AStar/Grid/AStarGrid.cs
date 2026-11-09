using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Pathfinding2D
{
    [RequireComponent(typeof(AStarPathfinding))]
    public class AStarGrid : MonoBehaviour
    {
        [Header("Grid properties")]
        [SerializeField] private Vector3 gridCenter = Vector3.zero;
        [SerializeField] private float cellSize;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private bool createGridOnAwake = true;

        [Header("Obstacle properties")]
        [SerializeField] private LayerMask obstacleLayer;
        [Tooltip("The diameter (in world units) used for obstacle detection within each cell. Larger values increase sensitivity to obstacles.")]
        [SerializeField] private float detectionDiameter = 1f;
        [Tooltip("Determines whether diagonal movement is allowed through tight gaps between two adjacent obstacles.")]
        [SerializeField] private bool allowCornerCutting = false;

        [Header("Penalty properties")]
        [Tooltip("The radius around each grid cell that is checked for nearby obstacles. Larger values mean the algorithm will “sense” obstacles from farther away.")]
        [SerializeField] private float penaltyCheckRadius = 0.5f;
        [Range(0, 100)]
        [Tooltip("Controls how strongly the A* algorithm avoids cells near obstacles.")]
        [SerializeField] private int penaltyStrength = 20;

        [Header("Gizmo properties")]
        [SerializeField] private Color walkableColor = Color.green;
        [SerializeField] private Color obstacleColor = Color.red;
        [SerializeField] private bool showGizmos = true;

        /// <summary>
        /// The total number of cells in the grid (width × height).
        /// </summary>
        public int Area
        {
            get
            {
                return width * height;
            }
        }
        public int Width => width;
        public int Height => height;

        public bool AllowCornerCutting => allowCornerCutting;

        private NodeGrid[,] grid;

        public NodeGrid[,] Grid => grid;

        private float detectionRadius => detectionDiameter / 2f;

        void Awake()
        {
            if (createGridOnAwake)
            {
                CreateGrid();
            }
        }

        /// <summary>
        /// Creates the grid by instantiating nodes and detecting obstacles.
        /// </summary>
        public void CreateGrid()
        {
            grid = new NodeGrid[width, height];

            float halfWidth = (width * cellSize) / 2f;
            float halfHeight = (height * cellSize) / 2f;

            Vector3 origin = gridCenter - new Vector3(halfWidth, halfHeight, 0f);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 worldCenter = origin + (new Vector3(x + 0.5f, y + 0.5f, 0f) * cellSize);
                    Vector2 checkSize = Vector2.one * cellSize * detectionRadius;

                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(obstacleLayer);
                    filter.useTriggers = false;

                    bool isWalkable = Physics2D.OverlapBox(worldCenter, checkSize, 0f, filter, new Collider2D[1]) == 0;

                    int proximityPenalty = 0;
                    if (isWalkable)
                    {
                        Collider2D hit = Physics2D.OverlapCircle(worldCenter, penaltyCheckRadius, obstacleLayer);
                        if (hit != null && !hit.isTrigger)
                        {
                            float distanceToObstacle = Vector2.Distance(worldCenter, hit.ClosestPoint(worldCenter));
                            float normalizedDistance = Mathf.Clamp01(1f - (distanceToObstacle / penaltyCheckRadius));
                            proximityPenalty = Mathf.RoundToInt(normalizedDistance * penaltyStrength);
                        }
                    }

                    grid[x, y] = new NodeGrid(worldCenter, isWalkable, proximityPenalty, x, y);
                }
            }
        }

        public int2 WorldToGrid(Vector3 worldPosition)
        {
            float halfWidth = (width * cellSize) / 2f;
            float halfHeight = (height * cellSize) / 2f;
            Vector3 origin = gridCenter - new Vector3(halfWidth, halfHeight, 0f);

            int x = Mathf.FloorToInt((worldPosition.x - origin.x) / cellSize);
            int y = Mathf.FloorToInt((worldPosition.y - origin.y) / cellSize);

            return new int2(Mathf.Clamp(x, 0, width - 1), Mathf.Clamp(y, 0, height - 1));
        }

        public Vector3 GridToWorld(int2 gridPos)
        {
            float halfWidth = (width * cellSize) / 2f;
            float halfHeight = (height * cellSize) / 2f;
            Vector3 origin = gridCenter - new Vector3(halfWidth, halfHeight, 0f);

            return origin + new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0) * cellSize;
        }

        public Vector2 GetRandomWalkablePoint()
        {
            if (grid == null) return Vector2.zero;

            List<NodeGrid> walkableNodes = new List<NodeGrid>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y].isWalkable)
                        walkableNodes.Add(grid[x, y]);
                }
            }

            if (walkableNodes.Count == 0)
                return Vector2.zero;

            NodeGrid randomNode = walkableNodes[UnityEngine.Random.Range(0, walkableNodes.Count)];
            return randomNode.worldPosition;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.DrawWireCube(gridCenter, new Vector3(width * cellSize, height * cellSize, 0f));
                if (grid != null && grid.GetLength(0) == width && grid.GetLength(1) == height)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            NodeGrid cell = grid[x, y];
                            Gizmos.color = ((cell.isWalkable) ? walkableColor : obstacleColor) * 0.5f;
                            Gizmos.DrawCube(cell.worldPosition, new Vector3(1f, 1f, 0f) * (cellSize - 0.05f));
                        }
                    }
                }
            }
        }
#endif
    }
}