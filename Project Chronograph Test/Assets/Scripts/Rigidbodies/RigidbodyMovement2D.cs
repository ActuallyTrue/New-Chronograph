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

    public void JumpPlayer(ref Rigidbody2D player, bool isGrounded, float maxJumpVelocity )
    {
        if (isGrounded)
        {
            player.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
        }

    }

    public void JumpPlayerRelease(ref Rigidbody2D player, float minJumpVelocity)
    {
        if (player.velocity.y > minJumpVelocity)
        {
            player.velocity = new Vector2(rb.velocity.x, minJumpVelocity);
        }
    }
}
