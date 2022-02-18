using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
    [SerializeField] private GameObject _eventPointPrefab;
    
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
        AssignFirstPlayerControlledFootballers();
    }
    
    private void AssignPlayersToTeams() {
        foreach (GameObject player in topTeamPlayers) {
            player.GetComponent<Footballer>().FootballerTeam = Team.Top;
        }
        foreach (GameObject player in botTeamPlayers) {
            player.GetComponent<Footballer>().FootballerTeam = Team.Bot;
        }
    }

    private void AssignFirstPlayerControlledFootballers() {
        if (isTopTeamControlledByPlayer) {
            foreach (GameObject player in topTeamPlayers) {
                if (player.GetComponent<PlayerController>().enabled) {
                    topTeamPlayerControlledFootballer = player;
                }
                break;
            }
        }

        if (isBotTeamControlledByPlayer) {
            foreach (GameObject player in botTeamPlayers) {
                if (player.GetComponent<PlayerController>().enabled) {
                    botTeamPlayerControlledFootballer = player;
                }
                break;
            }
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

    private void EnablePlayerController(GameObject footballer) {
        footballer.GetComponent<PlayerController>().enabled = true;
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

    public List<GameObject> GetTeamPlayers(Team footballerTeam) {
        return footballerTeam == Team.Top ? topTeamPlayers : botTeamPlayers;
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
        List<GameObject> allTeamPlayers = ballForTeam == Team.Top ? topTeamPlayers : botTeamPlayers;
        GameObject thrower = FootballHelpers.GetClosestTarget(position, allTeamPlayers);
        DisablePlayerControllers();
        thrower.GetComponent<BasicAI>().SetupThrowIn(position);
        _ballController.OnFootballerPossessionExit();
        _ballController.SetNewBallPosition(position);
        
        // create event point so other footballers can't come to close
        Instantiate(_eventPointPrefab, position, Quaternion.identity);
    }

    public void ThrowInReady(GameObject thrower, Team throwerTeam) {
        // if team doing throw in is controlled by player, gain control over thrower
        if ((throwerTeam == Team.Top && isTopTeamControlledByPlayer) || (throwerTeam == Team.Bot && isBotTeamControlledByPlayer)) {
            PlayerController playerController = thrower.GetComponent<PlayerController>();
            thrower.GetComponent<BasicAI>().EventCompleted();
            playerController.enabled = true;
            playerController.EventType = FootballEventType.ThrowIn;
        } else {
            
        }

        // if team not throwing in is controlled by player, gain control over footballer closest to thrower
        Team otherTeam = throwerTeam == Team.Top ? Team.Bot : Team.Top;
        if ((otherTeam == Team.Top && isTopTeamControlledByPlayer) || (otherTeam == Team.Bot && isBotTeamControlledByPlayer)) {
            List<GameObject> allTeamPlayers = otherTeam == Team.Top ? topTeamPlayers : botTeamPlayers;
            GameObject closestFootballer = FootballHelpers.GetClosestTarget(thrower.transform.position, allTeamPlayers);
            closestFootballer.GetComponent<PlayerController>().enabled = true;
        }
    }
}