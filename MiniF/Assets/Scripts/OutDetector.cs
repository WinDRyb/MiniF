using UnityEngine;

public class OutDetector : MonoBehaviour {

    private MatchController _matchController;
    private BallController _ballController;
    
    private void Awake() {
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
        _ballController =  GameObject.FindWithTag("Ball").GetComponent<BallController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ball")) {
            if (_ballController.IsInPlay) {
                Team teamInPossession = _ballController.LastTeamInPossessionOfBall;
                Team ballForTeam = teamInPossession == Team.Top ? Team.Bot : Team.Top;
                Vector3 throwInPosition = new Vector3(other.transform.position.x, other.transform.position.y, 0f);
                _matchController.SetupEvent(FootballEventType.ThrowIn, throwInPosition, ballForTeam);

                _ballController.IsInPlay = false;
            }
        }
    }
}
