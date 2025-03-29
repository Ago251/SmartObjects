using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class LookWhereYouMoveSteeringBehaviour : SteeringBehaviour
{

    public override SteeringOutput GetSteering(Agent agent)
    {
        // Calcola l'orientamento target basato sulla velocità lineare
        var targetOrientation = math.atan2(agent.LinearVelocity.x, agent.LinearVelocity.z);

        // Normalizza l'orientamento dell'agente e il target
        var currentOrientation = MapToRange(agent.Orientation);

        // Calcola la differenza tra l'orientamento target e quello attuale
        var delta = targetOrientation - currentOrientation;

        // Normalizza la differenza tra -PI e PI
        var targetAngularSpeed = MapToRange(delta);


        // Calcola la direzione della rotazione
        var direction = math.sign(targetAngularSpeed);
        var targetAngularVelocity = direction * math.abs(targetAngularSpeed);

        // Calcola l'output di sterzata
        var result = new SteeringOutput
        {
            Linear = 0,
            Angular = (targetAngularVelocity - agent.AngularVelocity) / Time.fixedDeltaTime
        };

        // Clampa la velocità angolare all'intervallo massimo consentito
        result.Angular = math.sign(result.Angular) * math.clamp(math.abs(result.Angular), 0, agent.MaxAngularSpeed);

        return result;
    }

    private float MapToRange(float angle)
    {
        // Normalizza l'angolo tra -PI e PI
        angle = math.fmod(angle + math.PI, 2 * math.PI);
        if (angle < 0)
        {
            angle += 2 * math.PI;
        }
        return angle - math.PI;
    }

}
