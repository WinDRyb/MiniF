using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
    [SerializeField] private bool isTopTeamControlledByPlayer;
    [SerializeField] private bool isBotTeamControlledByPlayer;
    
    [SerializeField] private Transform topGoalTransform;
    [SerializeField] private Transform botGoalTransform;
    [SerializeField] private List<GameObject> allPlayers;
    [SerializeField] private List<GameObject> topTeamPlayers;
    [SerializeField] private List<GameObject> botTeamPlayers;

    private GameObject topTeamPlayerControlledFootballer;
    private GameObject botTeamPlayerControlledFootballer;

    private BallController _ballController;

    private void Awake() {
        _ballController = GameObject.FindWithTag("Ball").GetComponent<BallController>();
    }

    private void Start() {
        AssignPlayersToTeams();
    }
    
    private void AssignPlayersToTeams() {
        foreach (GameObject player in topTeamPlayers) {
            player.GetComponent<Footballer>().FootballerTeam = Team.Top;
        }
        foreach (GameObject player in botTeamPlayers) {
            player.GetComponent<Footballer>().FootballerTeam = Team.Bot;
        }
    }

    private void DisablePlayerControllers() {
        if (topTeamPlayerControlledFootballer) {
            topTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = false;
        }
        if (botTeamPlayerControlledFootballer) {
            botTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = false;
        }
    }
    
    private void DisableFootballerMovementControllers(GameObject footballer) {
        PlayerController playerController = footballer.GetComponent<PlayerController>();
        if (playerController) {
            playerController.enabled = false;
            print(footballer.GetComponent<BasicAI>().enabled);
        }
        footballer.GetComponent<BasicAI>().enabled = false;
        print(footballer.GetComponent<BasicAI>().enabled);
    }
    
    public void SetPlayerControlledFootballer(GameObject footballer, Team footballerTeam) {
        if (isTopTeamControlledByPlayer && footballerTeam == Team.Top) {
            if (topTeamPlayerControlledFootballer) {
                // disable previous footballer PlayerController
                topTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = false;
            }
            topTeamPlayerControlledFootballer = footballer;
            // enable next footballer PlayerController
            topTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = true;
        } else if (isBotTeamControlledByPlayer && footballerTeam == Team.Bot) {
            if (botTeamPlayerControlledFootballer) {
                // disable previous footballer PlayerController
                botTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = false;
            }
            botTeamPlayerControlledFootballer = footballer;
            // enable next footballer PlayerController
            botTeamPlayerControlledFootballer.GetComponent<PlayerController>().enabled = true;
        }
    }
    
    public List<GameObject> GetAllPlayers() {
        return allPlayers;
    }

    public Vector3 GetOpponentsGoalPosition(Team footballerTeam) {
        return footballerTeam == Team.Top ? botGoalTransform.position : topGoalTransform.position;
    }

    public Transform GetTopGoalTransform() {
        return topGoalTransform;
    }
    
    public Transform GetBotGoalTransform() {
        return botGoalTransform;
    }

    public void SetupThrowIn(Vector3 position, Team ballForTeam) {
        //tmp reverse
        List<GameObject> allTeamPlayers = ballForTeam == Team.Bot ? topTeamPlayers : botTeamPlayers;
        GameObject thrower = FootballHelpers.GetClosestTarget(position, allTeamPlayers);
        DisablePlayerControllers();
        thrower.GetComponent<BasicAI>().SetupThrowIn(position);
        _ballController.SetNewBallPosition(position);
    }

    public void ThrowInReady(GameObject thrower, Team throwerTeam) {
        if (throwerTeam == Team.Top && isTopTeamControlledByPlayer) {
            
        } else if (throwerTeam == Team.Bot && isBotTeamControlledByPlayer) {
            
        } else {
            
        }
    }
}