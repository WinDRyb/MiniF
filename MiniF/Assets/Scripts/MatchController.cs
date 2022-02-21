using System;
using System.Collections;
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

    private int multipleTargetsEventCounter;
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
                    break;
                }
            }
        }

        if (isBotTeamControlledByPlayer) {
            foreach (GameObject player in botTeamPlayers) {
                if (player.GetComponent<PlayerController>().enabled) {
                    botTeamPlayerControlledFootballer = player;
                    break;
                }
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

    private Team GetOtherTeam(Team team) {
        return team == Team.Top ? Team.Bot : Team.Top;
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
    
    public List<GameObject> GetOtherTeamPlayers(Team footballerTeam) {
        return footballerTeam == Team.Bot ? topTeamPlayers : botTeamPlayers;
    }
    
    public List<GameObject> GetAllPlayers() {
        return allPlayers;
    }

    public Vector3 GetTeamGoalPosition(Team footballerTeam) {
        return footballerTeam == Team.Top ? topGoalTransform.position : botGoalTransform.position;
    }
    
    public Vector3 GetOpponentsGoalPosition(Team footballerTeam) {
        return footballerTeam == Team.Bot ? topGoalTransform.position : botGoalTransform.position;
    }

    public Transform GetTopGoalTransform() {
        return topGoalTransform;
    }
    
    public Transform GetBotGoalTransform() {
        return botGoalTransform;
    }

    private IEnumerator SetBallPositionAfterDelay(Vector3 position, float delayTime) {
        _ballController.OnFootballerPossessionExit();
        yield return new WaitForSeconds(delayTime);
        
        _ballController.SetNewBallPosition(position);
    }
    
    public void SetupEvent(FootballEventType eventType, Vector3 position, Team ballForTeam) {
        DisablePlayerControllers();
        _ballController.OnFootballerPossessionExit();
        switch (eventType) {
            case FootballEventType.ThrowIn: case FootballEventType.GoalkeeperKickOff: case FootballEventType.Corner:
                SetupSinglePlayerEvent(eventType, position, ballForTeam);
                break;
            case FootballEventType.KickOff:
                Vector3 kickOffTakerPosition = ballForTeam == Team.Top ? Vector3.down * 0.05f : Vector3.up * 0.05f;
                SetupKickOff(kickOffTakerPosition, ballForTeam);
                break;
        }
    }

    public void EventReady(FootballEventType eventType, GameObject footballer, Team footballerTeam) {
        switch (eventType) {
            case FootballEventType.ThrowIn: case FootballEventType.GoalkeeperKickOff: case FootballEventType.Corner:
                SetupSinglePlayerReady(eventType, footballer, footballerTeam);
                break;
            case FootballEventType.KickOff: case FootballEventType.KickOffTaker:
                KickOffReady();
                break;
        }
    }

    public void AllPlayersEventComplete() {
        foreach (GameObject footballer in allPlayers) {
            footballer.GetComponent<BasicAI>().EventCompleted();
        }
    }

    private void ApplyEventControllers(FootballEventType eventType, GameObject eventTaker, Team eventTakerTeam) {
        // if team carrying out event is controlled by player, gain control over event taker
        if ((eventTakerTeam == Team.Top && isTopTeamControlledByPlayer) || (eventTakerTeam == Team.Bot && isBotTeamControlledByPlayer)) {
            PlayerController playerController = eventTaker.GetComponent<PlayerController>();
            eventTaker.GetComponent<BasicAI>().EventCompleted();
            SetPlayerControlledFootballer(eventTaker, eventTakerTeam);
            //playerController.enabled = true;
            playerController.EventType = eventType;
        } else {
            
        }

        // if team waiting for event completion is controlled by player, gain control over footballer closest to event taker
        Team otherTeam = GetOtherTeam(eventTakerTeam);
        if ((otherTeam == Team.Top && isTopTeamControlledByPlayer) || (otherTeam == Team.Bot && isBotTeamControlledByPlayer)) {
            List<GameObject> allTeamPlayers = GetTeamPlayers(otherTeam);
            GameObject closestFootballer = FootballHelpers.GetClosestTarget(eventTaker.transform.position, allTeamPlayers);
            //closestFootballer.GetComponent<PlayerController>().enabled = true;
            SetPlayerControlledFootballer(closestFootballer, otherTeam);
        }
    }

    private void SetupSinglePlayerEvent(FootballEventType eventType, Vector3 position, Team ballForTeam) {
        List<GameObject> allTeamPlayers = GetTeamPlayers(ballForTeam);
        GameObject taker = FootballHelpers.GetClosestTarget(position, allTeamPlayers);
        taker.GetComponent<BasicAI>().SetupEvent(eventType, position);
        // position ball at throw in position after delay
        StartCoroutine(SetBallPositionAfterDelay(position, 1f));

        // create event point so other footballers can't come to close
        Instantiate(_eventPointPrefab, position, Quaternion.identity);
    }
    
    private void SetupSinglePlayerReady(FootballEventType eventType, GameObject taker, Team takerTeam) {
        ApplyEventControllers(eventType, taker, takerTeam);
    }

    private void SetupKickOff(Vector3 position, Team ballForTeam) {
        GameObject kickOffTaker = FootballHelpers.GetClosestTarget(position, GetTeamPlayers(ballForTeam));
        kickOffTaker.GetComponent<BasicAI>().SetupEvent(FootballEventType.KickOffTaker, position);
        foreach (GameObject footballer in allPlayers) {
            if (footballer == kickOffTaker) {
                continue;
            }

            BasicAI footballerAIScript = footballer.GetComponent<BasicAI>();
            footballerAIScript.SetupEvent(FootballEventType.KickOff, footballerAIScript.ResetPosition);
        }
        
        //Instantiate(_eventPointPrefab, Vector3.zero, Quaternion.identity);
    }

    private void KickOffReady() {
        // count ready footballers and play kick off after every one reached their default position
        multipleTargetsEventCounter++;
        if (multipleTargetsEventCounter == allPlayers.Count) {
            foreach (GameObject checkedFootballer in allPlayers) {
                BasicAI footballerAIScript = checkedFootballer.GetComponent<BasicAI>();
                if (footballerAIScript.EventType == FootballEventType.KickOffTaker) {
                    ApplyEventControllers(FootballEventType.KickOffTaker, checkedFootballer, checkedFootballer.GetComponent<Footballer>().FootballerTeam);
                    break;
                }
            }
            
            StartCoroutine(SetBallPositionAfterDelay(Vector3.zero, 0.5f));
            multipleTargetsEventCounter = 0;
        }
    }
    
}