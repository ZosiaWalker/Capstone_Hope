#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pathfinding2D
{
    [CustomEditor(typeof(AStarGrid))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AStarGrid scanner = (AStarGrid)target;
            if (GUILayout.Button("Scan"))
            {
                scanner.CreateGrid();
                SceneView.RepaintAll();
            }
        }
    }
}
#endif
