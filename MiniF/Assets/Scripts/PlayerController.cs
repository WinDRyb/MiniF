using System;
using UnityEngine;

[RequireComponent(typeof(Footballer))]
public class PlayerController : MonoBehaviour
{
    private bool move;
    private Vector3 rotation;

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
}
