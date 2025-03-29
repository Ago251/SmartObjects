using System.Collections.Generic;
using WorldInterface.MiniCity.Hospital;

public class RescueManager : Singleton<RescueManager>
{
    public List<HospitalSmartObject> Hospitals = new();
    public readonly List<IRescueable> emergencies = new();

    public void OnEmergency(IRescueable emergency)
    {
        emergencies.Add(emergency);
    }

    public void Register(HospitalSmartObject hospital)
    {
        Hospitals.Add(hospital);
    }
    
    public void Unregister(HospitalSmartObject hospital)
    {
        Hospitals.Remove(hospital);
    }
    
    public void Register(IRescueable rescueable)
    {
        rescueable.OnEmergency += Instance.OnEmergency;
    }
    
    public void Unregister(IRescueable rescueable)
    {
        rescueable.OnEmergency -= Instance.OnEmergency;
    }
}
