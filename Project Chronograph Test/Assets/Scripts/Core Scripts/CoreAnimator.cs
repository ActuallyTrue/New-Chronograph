using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreAnimator : MonoBehaviour {

    Animation anim;

    public enum CoreStates
    {
        Idle = 0,
        Possessed = 1
    }

    public CoreStates currentState;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (currentState) {
            case CoreStates.Idle:
                anim.Play();

                break;
            case CoreStates.Possessed:
                anim.Play();


                break;

        }
    }
}
