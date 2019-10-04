using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    // VECTOR METHODS WITH: returns same vector with a specified value changed
    public static Vector3 WithX(this Vector3 vector, float value) => new Vector3(value, vector.y, vector.z);
    public static Vector3 WithY(this Vector3 vector, float value) => new Vector3(vector.x, value, vector.z);
    public static Vector3 WithZ(this Vector3 vector, float value) => new Vector3(vector.x, vector.y, value);
    public static Vector2 WithX(this Vector2 vector, float value) => new Vector2(value, vector.y);
    public static Vector2 WithY(this Vector2 vector, float value) => new Vector2(vector.x, value);

    
    // VECTOR METHODS APPLY: performs an operation on each element of the vector(s)
    public static Vector3 Apply(this Vector3 vector, Func<float, float> func) => new Vector3(func(vector.x), func(vector.y), func(vector.z));
    public static Vector3 Apply(this Vector3 vector, Vector3 other, Func<float, float, float> func) => new Vector3(func(vector.x, other.x), func(vector.y, other.y), func(vector.z, other.z));
    public static Vector2 Apply(this Vector2 vector, Func<float, float> func) => new Vector2(func(vector.x), func(vector.y));
    public static Vector2 Apply(this Vector2 vector, Vector2 other, Func<float, float, float> func) => new Vector2(func(vector.x, other.x), func(vector.y, other.y));
    
    // some specific instances of apply methods (can add more if necessary)
    public static Vector3 MultElementWise(this Vector3 vector, Vector3 other) => vector.Apply(other, (e1, e2) => e1 * e2);
    public static Vector2 MultElementWise(this Vector2 vector, Vector2 other) => vector.Apply(other, (e1, e2) => e1 * e2);
    // ...


    // VECTOR METHODS TO: converts between types of vectors
    public static Vector3 ToVector3(this Vector2 vector, float z = 0) => new Vector3(vector.x, vector.y, z);
	public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);


    // RECT METHODS: various convenient things to do with rectangles (more can be added later)
	public static Rect RectFromCenter(Vector2 center, Vector2 size) => new Rect(center - size / 2, size);
	public static Vector2 ClampInRect(this Vector2 vector, Rect rect) => new Vector2(Mathf.Clamp(vector.x, rect.xMin, rect.xMax), Mathf.Clamp(vector.y, rect.yMin, rect.yMax));
    //...


    // CAMERA METHOD VIEWPORT: returns information about what regions in world space the camera can actually see
	public static Vector2 ViewportSize(this Camera camera) => camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f)) - camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
    public static Rect ViewportRect(this Camera camera) => RectFromCenter(camera.transform.position, ViewportSize(camera));


    // COLOR METHODS WITH: returns same color with a specified value changed
    public static Color WithRed(this Color color, float value) => new Color(value, color.g, color.b, color.a);
    public static Color WithGreen(this Color color, float value) => new Color(color.r, value, color.b, color.a);
    public static Color WithBlue(this Color color, float value) => new Color(color.r, color.g, value, color.a);
    public static Color WithAlpha(this Color color, float value) => new Color(color.r, color.g, color.b, value);


    // TIMER METHODS: facilitates implementation of timer floats
    // (in previous projects i had to write these over and over so to not reinvent the wheel many times, here they are)
    public static float Timer(float value, bool unscaled = false) => Mathf.Max(0, value - (unscaled ? Time.unscaledDeltaTime : Time.deltaTime));
    public static float FixedTimer(float value, bool unscaled = false) => Mathf.Max(0, value - (unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime));
    public static float TimerUp(float value, float max, bool unscaled = false) => Mathf.Min(max, value + (unscaled ? Time.unscaledDeltaTime : Time.deltaTime));
    public static float FixedTimerUp(float value, float max, bool unscaled = false) => Mathf.Min(max, value + (unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime));
}
