using System;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
public class PlayerController : MonoBehaviour {
    private Footballer _footballer;
    
    private Vector3 movement;
    private float actionPower;

    private FootballEventType eventType = FootballEventType.None;
    public FootballEventType EventType {
        get { return eventType; }
        set { eventType = value; }
    }
    
    private void Awake() {
        _footballer = GetComponent<Footballer>();
    }

    private void Update() {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;

        if (eventType == FootballEventType.None) {
            if (_footballer.HasBall) {
                if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Z)) {
                    actionPower += 10f * Time.deltaTime;
                }

                // make ground pass
                if (Input.GetKeyUp(KeyCode.C)) {
                    GameObject teammate = _footballer.Pass(actionPower);
                    // transplant controller if there was a target for pass
                    if (teammate) {
                        TransplantController(teammate);
                    }

                    actionPower = 0f;
                }

                // make high pass
                if (Input.GetKeyUp(KeyCode.X)) {
                    GameObject teammate = _footballer.Pass(actionPower, 0.5f);
                    // transplant controller if there was a target for pass
                    if (teammate) {
                        TransplantController(teammate);
                    }

                    actionPower = 0f;
                }

                // shot
                if (Input.GetKeyUp(KeyCode.Z)) {
                    Vector3 additionalDirection = Vector3.zero;
                    if (Input.GetKey(KeyCode.RightArrow)) {
                        additionalDirection = Vector3.right;
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow)) {
                        additionalDirection = Vector3.left;
                    }

                    _footballer.Shot(additionalDirection, actionPower, 0.3f);
                    actionPower = 0f;
                }
            }
            else {
                if (Input.GetKeyUp(KeyCode.Z)) {
                    ChangeWithoutBall();
                }

                if (Input.GetKeyUp(KeyCode.X)) {
                    _footballer.MakeSlideTackle();
                }
            }
        } else if (eventType != FootballEventType.None) {
            if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.X)) {
                actionPower += 10f * Time.deltaTime;
            }
            
            if (eventType == FootballEventType.ThrowIn) {
                if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Z)) {
                    // Item1 contains information if ball was thrown, Item2 is target (if there was one)
                    Tuple<bool, GameObject> throwResult = _footballer.ThrowInPass(actionPower * 0.3f);
                    if (throwResult.Item1) {
                        eventType = FootballEventType.None;
                        if (throwResult.Item2) {
                            TransplantController(throwResult.Item2);
                        }
                    }
                    actionPower = 0f;
                }
            }
        }
    }

    private void FixedUpdate() {
        switch (eventType) {
            case FootballEventType.None:
                _footballer.MoveInDirection(movement);
                break;
            case FootballEventType.ThrowIn:
                _footballer.ThrowInBoundedRotate(movement);
                break;
        }
    }
    
    private void OnEnable() {
        // deactivate AI behaviour of footballer
        GetComponent<BasicAI>().enabled = false;
    }
    
    private void OnDisable() {
        // activate AI behaviour of footballer
        GetComponent<BasicAI>().enabled = true;
    }

    // enable PlayerController on teammate and disable this controller
    private void TransplantController(GameObject nextPlayer) {
        // let MatchController know which footballer is controlled by player
        _footballer._MatchController.SetPlayerControlledFootballer(nextPlayer, _footballer.FootballerTeam);
    }

    private void ChangeWithoutBall() {
        Vector3 bound1;
        Vector3 bound2;

        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)) {
            bound1 = Vector3.left;
            bound2 = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)) {
            bound1 = Vector3.left;
            bound2 = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)) {
            bound1 = Vector3.right;
            bound2 = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) {
            bound1 = Vector3.right;
            bound2 = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            bound1 = Quaternion.Euler(0f, 0f, 30f) * Vector3.up;
            bound2 = Quaternion.Euler(0f, 0f, -30f) * Vector3.down;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            bound1 = Quaternion.Euler(0f, 0f, -30f) * Vector3.up;
            bound2 = Quaternion.Euler(0f, 0f, 30f) * Vector3.down;
        }
        else if (Input.GetKey(KeyCode.UpArrow)) {
            bound1 = Quaternion.Euler(0f, 0f, -30f) * Vector3.left;
            bound2 = Quaternion.Euler(0f, 0f, 30f) * Vector3.right;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            bound1 = Quaternion.Euler(0f, 0f, 30f) * Vector3.left;
            bound2 = Quaternion.Euler(0f, 0f, -30f) * Vector3.right;
        }
        else {
            bound1 = Vector3.zero;
            bound2 = Vector3.zero;
        }

        if (bound1 == Vector3.zero && bound2 == Vector3.zero) {
            GameObject teammate = FootballHelpers.GetClosestTarget(transform.position,
                _footballer._MatchController.GetTeamPlayers(_footballer.FootballerTeam));
            if (teammate) {
                TransplantController(teammate);
            }
        }
        else {
            GameObject teammate = FootballHelpers.GetActionTargetPosition(transform.position, bound1, bound2,
                _footballer._MatchController.GetTeamPlayers(_footballer.FootballerTeam));
            if (teammate) {
                TransplantController(teammate);
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawLine(transform.position, transform.position + transform.right);
    //     
    //     Vector3 vector1 = Quaternion.Euler(0f, 0f, 30f) * Vector3.left;
    //     Vector3 vector2 = Quaternion.Euler(0f, 0f, -30f) * Vector3.right;
    //     //Vector3 vector1 = Vector3.down;
    //     //Vector3 vector2 = Vector3.right;
    //     
    //     Gizmos.DrawLine(transform.position, transform.position + vector1 * 7f);
    //     Gizmos.DrawLine(transform.position, transform.position + vector2 * 7f);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(FootballHelpers.GetActionTargetPosition(transform.position, vector2, vector1, _footballer._MatchController.GetAllPlayers()).transform.position, 0.2f);
    // }
}