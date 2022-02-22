using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _scoreTextDisplay;
    
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private TextMeshProUGUI _victoryTextDisplay;
    
    [SerializeField] private string topTeamShortName;
    [SerializeField] private string botTeamShortName;

    private int botTeamScore;
    private int topTeamScore;
    
    private void Start() {
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay() {
        _scoreTextDisplay.SetText(botTeamShortName + " " + botTeamScore + " - " + topTeamScore + " " + topTeamShortName);
    }

    private void ShowVictoryScreen(string teamName) {
        _victoryTextDisplay.SetText(teamName + " wins!");
        _victoryPanel.SetActive(true);
    }
    
    public void GoalScored(Team byTeam) {
        if (byTeam == Team.Top) {
            topTeamScore++;
            if (topTeamScore == 10) {
                ShowVictoryScreen(topTeamShortName);
            }
        } else {
            botTeamScore++;
            if (botTeamScore == 10) {
                ShowVictoryScreen(botTeamShortName);
            }
        }

        UpdateScoreDisplay();
    }
}
