using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class GraphMoveBehaviour : SteeringBehaviour
{
    private readonly float _maxAcceleration = 100;

    [SerializeField]
    private float _reachRadius = 1;

    [SerializeField]
    private float _timeToTarget = 0.1f;

    private int _currentIndex = -1;

    [SerializeField]
    public WorldGraph _graph;

    //[SerializeField]
    public Transform _target { get; set; }

    [SerializeField]
    public bool _recalculatePath;

    private List<float3> _path;

    public override SteeringOutput GetSteering(Agent agent)
    {
        if (_recalculatePath)
        {
            RecalculatePath(agent);
            _recalculatePath = false;
        }

        if (_path == null)
        {
            return new SteeringOutput();
        }

        if (math.distance(_path[_currentIndex], agent.Position) <= _reachRadius)
        {
            _currentIndex = math.clamp(_currentIndex + 1, 0, _path.Count - 1);
        }

        var targetPosition = _path[_currentIndex];

        //Arrive
        var distance = math.distance(agent.Position, targetPosition);

        var targetSpeed = agent.MaxLinearSpeed;

        if (distance <= _reachRadius)
        {
            targetSpeed = 0;
        }

        var direction = math.normalizesafe(targetPosition - agent.Position);
        var targetVelocity = direction * targetSpeed;

        var result = new SteeringOutput { Linear = targetVelocity - agent.LinearVelocity };
        result.Linear /= _timeToTarget;

        result.Linear = math.normalizesafe(result.Linear) * math.min(math.length(result.Linear), _maxAcceleration);
        result.Angular = agent.Orientation;

        return result;
    }

    private void RecalculatePath(Agent agent)
    {
        _currentIndex = 0;
        var startNode = _graph.Graph.GetNearestGraphPoint(agent.Position);
        var endNode = _graph.Graph.GetNearestGraphPoint(_target.position);

        var graphPath = _graph.Graph.GetPathTo(startNode, endNode);
        graphPath.Add(_target.position);
        _path = graphPath;
    }
}