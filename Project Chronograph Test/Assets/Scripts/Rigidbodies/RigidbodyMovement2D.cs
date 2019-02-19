using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMovement2D : MonoBehaviour {

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    public float CalculatePlayerVelocity(float RBvelocity, Vector2 input, float moveSpeed,  ref float velocityXSmoothing, float accelerationTimeGrounded, float accelerationTimeAirborne, bool isGrounded)
    {
        float targetVelocityx = input.x * moveSpeed;
        return Mathf.SmoothDamp(RBvelocity, targetVelocityx, ref velocityXSmoothing, isGrounded ? accelerationTimeGrounded : accelerationTimeAirborne);
    }

    public void JumpPlayer(ref Vector2 input, bool isGrounded, float maxJumpVelocity )
    {
        if (isGrounded)
        {
            input.y = maxJumpVelocity;
        }

    }

    public void JumpPlayerRelease(ref Vector2 input, float minJumpVelocity)
    {
        if (input.y > minJumpVelocity)
        {
            input.y = minJumpVelocity;
        }
    }
}
