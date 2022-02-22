using UnityEngine;

public class GoalDetector : MonoBehaviour {
    
    private MatchController _matchController;
    private BallController _ballController;
    private UIController _uiController;
    
    private void Awake() {
        _matchController = GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
        _ballController = GameObject.FindWithTag("Ball").GetComponent<BallController>();
        _uiController = GameObject.FindWithTag("MatchController").GetComponent<UIController>();
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ball")) {
            if (_ballController.IsInPlay) {
                
                Team goalForTeam = other.transform.position.y > 0f ? Team.Bot : Team.Top;
                _uiController.GoalScored(goalForTeam);
                
                _matchController.SetupEvent(FootballEventType.KickOff, Vector3.zero, FootballHelpers.GetOtherTeam(goalForTeam));

                _ballController.IsInPlay = false;
            }
        }
    }
}
