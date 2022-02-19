using UnityEngine;

public class GoalDetector : MonoBehaviour {
    
    private MatchController _matchController;
    private BallController _ballController;
    
    private void Awake() {
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
        _ballController = GameObject.FindWithTag("Ball").GetComponent<BallController>();
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ball")) {
            if (_ballController.IsInPlay) {
                Team ballForTeam = other.transform.position.y > 0f ? Team.Top : Team.Bot;
                _matchController.SetupEvent(FootballEventType.KickOff, Vector3.zero, ballForTeam);

                _ballController.IsInPlay = false;
            }
        }
    }
}
