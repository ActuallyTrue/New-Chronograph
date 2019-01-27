using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    public float attackRange;
	
	// Update is called once per frame
	void Update () {

        if(timeBtwAttack <= 0) {

            //then you can attack
            if(Input.GetButtonDown("Possess")){

                Collider2D[] objectsToPossess = Physics2D.OverlapCircleAll(attackPos.position, attackRange);

            }

            timeBtwAttack = startTimeBtwAttack;
        }

        else{

            timeBtwAttack -= Time.deltaTime;
        }

	}
}
