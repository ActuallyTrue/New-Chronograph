using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumMovement : MonoBehaviour {

    private Rigidbody2D core;
    private DistanceJoint2D distance;
    private float length;
    public float leftPushRange;
    public float rightPushRange;
    public float velocityThreshold;
    public Vector2 previousVelocity;
    private float angularVelocity;
    private float currentAngle;
    private float linearVelocity;

    
    private void Start()
    {
        core = GetComponent<Rigidbody2D>();
        distance = GetComponent<DistanceJoint2D>();
        core.velocity = new Vector2(velocityThreshold, 0);
        length = distance.distance;
        
    }

    private void Update()
    {
        currentAngle = Mathf.Atan2(core.velocity.y, core.velocity.x);
        linearVelocity = core.velocity.x / Mathf.Cos(currentAngle);
        Push();
        previousVelocity = core.velocity;
    }

    public void Push() {

        if (linearVelocity >= 5.8f && linearVelocity <= 5.9f && Mathf.Sign(previousVelocity.x) == 1) {
            core.velocity = new Vector2 (6,0);
            Debug.Log("Push Given");
        }
        if (linearVelocity >= 5.8f && linearVelocity <= 5.9f && Mathf.Sign(previousVelocity.x) == -1) {
            core.velocity = new Vector2(-6, 0);
            Debug.Log("Push Given");
        }

    }
}
