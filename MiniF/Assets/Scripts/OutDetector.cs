using UnityEngine;

public class OutDetector : MonoBehaviour {

    private MatchController _matchController;
    
    private void Awake() {
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ball")) {
            BallController _ballController = other.GetComponent<BallController>();
            if (_ballController.IsInPlay) {
                Team teamInPossession = _ballController.TeamInPossessionOfBall;
                Team ballForTeam = teamInPossession == Team.Top ? Team.Bot : Team.Top;
                Vector3 throwInPosition = new Vector3(other.transform.position.x, other.transform.position.y, 0f);
                _matchController.SetupThrowIn(throwInPosition, ballForTeam);

                _ballController.IsInPlay = false;
            }
        }
    }
}
