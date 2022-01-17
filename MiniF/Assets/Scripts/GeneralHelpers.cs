using UnityEngine;

public static class GeneralHelpers
{
   public static int RandomSign()
   {
      return Random.Range(0, 2) * 2 - 1;
   }

   // returns true if target1 is closer to position than target2
   public static bool IsCloser(Vector3 position, Vector3 target1, Vector3 target2)
   {
      if ((target1 - position).sqrMagnitude < (target2 - position).sqrMagnitude)
      {
         return true;
      }
      else
      {
         return false;
      }
   }
}
