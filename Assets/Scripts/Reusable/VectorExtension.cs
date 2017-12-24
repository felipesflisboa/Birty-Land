using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension {
    /// <summary>
    /// Returns a new Vector with a.x*b.x, a.y*b.y, a.z*b.z.
    /// </summary>
    public static Vector3 Multiply(this Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector2 Multiply(this Vector2 a, Vector2 b) {
        return new Vector2(a.x * b.x, a.y * b.y);
    }

    /// <summary>
    /// Returns a new Vector with a.x/b.x, a.y/b.y, a.z/b.z.
    /// </summary>
    public static Vector3 Divide(this Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector2 Divide(this Vector2 a, Vector2 b) {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    /// <summary>
    /// Parse into Vector3 moving the Y value to Z axis.
    /// </summary>
    public static Vector3 YToZ(this Vector2 vector2) {
        return new Vector3(vector2.x, 0f, vector2.y);
    }

    /// <summary>
    /// Parse into Vector2 moving the Z value to Y axis.
    /// </summary>
    public static Vector2 XZToV2(this Vector3 vector3) {
        return new Vector2(vector3.x, vector3.z);
    }
}