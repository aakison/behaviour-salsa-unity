using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Represents a single range for a float in a range [minimum, maximum).
/// </summary>
[Serializable]
public class Range {

    [Tooltip("The inclusive lower bound of the range.")]
    public float minimum;

    [Tooltip("The exclusive upper bound of the range.")]
    public float maximum;

    /// <summary>
    /// Indicates whether the indicated value is in the range.
    /// </summary>
    public bool Contains(float x) {
        return minimum <= x && x < maximum;
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Range))]
public class RangeDrawer : PropertyDrawer {
    const int curveWidth = 50;
    const float min = 0;
    const float max = 1;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        if(label.text.ToLower().StartsWith("min max ")) {
            label.text = label.text.Substring(8);
        }
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var minCaption = position;
        minCaption.width = 25;
        var minValue = minCaption;
        minValue.xMin += 27;
        minValue.width = 80;
        var maxCaption = minValue;
        maxCaption.xMin += 84;
        maxCaption.width = 28;
        var maxValue = maxCaption;
        maxValue.xMin += 30;
        maxValue.width = 80;

        var min = property.FindPropertyRelative("minimum").floatValue;
        var max = property.FindPropertyRelative("maximum").floatValue;

        EditorGUI.LabelField(minCaption, "Min");
        min = EditorGUI.FloatField(minValue, min);
        EditorGUI.LabelField(maxCaption, "Max");
        max = EditorGUI.FloatField(maxValue, max);

        property.FindPropertyRelative("minimum").floatValue = min;
        property.FindPropertyRelative("maximum").floatValue = max;

        EditorGUI.EndProperty();
    }
}
#endif
