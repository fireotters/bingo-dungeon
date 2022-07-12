using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtensions
{
    /// <summary>
    /// Draws a field of view cone in the scene view. It currently only works for xy axis, z is always 0.
    /// </summary>
    public static void DrawWireFov2D(Vector2 center, Vector2 dir, float angleDegrees, float radius, int arcSegmentCount = 10)
    {
        Gizmos.DrawLine(center, center + dir.Rotated(-angleDegrees * 0.5f * Mathf.Deg2Rad) * radius);
        Gizmos.DrawLine(center, center + dir.Rotated(angleDegrees * 0.5f * Mathf.Deg2Rad) * radius);

        DrawArc2D(center, dir, angleDegrees, radius, arcSegmentCount);
    }

    /// <summary>
    /// Draws an arc in the scene view. It currently only works for xy axis, z is always 0.
    /// </summary>
    public static void DrawArc2D(Vector2 center, Vector2 dir, float arcDegrees, float radius, int arcSegmentCount = 10)
    {
        float angleStepRadians = (arcDegrees / arcSegmentCount) * Mathf.Deg2Rad;
        Vector2 newDir = dir.Rotated(-arcDegrees * 0.5f *Mathf.Deg2Rad);
        Vector2 prevPos = center + newDir * radius;
        for (int i = 0; i < arcSegmentCount; i++)
        {
            newDir = newDir.Rotated(angleStepRadians);
            Vector2 newPos = center + newDir* radius;
            Gizmos.DrawLine(prevPos, newPos);
            prevPos = newPos;
        }
    }
}
