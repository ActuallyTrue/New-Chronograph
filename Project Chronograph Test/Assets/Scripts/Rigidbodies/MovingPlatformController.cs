using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement2D))]
public class MovingPlatformController : MonoBehaviour {
    //MAKE SURE TO SET THE PASSENGERMASK TO WHATEVER YOU WANNA MOVE 13:51
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount; //above 3, it's basically just stopping and starting so we're clamping it from 0 to 2
    int fromWaypointIndex = 0;
    float percentBetweenWaypoints;
    float nextMoveTime;
    private Vector2 pointToMoveTo;

    RigidbodyMovement2D Movement;
    Rigidbody2D platrb;

    // Use this for initialization
    public void Start()
    {
        Movement = GetComponent<RigidbodyMovement2D>();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
        platrb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        pointToMoveTo = Movement.CalculatePlatformMovement(speed, ref fromWaypointIndex, ref percentBetweenWaypoints, ref globalWaypoints, cyclic, ref nextMoveTime, waitTime, easeAmount);
        Movement.MovePlatform(platrb, pointToMoveTo);
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
