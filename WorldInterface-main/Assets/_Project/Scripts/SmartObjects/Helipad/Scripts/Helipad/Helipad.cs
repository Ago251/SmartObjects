using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helipad : MonoBehaviour
{
    public bool IsAvailable {  get; set; }

    public HelicopterSmartObject TargetHelicopter { get; set; }

    [SerializeField, Min(0)]
    private float _detectionRadius = 2;

    [SerializeField]
    private LayerMask _detectionMask;

    private void Awake()
    {
        var overlappingColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _detectionMask);
        IsAvailable = !overlappingColliders.Any();

        if (overlappingColliders.Any())
        {
            var helicopter = overlappingColliders.First().GetComponentInParent<HelicopterSmartObject>();
            TargetHelicopter = helicopter;
        }

        string helipadCondition = IsAvailable ? "available" : "unavailable";
        Debug.Log($"HELIPAD {name} is {helipadCondition}");
    }

    private void OnEnable()
    {
        HelipadManager.Register(this);
    }

    private void OnDisable()
    {
        HelipadManager.Unregister(this);
    }
}
