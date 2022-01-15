using System;
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

        if (Input.GetKey(KeyCode.Space))
        {
            actionPower += 10f * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject teammate = _footballer.MakePass(actionPower);
            // transplant controller if there was a target for pass
            if (teammate)
            {
                TransplantController(teammate);
            }
            actionPower = 0f;
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
}
