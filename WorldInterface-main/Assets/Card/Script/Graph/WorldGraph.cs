using System;
using UnityEngine;

public class WorldGraph : MonoBehaviour
{
    [Serializable]
    public class Edge
    {
        public Transform Start;
        public Transform End;
        public int Weight = 1;
    }

    public Transform[] Nodes = Array.Empty<Transform>();
    public Edge[] Edges = Array.Empty<Edge>();

    private Graph _graph;

    public Graph Graph => _graph;

    private void Start()
    {
        _graph = new Graph();
        foreach (var node in Nodes)
        {
            _graph.AddNode(node.position);
        }

        foreach (var edge in Edges)
        {
            _graph.AddEdge(edge.Start.position, edge.End.position, edge.Weight);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var node in Nodes)
        {
            Gizmos.DrawSphere(node.position, .5f);
        }

        Gizmos.color = Color.yellow;
        foreach (var edge in Edges)
        {
            Gizmos.DrawLine(edge.Start.position, edge.End.position);
        }
    }
}
