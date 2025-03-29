using System;
using Unity.Mathematics;

public struct SteeringOutput
{
    public float3 Linear;
    public float Angular;
}

[Serializable]
public abstract class SteeringBehaviour
{
    public abstract SteeringOutput GetSteering(Agent agent);
}
