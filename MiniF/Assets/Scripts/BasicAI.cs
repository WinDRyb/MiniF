using System;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
public class BasicAI : MonoBehaviour {
    
    protected Footballer _footballerScript;
    protected BallController _ballController;
    protected Transform _ballTransform;
    protected Transform _topGoalTransform;
    protected Transform _botGoalTransform;
    protected MatchController _matchController;

    // footballer position when ball is in the middle of pitch
    [SerializeField] protected Vector3 defaultPosition;
    public Vector3 DefaultPosition => defaultPosition;
    // how far right can footballer go (x), how far towards opponent goal can footballer go (y)
    [SerializeField] protected Vector3 maxForwardPosition;
    // how far left can footballer go (x), how close to team goal can footballer go (y)
    [SerializeField] protected Vector3 maxBackwardPosition;
    // zone in which footballer goes towards ball
    [SerializeField] protected Bounds ballControlZone;

    protected FootballEventType eventType = FootballEventType.None;

    public FootballEventType EventType => eventType;

    protected Vector3 eventPositionTarget;
    protected bool isOnEventPosition;
    
    protected virtual void Awake() {
        _footballerScript = GetComponent<Footballer>();
        GameObject ball = GameObject.FindWithTag("Ball");
        _ballController = ball.GetComponent<BallController>();
        _ballTransform = ball.GetComponent<Transform>();
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
        _topGoalTransform = _matchController.GetTopGoalTransform();
        _botGoalTransform = _matchController.GetBotGoalTransform();

        // set big z of bounds extents so it works for ball in air
        ballControlZone.extents = new Vector3(ballControlZone.extents.x, ballControlZone.extents.y, 100f);
    }

    protected virtual void FixedUpdate() {
        switch (eventType) {
            case FootballEventType.None:
                Move();
                break;
            case FootballEventType.ThrowIn: case FootballEventType.KickOff: case FootballEventType.KickOffTaker: case FootballEventType.GoalkeeperKickOff: 
            case FootballEventType.Corner:
                MoveToEventPosition(eventPositionTarget);
                break;
        }
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
            float x = ((_ballTransform.position.x - defaultPosition.x) / 2f) + defaultPosition.x;
            x = Mathf.Clamp(x, maxBackwardPosition.x, maxForwardPosition.x);
            
            // forward and backward orientation of team
            Vector3 direction = _ballTransform.position.y >= defaultPosition.y
                ? (maxForwardPosition - maxBackwardPosition).normalized
                : (maxBackwardPosition - maxForwardPosition).normalized;

            if (_footballerScript.FootballerTeam == Team.Top) {
                direction *= -1;
            }

            // calculate position of footballer for current ball position
            float y = ((Mathf.Abs(_ballTransform.position.y - defaultPosition.y) / 2f) * direction.y) + defaultPosition.y;
            
            // clamp position with forward and backward limits
            y = maxForwardPosition.y > maxBackwardPosition.y
                ? Mathf.Clamp(y, maxBackwardPosition.y, maxForwardPosition.y)
                : Mathf.Clamp(y, maxForwardPosition.y, maxBackwardPosition.y);

            Vector3 targetPosition = new Vector3(x, y, 0f);
            _footballerScript.MoveInDirection((targetPosition - transform.position).normalized);
        }
    }


    protected virtual bool MoveToPosition(Vector3 position) {
        _footballerScript.MoveInDirection((position - transform.position).normalized);
        if (Vector3.Distance(transform.position, position) < 0.1f) {
            return true;
        }

        return false;
    }

    protected virtual void MoveToEventPosition(Vector3 eventPosition) {
        if (MoveToPosition(eventPosition) && !isOnEventPosition) {
            EventReady();
        }
    }
    
    public virtual void SetupEvent(FootballEventType footballEventType, Vector3 eventPosition) {
        eventType = footballEventType;
        eventPositionTarget = eventPosition;
    }

    public virtual void EventReady() {
        _matchController.EventReady(eventType, gameObject, _footballerScript.FootballerTeam);
        isOnEventPosition = true;
    }
    
    public virtual void EventCompleted() {
        eventType = FootballEventType.None;
        isOnEventPosition = false;
    }
    
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(ballControlZone.center, ballControlZone.size);
    }
}
