using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Range01Vector2Attribute))]
public class Range01Vector2Drawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3f + 4f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Vector2 value = property.vector2Value;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect labelRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect sliderXRect = new Rect(position.x, position.y + lineHeight + 2f, position.width, lineHeight);
        Rect sliderYRect = new Rect(position.x, position.y + (lineHeight + 2f) * 2f, position.width, lineHeight);

        EditorGUI.LabelField(labelRect, label);

        EditorGUI.indentLevel++;
        value.x = EditorGUI.Slider(sliderXRect, "X (-1 = left, 0 = center, 1 = right)", value.x, -1f, 1f);
        value.y = EditorGUI.Slider(sliderYRect, "Y (-1 = bottom, 0 = center, 1 = top)", value.y, -1f, 1f);
        EditorGUI.indentLevel--;

        property.vector2Value = value;

        EditorGUI.EndProperty();
    }
}