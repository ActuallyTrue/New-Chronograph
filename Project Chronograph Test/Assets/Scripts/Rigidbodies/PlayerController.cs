using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class PlayerController : MonoBehaviour {

    //master movement class
    RigidbodyMovement2D Movement;

    //variables for getting input and controlling speed

    private Vector2 moveInput;
    public Vector2 MoveInput
    {
        get
        {
            return moveInput;
        }
    }
    [HideInInspector]
    public Vector2 pushInput; //Input specifically for when you possess a push core
    public float moveSpeed = 6f;
    public float dashSpeed = 10f;
    public float accelerationTimeAirborne;
    private float velocityXSmoothing;
    public float moveAfterLaunchTime;
    private float moveAfterLaunchTimer;
    private int dashDir; //1 is right, 2 is diag down right, and so on clockwise (0 is null basically)
    private int lastDir;

    //variables for variable jump height
    public float maxJumpVelocity;
    public float minJumpVelocity;

    //Boolean to decide if you can possess or not
    [HideInInspector]
    public bool canPossess = true; //public so that boost zones can give you your dash back

    //placeholder name for the variable, stands for an object that you're going to possess
    public Rigidbody2D coreRB;
 
    //the player's rigidbody
    private Rigidbody2D rb;

    //for turning the player around
    private bool facingRight = true;
    //so that we can use the run start up frames
    private bool wasJustIdle;

    //Everything for being grounded
    [HideInInspector]
    public bool isGrounded;
    public float checkRadius;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    //Everything for Wall Jumping
    private bool touchingRightWall;
    private bool touchingLeftWall;
    public LayerMask whatIsWall;
    public Transform rightWallCheck;
    public Transform leftWallCheck;
    private Transform wallCheckChanger;
    public float wallSlideDrag;
    public Vector2 wallJumpOffVelocity;
    private int lastWallDir;
    public float afterWallJumpTimerOriginal;
    private float afterWallJumpTimer;

    public float possessionTimerOriginal;
    private float possessionTimer;

    //dashing and possessing booleans for deciding if you can enter an object or not
    [HideInInspector]
    public bool dashing; 
    [HideInInspector]
    public bool possessing; //dashing is public so that the blast core can know whether or not to get the player component.
    [HideInInspector]
    public bool isCancelledPressed; //so that Push Cores can know whether or not to push the player early or not

    [HideInInspector]
    public BoxCollider2D boxCollider;

    private Vector3 playerScale;

    GameObject nonCollideCore;
    MovingCore_Controller MovingCoreController;
    PushCore_controller PushCoreController;
    BoostZoneController BoostZoneController;
    SpriteRenderer spriteRenderer;

    private Animator playerAnim;

    private bool isPushCore;

 
    //This is so that the camera controller will work
    [HideInInspector]
    public Bounds bounds;

    public CameraFollow cameraController;

    public enum PlayerStates
    {
        Idle = 0,
        Moving = 1,
        JumpingUp = 2,
        Falling = 3,
        DashStartUp = 4,
        Dashing = 5,
        PossessingCollide = 6,
        PossessingNonCollide = 7,
        WallSliding = 8,
        JumpingOffWall = 9,
        Boosting = 10

    }

    public PlayerStates currentState;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        //HumanMovementStateMachine humanMovementStateMachine = new HumanMovementStateMachine();
        //humanMovementStateMachine.OnEnter();
    }
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<RigidbodyMovement2D>();
        playerAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerScale = transform.localScale;
        afterWallJumpTimer = afterWallJumpTimerOriginal;
        possessionTimer = possessionTimerOriginal;
        bounds = boxCollider.bounds;
        moveAfterLaunchTimer = moveAfterLaunchTime;
    }


    private void FixedUpdate()
    {

            if (currentState == PlayerStates.Idle || currentState == PlayerStates.Moving || currentState == PlayerStates.JumpingUp || currentState == PlayerStates.Falling || currentState == PlayerStates.JumpingOffWall && afterWallJumpTimer <= 0)
            {
                afterWallJumpTimer = afterWallJumpTimerOriginal;

                if (isGrounded)
                {
                    //this line literally moves the character by changing its velocity directly
                    rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
                }
                else
                {
                    float targetVelocityX = moveInput.x * moveSpeed;
                    rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeAirborne), rb.velocity.y);
                }
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

        if(currentState == PlayerStates.JumpingOffWall && afterWallJumpTimer > 0) {
            afterWallJumpTimer -= Time.deltaTime;
        }
    }

    const string Player_State = "PlayerState";

    // Update is called once per frame
    void Update()
    {
        if(moveInput.x != 0)
        {
            lastDir = (int)Mathf.Sign(moveInput.x);
        }
        playerAnim.SetInteger("PlayerState", (int)currentState);
        playerAnim.SetBool("wasJustIdle", wasJustIdle);
        playerAnim.SetFloat("moveInput", Mathf.Abs(moveInput.x));
        playerAnim.SetInteger("DashDir", dashDir);

        //checks if you're grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        touchingRightWall = Physics2D.OverlapCircle(rightWallCheck.position, checkRadius, whatIsWall);

        touchingLeftWall = Physics2D.OverlapCircle(leftWallCheck.position, checkRadius, whatIsWall);


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
                wasJustIdle = true;
                if (moveInput.x > 0 || moveInput.x < 0)
                {
                    currentState = PlayerStates.Moving;
                }
                if (Input.GetButton("Jump")) {
                    currentState = PlayerStates.JumpingUp;
                }
                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
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
                    wasJustIdle = false;
                    currentState = PlayerStates.JumpingUp;
                }

                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    wasJustIdle = false;
                    currentState = PlayerStates.DashStartUp;
                }
                wasJustIdle = false;
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
                    currentState = PlayerStates.DashStartUp;
                }

                if (rb.velocity.y <= 0) {
                    currentState = PlayerStates.Falling;
                }

                break;
            case PlayerStates.Falling:
                //if you get sent out of a push core, this must be set to false so that the same push core won't push you just from touching it again
                isCancelledPressed = false; 
                if (isGrounded == true) {
                    if (Mathf.Abs(moveInput.x) > 0) {
                        currentState = PlayerStates.Moving;
                    }
                    currentState = PlayerStates.Idle;
                }
                //if you can possess then dash in whatever direction you're pressing
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    currentState = PlayerStates.DashStartUp;
                }

                if (touchingRightWall /*&& moveInput.x > 0*/  || touchingLeftWall /*&& moveInput.x < 0*/) {
                    currentState = PlayerStates.WallSliding;
                }
                break;
            case PlayerStates.WallSliding:
                //so the player can dash out of the wallsliding state
                if (Input.GetButtonDown("Possess") && canPossess)
                {
                    dashing = true;
                    canPossess = false;
                    currentState = PlayerStates.DashStartUp;
                }
                if (isGrounded)
                {
                    rb.drag = 0;
                    currentState = PlayerStates.Idle;
                }
                else if (touchingRightWall && moveInput.x > 0)
                {
                    rb.drag = wallSlideDrag;
                    if (Input.GetButtonDown("Jump")) {
                        rb.drag = 0;
                        lastWallDir = 1;
                        Movement.WallJumpPlayer(ref rb, new Vector2((-lastWallDir) * wallJumpOffVelocity.x, wallJumpOffVelocity.y));
                        currentState = PlayerStates.JumpingOffWall;
                    }
                }
                else if (touchingLeftWall && moveInput.x < 0)
                {
                    rb.drag = wallSlideDrag;
                    if (Input.GetButtonDown("Jump"))
                    {
                        rb.drag = 0;
                        lastWallDir = -1;
                        Movement.WallJumpPlayer(ref rb, new Vector2((-lastWallDir) * wallJumpOffVelocity.x, wallJumpOffVelocity.y));
                        currentState = PlayerStates.JumpingOffWall;
                    }
                }

                else
                {
                    rb.drag = 0;
                    currentState = PlayerStates.Falling;
                }


                break;
            case PlayerStates.JumpingOffWall:

                if (rb.velocity.y <= 0)
                {
                    //rb.drag = 0;
                    currentState = PlayerStates.Falling;
                }
                break;
            case PlayerStates.DashStartUp:
                if ((moveInput.x < 0.1f && moveInput.x > -0.1) && (moveInput.y < 0.1f && moveInput.y > -0.1f))
                {
                    if (lastDir == 1)
                    {
                        //right
                        dashDir = 1;
                        StartCoroutine(DashRight(rb));
                    }
                    if (lastDir == -1)
                    {
                        //left
                        dashDir = 5;
                        StartCoroutine(DashLeft(rb));
                    }
                    
                }
                if (moveInput.x > 0.1f && (moveInput.y < 0.1f && moveInput.y > -0.1f))
                {
                    //right
                    dashDir = 1;
                    StartCoroutine(DashRight(rb));
                }
                if (moveInput.x > 0.1f && moveInput.y < -0.1f)
                {
                    //down right
                    dashDir = 2;
                    StartCoroutine(DashDownRight(rb));
                }
                if ((moveInput.x < 0.1f && moveInput.x > -0.1) && moveInput.y < -0.1f)
                {
                    //down
                    dashDir = 3;
                    StartCoroutine(DashDown(rb));
                }
                if (moveInput.x < -0.1f && moveInput.y < -0.1f)
                {
                    //down left
                    dashDir = 4;
                    StartCoroutine(DashDownLeft(rb));
                }
                if (moveInput.x < -0.1f && (moveInput.y < 0.1f && moveInput.y > -0.1f))
                {
                    //left
                    dashDir = 5;
                    StartCoroutine(DashLeft(rb));
                }
                if (moveInput.x < -0.1f && moveInput.y > 0.1f)
                {
                    //up left
                    dashDir = 6;
                    StartCoroutine(DashUpLeft(rb, facingRight));
                }
                if ((moveInput.x < 0.1f && moveInput.x > -0.1) && moveInput.y > 0.1f)
                {
                    //up
                    dashDir = 7;
                    StartCoroutine(DashUp(rb));
                }
                if (moveInput.x > 0.1f && moveInput.y > 0.1f)
                {
                    //up right
                    dashDir = 8;
                    StartCoroutine(DashUpRight(rb));
                }
                break;
            case PlayerStates.Dashing:
                //if you're currently possessing something and you press the possess button, you pop out. (doesn't work currently, jumping out will transfer velocity, possessing out will make you dash out)
                if (!dashing)
                {
                    currentState = PlayerStates.Falling;
                }
                break;
            case PlayerStates.PossessingCollide:
                spriteRenderer.color = Color.clear;
                //if you're currently possessing something and you press the possess button, you pop out. (doesn't work currently, jumping out will transfer velocity, possessing out will make you dash out)
                if (possessing && Input.GetButtonDown("Jump") || possessionTimer <= 0)
                {
                    possessionTimer = possessionTimerOriginal;
                    RevertParent();
                    if(coreRB.velocity == new Vector2(0, 0)) 
                    {
                        canPossess = true;
                        isGrounded = true;
                        Movement.JumpPlayer(ref rb, isGrounded, maxJumpVelocity);
                        spriteRenderer.color = Color.white;
                        currentState = PlayerStates.JumpingUp;
                    }
                    else 
                    {
                        rb.velocity = TransferVelocity(coreRB, rb);
                        if (rb.velocity.y >= 0)
                        {
                            canPossess = true;
                            spriteRenderer.color = Color.white;
                            currentState = PlayerStates.JumpingUp;
                        }
                        else if (rb.velocity.y == 0)
                        {
                            canPossess = true;
                            rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
                            spriteRenderer.color = Color.white;
                            currentState = PlayerStates.Boosting;
                        }
                        else
                        {
                            canPossess = true; // so that you can dash again after unPossessing an object
                            spriteRenderer.color = Color.white;
                            currentState = PlayerStates.Falling;
                        }
                    }

                }

                possessionTimer -= Time.deltaTime;
                break;
            case PlayerStates.PossessingNonCollide:
                spriteRenderer.color = Color.clear;
                //if you're not possessing a push core, then we'll run through the normal moving core code
                if (!isPushCore)
                {
                    //if you're currently possessing something and you press the possess button, you pop out. (doesn't work currently, jumping out will transfer velocity, possessing out will make you dash out)
                    if (possessing && Input.GetButtonDown("Jump") || possessionTimer <= 0)
                    {
                        possessionTimer = possessionTimerOriginal;
                        RevertParent();
                        if (MovingCoreController.currentXVelocity == 0 && MovingCoreController.currentYVelocity == 0)
                        {
                            canPossess = true;
                            isGrounded = true;
                            Movement.JumpPlayer(ref rb, isGrounded, maxJumpVelocity);
                            spriteRenderer.color = Color.white;
                            currentState = PlayerStates.JumpingUp;
                        }
                        else
                        {
                            rb.velocity = TransferVelocity(coreRB, rb);
                            if(rb.velocity.y > 0) {
                                canPossess = true;
                                spriteRenderer.color = Color.white;
                                currentState = PlayerStates.JumpingUp;
                            }
                            else if(rb.velocity.y == 0) 
                            {
                                canPossess = true;
                                rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
                                spriteRenderer.color = Color.white;
                                currentState = PlayerStates.Boosting;
                            }
                            else {
                                canPossess = true; // so that you can dash again after unPossessing an object
                                spriteRenderer.color = Color.white;
                                currentState = PlayerStates.Falling;
                            }

                        }
                    }

                    //Changing speed for moving cores 
                    if (possessing && Input.GetButton("FastButton"))
                    {
                        MovingCoreController.currentState = MovingCore_Controller.CoreStates.SpedUp;
                    }
                    else if (possessing && Input.GetButton("SlowButton"))
                    {
                        MovingCoreController.currentState = MovingCore_Controller.CoreStates.SlowedDown;
                    }
                    else
                    {
                        MovingCoreController.currentState = MovingCore_Controller.CoreStates.Default;
                    }
                }

                //if you are possessing a push core, then
                else {
                    pushInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    isCancelledPressed = Input.GetButtonDown(AllText.someText /*"Jump"*/);
                    if (PushCoreController.pushTimer <= 0 || isCancelledPressed)
                    {
                        possessionTimer = possessionTimerOriginal;
                        RevertParent();
                        //rb.velocity = TransferVelocity(coreRB, rb);
                        canPossess = true; // so that you can dash again after unPossessing an object
                        spriteRenderer.color = Color.white;
                        currentState = PlayerStates.Falling;
                    }
                }

                possessionTimer -= Time.deltaTime;
                break;
            case PlayerStates.Boosting:

                if (moveAfterLaunchTimer <= 0)
                {
                    moveAfterLaunchTimer = moveAfterLaunchTime;
                    currentState = PlayerStates.Falling;
                }

                moveAfterLaunchTimer -= Time.deltaTime;



                //if (rb.velocity.y <= 0)
                //{
                //    currentState = PlayerStates.Falling;
                //}
                //if (isGrounded == true) {
                //    currentState = PlayerStates.Idle;
                //}
                //if (touchingRightWall /*&& moveInput.x > 0*/  || touchingLeftWall /*&& moveInput.x < 0*/)
                //{
                //    currentState = PlayerStates.WallSliding;
                //}
                //we can possibly make the player be able to dash out of the boosting state
                //if (Input.GetButtonDown("Possess") && canPossess)
                //{
                //    dashing = true;
                //    canPossess = false;
                //    currentState = PlayerStates.DashStartUp;
                //}
                break;
        }

	}

    //A ton of coroutines with the same logic, the diagonal ones just use trig to get the right speeds to go at the desired dash speed diagonally.
    IEnumerator DashRight(Rigidbody2D player) {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2 && dashing) {
            player.velocity = new Vector2(dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    IEnumerator DashUp(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(0, dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    IEnumerator DashUpRight(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(dashSpeedX, dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    //Zane broke this thing.
    // This is the template for condensing many of these coroutines 
    IEnumerator DashUpLeft(Rigidbody2D player, bool isRight)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(isRight ? dashSpeedX : -dashSpeedX, dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        canPossess = false;
        dashDir = 0;
        dashing = false;
    }

    IEnumerator DashLeft(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(-dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    IEnumerator DashDownLeft(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(-dashSpeedX, -dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    IEnumerator DashDown(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(0, -dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    IEnumerator DashDownRight(Rigidbody2D player)
    {
        currentState = PlayerStates.Dashing;
        float timer = 0;
        float dashSpeedX = dashSpeed * Mathf.Cos(Mathf.PI / 4);
        float dashSpeedY = dashSpeed * Mathf.Sin(Mathf.PI / 4);
        while (timer < 0.2 && dashing)
        {
            player.velocity = new Vector2(dashSpeedX, -dashSpeedY);
            timer += Time.deltaTime;
            yield return null;
        }
        player.velocity = Vector2.zero;
        dashDir = 0;
        canPossess = false;
        dashing = false;
    }

    //Changes the player's parent to whatever it's trying to possess
    void ChangeParent(Rigidbody2D core)
    {
        boxCollider.enabled = false;
        rb.isKinematic = true;
        transform.parent = core.transform;
        transform.position = core.transform.position;
    }

    void NonCollideChangeParent(GameObject core)
    {
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

    }


    //for the future
    //if the vfrom y velocity is positive, then add the player's jump velocity onto it and put go into the jumping up state
    //if the vfrom y velocity is negative, then just go through it how it is already but put you into the falling state
    //also, you need to add variable push strengths for push cores!!!!
    //transfers the velocity from one rigidbody to another
    Vector2 TransferVelocity(Rigidbody2D from, Rigidbody2D player)
    {
       if(from.gameObject.layer == 10) {
            return TransferVelocityFromMovingCore(from, player);
       }
        Vector2 vFrom = from.velocity;
       Vector2 vTo = player.velocity;
       vTo.x = 20f * vFrom.x;
       vTo.y = 20f * vFrom.y;
        Debug.Log(vTo + " Given");
        return vTo;
    }

    Vector2 TransferVelocityFromMovingCore(Rigidbody2D from, Rigidbody2D player)
    {
        Vector2 vFrom = new Vector2(MovingCoreController.currentXVelocity, MovingCoreController.currentYVelocity);
        Vector2 vTo = player.velocity;
        vTo.x = 3f * vFrom.x;
        vTo.y = 3f * vFrom.y;
        Debug.Log(vTo + " Given");
        return vTo;
    }


    //if you collide with a possessible object while dashing, you should go inside of it
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing) {
            if (collision.gameObject.layer == 11) //Layer 11 is for cores that you can physically collide with
            {
                //stopping the player so that they don't start possessing with an initial velocity
                rb.velocity = new Vector2(0, 0);
                Debug.Log("right here");
                possessing = true;
                dashing = false;
                coreRB = collision.rigidbody;
                Debug.Log(coreRB);
                ChangeParent(coreRB);
                currentState = PlayerStates.PossessingCollide;
            }
        }

        if (collision.gameObject.tag == "Platform")
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dashing)
        {
            Debug.Log("It's Happening!!!");
            if (collision.gameObject.layer == 10) //Layer 10 is for cores that you can't physically collide with
            {
                if(collision.gameObject.tag == "PushCore") {
                    //stopping the player so that they don't start possessing with an initial velocity
                    rb.velocity = new Vector2(0, 0);
                    isPushCore = true;
                    possessing = true;
                    dashing = false;
                    coreRB = collision.gameObject.GetComponent<Rigidbody2D>();
                    PushCoreController = collision.gameObject.GetComponent<PushCore_controller>();
                    nonCollideCore = collision.gameObject;
                    NonCollideChangeParent(nonCollideCore);
                    currentState = PlayerStates.PossessingNonCollide;
                }
                else {
                    //stopping the player so that they don't start possessing with an initial velocity
                    rb.velocity = new Vector2(0, 0);
                    possessing = true;
                    dashing = false;
                    isPushCore = false;
                    coreRB = collision.gameObject.GetComponent<Rigidbody2D>();
                    MovingCoreController = collision.gameObject.GetComponent<MovingCore_Controller>();
                    nonCollideCore = collision.gameObject;
                    NonCollideChangeParent(nonCollideCore);
                    currentState = PlayerStates.PossessingNonCollide;
                }

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dashing)
        {
            Debug.Log("It's Happening!!!");
            if (collision.gameObject.layer == 10) //Layer 10 is for cores that you can't physically collide with
            {
                if(collision.gameObject.tag == "PushCore") {
                    //stopping the player so that they don't start possessing with an initial velocity
                    rb.velocity = new Vector2(0, 0);
                    isPushCore = true;
                    possessing = true;
                    dashing = false;
                    coreRB = collision.gameObject.GetComponent<Rigidbody2D>();
                    PushCoreController = collision.gameObject.GetComponent<PushCore_controller>();
                    nonCollideCore = collision.gameObject;
                    NonCollideChangeParent(nonCollideCore);
                    currentState = PlayerStates.PossessingNonCollide;
                }
                else {
                    //stopping the player so that they don't start possessing with an initial velocity
                    rb.velocity = new Vector2(0, 0);
                    possessing = true;
                    dashing = false;
                    isPushCore = false;
                    coreRB = collision.gameObject.GetComponent<Rigidbody2D>();
                    MovingCoreController = collision.gameObject.GetComponent<MovingCore_Controller>();
                    nonCollideCore = collision.gameObject;
                    NonCollideChangeParent(nonCollideCore);
                    currentState = PlayerStates.PossessingNonCollide;
                }

            }
        }
    }




    //flips the player around so we don't have to make more animations
    void Flip()
    {
        //Whenever we turn the player around, the right wall check becomes the left wall check,
        //so we hange the orientations of the wall checkers so that the wall jump code still works
        wallCheckChanger = rightWallCheck;
        rightWallCheck = leftWallCheck;
        leftWallCheck = wallCheckChanger;
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        playerScale.x = -playerScale.x;

    }
}
