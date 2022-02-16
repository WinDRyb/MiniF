using System;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
public class BasicAI : MonoBehaviour {
    
    protected Footballer _footballerScript;
    protected BallController _ballController;
    protected Transform _ballTransform;
    protected Transform _topGoalTransform;
    protected Transform _botGoalTransform;

    // footballer position when ball is in the middle of pitch
    [SerializeField] protected Vector3 defaultPosition;
    // how far towards opponent goal footballer can go
    [SerializeField] protected Vector3 maxForwardPosition;
    // how close to team goal footballer can go
    [SerializeField] protected Vector3 maxBackwardPosition;
    
    // zone in which footballer goes towards ball
    [SerializeField] protected Bounds ballControlZone;

    protected virtual void Awake() {
        _footballerScript = GetComponent<Footballer>();
        GameObject ball = GameObject.FindWithTag("Ball");
        _ballController = ball.GetComponent<BallController>();
        _ballTransform = ball.GetComponent<Transform>();
        _topGoalTransform = GameObject.FindWithTag("TopGoal").GetComponent<Transform>();
        _botGoalTransform = GameObject.FindWithTag("BotGoal").GetComponent<Transform>();
    }

    protected virtual void FixedUpdate() {
        Move();
    }

    protected virtual void Move() {
        // if ball is in control zone
        if (ballControlZone.Contains(_ballTransform.position) && _ballController.TeamInPossessionOfBall != _footballerScript.FootballerTeam) {
            // seize ball control if no one controls it
            if (_ballController.TeamInPossessionOfBall == Team.None) {
                _footballerScript.MoveInDirection((_ballTransform.position - transform.position).normalized);
            }
            // if opponent has ball, move to block path to team goal
            else {
                Vector3 offsetTowardsTeamGoal = _footballerScript.FootballerTeam == Team.Top
                    ? (_topGoalTransform.position - _ballTransform.position).normalized * 1.5f
                    : (_botGoalTransform.position - _ballTransform.position).normalized * 1.5f;
                
                _footballerScript.MoveInDirection((_ballTransform.position + offsetTowardsTeamGoal - transform.position).normalized);
            }
        } 
        // ball is not in control zone
        else {
            // orientation of team forward and backward 
            Vector3 direction = _ballTransform.position.y >= defaultPosition.y
                ? (maxForwardPosition - maxBackwardPosition).normalized
                : (maxBackwardPosition - maxForwardPosition).normalized;
            
            // calculate position of footballer for current ball position
            float y = ((Mathf.Abs(_ballTransform.position.y - defaultPosition.y) / 2f) * direction.y) + defaultPosition.y;

            // clamp position with forward and backward limits
            y = maxForwardPosition.y > maxBackwardPosition.y
                ? Mathf.Clamp(y, maxBackwardPosition.y, maxForwardPosition.y)
                : Mathf.Clamp(y, maxForwardPosition.y, maxBackwardPosition.y);

            Vector3 targetPosition = new Vector3(defaultPosition.x, y, 0f);
            _footballerScript.MoveInDirection((targetPosition - transform.position).normalized);
        }
    }
}
