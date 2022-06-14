using UnityEngine;

public static class GeneralHelpers {
    public static int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    // returns true if target1 is closer to position than target2
    public static bool IsCloser(Vector3 position, Vector3 target1, Vector3 target2) {
        if ((target1 - position).sqrMagnitude < (target2 - position).sqrMagnitude) {
            return true;
        } 
        
        return false;
    }
    
    // returns true if vector is between bounds
    public static bool IsVectorBetweenBounds(Vector3 vector, Vector3 bound1, Vector3 bound2) {
        float angleArea = Vector3.Angle(bound1, bound2);

        if (Vector3.Angle(bound1, vector) < angleArea && Vector3.Angle(bound2, vector) < angleArea) {
            return true;
        }

        return false;
    }
}