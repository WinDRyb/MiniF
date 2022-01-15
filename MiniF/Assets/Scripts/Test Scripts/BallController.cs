using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private HingeJoint _hingeJoint;
    private float ballMass;
    private bool repairJoingAnchor;
    private Vector3 frontAnchorPoint = Vector3.right * 0.3f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        ballMass = _rigidbody.mass;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Footballer"))
        {
           OnFootballerPossessionEnter(collision.gameObject);
        }
    }

    private void OnFootballerPossessionEnter(GameObject footballer)
    {
        // create and setup new FixedJoint
        _hingeJoint = gameObject.AddComponent<HingeJoint>();
        _hingeJoint.connectedBody = footballer.GetComponent<Rigidbody>();
        _hingeJoint.autoConfigureConnectedAnchor = false;
        //_hingeJoint.connectedAnchor = Vector3.right * 0.3f;
        repairJoingAnchor = true;

        // lower mass of ball so it doesn't make footballer move slower
        _rigidbody.mass = 0.001f;
    }

    private void OnFootballerPossessionExit()
    {
        // remove FixedJoint
        Destroy(_hingeJoint);
        // set back normal ball mass
        _rigidbody.mass = ballMass;
    }

    private void FixedUpdate()
    {
        if (repairJoingAnchor)
        {
            RepairHingeJointAnchorPosition();
        }
    }

    private void RepairHingeJointAnchorPosition()
    {
        // slowly move ball to right anchor point
        _hingeJoint.connectedAnchor = Vector3.MoveTowards(_hingeJoint.connectedAnchor, frontAnchorPoint, 0.01f);
        // stop when vectors are approximately even
        if (_hingeJoint.connectedAnchor == frontAnchorPoint)
        {
            repairJoingAnchor = false;
        }
    }
}
