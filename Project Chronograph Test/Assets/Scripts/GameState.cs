using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    //Create states for game
    public enum GameStates
    {
        None = 0,
        Playing = 1,
        Menu = 2,
        Loading = 3
    }

    public GameStates currentState = GameStates.Playing;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region None

    IEnumerator None_EnterState()
    {
        while(true)
        {
            //Halt everything.
        }
    }

    #endregion

    #region Playing

    IEnumerator Playing_EnterState()
    {
        while (true)
        {
            //Check things baby.
        }

        //Function for detecting scene playing
        //turn off when happening
    }

    #endregion

    #region Menu
    IEnumerator Menu_EnterState()
    {
        //Open up the menu 
        while (true)
        {
            //Menu stuff
        }
    }

    #endregion

    #region Loading

    IEnumerator Loading_EnterState()
    {
        //Play a level transition and load the next level.
        while(true)
        {
            //Transition
        }
    }

    #endregion

}
