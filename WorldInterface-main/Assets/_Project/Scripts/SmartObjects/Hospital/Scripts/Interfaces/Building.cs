using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IRescueable
{
    public event Action<IRescueable> OnEmergency;

    [SerializeField]
    private List<GameObject> agents = new List<GameObject> ();

    private void OnEnable()
    {
        RescueManager.Instance.Register(this);
        OnEmergency?.Invoke(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public List<GameObject> Rescue(int capacityMax)
    {
        var totalRescues = Mathf.Clamp(capacityMax, 0, agents.Count);
        var rescues = agents.GetRange(0, totalRescues);
        agents.RemoveRange(0, totalRescues);
        return rescues;
    }
}
