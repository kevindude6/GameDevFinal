using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour, IInteractable {

    public Sprite openDoor;
    public Sprite closeDoor;
    public AudioSource source;
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;
    public bool doorOpen = false;
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
        if (!doorOpen)
        {
            doorOpen = true;
            render.sprite = null;
            mCollider.enabled = false;
            source.PlayOneShot(openDoorSound);
        }
        else
        {
            doorOpen = false;
            render.sprite = closeDoor;
            mCollider.enabled = true;
            source.PlayOneShot(closeDoorSound);
        }

        
        return 0;
    }

     /*
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (doorOpen)
        {
            tileData.sprite = openDoor;
            tileData.colliderType = ColliderType.None;
        }
        else
        {
            tileData.sprite = closeDoor;
            tileData.colliderType = ColliderType.Sprite;
        }
    }
    */
}
