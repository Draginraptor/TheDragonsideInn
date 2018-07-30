using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float maxSpeed = 5f;
    public float moveForce = 10f;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    // Occurs every physics step
    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // If speed is less than maxSpeed, allow the addition of a force
        if (h * rb2d.velocity.x < maxSpeed)
        {
            rb2d.AddForce(Vector2.right * h * moveForce);
        }

        // If speed is less than maxSpeed, allow the addition of a force
        if (v * rb2d.velocity.y < maxSpeed)
        {
            rb2d.AddForce(Vector2.up * v * moveForce);
        }

        // Clamp speeds to be at maxSpeed
        Mathf.Clamp(rb2d.velocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(rb2d.velocity.y, -maxSpeed, maxSpeed);


    }
}
