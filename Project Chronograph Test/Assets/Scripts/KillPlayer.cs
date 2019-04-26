using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{

    public PlayerController player;
    public Animator transitionsAnim;
    public string levelName;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.layer == 9)
        {
            Debug.Log("Door is now in Active");
            StartCoroutine(ResetLevel());
        }
    }

    IEnumerator ResetLevel()
    {
        player.enabled = false;
        //play death animation
        transitionsAnim.SetTrigger("end2"); //play a level reset transition, not end2. 
        yield return new WaitForSeconds(1.5f);
        player.enabled = true;
        SceneManager.LoadScene(levelName);
    }
}