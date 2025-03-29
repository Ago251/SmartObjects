using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldInterface;

public class HelipadManager : Singleton<HelipadManager>
{
    private List<Helipad> _helipads = new List<Helipad>();

    public static void Register(Helipad helipad)
    {
        Instance._helipads.Add(helipad);
    }

    public static void Unregister(Helipad helipad)
    {
        Instance._helipads.Remove(helipad);
    }

    public bool AnyHelipadsAvailable()
    {
        return _helipads.Any(x => x.IsAvailable);
    }

    public Helipad GetAvailableHelipad(HelicopterSmartObject helicopter, Helipad previousHelipad)
    {
        if (!AnyHelipadsAvailable())
        {
            return null;
        }

        var firstAvailableHelipad = _helipads.Where(x => x.IsAvailable)
                                             .Where(x => x != previousHelipad)
                                             .Random();

        firstAvailableHelipad.IsAvailable = false;
        firstAvailableHelipad.TargetHelicopter = helicopter;

        return firstAvailableHelipad;
    }

    public Helipad FreeHelipadPosition(HelicopterSmartObject helicopter)
    {
        var helipad = _helipads.Find(x => x.TargetHelicopter == helicopter);

        helipad.IsAvailable = true;
        helipad.TargetHelicopter = null;

        return helipad;
    }
}
