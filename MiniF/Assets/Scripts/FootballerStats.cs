using UnityEngine;

public class FootballerStats : MonoBehaviour
{
   [Range(1f, 3f), SerializeField] private float movementSpeed = 1f;
   [Range(0.8f, 0.95f), SerializeField] private float movementWithBallSlow = 0.88f;
   [Range(100f, 150f), SerializeField] private float rotationSpeed = 120f;
   
   [Range(5f, 10f), SerializeField] private float maxActionPower = 10f;
   [Range(1f, 15f), SerializeField] private float passAccuracy = 10f;
   
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
   
   public float MovementWithBallSlow
   {
      get { return movementWithBallSlow; }
      set {
         if (value >= 0.8f && value <= 0.95f)
         {
            movementWithBallSlow = value;
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
   
   public float MaxActionPower
   {
      get { return maxActionPower; }
      set {
         if (value >= 2f && value <= 5f)
         {
            maxActionPower = value;
         }
      }
   }
   
   public float PassAccuracy
   {
      get { return passAccuracy; }
      set {
         if (value >= 1f && value <= 15f)
         {
            passAccuracy = value;
         }
      }
   }
}
