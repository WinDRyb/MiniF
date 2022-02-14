using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour {
    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private HingeJoint _hingeJoint;
    private CapsuleCollider _currentFootballerCollider;
    private Footballer _currentFootballerScript;
    private float ballMass;
    private bool repairJointAnchor;
    private Vector3 frontAnchorPoint = Vector3.right * 0.3f;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        ballMass = _rigidbody.mass;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Footballer")) {
            _currentFootballerScript = collision.gameObject.GetComponent<Footballer>();
                
            if (!_currentFootballerCollider) {
                OnFootballerPossessionEnter(collision.gameObject);
            }

            if (_currentFootballerCollider && _currentFootballerScript.IsDuringSlide) {
                OnFootballerPossessionEnter(collision.gameObject);
            }
        }
    }

    private void OnFootballerPossessionEnter(GameObject footballer) {
        // create and setup new HingeJoint
        _hingeJoint = gameObject.AddComponent<HingeJoint>();
        _hingeJoint.connectedBody = footballer.GetComponent<Rigidbody>();
        _hingeJoint.autoConfigureConnectedAnchor = false;
        repairJointAnchor = true;

        // lower mass of ball so it doesn't make footballer move slower
        _rigidbody.mass = 0.001f;

        // let footballer know that he is in possession of ball now
        _currentFootballerScript.HasBall = true;
        _currentFootballerCollider = footballer.GetComponent<CapsuleCollider>();
        // disable collision so HingeJoint can be corrected
        Physics.IgnoreCollision(_collider, _currentFootballerCollider, true);
    }

    public void OnFootballerPossessionExit() {
        // after loss of possession ignore collisions between footballer and ball for short while
        Physics.IgnoreCollision(_collider, _currentFootballerCollider, true);
        // call coroutine to reenable it soon
        StartCoroutine(ReEnableCollisions(_currentFootballerCollider, 0.5f));

        // in case position of anchor is still changing
        repairJointAnchor = false;
        // remove HingeJoint
        Destroy(_hingeJoint);
        // set back normal ball mass
        _rigidbody.mass = ballMass;

        // footballer is not in possession of ball
        _currentFootballerScript.HasBall = false;
        _currentFootballerCollider = null;
    }

    private void FixedUpdate() {
        if (repairJointAnchor) {
            RepairHingeJointAnchorPosition();
        }
    }

    private void RepairHingeJointAnchorPosition() {
        // slowly move ball to right anchor point
        _hingeJoint.connectedAnchor = Vector3.MoveTowards(_hingeJoint.connectedAnchor, frontAnchorPoint, 0.01f);
        // stop when vectors are approximately even
        if (_hingeJoint.connectedAnchor == frontAnchorPoint) {
            repairJointAnchor = false;
            // enable collisions with footballer when HingeJoint is in the right place
            Physics.IgnoreCollision(_collider, _currentFootballerCollider, false);
        }
    }

    IEnumerator ReEnableCollisions(CapsuleCollider footballerCollider, float delayTime) {
        yield return new WaitForSeconds(delayTime);

        // enable collisions with previous footballer
        Physics.IgnoreCollision(_collider, footballerCollider, false);
    }
}