#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pathfinding2D
{
    [CustomPropertyDrawer(typeof(NumberOfRaysPreviewAttribute))]
    public class NumberOfRaysDrawer : PropertyDrawer
    {
        private const float previewHeight = 100f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Slider/int field height + preview height + spacing
            return EditorGUIUtility.singleLineHeight + previewHeight + 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the int field
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label);

            // Draw the preview
            position.y += EditorGUIUtility.singleLineHeight + 2f;
            position.height = previewHeight;
            DrawRaysPreview(position, property.intValue);
        }

        private void DrawRaysPreview(Rect rect, int rayCount)
        {
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            Vector2 center = rect.center;

            // Radius tied to previewHeight
            float radius = previewHeight * 0.4f;
            // Arrow size also tied to previewHeight
            float arrowSize = previewHeight * 0.06f;

            Handles.BeginGUI();
            Handles.color = Color.red;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = (360f / rayCount) * i;
                Vector2 dir = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
                Vector2 end = center + dir * radius;

                // Draw main line
                Handles.DrawLine(center, end);

                // Draw outward-pointing arrowhead (scaled)
                Vector2 rightWing = Quaternion.Euler(0, 0, -150) * dir * arrowSize;
                Vector2 leftWing = Quaternion.Euler(0, 0, 150) * dir * arrowSize;

                Handles.DrawAAConvexPolygon(end, end + rightWing, end + leftWing);
            }

            Handles.EndGUI();
        }
    }
}
#endif