#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pathfinding2D
{
    [CustomPropertyDrawer(typeof(AreaOfSight))]
    public class AreaOfSightDrawer : PropertyDrawer
    {
        private const float PreviewHeight = 100f;
        private const float VerticalSpacing = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight
                 + EditorGUIUtility.singleLineHeight
                 + VerticalSpacing
                 + PreviewHeight
                 + VerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // --- Label ---
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // --- Slider ---
            SerializedProperty areaProp = property.FindPropertyRelative("alignment");
            Rect sliderRect = new Rect(
                position.x,
                labelRect.yMax,
                position.width,
                EditorGUIUtility.singleLineHeight
            );
            EditorGUI.PropertyField(sliderRect, areaProp);

            // --- Preview ---
            Rect previewRect = new Rect(
                position.x,
                sliderRect.yMax + VerticalSpacing,
                position.width,
                PreviewHeight
            );
            DrawArcPreview(previewRect, areaProp.floatValue);
        }

        private void DrawArcPreview(Rect rect, float areaOfSight)
        {
            // Background
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            float angle = Mathf.Acos(areaOfSight) * Mathf.Rad2Deg;
            float radius = Mathf.Min(rect.width, rect.height) * 0.45f;
            Vector2 center = rect.center;

            Handles.BeginGUI();

            // Filled arc
            Handles.color = new Color(1f, 1f, 0f, 0.4f);
            Handles.DrawSolidArc(center, Vector3.forward,
                Quaternion.Euler(0, 0, -angle) * Vector3.up,
                angle * 2,
                radius);

            // Outline
            Handles.color = Color.yellow;
            Handles.DrawWireArc(center, Vector3.forward,
                Quaternion.Euler(0, 0, -angle) * Vector3.up,
                angle * 2,
                radius);

            // --- Draw forward arrow ---
            Vector2 arrowTip = new Vector2(center.x, center.y + radius); // bottom point
            float arrowSize = Mathf.Min(rect.width, rect.height) * 0.1f; // scales with preview size

            Vector3[] arrowPoints = new Vector3[]
            {
            arrowTip,
            arrowTip + new Vector2(-arrowSize * 0.5f, -arrowSize),
            arrowTip + new Vector2( arrowSize * 0.5f, -arrowSize)
            };

            // Line from center to arrow tip
            Handles.color = Color.red;
            Handles.DrawLine(center, arrowTip);

            // Filled arrow triangle
            Handles.DrawAAConvexPolygon(arrowPoints);

            Handles.EndGUI();
        }
    }
}
#endif