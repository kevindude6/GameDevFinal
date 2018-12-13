
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEntity : AEntity {

    // Use this for initialization

    
    public AudioClip hurtSound;
    public bool trackingPlayer = false;
    public GameObject player;
  

	void Start () {

        EHealth = 100;
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if(trackingPlayer)
        {
            if (!player) return;
            if(Vector3.Distance(player.transform.position,this.transform.position) < trackingRange)
            {
                velocity += Vector3.Normalize(player.transform.position - this.transform.position) * accel * Time.deltaTime;
                if (velocity.magnitude > maxSpeed)
                    velocity = Vector3.Normalize(velocity) * maxSpeed;

               

                
                
            }
            else
            {
                velocity -= velocity.normalized*accel * Time.deltaTime;
            }
        }

        if (velocity.magnitude > 0.2)
        {
            mState = State.RUNNING;
            SignalAnimatorFromState();
        }
        else
        {
            mState = State.IDLE;
            SignalAnimatorFromState();
        }
        this.transform.position += velocity * Time.deltaTime;
    }

    
    public override void Hurt(float damage)
    {
        EHealth -= damage;
        source.PlayOneShot(hurtSound,0.5f);
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}
