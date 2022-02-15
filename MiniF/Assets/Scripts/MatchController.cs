using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {
    [SerializeField] private List<GameObject> allPlayers;
    [SerializeField] private List<GameObject> topTeamPlayers;
    [SerializeField] private List<GameObject> botTeamPlayers;
    private Vector3 topGoalPosition;

    public List<GameObject> GetAllPlayers() {
        return allPlayers;
    }

    /*public Vector3 GetGoalPosition() {
        
    }*/
}