using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FootballerStats))]
public class Footballer : MonoBehaviour
{
    private bool hasBall;
    public bool HasBall
    {
        get { return hasBall; }
        set { hasBall = value; }
    }

    private Rigidbody _rigidbody;
    private FootballerStats _footballerStats;
    private Rigidbody _ballRigidbody;
    private BallController _ballController;
    
    private MatchController _matchController;

    public MatchController _MatchController
    {
        get { return _matchController; }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _footballerStats = GetComponent<FootballerStats>();
        GameObject ball = GameObject.FindWithTag("Ball");
        _ballRigidbody = ball.GetComponent<Rigidbody>();
        _ballController = ball.GetComponent<BallController>();
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 velocity;
        if (hasBall)
        {
            velocity = (targetPosition - transform.position).normalized * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow;
        }
        else
        {
            velocity = (targetPosition - transform.position).normalized * _footballerStats.MovementSpeed;
        }
        _rigidbody.velocity = velocity;
    }

    public void MoveForward()
    {
        if (hasBall)
        {
            _rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * _footballerStats.MovementSpeed * _footballerStats.MovementWithBallSlow);
        }
        else
        {
            _rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * _footballerStats.MovementSpeed);
        }
    }

    public void CharacterRotate(Vector3 rotation)
    {
        Quaternion rot = Quaternion.Euler(rotation * Time.fixedDeltaTime * _footballerStats.RotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * rot);
    }

    // returns true if there was a target for pass
    public GameObject MakePass(float power, float zVelocity=0f)
    {
        if (hasBall)
        {
            Vector3 bound1 = Quaternion.Euler(0f, 0f, 25f) * transform.right;
            Vector3 bound2 = Quaternion.Euler(0f, 0f, -25f) * transform.right;
            GameObject target = FootballHelpers.GetActionTargetPosition(transform.position, bound1, bound2, _matchController.GetAllPlayers());

            Vector3 direction;
            // pass ahead it there is no target, otherwise pass to target
            if (target)
            {
                direction = (target.transform.position - transform.position).normalized;
            }
            else
            {
                direction = transform.right;
            }

            // set how high will pass go
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
}
