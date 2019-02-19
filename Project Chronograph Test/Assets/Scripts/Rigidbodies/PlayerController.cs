using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class PlayerController : MonoBehaviour {

    RigidbodyMovement2D Movement;

    public float moveSpeed = 6f;

    //these variables give a more intuitive way to assing gravity and jump velocity rather than changing them directly
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    float maxJumpVelocity;
    float minJumpVelocity;



    private Vector2 input;
    private float velocityXSmoothing;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public float checkRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<RigidbodyMovement2D>();
        //just some physics to set the gjump velocity
        maxJumpVelocity = 20;
        minJumpVelocity = 9.8f;
        print("Gravity: " + rb.gravityScale + " Jump Velocity: " + maxJumpVelocity);

    }
	
	// Update is called once per frame
	void Update () {
        Vector2 lastVelocity = rb.velocity;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        input = new Vector2(Input.GetAxisRaw("Horizontal"), rb.velocity.y);
        input.x = Movement.CalculatePlayerVelocity(rb.velocity.x, input, moveSpeed, ref velocityXSmoothing, accelerationTimeGrounded, accelerationTimeAirborne, isGrounded);

        if (Input.GetButtonDown("Jump"))
        {
            Movement.JumpPlayer(ref input, isGrounded, maxJumpVelocity);
        }
        if (Input.GetButtonUp("Jump"))
        {
            Movement.JumpPlayerRelease(ref input, minJumpVelocity);
        }

        rb.velocity = input;

	}

    void Flip()
    {

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }
}
