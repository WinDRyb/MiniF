using UnityEngine;

public class FootballerStats : MonoBehaviour
{
   [Range(1f, 3f), SerializeField] private float movementSpeed = 1f;
   [Range(100f, 150f), SerializeField] private float rotationSpeed = 120f;
   
   public float MovementSpeed
   {
      get { return movementSpeed; }
      set {
         if (value >= 1f && value <= 3f)
         {
            movementSpeed = value;
         }
      }
   }
   
   public float RotationSpeed
   {
      get { return rotationSpeed; }
      set {
         if (value >= 100f && value <= 150f)
         {
            rotationSpeed = value;
         }
      }
   }
}
