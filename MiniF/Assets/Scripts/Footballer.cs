using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FootballerStats))]
public class Footballer : MonoBehaviour {
    private Team footballerTeam;
    public Team FootballerTeam {
        get { return footballerTeam; }
        set { footballerTeam = value; }
    }
    
    private bool hasBall;
    public bool HasBall {
        get { return hasBall; }
        set { hasBall = value; }
    }

    private bool isImmobilized;
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

    private void RotateInDirection(Vector3 direction) {
        if (direction != Vector3.zero) {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            targetRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _footballerStats.RotationSpeed * Time.fixedDeltaTime);
        
            _rigidbody.MoveRotation(targetRotation);
        }
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
            Vector3 velocity = direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow;
            velocity.z = _rigidbody.velocity.z;
            _rigidbody.velocity = velocity;
        } else {
            Vector3 velocity = direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed;
            velocity.z = _rigidbody.velocity.z;
            _rigidbody.velocity = velocity;
        }

        // rotate in movement direction
        RotateInDirection(direction);
    }

    // returns true if footballer is within epsilon distance
    public bool MoveInDirectionWithDistanceCheck(Vector3 direction, Vector3 targetPosition, float epsilon) {
        // don't do anything if is close enough to target
        if (Vector3.Distance(transform.position, targetPosition) < epsilon) {
            _rigidbody.velocity = Vector3.zero;
            return true;
        }

        // don't do anything if footballer is immobilized
        if (isImmobilized) {
            return false;
        }
        
        if (hasBall) {
            Vector3 velocity = direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow;
            velocity.z = _rigidbody.velocity.z;
            _rigidbody.velocity = velocity;
        } else {
            Vector3 velocity = direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed;
            velocity.z = _rigidbody.velocity.z;
            _rigidbody.velocity = velocity;
        }

        // rotate in movement direction
        RotateInDirection(direction);
        return false;
    }

    // used by EventMovementController, returns footballer position
    public Vector3 MoveToPosition(Vector3 targetPosition) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        _rigidbody.velocity = direction * Time.fixedDeltaTime * _footballerStats.MovementSpeed;
        return transform.position;
    }

    public void CharacterRotate(Vector3 rotation) {
        Quaternion rot = Quaternion.Euler(rotation * Time.fixedDeltaTime * _footballerStats.RotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * rot);
    }

    // returns gameobject if there was a target for pass
    public GameObject Pass(float power, float zVelocity = 0f) {
        if (!hasBall) {
            return null;
        }

        Vector3 bound1 = Quaternion.Euler(0f, 0f, 25f) * transform.right;
        Vector3 bound2 = Quaternion.Euler(0f, 0f, -25f) * transform.right;
        GameObject target = FootballHelpers.GetActionTargetPosition(transform.position, bound1, bound2,
            _matchController.GetTeamPlayers(FootballerTeam));

        Vector3 direction;
        // pass ahead if there is no target, otherwise pass to target
        if (target) {
            direction = target.transform.position - transform.position;
        }
        else {
            direction = transform.right;
        }
        direction = direction.normalized;
        
        // set how high will ball go
        direction.z = zVelocity;
        
        // normalize again this time including z direction
        direction = direction.normalized;

        float inaccuracy = UnityEngine.Random.Range(0f, 15f - _footballerStats.PassInaccuracy) * GeneralHelpers.RandomSign();
        direction = Quaternion.Euler(0f, 0f, inaccuracy) * direction;

        // clamp power with footballer stats
        power = Mathf.Clamp(power, 3f, _footballerStats.MaxActionPower);

        _ballController.OnFootballerPossessionExit();
        _ballRigidbody.velocity = direction * power;
        return target;
    }

    public void Shot(Vector3 additionalDirection, float power, float minZVelocity = 0f, float maxZVelocity = 0f) {
        if (!hasBall) {
            return;
        }
        
        Vector3 position = transform.position;
        Vector3 goalPosition = _matchController.GetOpponentsGoalPosition(footballerTeam);
        Vector3 targetPosition = goalPosition;

        // based on additionalDirection target closer to right or left post
        if (additionalDirection == Vector3.right) {
            targetPosition.x += 2f;
        } else if (additionalDirection == Vector3.left) {
            targetPosition.x -= 2f;
        }

        Vector3 targetVector = (targetPosition - position).normalized;
        float inaccuracy = Random.Range(0f, 15f - _footballerStats.ShotInaccuracy) * GeneralHelpers.RandomSign();

        // if footballer is facing away from goal shots have lower accuracy
        if (Vector3.Angle(transform.right, targetPosition) > 85f) {
            inaccuracy *= 3f;
        }

        targetVector.z = Random.Range(minZVelocity, maxZVelocity);

        // normalize again this time including z direction
        targetVector = targetVector.normalized;

        targetVector = Quaternion.Euler(0f, 0f, inaccuracy) * targetVector;
        // clamp power with footballer stats
        power = Mathf.Clamp(power, 3f, _footballerStats.MaxActionPower);

        _ballController.OnFootballerPossessionExit();
        _ballRigidbody.velocity = targetVector * power;
    }

    public void MakeSlideTackle() {
        if (isImmobilized) {
            return;
        }
        // move forward while sliding
        MoveTo(transform.position + transform.right, _footballerStats.SlideTackleSpeed);
        _footballerAnimationController.PlaySlideTackleAnimation();

        // immobilize footballer after slide tackle
        isImmobilized = true;
        isDuringSlide = true;
        StartCoroutine(DisableImmobilization(1f));
        StartCoroutine(DisableSliding(1f));
    }

    public void Fall() {
        if (isImmobilized) {
            return;
        }
        // immobilize footballer after falling
        _footballerAnimationController.PlayFallAnimation();
        
        if (hasBall) {
            _ballController.OnFootballerPossessionExit();
            
            // move ball a bit forward
            _ballRigidbody.velocity = transform.right * 2f;
        }
        
        isImmobilized = true;
        StartCoroutine(DisableImmobilization(1.5f));
    }

    public void ThrowInBoundedRotate(Vector3 direction) {
        // get direction towards center of pitch
        Vector3 pitchDirection = transform.position.x < 0f ? Vector3.right : Vector3.left;
        // rotation bounds
        Vector3 bound1 = Quaternion.Euler(0f, 0f, 75f) * pitchDirection;
        Vector3 bound2 = Quaternion.Euler(0f, 0f, -75f) * pitchDirection;

        float minY = Mathf.Min(bound1.y, bound2.y);
        float maxY = Mathf.Max(bound1.y, bound2.y);

        // don't rotate if there is no direction input and footballer rotation is in bounds
        if (direction == Vector3.zero && Mathf.Sign(transform.right.x) == Mathf.Sign(pitchDirection.x) && transform.right.y > minY && transform.right.y < maxY) {
            return;
        }
        
        // both bounds x are the same
        direction.x = bound1.x;
        // clamp direction before rotating
        direction.y = Mathf.Clamp(direction.y, minY, maxY);
        
        RotateInDirection(direction);
    }
    
    public void KickOffBoundedRotate(Vector3 direction, Vector3 defaultDirection) {
        // rotation bounds
        Vector3 bound1 = Quaternion.Euler(0f, 0f, 75f) * defaultDirection;
        Vector3 bound2 = Quaternion.Euler(0f, 0f, -75f) * defaultDirection;

        float minX = Mathf.Min(bound1.x, bound2.x);
        float maxX = Mathf.Max(bound1.x, bound2.x);

        // don't rotate if there is no direction input and footballer rotation is in bounds
        if (direction == Vector3.zero && Mathf.Sign(transform.right.y) == Mathf.Sign(defaultDirection.y) && transform.right.x > minX && transform.right.x < maxX) {
            return;
        }
        
        // both bounds y are the same
        direction.y = bound1.y;
        // clamp direction before rotating
        direction.x = Mathf.Clamp(direction.x, minX, maxX);
        
        RotateInDirection(direction);
    }
    
    public void CornerBoundedRotate(Vector3 direction) {
        Vector3 defaultDirection;

        if (transform.position.x < 0f) {
            if (transform.position.y < 0f) {
                defaultDirection = new Vector3(1f, 1f, 0f).normalized;
            } else {
                defaultDirection = new Vector3(1f, -1f, 0f).normalized;
            }
        } else {
            if (transform.position.y < 0f) {
                defaultDirection = new Vector3(-1f, 1f, 0f).normalized;
            } else {
                defaultDirection = new Vector3(-1f, -1f, 0f).normalized;
            }
        }
        
        // rotation bounds
        Vector3 bound1 = Quaternion.Euler(0f, 0f, 50f) * defaultDirection;
        Vector3 bound2 = Quaternion.Euler(0f, 0f, -50f) * defaultDirection;

        float minX = Mathf.Min(bound1.x, bound2.x);
        float maxX = Mathf.Max(bound1.x, bound2.x);
        float minY = Mathf.Min(bound1.y, bound2.y);
        float maxY = Mathf.Max(bound1.y, bound2.y);
        
        if (!GeneralHelpers.IsVectorBetweenBounds(transform.right, bound1, bound2)) {
            RotateInDirection(defaultDirection);
            return;
        }

        // clamp direction before rotating
        direction.x = Mathf.Clamp(direction.x, minX, maxX);
        direction.y = Mathf.Clamp(direction.y, minY, maxY);

        RotateInDirection(direction);
    }

    // first returned value is true if ball was thrown, false otherwise; second value is target to which ball was thrown, null if there was none
    public Tuple<bool, GameObject> ThrowInPass(float power) {
        Vector3 pitchDirection = transform.position.x < 0f ? Vector3.right : Vector3.left;
        // if footballer is not facing towards pitch, throw is not possible
        if (Mathf.Sign(transform.right.x) != Mathf.Sign(pitchDirection.x)) {
            return new Tuple<bool, GameObject>(false, null);
        }
        
        GameObject target = Pass(power, 0.5f);
        return new Tuple<bool, GameObject>(true, target);
    }

    IEnumerator DisableImmobilization(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        isImmobilized = false;
    }
    
    IEnumerator DisableSliding(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        isDuringSlide = false;
    }
}