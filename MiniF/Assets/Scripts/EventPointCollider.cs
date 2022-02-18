using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPointCollider : MonoBehaviour {

    private SphereCollider _collider;

    private void Awake() {
        SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>();
        for (int i = 0; i < colliders.Length; i++) {
            if (!colliders[i].isTrigger) {
                _collider = colliders[i];
                break;
            }
        }
        
        Physics.IgnoreCollision(_collider, GameObject.FindWithTag("Ball").GetComponent<SphereCollider>(), true);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Footballer")) {
            BasicAI basicAIScript = collision.gameObject.GetComponent<BasicAI>();
            if (basicAIScript.EventType != FootballEventType.None) {
                Physics.IgnoreCollision(_collider, collision.collider, true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Ball")) {
            // destroy EventPoint after collision with ball if it is in play
            if (other.gameObject.GetComponent<BallController>().IsInPlay) {
                Destroy(gameObject);
            }   
        }
    }
}
