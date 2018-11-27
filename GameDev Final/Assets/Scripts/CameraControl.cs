using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void LateUpdate()
    {
        Vector3 targetPos = target.transform.position + new Vector3(0,0,-10);
        this.transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10);
    }
}
