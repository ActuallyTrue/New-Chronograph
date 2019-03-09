using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

    //Create states for game
    public enum GameStates
    {
        playing = 0,
        menu = 1,
        loading = 2,
        scene = 3,
        script = 4
    }

    public GameStates currentState = GameStates.playing;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		switch(currentState)
        {
            case GameStates.playing: 
                //The level is in session
                //Check all necessary switches and camera work
                if (Input.GetKeyDown(KeyCode.Return))
                    {
                    Debug.Log("Game state in \"playing\"");
                    }
                if (Input.GetKeyDown(KeyCode.Escape))
                    {
                    currentState = GameStates.menu;
                    Debug.Log("Switching to \"menu\"");
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                    currentState = GameStates.loading;
                    Debug.Log("Switching to \"loading\"");
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                    {
                    currentState = GameStates.scene;
                    Debug.Log("Switching to \"scene\"");
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    currentState = GameStates.script;
                    Debug.Log("Switching to \"script\"");
                }
                break;
            case GameStates.menu:
                //The menu is open
                //Toggle buttons and animations
                if (Input.GetKeyDown(KeyCode.Return))
                    {
                    Debug.Log("Game state in \"menu\"");
                    }
                if (Input.GetKeyDown(KeyCode.Escape))
                    {
                    currentState = GameStates.playing;
                    }
                break;
            case GameStates.loading:
                //The game is loading a new level
                //Load a level transition
                if (Input.GetKeyDown(KeyCode.Return))
                    {
                    Debug.Log("Game state in \"loading\"");
                    }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    currentState = GameStates.playing;
                }
                break;
            case GameStates.scene:
                //The game is in a cutscene
                //Play presentation
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("Game state in \"scene\"");
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    currentState = GameStates.playing;
                }
                break;
            case GameStates.script:
                //The game is playing a scripted event
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("Game state in \"script\"");
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    currentState = GameStates.playing;
                }
                break;
        }
	}
}
