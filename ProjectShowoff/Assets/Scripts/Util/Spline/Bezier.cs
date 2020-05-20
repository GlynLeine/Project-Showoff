using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Bezier
{
    // Util func to accept array
    public static Vector3 EvaluateCurve(Vector3[] points, float t)
    {
        return EvaluateCurve(points[0], points[1], points[2], points[3], t);
    }

    // Same as EvaluateCubic but expanded out for optimisation.
    public static Vector3 EvaluateCurve(Vector3 a1, Vector3 c1, Vector3 c2, Vector3 a2, float t)
    {
        t = Mathf.Clamp01(t);
        return (1 - t) * (1 - t) * (1 - t) * a1 + 3 * (1 - t) * (1 - t) * t * c1 + 3 * (1 - t) * t * t * c2 + t * t * t * a2;
    }

    public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = EvaluateQuadratic(a, b, c, t);
        Vector3 p1 = EvaluateQuadratic(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
    }

    public static float EstimateCurveLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float controlNetLength = (p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p3).magnitude;
        float estimatedCurveLength = (p0 - p3).magnitude + controlNetLength / 2f;
        return estimatedCurveLength;
    }
}

