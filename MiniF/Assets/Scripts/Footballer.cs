using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FootballerStats))]
public class Footballer : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private FootballerStats _footballerStats;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _footballerStats = GetComponent<FootballerStats>();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 velocity = (targetPosition - transform.position).normalized * _footballerStats.MovementSpeed;
        _rigidbody.velocity = velocity;
    }

    public void MoveForward()
    {
        _rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * _footballerStats.MovementSpeed);
    }

    public void CharacterRotate(Vector3 rotation)
    {
        Quaternion rot = Quaternion.Euler(rotation * Time.fixedDeltaTime * _footballerStats.RotationSpeed);
        _rigidbody.MoveRotation(_rigidbody.rotation * rot);
    }
}
