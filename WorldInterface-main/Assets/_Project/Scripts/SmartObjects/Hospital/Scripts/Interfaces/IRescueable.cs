using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRescueable
{
    event Action<IRescueable> OnEmergency;

    List<GameObject> Rescue(int capacityMax);

    public GameObject GetGameObject();
}
