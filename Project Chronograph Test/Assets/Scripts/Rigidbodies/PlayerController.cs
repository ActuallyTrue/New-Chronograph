using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class PlayerController : MonoBehaviour {

    //master movement class
    RigidbodyMovement2D Movement;

    //variables for getting input and controlling speed
    Vector2 moveInput;
    public float moveSpeed = 6f;
    public float dashSpeed = 10;

    //variables for variable jump height
    public float maxJumpVelocity;
    public float minJumpVelocity;

    //Boolean to decide if you can possess or not
    bool canPossess = true;

    //placeholder name for the variable, stands for an object that you're going to possess
    public Rigidbody2D core;
 
    //the player's rigidbody
    private Rigidbody2D rb;

    //for turning the player around
    private bool facingRight = true;

    //Everything for being grounded
    private bool isGrounded;
    public float checkRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    //dashing, possessing, and canMove booleans for deciding if you can enter an object or not
    private bool dashing;
    private bool possessing;
    private bool canMove = true;

    private float afterPossessTimer;

    BoxCollider2D boxCollider;

    Vector3 playerScale;

    public enum PlayerStates
    {
        Idle = 0,
        Moving = 1,
        JumpingUp = 2,
        Falling = 3,
        DashStartUp = 4,
        Dashing = 5,
        Possesing = 6
    }

    public PlayerStates currentState;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<RigidbodyMovement2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerScale = transform.localScale;
    }


    private void FixedUpdate()
    {
       
        if (currentState == PlayerStates.Idle || currentState == PlayerStates.Moving || currentState == PlayerStates.JumpingUp || currentState == PlayerStates.Falling)
        {
            //this line literally moves the character by changing its velocity directly
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

            //code that flips the character so we don't have to make animations for walking in both directions
            if (facingRight == false && moveInput.x > 0)
            {
                Flip();
            }
            else if (facingRight == true && moveInput.x < 0)
            {
                Flip();
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        //checks if you're grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        //canPossess acts as a dash charge, once you land it refills.
        if (isGrounded)
        {
            canPossess = true;
        }
        //gets horizontal and vertical input so you can move and dash in all 8 directions
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        switch (currentState)
        {
            case PlayerStates.Idle:
                if (moveInput.x > 0 || moveInput.x < 0)
                {
                    currentState = PlayerStates.Moving;
                }
                if (Input.GetButtonDown("Jump")) {
                    currentState = PlayerStates.JumpingUp;
                }
                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    canMove = false;
                    currentState = PlayerStates.DashStartUp;
                }
                if (!isGrounded)
                {
                    currentState = PlayerStates.Falling;
                }

                break;
            case PlayerStates.Moving:

                if (moveInput.x < 0.01f && moveInput.x > -0.01f) {
                    currentState = PlayerStates.Idle;
                }
                if (Input.GetButtonDown("Jump"))
                {
                    currentState = PlayerStates.JumpingUp;
                }

                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    canMove = false;
                    currentState = PlayerStates.DashStartUp;
                }

                break;
            case PlayerStates.JumpingUp:
                //if you jump it changes your y velocity to the maxJumpVelocity
                Movement.JumpPlayer(ref rb, isGrounded, maxJumpVelocity);

                //if you release jump while your y velocity is above your minJumpVelocity, your velocity gets set to your min jump velocity (variable jump height)
                if (Input.GetButtonUp("Jump"))
                {
                    Movement.JumpPlayerRelease(ref rb, minJumpVelocity);
                    currentState = PlayerStates.Falling;
                }

                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    canMove = false;
                    currentState = PlayerStates.DashStartUp;
                }

                if (rb.velocity.y <= 0) {
                    currentState = PlayerStates.Falling;
                }

                break;
            case PlayerStates.Falling:
                if (isGrounded == true) {
                    currentState = PlayerStates.Idle;
                }
                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    canMove = false;
                    currentState = PlayerStates.DashStartUp;
                }
            
                break;
            case PlayerStates.DashStartUp:
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
                break;
            case PlayerStates.Dashing:
                //if you're currently possessing something and you press the possess button, you pop out. (doesn't work currently, jumping out will transfer velocity, possessing out will make you dash out)
                if (!dashing)
                {
                    currentState = PlayerStates.Falling;
                }
                break;
            case PlayerStates.Possesing:
                //if you're currently possessing something and you press the possess button, you pop out. (doesn't work currently, jumping out will transfer velocity, possessing out will make you dash out)
                if (possessing && Input.GetButtonDown("Cancel"))
                {
                    RevertParent();
                    rb.velocity = transferVelocity(core, rb);
                    currentState = PlayerStates.Falling;
                }
                break;
        }

	}

    //A ton of coroutines with the same logic, the diagonal ones just use trig to get the right speeds to go at the desired dash speed diagonally.
    IEnumerator DashRight(Rigidbody2D player) {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2) {
            player.velocity = new Vector2(dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashUp(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(0, dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        canPossess = false;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashUpRight(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
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
        canPossess = false;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashUpLeft(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
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
        canPossess = false;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashLeft(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(-dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashDownLeft(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
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
        dashing = false;
        canMove = true;
    }

    IEnumerator DashDown(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2)
        {
            player.velocity = new Vector2(0, -dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashing = false;
        canMove = true;
    }

    IEnumerator DashDownRight(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
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
        dashing = false;
        canMove = true;
    }

    //Changes the player's parent to whatever it's trying to possess
    void ChangeParent(Rigidbody2D core)
    {
        canMove = false;
        boxCollider.enabled = false;
        rb.isKinematic = true;
        transform.parent = core.transform;
        transform.position = core.transform.position;

    }

    //Revert the parent of object 2.
    void RevertParent()
    {
        possessing = false;
        transform.parent = null;
        rb.isKinematic = false;
        boxCollider.enabled = true;
        transform.localScale = playerScale;
        transform.rotation = Quaternion.Euler(0,0,0);
        canMove = true;

    }

    //transfers the velocity from one rigidbody to another
    Vector2 transferVelocity(Rigidbody2D from, Rigidbody2D player)
    {
       Vector2 vFrom = from.velocity;
       Vector2 vTo = player.velocity;
       vTo.x = 20f * vFrom.x;
       vTo.y = 20f * vFrom.y;
        Debug.Log(vTo + " Given");
        return vTo;
    }

    Vector2 transferVelocityPendulums(Rigidbody2D from, Rigidbody2D player)
    {
        
        float vFromAV = from.angularVelocity;
        float vFromLV = vFromAV*(Mathf.PI * 3) / 180;
        Vector2 vFrom = new Vector2(vFromLV*Mathf.Cos(from.rotation * Mathf.Deg2Rad), vFromLV * Mathf.Sin(from.rotation * Mathf.Deg2Rad));
        Vector2 vTo = player.velocity;
        vTo.x = 150f * vFrom.x;
        vTo.y = 150f * vFrom.y;
        Debug.Log(vTo + " Given");
        return vTo;
    }

    //if you collide with a possessible object while dashing, you should go inside of it (not currently working)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing) {
            canMove = false;
            Debug.Log("right here");
            possessing = true;
            core = collision.rigidbody;
            Debug.Log(core);
            ChangeParent(core);
            currentState = PlayerStates.Possesing;

        }
    }

    //flips the player around so we don't have to make more animations
    void Flip()
    {

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }
}
