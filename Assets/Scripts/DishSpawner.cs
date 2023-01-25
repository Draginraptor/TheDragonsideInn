using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishSpawner : MonoBehaviour {

    public GameObject dish;

	// Use this for initialization
	void Start () {
        // Create an instance of the dish at the GameObject for display
        Instantiate(dish, transform, false);
	}
}
