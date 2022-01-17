using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
public class PlayerController : MonoBehaviour
{
    private bool move;
    private Vector3 rotation;
    private float actionPower;
    
    private Footballer _footballer;

    private void Awake()
    {
        _footballer = GetComponent<Footballer>();
    }

    private void Update()
    {
        move = false;
        rotation = Vector3.zero;
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            move = true;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotation = Vector3.forward;
        } 
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation = Vector3.back;
        }

        if (_footballer.HasBall)
        {
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
            {
                actionPower += 10f * Time.deltaTime;
            }

            // make ground pass
            if (Input.GetKeyUp(KeyCode.Z))
            {
                GameObject teammate = _footballer.MakePass(actionPower);
                // transplant controller if there was a target for pass
                if (teammate)
                {
                    TransplantController(teammate);
                }
                actionPower = 0f;
            }
        
            // make high pass
            if (Input.GetKeyUp(KeyCode.X))
            {
                GameObject teammate = _footballer.MakePass(actionPower, 0.5f);
                // transplant controller if there was a target for pass
                if (teammate)
                {
                    TransplantController(teammate);
                }
                actionPower = 0f;
            }    
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                ChangeWithoutBall();
            }    
        }
        
    }

    private void FixedUpdate()
    {
        if (move)
        {
            _footballer.MoveForward();
        }

        if (rotation != Vector3.zero)
        {
            _footballer.CharacterRotate(rotation);
        }
    }

    private void TransplantController(GameObject nextPlayer)
    {
        nextPlayer.AddComponent<PlayerController>();
        Destroy(this);
    }

    private void ChangeWithoutBall()
    {
        Vector3 bound1;
        Vector3 bound2;

        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            bound1 = Vector3.left;
            bound2 = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            bound1 = Vector3.left;
            bound2 = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            bound1 = Vector3.right;
            bound2 = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            bound1 = Vector3.right;
            bound2 = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            bound1 = Quaternion.Euler(0f, 0f, 30f) * Vector3.up;
            bound2 = Quaternion.Euler(0f, 0f, -30f) * Vector3.down;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            bound1 = Quaternion.Euler(0f, 0f, -30f) * Vector3.up;
            bound2 = Quaternion.Euler(0f, 0f, 30f) * Vector3.down;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            bound1 = Quaternion.Euler(0f, 0f, -30f) * Vector3.left;
            bound2 = Quaternion.Euler(0f, 0f, 30f) * Vector3.right;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            bound1 = Quaternion.Euler(0f, 0f, 30f) * Vector3.left;
            bound2 = Quaternion.Euler(0f, 0f, -30f) * Vector3.right;
        }
        else
        {
            bound1 = Vector3.zero;
            bound2 = Vector3.zero;
        }

        if (bound1 == Vector3.zero && bound2 == Vector3.zero)
        {
            GameObject teammate = FootballHelpers.GetClosestTarget(transform.position,
                _footballer._MatchController.GetAllPlayers());
            if (teammate)
            {
                TransplantController(teammate);
            }
        }
        else
        {
            GameObject teammate = FootballHelpers.GetActionTargetPosition(transform.position, bound1, bound2,
                _footballer._MatchController.GetAllPlayers());
            if (teammate)
            {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }
}
