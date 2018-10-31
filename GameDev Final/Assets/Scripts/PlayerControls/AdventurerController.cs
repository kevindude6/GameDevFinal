using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerController : MonoBehaviour {

    Animator anim;
    SpriteRenderer sRenderer;
    public float runSpeed = 20;
    // Use this for initialization
    int idleHash = Animator.StringToHash("Idle2");
    int runHash = Animator.StringToHash("Run");
    int runBooleanHash = Animator.StringToHash("Running");
	void Start () {
        anim = GetComponent<Animator>();
        sRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (move.magnitude*runSpeed > (move.normalized*runSpeed).magnitude) { move = move.normalized * runSpeed; }
        else { move = move * runSpeed; }
        //Debug.Log(move);
        //ANIM
        if(move.magnitude!=0)
        {
            //anim.SetTrigger(runHash);
            anim.SetBool(runBooleanHash, true);
            //Debug.Log("Setting trigger");
            if (move.x < 0)
            {
                sRenderer.flipX = true;
            }
            else
            {
                sRenderer.flipX = false;
            }
        }
        else
        {
           // anim.SetTrigger(idleHash);
            anim.SetBool(runBooleanHash, false);
        }

        //MOVEMENT
        transform.position = transform.position + new Vector3(move.x, move.y)*Time.deltaTime;
	}
}
