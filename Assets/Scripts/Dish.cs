using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour {

    public string DishName;
    public GameObject fireOutput, waterOutput, windOutput, earthOutput = null;

    public static GameObject ConvertDish(GameObject input, GameObject output)
    {
        // If no actual output is given, return the original dish
        if(output == null)
        {
            return input;
        }

        // Create an instance of output where the input is, parent to world
        GameObject instance = Instantiate(output, input.transform.position, Quaternion.identity, null);

        // Destroy the input
        Destroy(input);

        // Return the created output
        return instance;
    }
}
