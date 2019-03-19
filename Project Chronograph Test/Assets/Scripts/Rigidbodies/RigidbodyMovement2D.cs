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

    //platform movement stuff

    public void MovePlatform(Rigidbody2D rb, Vector2 pointToMoveTo)
    {
        rb.MovePosition(pointToMoveTo * Time.deltaTime);

    }

    //passing refs through these so that we can change the indexes and waypoints from this script
    public Vector2 CalculatePlatformMovement(float speed, ref int fromWaypointIndex, ref float percentBetweenWaypoints, ref Vector3[] globalWaypoints, bool cyclic, ref float nextMoveTime, float waitTime, float easeAmount)
    {

        //if the current time is less than the future moveTime, don't move
        if (Time.time < nextMoveTime)
        {
            return Vector2.zero;
        }

        //so that the indexes reset if they go out of bounds
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = PlatformEasing(percentBetweenWaypoints, easeAmount);

        //Lerp, or linear interpolation is a lot easier than it sounds, all it is is finding the line between two points so that you can find a point in between them
        Vector2 newPos = Vector2.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - (Vector2)transform.position;

    }

    //this function uses an easing function which gives a value according to the percentage of completeness so we can make platforms go slow at the beginning, speed up, then slow at the end
    public float PlatformEasing(float x, float easeAmount)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

}
