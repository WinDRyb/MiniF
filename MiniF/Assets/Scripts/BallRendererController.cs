using UnityEngine;

public class BallRendererController : MonoBehaviour {
    private Vector3 parentPosition;
    private Vector3 nextPosition;

    private void Update() {
        parentPosition = transform.parent.position;
        nextPosition.x = parentPosition.x;
        nextPosition.y = parentPosition.y + parentPosition.z;
        nextPosition.z = 0f;
        transform.position = nextPosition;
    }
}