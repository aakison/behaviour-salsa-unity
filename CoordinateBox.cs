using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Relentless {

    /// <summary>
    /// Represents a bounding box of all coordinates from [minimum, maximum).
    /// </summary>
    public struct CoordinateBox {

        public Coordinate minimum;

        public Coordinate maximum;

        /// <summary>
        /// Construct a new CoordinateBox over the space [min, max).
        /// </summary>
        /// <param name="min">The inclusive lower bound of the coordinate box.</param>
        /// <param name="max">The exclusive upper bound of the coordinate box.</param>
        public CoordinateBox(Coordinate min, Coordinate max) {
            minimum = min;
            maximum = max;
        }

        public Coordinate Dimensions {
            get {
                return maximum - minimum;
            }
        }

        public void ForEach(Action<Coordinate> action) {
            if(action == null) {
                throw new ArgumentNullException("action");
            }
            for(var x = minimum.x; x < maximum.x; ++x) {
                for(var y = minimum.y; y < maximum.y; ++y) {
                    for(var z = minimum.z; z < maximum.z; ++z) {
                        action(new Coordinate(x, y, z));
                    }
                }
            }
        }

        public bool Contains(Coordinate coordinate) {
            // TODO: Optimize here with short circuit evaluation if necessary.
            var containsX = minimum.x <= coordinate.x && coordinate.x < maximum.x;
            var containsY = minimum.y <= coordinate.y && coordinate.y < maximum.y;
            var containsZ = minimum.z <= coordinate.z && coordinate.z < maximum.z;
            return containsX && containsY && containsZ;
        }

        /// <summary>
        /// Checks equality with a pair-wise evaluation of components.
        /// </summary>
        public static bool operator ==(CoordinateBox lhs, CoordinateBox rhs) {
            return lhs.minimum == rhs.minimum && lhs.maximum == rhs.maximum;
        }

        /// <summary>
        /// Checks inequality with a pair-wise evaluation of components.
        /// </summary>
        public static bool operator !=(CoordinateBox lhs, CoordinateBox rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj) {
            if(obj is CoordinateBox) {
                return this == (CoordinateBox)obj;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            return minimum.GetHashCode() ^ maximum.GetHashCode();
        }

        public void ForEachExcept(CoordinateBox exclude, Action<Coordinate> action) {
            // TODO: Optimize here by unrolling loops and jumping over the large runs of numbers.
            ForEach(coord => {
                if(!exclude.Contains(coord)) {
                    action(coord);
                }
            });
        }

        public override string ToString() {
            return String.Format("CB[{0}, {1})", minimum, maximum);
        }
    }


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(CoordinateBox))]
public class CoordinateBoxDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginProperty(position, label, property);

        var minx = property.FindPropertyRelative("minimum").FindPropertyRelative("x").intValue;
        var miny = property.FindPropertyRelative("minimum").FindPropertyRelative("y").intValue;
        var minz = property.FindPropertyRelative("minimum").FindPropertyRelative("z").intValue;

        var maxx = property.FindPropertyRelative("maximum").FindPropertyRelative("x").intValue;
        var maxy = property.FindPropertyRelative("maximum").FindPropertyRelative("y").intValue;
        var maxz = property.FindPropertyRelative("maximum").FindPropertyRelative("z").intValue;

        var center = new Vector3((minx + maxx) / 2.0f, (miny + maxy) / 2.0f, (minz + maxz) / 2.0f);
        var size = new Vector3(maxx - minx, maxy - miny, maxz - minz);
        var bounds = new Bounds(center, size);

        EditorGUI.BoundsField(position, label, bounds);

        EditorGUI.EndProperty();
    }
}

#endif

}
