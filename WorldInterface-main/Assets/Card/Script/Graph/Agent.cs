using System;
using Unity.Mathematics;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField]
    private float _maxLinearSpeed = 10;

    [SerializeField]
    private float _maxAngularSpeed = 360f;

    [SerializeField]
    private float _radius = .5f;

    [SerializeReference, SubclassSelector]
    public SteeringBehaviour[] _steering = Array.Empty<SteeringBehaviour>();

    public float MaxLinearSpeed => _maxLinearSpeed;
    public float MaxAngularSpeed => _maxAngularSpeed;
    public float Radius => _radius;

    public float3 LinearVelocity { get; private set; }
    public float AngularVelocity { get; private set; }

    public float3 Position
    {
        get => transform.position;
        private set => transform.position = value;
    }

    public float Orientation
    {
        get => math.radians(transform.eulerAngles.y);
        private set => transform.rotation = quaternion.Euler(0, value, 0);
    }

    public void FixedUpdate()
    {
        foreach (var currentSteering in _steering)
        {
            var steering = currentSteering.GetSteering(this);
            LinearVelocity += steering.Linear * Time.fixedDeltaTime;
            AngularVelocity += steering.Angular * Time.fixedDeltaTime;
        }

        LinearVelocity = math.normalizesafe(LinearVelocity) * math.min(math.length(LinearVelocity), MaxLinearSpeed);
        AngularVelocity = math.clamp(math.abs(AngularVelocity), 0, MaxAngularSpeed) * math.sign(AngularVelocity);

        Position += LinearVelocity * Time.fixedDeltaTime;
        Orientation += AngularVelocity * Time.fixedDeltaTime;
    }
}
