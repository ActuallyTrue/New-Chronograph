using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class PlayerController : MonoBehaviour {

    RigidbodyMovement2D Movement;

    Vector2 moveInput;
    public float moveSpeed = 6f;
    public float dashSpeed = 10;

    public float maxJumpVelocity;
    public float minJumpVelocity;

    bool canPossess = true;

    public Rigidbody2D husk;

    public float afterPossessTimer;

    private Vector2 input;
    private float velocityXSmoothing;
 
    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public float checkRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    private bool possessing;
    private bool canMove = true;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<RigidbodyMovement2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (isGrounded) {
            canPossess = true;
        }
        if (canMove)
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }

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
            canMove = false;
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
        canMove = true;

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
        canMove = true;
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
        canMove = true;
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
        canMove = true;
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
        canMove = true;
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
        canMove = true;
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
        canMove = true;
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
        canMove = true;
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
