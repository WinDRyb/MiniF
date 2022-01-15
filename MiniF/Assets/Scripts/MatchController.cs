using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] private List<GameObject> allPlayers;
    [SerializeField] private List<GameObject> topTeamPlayers;
    [SerializeField] private List<GameObject> botTeamPlayers;

    public List<GameObject> GetAllPlayers()
    {
        return allPlayers;
    }

    public List<Vector3> GetAllPlayersPositions()
    {
        List<Vector3> positions = new List<Vector3>(allPlayers.Count);
        foreach (GameObject player in allPlayers)
        {
            positions.Add(player.transform.position);
        }

        return positions;
    }
}
