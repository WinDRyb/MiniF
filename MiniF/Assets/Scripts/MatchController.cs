using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
    [SerializeField] private Transform topGoalTransform;
    [SerializeField] private Transform botGoalTransform;
    [SerializeField] private List<GameObject> allPlayers;
    [SerializeField] private List<GameObject> topTeamPlayers;
    [SerializeField] private List<GameObject> botTeamPlayers;


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
    
    public List<GameObject> GetAllPlayers() {
        return allPlayers;
    }

    public Vector3 GetOpponentsGoalPosition(Team footballerTeam) {
        return footballerTeam == Team.Top ? botGoalTransform.position : topGoalTransform.position;
    }
}