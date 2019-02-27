using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class PlayerController : MonoBehaviour {

    RigidbodyMovement2D Movement;

    Vector2 moveInput;
    public float moveSpeed = 6f;
    public float dashSpeed = 10;

    //these variables give a more intuitive way to assing gravity and jump velocity rather than changing them directly
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    float maxJumpVelocity;
    float minJumpVelocity;

    bool canPossess = true;

    public Rigidbody2D husk;

    public float afterPossessTimer;

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

    private bool possessing;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<RigidbodyMovement2D>();
        //just some physics to set the gjump velocity
        maxJumpVelocity = 20;
        minJumpVelocity = 9.8f;
        print("Gravity: " + rb.gravityScale + " Jump Velocity: " + maxJumpVelocity);

    }

    private void FixedUpdate()
    {

        if (isGrounded) {
            canPossess = true;
        }

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float targetVelocityx = moveInput.x * moveSpeed;

        float wantedVelocity = Mathf.SmoothDamp(rb.velocity.x, targetVelocityx, ref velocityXSmoothing, isGrounded ? accelerationTimeGrounded : accelerationTimeAirborne);
        rb.velocity = new Vector2(wantedVelocity, rb.velocity.y);

        if (facingRight == false && moveInput.x > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput.x < 0) {
            Flip();
        }
    }

    // Update is called once per frame
    void Update () {
        Debug.Log(canPossess);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (Input.GetButtonDown("Jump"))
        {
            Movement.JumpPlayer(ref rb, isGrounded, maxJumpVelocity);
        }
        if (Input.GetButtonUp("Jump"))
        {
            Movement.JumpPlayerRelease(ref rb, minJumpVelocity);
        }

        if (Input.GetButtonDown("Possess") && canPossess)
        {
            rb.isKinematic = true;
            canPossess = false;
            if ((moveInput.x < 0.1f && moveInput.x > -0.1) && (moveInput.y < 0.1f && moveInput.y > -0.1f))
            {
                StartCoroutine(DashRight(rb));
            }
            if (moveInput.x > 0.1f && moveInput.y > 0.1f)
            {
                //up right
                StartCoroutine(DashUpRight(rb));
            }
            if ((moveInput.x < 0.1f && moveInput.x > -0.1) && moveInput.y > 0.1f)
            {
                //up
                StartCoroutine(DashUp(rb));
            }
            if (moveInput.x < -0.1f && moveInput.y > 0.1f)
            {
                //up left
                StartCoroutine(DashUpLeft(rb));
            }
            if (moveInput.x > 0.1f && (moveInput.y < 0.1f && moveInput.y > -0.1f))
            {
                //right
                StartCoroutine(DashRight(rb));
            }
            if (moveInput.x > 0.1f && moveInput.y < -0.1f)
            {
                //down right
                StartCoroutine(DashDownRight(rb));
            }
            if ((moveInput.x < 0.1f && moveInput.x > -0.1) && moveInput.y < -0.1f)
            {
                //down
                StartCoroutine(DashDown(rb));
            }
            if (moveInput.x < -0.1f && moveInput.y < -0.1f)
            {
                //down left
                StartCoroutine(DashDownLeft(rb));
            }
            if (moveInput.x < -0.1f && (moveInput.y < 0.1f && moveInput.y > -0.1f))
            {
                //left
                StartCoroutine(DashLeft(rb));
            }
        }

	}


    IEnumerator DashRight(Rigidbody2D player) {
        float timer = 0;
        while (timer < 0.2) {
            player.velocity = new Vector2(dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;
        
    }

    IEnumerator DashUp(Rigidbody2D player)
    {
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(0, dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashUpRight(Rigidbody2D player)
    {
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2)
        {
            player.velocity = new Vector2(dashSpeedX, dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashUpLeft(Rigidbody2D player)
    {
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2)
        {
            player.velocity = new Vector2(-dashSpeedX, dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashLeft(Rigidbody2D player)
    {
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(-dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashDownLeft(Rigidbody2D player)
    {
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2)
        {
            player.velocity = new Vector2(-dashSpeedX, -dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashDown(Rigidbody2D player)
    {
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(0, -dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }

    IEnumerator DashDownRight(Rigidbody2D player)
    {
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2)
        {
            player.velocity = new Vector2(dashSpeedX, -dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        player.isKinematic = false;

    }


    void ChangeParent(Rigidbody2D husk)
    {
        transform.parent = husk.transform;
        transform.position = husk.transform.position;


    }

    //Revert the parent of object 2.
    void RevertParent()
    {
        possessing = false;
        transform.parent = null;

    }

    Vector2 transferVelocity(Rigidbody2D from, Rigidbody2D player)
    {
            Vector2 vFrom = from.velocity;
            Vector2 vTo = player.velocity;
        vTo.x = 150f * vFrom.x;
        vTo.y = 150f * vFrom.y;
        return vTo;
    }


    void Flip()
    {

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }
}
