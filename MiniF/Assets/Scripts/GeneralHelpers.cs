using UnityEngine;

public static class GeneralHelpers
{
   public static int RandomSign()
   {
      return Random.Range(0, 2) * 2 - 1;
   }
}
