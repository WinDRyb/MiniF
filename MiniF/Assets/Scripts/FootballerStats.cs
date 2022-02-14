using UnityEngine;

public class FootballerStats : MonoBehaviour {
    [Range(80f, 120f), SerializeField] private float movementSpeed = 100f;
    [Range(0.8f, 0.95f), SerializeField] private float movementWithBallSlow = 0.88f;
    [Range(260f, 360f), SerializeField] private float rotationSpeed = 300f;
    [Range(1.5f, 2.5f), SerializeField] private float slideTackleSpeed = 2f;

    [Range(5f, 10f), SerializeField] private float maxActionPower = 10f;
    [Range(1f, 15f), SerializeField] private float passAccuracy = 10f;

    public float MovementSpeed {
        get { return movementSpeed; }
        set {
            if (value >= 80f && value <= 120f) {
                movementSpeed = value;
            }
        }
    }

    public float MovementWithBallSlow {
        get { return movementWithBallSlow; }
        set {
            if (value >= 0.8f && value <= 0.95f) {
                movementWithBallSlow = value;
            }
        }
    }

    public float RotationSpeed {
        get { return rotationSpeed; }
        set {
            if (value >= 260f && value <= 360f) {
                rotationSpeed = value;
            }
        }
    }
    
    public float SlideTackleSpeed {
        get { return slideTackleSpeed; }
        set {
            if (value >= 1.5f && value <= 2.5f) {
                slideTackleSpeed = value;
            }
        }
    }

    public float MaxActionPower {
        get { return maxActionPower; }
        set {
            if (value >= 2f && value <= 5f) {
                maxActionPower = value;
            }
        }
    }

    public float PassAccuracy {
        get { return passAccuracy; }
        set {
            if (value >= 1f && value <= 15f) {
                passAccuracy = value;
            }
        }
    }
}