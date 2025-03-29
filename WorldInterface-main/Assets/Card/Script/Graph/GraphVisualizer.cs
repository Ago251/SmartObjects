using System;
using Unity.Mathematics;
using UnityEngine;

public class GraphVisualizer : MonoBehaviour
{
    [SerializeField] private WorldGraph _graph;

    [SerializeField]
    private Transform _startNode;

    [SerializeField]
    private Transform _endNode;

    [SerializeField]
    private bool _execute;

    private float3[] _path = Array.Empty<float3>();

    private void Update()
    {
        if (!_execute)
        {
            return;
        }

        FindPath();
        _execute = false;
    }

    private void FindPath()
    {
        if (_graph == null || _graph.Graph == null)
        {
            return;
        }

        if (_startNode == null)
        {
            return;
        }

        if (_endNode == null)
        {
            return;
        }

        _path = _graph.Graph.GetPathTo(_startNode.position, _endNode.position).ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (var i = 0; i < _path.Length - 1; i++)
        {
            Gizmos.DrawLine(_path[i], _path[i + 1]);
        }

        Gizmos.color = Color.green;
        foreach (var position in _path)
        {
            Gizmos.DrawWireSphere(position, .6f);
        }
    }
}
