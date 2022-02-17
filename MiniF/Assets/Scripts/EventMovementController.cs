using System.Collections.Generic;
using UnityEngine;

public class EventMovementController : MonoBehaviour {

    enum EventType {
        ThrowIn
    }

    private bool isEventPlaying;
    private EventType eventType;
    private List<GameObject> footballers;
    private List<Footballer> footballerScripts;
    private List<Vector3> positions;

    private void FixedUpdate() {
        if (isEventPlaying) {
            switch (eventType) {
                case EventType.ThrowIn:
                    if (Vector3.Distance(footballerScripts[0].MoveToPosition(positions[0]), positions[0]) < 0.1f) {
                        ThrowInReady();
                    }
                    break;
            }
        }
    }

    public void SetupThrowInPositions(GameObject thrower, Vector3 throwInPosition) {
        footballers.Add(thrower);
        footballerScripts.Add(thrower.GetComponent<Footballer>());
        positions.Add(throwInPosition);
    }

    private void ThrowInReady() {
        
    }
    
    
}
