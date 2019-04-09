using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZoneController : MonoBehaviour
{

    Rigidbody2D playerRB;
    public Vector2 boostVector;
    public float timeUntilBoost;
    private float boostTimer;

    private void Start()
    {
        boostTimer = timeUntilBoost;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (boostTimer >= 0)
        {
            boostTimer -= Time.deltaTime;
        }
        else
        {
            playerRB.velocity = boostVector;
            boostTimer = timeUntilBoost;
            playerRB = null;
        }
    }

}
