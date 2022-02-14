using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public static class FootballHelpers {
    // returns closest target of action eg. pass or shot
    // takes as arguments 2 Vectors between which search is done 
    public static Vector3 GetActionTargetPosition(Vector3 actionPosition, Vector3 bound1, Vector3 bound2,
        List<Vector3> targetList) {
        float angleArea = Vector3.Angle(bound1, bound2);
        Vector3 closestTarget = Vector3.zero;
        float closestTargetDistance = float.MaxValue;

        foreach (Vector3 target in targetList) {
            // can't target itself
            if (target == actionPosition) {
                continue;
            }

            // target is between given vectors
            if (Vector3.Angle(bound1, target - actionPosition) < angleArea && Vector3.Angle(bound2, target - actionPosition) < angleArea) {
                float targetDistance = (target - actionPosition).sqrMagnitude;
                if (targetDistance < closestTargetDistance) {
                    closestTargetDistance = targetDistance;
                    closestTarget = target;
                }
            }
        }

        return closestTarget;
    }

    public static GameObject GetActionTargetPosition(Vector3 actionPosition, Vector3 bound1, Vector3 bound2,
        List<GameObject> targetList) {
        float angleArea = Vector3.Angle(bound1, bound2);
        GameObject closestTarget = null;
        float closestTargetDistance = float.MaxValue;

        foreach (GameObject target in targetList) {
            Vector3 targetPosition = target.transform.position;
            // can't target itself
            if (targetPosition == actionPosition) {
                continue;
            }

            // target is between given vectors
            if (Vector3.Angle(bound1, targetPosition - actionPosition) < angleArea && Vector3.Angle(bound2, targetPosition - actionPosition) < angleArea) {
                float targetDistance = (targetPosition - actionPosition).sqrMagnitude;
                if (targetDistance < closestTargetDistance) {
                    closestTargetDistance = targetDistance;
                    closestTarget = target;
                }
            }
        }

        return closestTarget;
    }

    public static GameObject GetClosestTarget(Vector3 position, List<GameObject> targetList) {
        GameObject closestTarget = null;
        float closestTargetDistance = float.MaxValue;

        foreach (GameObject target in targetList) {
            Vector3 targetPosition = target.transform.position;
            // can't target itself
            if (targetPosition == position) {
                continue;
            }

            float targetDistance = (targetPosition - position).sqrMagnitude;
            if (targetDistance < closestTargetDistance) {
                closestTargetDistance = targetDistance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }
}