using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 GetInverted(this Vector2 vector2) => new Vector2(vector2.y, vector2.x);

    /// <summary>
    /// Returns a rotated Vector2 radian grades
    /// </summary>
    /// <param name="v"></param>
    /// <param name="radianDegrees"></param>
    /// <returns></returns>
    public static Vector2 Rotated(this Vector2 v, float radianDegrees)
    {
        return new Vector2(
            v.x * Mathf.Cos(radianDegrees) - v.y * Mathf.Sin(radianDegrees),
            v.x * Mathf.Sin(radianDegrees) + v.y * Mathf.Cos(radianDegrees)
        );
    }

    public static float SquaredDistance(Vector2 a, Vector2 b) => (a - b).sqrMagnitude;
    //public static float DistanceFast(Vector2 a, Vector2 b) => MathsFast.FastMagnitude(a-b);

    public static Vector3 Rounded(this Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

    public static float Angle2D(Vector2 from, Vector2 to)
    {
        Vector2 direction = (to - from).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Iterates between start and end positions with count intermediate points. Start and end are NOT returned
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="intermediatePostitions"></param>
    /// <returns></returns>
    public static Vector3[] GetIntermediatePositions(Vector3 start, Vector3 end, int intermediatePostitions)
    {
        if(intermediatePostitions == 0) 
            return new Vector3[0];
        Vector3[] result = new Vector3[intermediatePostitions];
        float step = 1.0f / (intermediatePostitions + 1);

        for (int i = 0; i < intermediatePostitions; i++)
        {
            result[i] = Vector3.Lerp(start, end, step * (i+1));
        }

        return result;
    }

    /// <summary>
    /// Returns the intermediate position positionIndex of numOfPositions intermediatePositions starting at 1
    /// So if you cann this with (start, end, 1, 1) it will return the middle position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="numOfPositions"></param>
    /// <param name="positionIndex"></param>
    /// <returns></returns>
    public static Vector3 GetIntermediatePosition(Vector3 start, Vector3 end, int numOfPositions, int positionIndex)
    {
        float step = 1.0f / (numOfPositions + 1);
        return Vector3.Lerp(start, end, step * positionIndex);
    }

    /// <summary>
    /// Iterates between start and end positions with count intermediate points. Start and end are always returned
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="intermediatePostitions"></param>
    /// <returns></returns>
    public static Vector3[] GetIntermediatePositionsInclusive(Vector3 start, Vector3 end, int intermediatePostitions)
    {
        Vector3[] result = new Vector3[intermediatePostitions + 2];
        result[0] = start;
        result[intermediatePostitions + 1] = end;
        float step = 1.0f / (intermediatePostitions + 1);

        for (int i = 1; i < intermediatePostitions + 1; i++)
        {
            result[i] = Vector3.Lerp(start, end, step * i);
        }

        return result;
    }
}