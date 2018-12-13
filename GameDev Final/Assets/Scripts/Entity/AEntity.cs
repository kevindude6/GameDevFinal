using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class AEntity : MonoBehaviour
{
    public float EHealth;
    public AudioSource source;
    public Animator anim;

    public float trackingRange;
    public Vector3 velocity;
    public float maxSpeed;
    public float accel;
  


    public enum State { IDLE, RUNNING, ATTACKING };
    public State mState;

    public virtual void Initialize()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }
    public virtual void Hurt(float damage)
    {
        EHealth -= damage;
    }
    public virtual void SignalAnimatorFromState()
    {
        if(anim==null)
        {
           anim = GetComponent<Animator>();
        }
        if(anim)
        {
            switch (mState)
            {
                case State.IDLE:
                    anim.SetBool("running", false);
                    anim.SetBool("attacking", false);
                    anim.SetBool("idle", true);
                    break;
                case State.ATTACKING:
                    anim.SetBool("running", false);
                    anim.SetBool("attacking", true);
                    anim.SetBool("idle", false);
                    break;
                case State.RUNNING:
                    anim.SetBool("running", true);
                    anim.SetBool("attacking", false);
                    anim.SetBool("idle", false);
                    break;
            }
        }
    }
}
