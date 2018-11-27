using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {

    LayerMask mLayerMask;
    bool mStarted;
	// Use this for initialization
	void Start () {

       mLayerMask = LayerMask.GetMask("Default");
        mStarted = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Attack(bool left)
    {
        Debug.Log("Attack Called");
        Vector2 topLeft = new Vector2(transform.position.x + 0.25f, transform.position.y + 1f);
        Vector2 bottomRight = new Vector2(transform.position.x + 1f, transform.position.y - 1f);
        Collider2D[] colliders = Physics2D.OverlapAreaAll(topLeft,bottomRight);

        foreach(Collider2D c in colliders)
        {
            Debug.Log("Collider: " + c.name);
            if (c.gameObject.tag.Equals("Entity"))
            {
                MonoBehaviour[] scripts = c.gameObject.GetComponents<MonoBehaviour>();
                foreach(MonoBehaviour mb in scripts)
                {
                    if(mb is AEntity)
                    {
                        ((AEntity)mb).Hurt(10);
                    }
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        Vector2 topLeft = new Vector2(transform.position.x + 0.25f, transform.position.y + 1f);
        Vector2 bottomRight = new Vector2(transform.position.x + 1f, transform.position.y - 1f);
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (mStarted)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            // Gizmos.drawLine(transform.position + new Vector3(0.5f, 0, 0), new Vector3(0.5f, 1f, 10f));

            Gizmos.DrawLine(topLeft, bottomRight);
    }
}
