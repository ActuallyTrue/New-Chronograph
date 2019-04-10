using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZoneController : MonoBehaviour
{

    Rigidbody2D playerRB;
    PlayerController player;
    public Vector2 boostVector;
    public float timeUntilBoost;
    private float boostTimer;

    private void Start()
    {
        boostTimer = timeUntilBoost;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            player = collision.gameObject.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player.currentState != PlayerController.PlayerStates.Dashing)
        {
            if (boostTimer >= 0)
            {
                boostTimer -= Time.deltaTime;
            }
            else
            {
                playerRB.velocity = boostVector;
                player.currentState = PlayerController.PlayerStates.Boosting;
                player.canPossess = true;
                boostTimer = timeUntilBoost;
                playerRB = null;
                player = null;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        boostTimer = timeUntilBoost;

    }

}
