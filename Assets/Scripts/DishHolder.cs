using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishHolder : MonoBehaviour {
    
    private GameObject dish;
    
    // Public accessor with a custom set function
    public GameObject Dish
    {
        get
        {
            // Standard
            return this.dish;
        }
        set
        {
            // When this value is set (using =), this code runs

            this.dish = value;

            // For the purpose of moving the dish to its particular holder
            if(dish != null)
            {
                // Makes the dish visible
                this.dish.SetActive(true);

                // Sets the parent to be the holder
                this.dish.transform.parent = gameObject.transform;
                
                // Position at holder
                this.dish.transform.position = transform.position;
            }
        }
    }
}
