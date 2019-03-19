using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehavior : MonoBehaviour {

    public enum DoorStates
    {
        Default = 0, 
        Active = 1,  
    }
    public Animator transitionsAnim;

    public DoorStates currentState = DoorStates.Default;
    public string nextLevel;

    // Use this for initialization
    void Start () {

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
                StartCoroutine(LoadScene());
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

    IEnumerator LoadScene()
    {
        transitionsAnim.SetTrigger("end2");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nextLevel);

    }
}
