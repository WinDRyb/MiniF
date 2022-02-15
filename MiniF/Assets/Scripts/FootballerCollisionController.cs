using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class FootballerCollisionController : MonoBehaviour {

    private Footballer _footballerScript;
    private CapsuleCollider _collider;
    private Rigidbody _rigidbody;

    private void Awake() {
        _footballerScript = GetComponent<Footballer>();
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Footballer")) {
            CapsuleCollider otherFootballerCollider = collision.gameObject.GetComponent<CapsuleCollider>();
            Rigidbody otherFootballerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Transform otherFootballerTransform = collision.gameObject.GetComponent<Transform>();
            
            if (collision.gameObject.GetComponent<Footballer>().IsDuringSlide) {
                _footballerScript.Fall();
                
                // ignore collisions between these two footballers for a second
                Physics.IgnoreCollision(_collider, otherFootballerCollider, true);
                StartCoroutine(ReEnableCollisions(otherFootballerCollider, 1f));
                
                // add velocity to both of them
                otherFootballerRigidbody.velocity = collision.relativeVelocity.magnitude * otherFootballerTransform.right;
                _rigidbody.velocity = collision.relativeVelocity.magnitude * transform.right;
            } else {
                // ignore collisions between these two footballers for a second
                Physics.IgnoreCollision(_collider, otherFootballerCollider, true);
                StartCoroutine(ReEnableCollisions(otherFootballerCollider, 1f));
            }
        }
    }
    
    IEnumerator ReEnableCollisions(CapsuleCollider footballerCollider, float delayTime) {
        yield return new WaitForSeconds(delayTime);

        // enable collisions with footballer
        Physics.IgnoreCollision(_collider, footballerCollider, false);
    }
}
