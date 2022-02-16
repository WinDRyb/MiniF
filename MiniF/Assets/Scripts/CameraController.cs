using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    
    private Camera _camera;
    
    [SerializeField] private Transform _ballPosition;

    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    
    private Vector3 position = Vector3.zero;
    
    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate() {
        position = Vector3.SmoothDamp(transform.position, _ballPosition.position, ref velocity, smoothTime);
        position.z = transform.position.z;
        transform.position = position;
    }
}
