using UnityEngine;

public class FootballerStats : MonoBehaviour {
    [Range(180f, 220f), SerializeField] private float movementSpeed = 200f;
    [Range(0.8f, 0.95f), SerializeField] private float movementWithBallSlow = 0.88f;
    [Range(450f, 550f), SerializeField] private float rotationSpeed = 500f;
    [Range(1.5f, 2.5f), SerializeField] private float slideTackleSpeed = 2f;

    [Range(7f, 10f), SerializeField] private float maxActionPower = 9f;
    [Range(1f, 15f), SerializeField] private float passInaccuracy = 10f;
    [Range(1f, 15f), SerializeField] private float shotInaccuracy = 10f;

    public float MovementSpeed {
        get { return movementSpeed; }
        set {
            if (value >= 180f && value <= 220f) {
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
            if (value >= 450f && value <= 550f) {
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
            if (value >= 7f && value <= 10f) {
                maxActionPower = value;
            }
        }
    }

    public float PassInaccuracy {
        get { return passInaccuracy; }
        set {
            if (value >= 1f && value <= 15f) {
                passInaccuracy = value;
            }
        }
    }
    
    public float ShotInaccuracy {
        get { return shotInaccuracy; }
        set {
            if (value >= 1f && value <= 15f) {
                shotInaccuracy = value;
            }
        }
    }
}