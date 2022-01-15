using System.Collections.Generic;
using UnityEngine;

public static class FootballHelpers
{
    // returns closest target of action eg. pass or shot
    // takes as arguments 2 Vectors between which search is done 
    public static Vector3 GetActionTargetPosition(Vector3 actionPosition, Vector3 bound1, Vector3 bound2, List<Vector3> targetList)
    {
        float angleArea = Vector3.Angle(bound1, bound2);
        Vector3 closestTarget = Vector3.zero;
        float closestTargetDistance = float.MaxValue;
        
        foreach (Vector3 target in targetList)
        {
            // target is between given vectors
            if (Vector3.Angle(bound1, target - actionPosition) < angleArea && Vector3.Angle(bound2, target - actionPosition) < angleArea)
            {
                float targetDistance = Vector3.Distance(actionPosition, target);
                if (targetDistance < closestTargetDistance)
                {
                    closestTarget = target;
                }
            }
        }

        return closestTarget;
    }
}
