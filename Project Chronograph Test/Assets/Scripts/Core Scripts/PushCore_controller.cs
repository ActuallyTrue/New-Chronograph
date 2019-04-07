using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCore_controller : MonoBehaviour {

    public PlayerController playerController;
   public Rigidbody2D playerRB;
    public float pushTime;
    [HideInInspector]
    public float pushTimer;
    public float pushXvector;
    public float pushYvector;
    private bool justPushed;

	// Use this for initialization
	void Start () {
        pushTimer = pushTime;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(pushTimer);
        if(playerController != null && playerRB != null) 
        { 
            if(pushTimer >= 0 && !playerController.isCancelledPressed)
            {
                pushTimer -= Time.deltaTime;
            }
            //once the timer is up, we get the player's input and push them away
            else {
                justPushed = true;
                pushTimer = pushTime;
                //idle
                if ((playerController.pushInput.x < 0.1f && playerController.pushInput.x > -0.1) && (playerController.pushInput.y < 0.1f && playerController.pushInput.y > -0.1f))
                {
                    //right
                    playerRB.velocity = new Vector2(pushXvector, 0);
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x > 0.1f && playerController.pushInput.y > 0.1f)
                {
                    //up right
                    playerRB.velocity = new Vector2(pushXvector * Mathf.Cos(Mathf.PI / 4), pushYvector * Mathf.Sin(Mathf.PI / 4));
                    RemovePlayerComponents();
                }
                else if ((playerController.pushInput.x < 0.1f && playerController.pushInput.x > -0.1) && playerController.pushInput.y > 0.1f)
                {
                    //up
                    playerRB.velocity = new Vector2(0, pushYvector);
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x < -0.1f && playerController.pushInput.y > 0.1f)
                {
                    //up left
                    playerRB.velocity = new Vector2(-pushXvector * Mathf.Cos(Mathf.PI / 4), pushYvector * Mathf.Sin(Mathf.PI / 4));
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x > 0.1f && (playerController.pushInput.y < 0.1f && playerController.pushInput.y > -0.1f))
                {
                    //right
                    playerRB.velocity = new Vector2(pushXvector, 0);
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x > 0.1f && playerController.pushInput.y < -0.1f)
                {
                    //down right
                    playerRB.velocity = new Vector2(pushXvector * Mathf.Cos(Mathf.PI / 4), -pushYvector * Mathf.Sin(Mathf.PI / 4));
                    RemovePlayerComponents();
                }
                else if ((playerController.pushInput.x < 0.1f && playerController.pushInput.x > -0.1) && playerController.pushInput.y < -0.1f)
                {
                    //down
                    playerRB.velocity = new Vector2(0, -pushYvector);
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x < -0.1f && playerController.pushInput.y < -0.1f)
                {
                    //down left
                    playerRB.velocity = new Vector2(-pushXvector * Mathf.Cos(Mathf.PI / 4), -pushYvector * Mathf.Sin(Mathf.PI / 4));
                    RemovePlayerComponents();
                }
                else if (playerController.pushInput.x < -0.1f && (playerController.pushInput.y < 0.1f && playerController.pushInput.y > -0.1f))
                {
                    //left
                    playerRB.velocity = new Vector2(-pushXvector, 0);
                    RemovePlayerComponents();
                }
            }
        }
        else 
        {
            pushTimer = pushTime;
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            //I know this isn't the most elegant solution, as it will always get these components of the
            //player whenever it touches the player, but everything I've tried has been inconsistent
            //at best, so I'm just going to leave it like this because it's consistent this way.
            playerController = collision.GetComponent<PlayerController>();
            playerRB = collision.GetComponent<Rigidbody2D>();

        }
    }

    private void RemovePlayerComponents() {
        playerController = null;
        playerRB = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the player exits the blast core, then we set the PlayerController variable to null.
        if (collision.gameObject.layer == 9)
        {
            playerController = null;
            playerRB = null;
        }
    }
}
