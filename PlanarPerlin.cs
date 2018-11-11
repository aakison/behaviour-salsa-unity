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
/// A editable version of perlin noise for creating one dimensional noise.
/// </summary>
[Serializable]
public class PlanarPerlin {

    [Tooltip("The minimum value for the perlin noise.")]
    public float minimum;

    [Tooltip("The maximum value for the perlin noise.")]
    public float maximum;

    [Tooltip("The frequency of the nose, 0 => no variation, 1 and up => lots of static, control in both dimensions.")]
    public float frequency;

    [Tooltip("The offset to provide a pseudo-seed to the noise.")]
    public float offset;

    [Tooltip("Multiplied by y-coordinate to create horizontal effects.")]
    public AnimationCurve horizontalBands;

    [Tooltip("Determines if the result is continuous between minimum & maximum or snapped to {0, 1}.")]
    public bool continuous = true;

    /// <summary>
    /// Evaluates the noise at a given input value based on the properties.
    /// </summary>
    public float Evaluate(Vector2 domain)
    {
        var scaled = frequency * domain + new Vector2(offset, offset);
        var perlin = Mathf.PerlinNoise(scaled.x, scaled.y);
        var banded = horizontalBands?.Evaluate(domain.y) ?? 1;
        var percent = Mathf.InverseLerp(0, 1, banded * perlin);
        var value = Mathf.Lerp(minimum, maximum, percent);
        if(continuous) {
            return value;
        }
        else {
            return value >= 0.5f ? 1 : 0;
        }
    }

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(PlanarPerlin))]
public class PlanarPerlinDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        var minProperty = property.FindPropertyRelative("minimum");
        var maxProperty = property.FindPropertyRelative("maximum");
        var freqProperty = property.FindPropertyRelative("frequency");
        var offsetProperty = property.FindPropertyRelative("offset");
        var horizonalBandsProperty = property.FindPropertyRelative("horizontalBands");
        var continuousProperty = property.FindPropertyRelative("continuous");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var minRect = new Rect(position.xMin, position.yMin, position.width - previewSize - 5, 20);
        EditorGUI.LabelField(minRect, new GUIContent("Min", "The minimum value for the perlin noise."));
        minRect.xMin += 30;
        var minValue = EditorGUI.Slider(minRect, minProperty.floatValue, -16, 1);

        var maxRect = new Rect(position.xMin, position.yMin + 22, position.width - previewSize - 5, 20);
        EditorGUI.LabelField(maxRect, "Max");
        maxRect.xMin += 30;
        var maxValue = EditorGUI.Slider(maxRect, maxProperty.floatValue, 0, 16);

        var freqRect = new Rect(position.xMin, position.yMin + 44, position.width - previewSize - 5, 20);
        EditorGUI.LabelField(freqRect, "Freq");
        freqRect.xMin += 30;
        var freqValue = EditorGUI.Slider(freqRect, freqProperty.floatValue, 0, 1);

        var horizRect = new Rect(position.xMin, position.yMin + 66, position.width - previewSize - 5, 20);
        EditorGUI.LabelField(horizRect, new GUIContent("Horiz", "Multiplied by y-coordinate to create horizontal effects"));
        horizRect.xMin += 30;
        var horizValue = EditorGUI.CurveField(horizRect, horizonalBandsProperty.animationCurveValue);

        var contRect = new Rect(position.xMin, position.yMin + 88, position.width - 30, 20);
        EditorGUI.LabelField(contRect, new GUIContent("Cont", "Does the function return a continuous [0, 1] or discrete value {0, 1}."));
        contRect.xMin += 30;
        var contValue = EditorGUI.Toggle(contRect, continuousProperty.boolValue);

        shadow.minimum = minValue;
        shadow.maximum = maxValue;
        shadow.frequency = freqValue;
        shadow.offset = offsetProperty.floatValue;
        shadow.continuous = contValue;
        shadow.horizontalBands = horizValue;

        var tex = CreateTexturePreview();
        var rect = new Rect(position.xMax - previewSize, position.yMin, previewSize, previewSize);
        EditorGUI.DrawPreviewTexture(rect, tex);

        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
        horizonalBandsProperty.animationCurveValue = horizValue;
        continuousProperty.boolValue = contValue;
        if(freqProperty.floatValue != freqValue) {
            freqProperty.floatValue = freqValue;
            offsetProperty.floatValue = shadow.offset + freqValue;
        }
        EditorGUI.EndProperty();
    }

    private PlanarPerlin shadow = new PlanarPerlin();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return previewSize + 22 + 22;
    }

    private const int previewSize = 88;

    private Texture2D CreateTexturePreview()
    {
        var t = new Texture2D(previewSize, previewSize);
        t.hideFlags = HideFlags.HideAndDontSave;
        for(int x = 0; x < previewSize; ++x) {
            for(int y = 0; y < previewSize; ++y) {
                var pos = new Vector2(x, previewSize - y);
                var range = Mathf.Clamp01(shadow.Evaluate(pos));
                t.SetPixel(x, y, new Color(range, range, 0.5f));
            }
        }
        t.Apply();
        return t;
    }

}

#endif
