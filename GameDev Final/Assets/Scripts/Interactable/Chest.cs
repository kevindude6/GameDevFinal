using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{

    public Sprite openedChest;
    public Sprite closedChest;
    public AudioSource source;
    public AudioClip openChestSound;
    public bool chestOpen = false;
    private BoxCollider2D mCollider;
    private SpriteRenderer render;
    float interactDelay = 0;

    void Start()
    {
        mCollider = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>();
    }
    public int OnInteract()
    {
        if (Time.time < interactDelay)
            return -1;
        else
        {
            interactDelay = Time.time + 0.75f;
        }
        if (!chestOpen)
        {
            chestOpen = true;
            render.sprite = openedChest;
            source.PlayOneShot(openChestSound);
        }



        return 0;
    }
}
