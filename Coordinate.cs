using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
// For property drawer when editing.
using UnityEditor;
#endif

/// <summary>
/// An integer version of Vector3 that is useful for manipulation of blocks which must be aligned on integer boundaries.
/// Follows all conventions of Vector3 and adds some set operations useful for block manipulation such as Shell and VonNeumanNeighborhood.
/// </summary>
[Serializable]
public struct Coordinate {

    /// <summary>
    /// X component of the Coordinate.
    /// </summary>
    public int x;

    /// <summary>
    /// Y component of the Coordinate.
    /// </summary>
    public int y;

    /// <summary>
    /// Z component of the Coordinate.
    /// </summary>
    public int z;

    /// <summary>
    /// For use when the coordinate is in two dimensions.
    /// </summary>
    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
        this.z = 0;
    }

    /// <summary>
    /// Creates a new Coordinate with given x, y and z components.
    /// </summary>
    public Coordinate(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Creates a new Coordinate with given x, y and z components.
    /// No check is done for overflow on float to short conversion.
    /// </summary>
    public Coordinate(float x, float y, float z) {
        this.x = (int)x;
        this.y = (int)y;
        this.z = (int)z;
    }

    /// <summary>
    /// Creates a new Coordinate with given x, y and z components.
    /// No check is done for overflow on Vector3 components to short conversion.
    /// </summary>
    public Coordinate(Vector3 source) {
        this.x = (int)source.x;
        this.y = (int)source.y;
        this.z = (int)source.z;
    }

    public override bool Equals(object obj) {
        if(obj is Coordinate) {
            return this == (Coordinate)obj;
        }
        else {
            return false;
        }
    }

    public override int GetHashCode() {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    /// <summary>
    /// Checks equality with a pair-wise evaluation of components.
    /// </summary>
    public static bool operator ==(Coordinate lhs, Coordinate rhs) {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    /// <summary>
    /// Checks inequality with a pair-wise evaluation of components.
    /// </summary>
    public static bool operator !=(Coordinate lhs, Coordinate rhs) {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Allows conversion from Vector3 to Coordinate without a cast.
    /// Although precision is lost this is still implicit as Coordinate is a special case of a Vector3, not specifically a less-precise version.
    /// </summary>
    public static explicit operator Coordinate(Vector3 source) {
        return new Coordinate(source);
    }

    /// <summary>
    /// Allows conversion from Coordinate to Vector3.
    /// </summary>
    public static explicit operator Vector3(Coordinate source) {
        return new Vector3(source.x, source.y, source.z);
    }

    /// <summary>
    /// Addition of two coordinates performs pair-wise addition, just as Vector3.
    /// </summary>
    public static Coordinate operator +(Coordinate lhs, Coordinate rhs) {
        return new Coordinate(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    }

    /// <summary>
    /// Subtraction of two coordinates performs pair-wise subtraction, just as Vector3.
    /// </summary>
    public static Coordinate operator -(Coordinate lhs, Coordinate rhs) {
        return new Coordinate(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
    }

    public static Coordinate operator *(int lhs, Coordinate rhs) {
        return new Coordinate(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
    }

    public static Coordinate operator *(Coordinate lhs, int rhs) {
        return new Coordinate(rhs * lhs.x, rhs * lhs.y, rhs * lhs.z);
    }

    /// <summary>
    /// Returns all of the Coordinates in the box shell defined in the region [start, end).
    /// That is, inclusive of the start coordinate and exclusive of the end coordinate.
    /// </summary>
    public static IEnumerable<Coordinate> Shell(Coordinate start, Coordinate end) {
        if(start.x < end.x || start.y < end.y || start.z < end.z) {
            // Do the front and back of the shell.
            for(var x = start.x; x < end.x; ++x) {
                for(var y = start.x; y < end.y; ++y) {
                    yield return new Coordinate(x, y, start.z);
                    yield return new Coordinate(x, y, end.z - 1);
                }
            }
            // Do the top and bottom of the shell, except where already done above.
            for(var x = start.x; x < end.x; ++x) {
                for(var z = start.z + 1; z < end.z - 1; ++z) {
                    yield return new Coordinate(x, start.y, z);
                    yield return new Coordinate(x, end.y - 1, z);
                }
            }
            // Do the left and right, except where already done above.
            for(var y = start.y + 1; y < end.y - 1; ++y) {
                for(var z = start.z + 1; z < end.z - 1; ++z) {
                    yield return new Coordinate(start.x, y, z);
                    yield return new Coordinate(end.x - 1, y, z);
                }
            }
        }
    }

    /// <summary>
    /// Returns all of the coordinates in the box defined by the region [start, end).
    /// That is, inclusive of the start coordinate and exclusive of the end coordinate.
    /// </summary>
    public static IEnumerable<Coordinate> Solid(Coordinate start, Coordinate end) {
        if(start.x < end.x || start.y < end.y || start.z < end.z) {
            for(var x = start.x; x < end.x; ++x) {
                for(var y = start.y; y < end.y; ++y) {
                    for(var z = start.z; z < end.z; ++z) {
                        yield return new Coordinate(x, y, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the absolute coordinates for all of the neighbors that share a face with this coordinate, of which there are six.
    /// </summary>
    public IEnumerable<Coordinate> VonNeumanNeighbors() {
        yield return this + Coordinate.Forward;
        yield return this + Coordinate.Back;
        yield return this + Coordinate.Left;
        yield return this + Coordinate.Right;
        yield return this + Coordinate.Up;
        yield return this + Coordinate.Down;
    }

    /// <summary>
    /// Returns the absolute coordinates for all of the neighbors that share a vertex with this coordinate, of which there are 26.
    /// </summary>
    public IEnumerable<Coordinate> MooreNeighbors() {
        for(short x = -1; x <= 1; ++x) {
            for(short y = -1; y <= 1; ++y) {
                for(short z = -1; z <= 1; ++z) {
                    if(x != 0 && y != 0 && z != 0) {
                        yield return this + new Coordinate(x, y, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the distance between two points when only travelling along the axes.
    /// This Manhattan distance, or city-block distance, can be visualized as the distance it takes to drive from coordinate to another in a city.
    /// </summary>
    public int ManhattanDistance(Coordinate rhs) {
        return Mathf.Abs(x - rhs.x) + Mathf.Abs(y - rhs.y) + Mathf.Abs(z - rhs.z);
    }

    /// <summary>
    /// Returns the Axis-Aligned bounding box around the given coordinates.
    /// </summary>
    public static Bounds Aabb(Coordinate from, Coordinate to) {
        var center = 0.5f * (Vector3)(from + to);
        var size = new Vector3(Mathf.Abs(to.x - from.x) + 1, Mathf.Abs(to.y - from.y) + 1, Mathf.Abs(to.z - from.z) + 1);
        return new Bounds(center, size);
    }

    /// <summary>
    /// Shorthand for writing Coordinate(0, 0, 0).
    /// </summary>
    public readonly static Coordinate Zero = new Coordinate(0, 0, 0);

    /// <summary>
    /// Shorthand for writing Coordinate(1, 1, 1).
    /// </summary>
    public readonly static Coordinate One = new Coordinate(1, 1, 1);

    /// <summary>
    /// Shorthand for writing Coordinate(0, 0, 1).
    /// </summary>
    public readonly static Coordinate Forward = new Coordinate(0, 0, 1);

    /// <summary>
    /// Shorthand for writing Coordinate(0, 0, -1).
    /// </summary>
    public readonly static Coordinate Back = new Coordinate(0, 0, -1);

    /// <summary>
    /// Shorthand for writing Coordinate(-1, 0, 0).
    /// </summary>
    public readonly static Coordinate Left = new Coordinate(-1, 0, 0);

    /// <summary>
    /// Shorthand for writing Coordinate(1, 0, 0).
    /// </summary>
    public readonly static Coordinate Right = new Coordinate(1, 0, 0);

    /// <summary>
    /// Shorthand for writing Coordinate(0, 1, 0).
    /// </summary>
    public readonly static Coordinate Up = new Coordinate(0, 1, 0);

    /// <summary>
    /// Shorthand for writing Coordinate(0, -1, 0).
    /// </summary>
    public readonly static Coordinate Down = new Coordinate(0, -1, 0);

    /// <summary>
    /// Converts the coordinate to a string of the format C(x, y, z).
    /// </summary>
    public override string ToString() {
        return $"C({x}, {y}, {z})";
    }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Coordinate))]
public class IngredientDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginProperty(position, label, property);

        var x = property.FindPropertyRelative("x");
        var y = property.FindPropertyRelative("y");
        var z = property.FindPropertyRelative("z");

        var vector3 = new Vector3(x.intValue, y.intValue, z.intValue);
        vector3 = EditorGUI.Vector3Field(position, label, vector3);

        x.intValue = (int)vector3.x;
        y.intValue = (int)vector3.y;
        z.intValue = (int)vector3.z;

        EditorGUI.EndProperty();
    }
}

#endif

