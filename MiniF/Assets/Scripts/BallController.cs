using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour {
    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private HingeJoint _hingeJoint;
    private CapsuleCollider _currentFootballerCollider;
    private Footballer _currentFootballerScript;
    private MatchController _matchController;
    
    private bool repairJointAnchor;
    private Vector3 frontAnchorPoint = Vector3.right * 0.3f;

    private bool isInPlay = true;
    public bool IsInPlay {
        get { return isInPlay; }
        set { isInPlay = value; }
    }

    private Team teamInPossessionOfBall = Team.None;
    public Team TeamInPossessionOfBall {
        get { return teamInPossessionOfBall; }
        set { teamInPossessionOfBall = value; }
    } 
    
    private Team lastTeamInPossessionOfBall = Team.None;
    public Team LastTeamInPossessionOfBall {
        get { return lastTeamInPossessionOfBall; }
        set { lastTeamInPossessionOfBall = value; }
    } 
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Footballer")) {
            if (!_currentFootballerCollider) {
                OnFootballerPossessionEnter(collision.gameObject);
            }

            if (_currentFootballerCollider && collision.gameObject.GetComponent<Footballer>().IsDuringSlide) {
                OnFootballerPossessionExit();
                OnFootballerPossessionEnter(collision.gameObject);
            }
        }
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
    
    private void OnFootballerPossessionEnter(GameObject footballer) {
        // create and setup new HingeJoint
        _hingeJoint = gameObject.AddComponent<HingeJoint>();
        _hingeJoint.connectedBody = footballer.GetComponent<Rigidbody>();
        _hingeJoint.autoConfigureConnectedAnchor = false;
        repairJointAnchor = true;

        // let footballer know that he is in possession of ball now
        _currentFootballerScript = footballer.GetComponent<Footballer>();
        _currentFootballerScript.HasBall = true;
        _currentFootballerCollider = footballer.GetComponent<CapsuleCollider>();

        // set team in possession of ball
        teamInPossessionOfBall = _currentFootballerScript.FootballerTeam;
        lastTeamInPossessionOfBall = _currentFootballerScript.FootballerTeam;
        
        // change player controlled footballer when ai controlled teammate captured ball
        if (isInPlay) {
            _matchController.SetPlayerControlledFootballer(footballer, lastTeamInPossessionOfBall);
        }

        // disable collision so HingeJoint can be corrected
        Physics.IgnoreCollision(_collider, _currentFootballerCollider, true);
    }

    public void OnFootballerPossessionExit() {
        if (!_currentFootballerCollider) {
            return;
        }
        // after loss of possession ignore collisions between footballer and ball for short while
        Physics.IgnoreCollision(_collider, _currentFootballerCollider, true);
        // call coroutine to reenable it soon
        StartCoroutine(ReEnableCollisions(_currentFootballerCollider, 0.5f));

        // in case position of anchor is still changing
        repairJointAnchor = false;
        // remove HingeJoint
        Destroy(_hingeJoint);

        teamInPossessionOfBall = Team.None;

        // footballer is not in possession of ball
        _currentFootballerScript.HasBall = false;
        _currentFootballerCollider = null;
    }

    public void SetNewBallPosition(Vector3 position) {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.rotation = Quaternion.Euler(Vector3.zero);
        _rigidbody.position = position;
    }
}