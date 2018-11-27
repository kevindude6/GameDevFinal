using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
public class AdventurerController : MonoBehaviour {

    public enum EntityState {RUNNING, STANDING, ATTACKING};
    
    Animator anim;
    SpriteRenderer sRenderer;
    AudioSource source;
    //ANIM TIMES
    const float ATTACKONETIME = 0.500f;



    public bool blockingMovement = false;
    //public bool isRunning = false;
    public EntityState currentState = EntityState.STANDING;
    public float blockingTime;
    public float runSpeed = 20;
    // Use this for initialization
    int idleHash = Animator.StringToHash("Idle2");
    int runHash = Animator.StringToHash("Run");
    int runBooleanHash = Animator.StringToHash("Running");
    int attackOneHash = Animator.StringToHash("AttackBool");

    //AudioClips
    public bool runSoundAlternate = false;
    public AudioClip runSoundOne;
    public AudioClip runSoundTwo;
    public AudioClip attackSound;

	void Start () {
        anim = GetComponent<Animator>();
        sRenderer = GetComponent<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        
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
            currentState = EntityState.RUNNING;
          
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
            currentState = EntityState.STANDING;
            
        }
        if(Input.GetAxis("Attack1") != 0)
        {
            //anim.SetBool(runBooleanHash, false);
            anim.SetBool(attackOneHash,true);
            if(blockingMovement!=true)
            {
                blockingTime = Time.time + ATTACKONETIME;
   
                source.PlayOneShot(attackSound,0.7f);
            }
         
            blockingMovement = true;
            
        }
        else
        {
            anim.SetBool(attackOneHash, false);
        }


        if(Input.GetAxis("Interact")!=0)
        {
            GameObject[] entities = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (GameObject e in entities)
            {
                if (Vector3.Distance(e.transform.position, transform.position) < 1.5f)
                {
                    MonoBehaviour[] list = e.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour mb in list)
                    {
                        if (mb is IInteractable)
                        {
                            IInteractable interactable = (IInteractable)mb;
                            interactable.OnInteract();
                            //Debug.Log("Interacting with something");
                        }
                    }
                }
            }
        }

        
        if(blockingMovement)
        {
            if (Time.time > blockingTime)
                blockingMovement = false;
            else
                move = new Vector2(0, 0);
        }

        

        //MOVEMENT
        transform.position = transform.position + new Vector3(move.x, move.y)*Time.deltaTime;

        DoSound();
	}

   
    private void DoSound()
    {
        if(currentState == EntityState.RUNNING)
        {
            if (!source.isPlaying)
                if (runSoundAlternate) {source.PlayOneShot(runSoundOne,0.3f); runSoundAlternate = false; }
                else { source.PlayOneShot(runSoundTwo,0.3f); runSoundAlternate = true; }
        }
        else
        {
            runSoundAlternate = false;
        }
    }
}
