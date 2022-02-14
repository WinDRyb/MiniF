using System;
using UnityEngine;

public class TargetTest : MonoBehaviour {
    private MatchController _matchController;

    private void Awake() {
        _matchController = FindObjectOfType<MatchController>();
    }


    private void Update() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.back * 0.5f);
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(Vector3.forward * 0.5f);
        }

        if (Input.GetKey(KeyCode.Space)) {
            Vector3 vector1 = Quaternion.Euler(0f, 0f, 25f) * transform.right;
            Vector3 vector2 = Quaternion.Euler(0f, 0f, -25f) * transform.right;
            //Debug.Log(FootballHelpers.GetActionTargetPosition(transform.position, vector1, vector2, _matchController.GetAllPlayersPositions()));
            Debug.Log(Vector3.Angle(vector1, vector2));
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + transform.right);

        Vector3 vector1 = Quaternion.Euler(0f, 0f, 25f) * transform.right;
        Vector3 vector2 = Quaternion.Euler(0f, 0f, -25f) * transform.right;

        Gizmos.DrawLine(transform.position, transform.position + vector1 * 7f);
        Gizmos.DrawLine(transform.position, transform.position + vector2 * 7f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FootballHelpers.GetActionTargetPosition(transform.position, vector1, vector2, _matchController.GetAllPlayers()).transform.position, 0.2f);
    }
}