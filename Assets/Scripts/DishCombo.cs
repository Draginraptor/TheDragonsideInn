using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishCombo : MonoBehaviour {
    
    public GameObject firstDish;
    public GameObject secondDish;
    public GameObject outputDish;

    public bool CanCombine(GameObject dishOne, GameObject dishTwo)
    {
        Dish one = dishOne.GetComponent<Dish>();
        Dish two = dishTwo.GetComponent<Dish>();
        Dish first = firstDish.GetComponent<Dish>();
        Dish second = secondDish.GetComponent<Dish>();

        // Check if the instance is a combo involving the 2 inputs
        if (one.DishName == first.DishName)
        {
            if(two.DishName == second.DishName)
            {
                return true;
            }
        }
        else if (one.DishName == second.DishName)
        {
            if(two.DishName == first.DishName)
            {
                return true;
            }
        }

        return false;
    }
}
