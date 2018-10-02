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
public class LinearPerlin {

    [Tooltip("The minimum value for the perlin noise.")]
    public float minimum;

    [Tooltip("The maximum value for the perlin noise.")]
    public float maximum;

    [Tooltip("The frequency of the nose, 0 => no variation, 1 and up => lots of static.")]
    public float frequency;

    [Tooltip("The offset to provide a pseudo-seed to the noise.")]
    public float offset;

    /// <summary>
    /// Evaluates the noise at a given input value based on the properties.
    /// </summary>
    public float Evaluate(float domain)
    {
        var scaled = frequency * domain;
        var perlin = Mathf.PerlinNoise(scaled, offset);
        var percent = Mathf.InverseLerp(0, 1, perlin);
        var value = Mathf.Lerp(minimum, maximum, percent);
        return value;
    }

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LinearPerlin))]
public class LinearPerlinDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        var minProperty = property.FindPropertyRelative("minimum");
        var maxProperty = property.FindPropertyRelative("maximum");
        var freqProperty = property.FindPropertyRelative("frequency");
        var offsetProperty = property.FindPropertyRelative("offset");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var minRect = new Rect(position.xMin, position.yMin, position.width - 71, 20);
        EditorGUI.LabelField(minRect, new GUIContent("Min", "The minimum value for the perlin noise."));
        minRect.xMin += 30;
        var minValue = EditorGUI.Slider(minRect, minProperty.floatValue, -16, 64);

        var maxRect = new Rect(position.xMin, position.yMin + 22, position.width - 71, 20);
        EditorGUI.LabelField(maxRect, "Max");
        maxRect.xMin += 30;
        var maxValue = EditorGUI.Slider(maxRect, maxProperty.floatValue, 0, 128);

        var freqRect = new Rect(position.xMin, position.yMin + 44, position.width - 71, 20);
        EditorGUI.LabelField(freqRect, "Freq");
        freqRect.xMin += 30;
        var freqValue = EditorGUI.Slider(freqRect, freqProperty.floatValue, 0, 1);

        shadow.minimum = minValue;
        shadow.maximum = maxValue;
        shadow.frequency = freqValue;
        shadow.offset = offsetProperty.floatValue;

        var tex = CreateTexturePreview();
        var rect = new Rect(position.xMax - 66, position.yMin, 64, 64);
        EditorGUI.DrawPreviewTexture(rect, tex);

        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
        if(freqProperty.floatValue != freqValue) {
            freqProperty.floatValue = freqValue;
            offsetProperty.floatValue = shadow.offset + freqValue;
        }
        EditorGUI.EndProperty();
    }

    private LinearPerlin shadow = new LinearPerlin();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 64;
    }

    private Texture2D CreateTexturePreview()
    {
        var t = new Texture2D(64, 64);
        t.hideFlags = HideFlags.HideAndDontSave;
        for(int x = 0; x < 64; ++x) {
            var range = Mathf.RoundToInt(Mathf.Clamp(shadow.Evaluate(x), 0, 64));
            t.SetPixels(x, 32 - range / 2, 1, range, Whites);
        }
        t.Apply();
        return t;
    }

    private Color[] Whites {
        get {
            if(colors == null) {
                colors = new Color[64];
                for(int i = 0; i < 64; ++i) {
                    colors[i] = Color.white;
                }
            }
            return colors;
        }
    }
    private Color[] colors;

}

#endif
