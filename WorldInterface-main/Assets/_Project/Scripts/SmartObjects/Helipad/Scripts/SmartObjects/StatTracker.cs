using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public Stat[] Stat;

    public Stat GetStatByType(StatType type)
    {
        foreach (var stat in Stat)
        {
            if (stat.Type == type)
            {
                return stat;
            }
        }

        return null;
    }
}
