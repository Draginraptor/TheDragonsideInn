using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour {

    // Useful for short SFX
    // Instantiate, causing the AudioSource to play on Awake
    // Then the Invoked function will cause the GameObject to self-destruct

    public float delay = 3f;
    
	void Start () {
        Invoke("DestroySelf", delay);
	}
	
	void DestroySelf()
    {
        Destroy(gameObject);
    }
}
