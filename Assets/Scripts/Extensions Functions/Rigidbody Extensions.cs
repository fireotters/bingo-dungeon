using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public static class Rigidbody_Extensions
{
    /// <summary>
    /// Makes the rigidbody accelerate to targetVelocity with maxAcceleration acceleration.
    /// </summary>
    public static float2 AccelerateTo2D(this Rigidbody2D rb, Vector2 targetVelocity, float maxAcceleration = float.PositiveInfinity)
    {
        float2 deltaVelocity = targetVelocity - rb.velocity;
        float2 acceleration = deltaVelocity / Time.deltaTime;
        
        if (lengthsq(acceleration) > maxAcceleration * maxAcceleration)
            acceleration = normalize(acceleration) * maxAcceleration;

        rb.AddForce(acceleration / rb.mass, ForceMode2D.Force);
        return acceleration;
    }

    /// <summary>
    /// Makes the rigidbody accelerate to targetVelocity with maxAcceleration acceleration.
    /// Inverse mass is needed as Physics2D doesn't have an acceleration mode, so it's needed to manually calculate the acceleration
    /// correctly.
    /// </summary>
    public static float2 AccelerateTo2D(this Rigidbody2D rb, Vector2 targetVelocity, float inverseMass, float maxAcceleration = float.PositiveInfinity)
    {
        float2 deltaVelocity = targetVelocity - rb.velocity;
        float2 acceleration = deltaVelocity / Time.deltaTime;
        
        if (lengthsq(acceleration) > maxAcceleration * maxAcceleration)
            acceleration = normalize(acceleration) * maxAcceleration;

        rb.AddForce(acceleration * inverseMass, ForceMode2D.Force);
        return acceleration;
    }

    /// <summary>
    /// Makes the rigidbody accelerate to targetVelocity with maxAcceleration acceleration.
    /// </summary>
    public static float3 AccelerateTo(this Rigidbody rb, Vector3 targetVelocity, float maxAcceleration = float.PositiveInfinity)
    {
        Vector3 deltaVelocity = targetVelocity - rb.velocity;
        float3 acceleration = deltaVelocity / Time.deltaTime;

        if (lengthsq(acceleration) > maxAcceleration * maxAcceleration)
            acceleration = normalize(acceleration) * maxAcceleration;

        rb.AddForce(acceleration, ForceMode.Acceleration);
        return acceleration;
    }
}