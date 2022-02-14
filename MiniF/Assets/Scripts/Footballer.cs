using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FootballerStats))]
public class Footballer : MonoBehaviour {
    private bool isImmobilized;
    private bool hasBall;
    public bool HasBall {
        get { return hasBall; }
        set { hasBall = value; }
    }

    private bool isDuringSlide;
    public bool IsDuringSlide {
        get { return isDuringSlide; }
    }

    private Rigidbody _rigidbody;
    private FootballerStats _footballerStats;
    private FootballerAnimationController _footballerAnimationController;
    private Rigidbody _ballRigidbody;
    private BallController _ballController;
    private MatchController _matchController;
    public MatchController _MatchController {
        get { return _matchController; }
    }
    
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _footballerStats = GetComponent<FootballerStats>();
        _footballerAnimationController = GetComponentInChildren<FootballerAnimationController>();
        GameObject ball = GameObject.FindWithTag("Ball");
        _ballRigidbody = ball.GetComponent<Rigidbody>();
        _ballController = ball.GetComponent<BallController>();
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
    }

    public void MoveTo(Vector3 targetPosition, float power) {
        Vector3 velocity = (targetPosition - transform.position) * power;
        _rigidbody.velocity = velocity;
    }

    public void MoveForward() {
        if (hasBall) {
            _rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow);
        }
        else {
            _rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * _footballerStats.MovementSpeed);
        }
    }

    public void MoveInDirection(Vector3 direction) {
        // don't do anything if footballer is immobilized
        if (isImmobilized) {
            return;
        }
        
        if (hasBall) {
            //_rigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow);
            _rigidbody.velocity = (transform.position + direction - transform.position) * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow;
        }
        else {
            //_rigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed);
            _rigidbody.velocity = (transform.position + direction - transform.position) * Time.fixedDeltaTime * _footballerStats.MovementSpeed;
        }

        // rotate in movement direction
        if (direction != Vector3.zero) {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            targetRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _footballerStats.RotationSpeed * Time.fixedDeltaTime);
        
            _rigidbody.MoveRotation(targetRotation);
        }
    }

    public void CharacterRotate(Vector3 rotation) {
        Quaternion rot = Quaternion.Euler(rotation * Time.fixedDeltaTime * _footballerStats.RotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * rot);
    }

    // returns gameobject if there was a target for pass
    public GameObject MakePass(float power, float zVelocity = 0f) {
        if (hasBall) {
            Vector3 bound1 = Quaternion.Euler(0f, 0f, 25f) * transform.right;
            Vector3 bound2 = Quaternion.Euler(0f, 0f, -25f) * transform.right;
            GameObject target = FootballHelpers.GetActionTargetPosition(transform.position, bound1, bound2,
                _matchController.GetAllPlayers());

            Vector3 direction;
            // pass ahead if there is no target, otherwise pass to target
            if (target) {
                direction = (target.transform.position - transform.position).normalized;
            }
            else {
                direction = transform.right;
            }

            // set how high will ball go
            direction.z = zVelocity;

            float accuracy = UnityEngine.Random.Range(0f, 15f - _footballerStats.PassAccuracy) * GeneralHelpers.RandomSign();
            direction = Quaternion.Euler(0f, 0f, accuracy) * direction;

            // clamp power with footballer stats
            power = Mathf.Clamp(power, 3f, _footballerStats.MaxActionPower);

            _ballController.OnFootballerPossessionExit();
            _ballRigidbody.velocity = direction * power;
            return target;
        }

        return null;
    }

    public void MakeSlideTackle() {
        if (isImmobilized) {
            return;
        }
        // move forward while sliding
        MoveTo(transform.position + transform.right, _footballerStats.SlideTackleSpeed);
        // immobilize footballer after slide tackle
        isImmobilized = true;
        isDuringSlide = true;
        _footballerAnimationController.PlaySlideTackleAnimation();

        StartCoroutine(DisableImmobilization(1f));
        StartCoroutine(DisableSliding(1f));
    }
    
    IEnumerator DisableImmobilization(float time) {
        yield return new WaitForSeconds(time);
        isImmobilized = false;
    }
    
    IEnumerator DisableSliding(float time) {
        yield return new WaitForSeconds(time);
        isDuringSlide = false;
    }
}