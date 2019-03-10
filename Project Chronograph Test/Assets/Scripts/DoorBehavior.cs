using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour {

    public enum DoorStates
    {
        Default = 0, 
        Active = 1,  
    }

    public DoorStates currentState = DoorStates.Default;
    public string NextLevel;
    public GameObject Transition;
    //Bool with transition set to black

    // Use this for initialization
    void Start () {
		//Play the fade in transition once
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentState)
        {
            case DoorStates.Default:
                //Play glowing animation on loop
                Debug.Log("Door is in Default.");
                break;

            case DoorStates.Active:
                Debug.Log("Door is now in Active");
                //Play Possessed animation 
                //Play closing animation
                //Start the level loading coroutine
                //Reset to Default
                break;
        }
	}

    void OnTriggerEnter2D(Collider2D target)
    {
        switch (currentState)
        { 
            case DoorStates.Default:
                //This needs the player to be in possession mode
                if (target.tag == "Player")
                {
                    currentState = DoorStates.Active;
                }
                break;
            case DoorStates.Active:
                break; 
        }
    }
}
