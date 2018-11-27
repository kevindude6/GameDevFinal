
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEntity : AEntity {

    // Use this for initialization

    public AudioClip hurtSound;
    
	void Start () {

        EHealth = 100;
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    public override void Hurt(float damage)
    {
        EHealth -= damage;
        source.PlayOneShot(hurtSound,0.5f);
    }
}
