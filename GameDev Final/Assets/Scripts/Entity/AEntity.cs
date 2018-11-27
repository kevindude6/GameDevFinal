using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class AEntity : MonoBehaviour
{
    public float EHealth;
    public AudioSource source;

    public virtual void Initialize()
    {
        source = GetComponent<AudioSource>();
    }
    public virtual void Hurt(float damage)
    {
        EHealth -= damage;
    }
}
