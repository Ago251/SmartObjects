using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace WorldInterface.MiniCity.Hospital
{
    [Serializable]
    public struct Seat
    {
        public GameObject Agent;
        public StatTracker StatTracker;

        public void Release()
        {
            Agent = null;
            StatTracker = null;
        }
    }
}
