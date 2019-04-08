using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement2D))]
public class MovingCore_Controller : MonoBehaviour {
    public bool isBlastCore; //for making a moving core a blast core or not
    public bool isMoveWhenPossessed; // for making a moving core only move when possessed
    public float explodeTime; //how long in seconds before a blast core will explode
    //MAKE SURE TO SET THE PASSENGERMASK TO WHATEVER YOU WANNA MOVE 13:51
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public float defaultSpeed;
    public float fastSpeed;
    public float slowSpeed;
    public bool cyclic;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount; //above 3, it's basically just stopping and starting so we're clamping it from 0 to 2
    int fromWaypointIndex = 0;
    float percentBetweenWaypoints;
    float nextMoveTime;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    private Vector3 oldPoint;
    [HideInInspector]
    private Vector3 currentPoint;
    [HideInInspector]
    public float currentXVelocity;
    [HideInInspector]
    public float currentYVelocity;

    Movement2D Movement;
    //[HideInInspector]
    public PlayerController player;

    private bool justGotPossessed;
    //[HideInInspector]
    public float explodeTimer;


    public enum CoreStates
    {
        Default = 0,
        SpedUp = 1,
        SlowedDown = 2
    }

    public CoreStates currentState;

    // Use this for initialization
    public void Start()
    {
        currentState = CoreStates.Default;
        oldPoint = transform.position;
        Movement = GetComponent<Movement2D>();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if the blast core bool is not checked, then it will act like a normal moving core
        if (!isBlastCore && !isMoveWhenPossessed)
        {
            //normal moving core switch case
            switch (currentState)
            {
                case CoreStates.Default:
                    currentPoint = transform.position;
                    currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                    currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                    Movement.UpdateRaycastOrigins();

                    velocity = Movement.CalculatePlatformMovement(defaultSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                    Movement.CalculatePassengerMovement(velocity);
                    Movement.MovePassengers(true);
                    Movement.MovePlatform(velocity);
                    Movement.MovePassengers(false);
                    oldPoint = currentPoint;
                    break;
                case CoreStates.SpedUp:
                    currentPoint = transform.position;
                    currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                    currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                    Movement.UpdateRaycastOrigins();

                    velocity = Movement.CalculatePlatformMovement(fastSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                    Movement.CalculatePassengerMovement(velocity);
                    Movement.MovePassengers(true);
                    Movement.MovePlatform(velocity);
                    Movement.MovePassengers(false);
                    oldPoint = currentPoint;
                    break;
                case CoreStates.SlowedDown:
                    currentPoint = transform.position;
                    currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                    currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                    Movement.UpdateRaycastOrigins();

                    velocity = Movement.CalculatePlatformMovement(slowSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                    Movement.CalculatePassengerMovement(velocity);
                    Movement.MovePassengers(true);
                    Movement.MovePlatform(velocity);
                    Movement.MovePassengers(false);
                    oldPoint = currentPoint;
                    break;

            }
        }
        //if both are checked, then we want the moving core to act like both at the same time
        else if(isBlastCore && isMoveWhenPossessed) {
            //if the player is not possessing the blast core, it will not move
            if (player == null)
            {
                explodeTimer = explodeTime;
                //blast core switch case statement
                switch (currentState)
                {
                    case CoreStates.Default:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(0, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }
                        //setting this to true at the end of every frame the core is moving normally to appropriately change
                        //the timer variable whenever the blast core gets possessed
                        justGotPossessed = true;
                        break;

                }
            }
            //if the player is possessing the blast core then it will explode
            else
            {
                if (justGotPossessed)
                {
                    explodeTimer = explodeTime;
                    justGotPossessed = false;
                }
                switch (currentState)
                {
                    case CoreStates.Default:
                        Debug.Log("hi");
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(defaultSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;
                    case CoreStates.SpedUp:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(fastSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;
                    case CoreStates.SlowedDown:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(slowSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //explodeTimer = explodeTime
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;

                }
            }
        }
        //if the blast core bool is checked, it will act like a blast core
        else if(isBlastCore) {
            //if the player is not possessing the blast core, it will act like normal
            if (player == null)
            {
                explodeTimer = explodeTime;
                //blast core switch case statement
                switch (currentState)
                {
                    case CoreStates.Default:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(defaultSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }
                        //setting this to true at the end of every frame the core is moving normally to appropriately change
                        //the timer variable whenever the blast core gets possessed
                        justGotPossessed = true;
                        break;

                }
            }
            //if the player is possessing the blast core then it will explode
            else
            {
                if (justGotPossessed)
                {
                    explodeTimer = explodeTime;
                    justGotPossessed = false;
                }
                switch (currentState)
                {
                    case CoreStates.Default:
                        Debug.Log("hi");
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(defaultSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;
                    case CoreStates.SpedUp:
                    //to stop errors from showing up if we want the blast core to be stationary
                       if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(fastSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;
                    case CoreStates.SlowedDown:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(slowSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }

                        if (explodeTimer <= 0)
                        {
                            //explodeTimer = explodeTime
                            //THIS IS WHERE WE WILL START THE KILL PLAYER FUCTION
                            //LIKE: player.KillPlayer();
                        }
                        explodeTimer -= Time.deltaTime;
                        break;

                }
            }
        }

        else if (isMoveWhenPossessed) {

            //if the player is not possessing the blast core, it will act like normal
            if (player == null)
            {
                explodeTimer = explodeTime;
                //blast core switch case statement
                switch (currentState)
                {
                    case CoreStates.Default:
                        //to stop errors from showing up if we want the blast core to be stationary
                        if (localWaypoints.Length != 0)
                        {
                            currentPoint = transform.position;
                            currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                            currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                            Movement.UpdateRaycastOrigins();

                            velocity = Movement.CalculatePlatformMovement(0, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                            Movement.CalculatePassengerMovement(velocity);
                            Movement.MovePassengers(true);
                            Movement.MovePlatform(velocity);
                            Movement.MovePassengers(false);
                            oldPoint = currentPoint;
                        }
                        //setting this to true at the end of every frame the core is moving normally to appropriately change
                        //the timer variable whenever the blast core gets possessed
                        justGotPossessed = true;
                        break;

                }
            }
            //if the player is possessing the blast core then it will act like normal
            else
            {
                //normal moving core switch case
                switch (currentState)
                {
                    case CoreStates.Default:
                        currentPoint = transform.position;
                        currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                        currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                        Movement.UpdateRaycastOrigins();

                        velocity = Movement.CalculatePlatformMovement(defaultSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                        Movement.CalculatePassengerMovement(velocity);
                        Movement.MovePassengers(true);
                        Movement.MovePlatform(velocity);
                        Movement.MovePassengers(false);
                        oldPoint = currentPoint;
                        break;
                    case CoreStates.SpedUp:
                        currentPoint = transform.position;
                        currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                        currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                        Movement.UpdateRaycastOrigins();

                        velocity = Movement.CalculatePlatformMovement(fastSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                        Movement.CalculatePassengerMovement(velocity);
                        Movement.MovePassengers(true);
                        Movement.MovePlatform(velocity);
                        Movement.MovePassengers(false);
                        oldPoint = currentPoint;
                        break;
                    case CoreStates.SlowedDown:
                        currentPoint = transform.position;
                        currentXVelocity = (transform.position.x - oldPoint.x) / Time.deltaTime;
                        currentYVelocity = (transform.position.y - oldPoint.y) / Time.deltaTime;
                        Movement.UpdateRaycastOrigins();

                        velocity = Movement.CalculatePlatformMovement(slowSpeed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
                        Movement.CalculatePassengerMovement(velocity);
                        Movement.MovePassengers(true);
                        Movement.MovePlatform(velocity);
                        Movement.MovePassengers(false);
                        oldPoint = currentPoint;
                        break;

                }
            }
            }

        }

    Vector3 GiveOldPosition(Vector3 position)
    {

        return position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            player = collision.GetComponent<PlayerController>();
            if (!player.possessing)
            {
                player = null;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the player exits the blast core, then we set the PlayerController variable to null.
        if (collision.gameObject.layer == 9)
        {
            player = null;
        }
    }

    public void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                //so that the waypoints are drawn in the correct position when playing and when not playing
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.right * size, globalWaypointPos + Vector3.right * size);
            }
        }
    }
}


