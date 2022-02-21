using UnityEngine;

public class EndLineDetector : MonoBehaviour {
    private MatchController _matchController;
    private BallController _ballController;

    [SerializeField] private Vector3 cornerPosition;
    [SerializeField] private Vector3 goalkeeperKickoffPosition;
    
    private void Awake() {
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
        _ballController =  GameObject.FindWithTag("Ball").GetComponent<BallController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ball")) {
            if (_ballController.IsInPlay) {
                Team teamInPossession = _ballController.LastTeamInPossessionOfBall;
                Team ballForTeam = teamInPossession == Team.Top ? Team.Bot : Team.Top;

                if (transform.position.y > 0) {
                    if (ballForTeam == Team.Top) {
                        _matchController.SetupEvent(FootballEventType.GoalkeeperKickOff, goalkeeperKickoffPosition, ballForTeam);
                    } else {
                        _matchController.SetupEvent(FootballEventType.Corner, cornerPosition, ballForTeam);
                    }
                } else {
                    if (ballForTeam == Team.Top) {
                        _matchController.SetupEvent(FootballEventType.Corner, cornerPosition, ballForTeam);
                    } else {
                        _matchController.SetupEvent(FootballEventType.GoalkeeperKickOff, goalkeeperKickoffPosition, ballForTeam);
                    }
                }

                _ballController.IsInPlay = false;
            }
        }
    }
}
