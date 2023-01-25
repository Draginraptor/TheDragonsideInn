using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

    // Ref: https://unity3d.com/learn/tutorials/topics/2d-game-creation/scrolling-repeating-backgrounds

    public float scrollSpeed = 5.0f;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {

        rb2d = GetComponent<Rigidbody2D>();

        // Give the object a velocity
        rb2d.velocity = Vector2.left * scrollSpeed;
	}
	
	void FixedUpdate () {
		// When transform.position.x is less than -20, move it to the end of the line
        if(transform.position.x < -20f)
        {
            transform.position = new Vector3(28f, transform.position.y);
        }
	}
}
