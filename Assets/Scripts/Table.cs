using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour {
    
    // Various indicators found on Table
    public GameObject cloud;
    public GameObject statusDisplay;
    public GameObject patienceDisplay;

    // With 0 being half heart, and 9 being five hearts
    public Sprite[] patienceLevels;
    public Sprite menu;

    // Various GameObjects to be manipulated to show diff statuses
    public GameObject customerLeft, customerRight, menuLeft, menuRight, dishLeft, dishRight;
    public Sprite[] dragons;

    // All SFX used by Table, Instantiate at appropriate time then left to self-destruct
    public GameObject customerSFX, menuSFX, dishSFX, paySFX, leaveSFX;

    // Properties that other classes need to check
    [HideInInspector]
    public bool isOccupied = false;
    [HideInInspector]
    public bool hasMenu = false;
    [HideInInspector]
    public Dish order = null;

    // Maxed at 10
    private int currentPatience = 10;

    // Retrieved from GM
    private float patienceCountdownDelay;
    private float menuReadTime;
    private float eatingTime;
    private GameObject[] dishes;

    // For ease of changing sprites
    private SpriteRenderer statusRenderer;
    private SpriteRenderer patienceRenderer;

    private void Awake()
    {
        statusRenderer = statusDisplay.GetComponent<SpriteRenderer>();
        patienceRenderer = patienceDisplay.GetComponent<SpriteRenderer>();

        // Initialise everything
        ResetTable();
    }

    private void Start()
    {
        // Retrieve info from GM instance
        patienceCountdownDelay = GM.instance.patienceCountdownDelay;
        menuReadTime = GM.instance.menuReadTime;
        eatingTime = GM.instance.eatingTime;
        dishes = GM.instance.dishes;
    }

    // First state: menu status, full patience, customers spawn
    public void SpawnCustomer()
    {
        // Display random customers
        customerLeft.GetComponent<SpriteRenderer>().sprite = RandomDragon();
        customerRight.GetComponent<SpriteRenderer>().sprite = RandomDragon();
        customerLeft.SetActive(true);
        customerRight.SetActive(true);

        isOccupied = true;

        // Set status and activate displays
        cloud.SetActive(true);
        statusDisplay.GetComponent<SpriteRenderer>().sprite = menu;
        patienceDisplay.SetActive(true);

        // Play SFX
        Instantiate(customerSFX);
        StartCoroutine("PatienceCountdown");
    }

    public void GiveMenu()
    {
        hasMenu = true;

        // Increase patience
        currentPatience += 2;
        currentPatience = Mathf.Clamp(currentPatience, 0, 10);
        UpdatePatienceDisplay();

        // Hide cloud temp
        cloud.SetActive(false);

        // Display menus on table
        menuLeft.SetActive(true);
        menuRight.SetActive(true);

        // Play SFX
        Instantiate(menuSFX);

        StopCoroutine("PatienceCountdown");
        StartCoroutine("ReadMenu");
    }

    // Delays order making
    IEnumerator ReadMenu()
    {
        yield return new WaitForSeconds(menuReadTime);
        
        // Fill in an order after the wait time
        int num = Random.Range(0, dishes.Length);
        order = dishes[num].GetComponent<Dish>();

        // Show cloud again
        cloud.SetActive(true);

        menuLeft.SetActive(false);
        menuRight.SetActive(false);

        // Display dish requested
        statusRenderer.sprite = dishes[num].GetComponent<SpriteRenderer>().sprite;

        StartCoroutine("PatienceCountdown");
    }

    // Countdown till customer leaves
    IEnumerator PatienceCountdown()
    {
        yield return new WaitForSeconds(patienceCountdownDelay);
        while (currentPatience > 0)
        {
            yield return new WaitForSeconds(patienceCountdownDelay);
            currentPatience -= 1;
            
            // Prevent from going to UpdatePatienceDisplay as it is a out of bounds error (array[-1])
            if (currentPatience == 0) break;
            UpdatePatienceDisplay();
        }
        Instantiate(leaveSFX);

        // If currentPatience is not kept above 0, the customers leave, and the player loses points
        GM.instance.ChangeScore(-50);

        // Note it down in GM
        GM.instance.lostCustomers++;

        ResetTable();
    }

    public bool CheckOrder(GameObject input)
    {
        if (input != null)
        {
            Dish inputDish = input.GetComponent<Dish>();

            // On success
            if (inputDish.DishName == order.DishName)
            {
                // Display dish
                dishLeft.GetComponent<SpriteRenderer>().sprite = input.GetComponent<SpriteRenderer>().sprite;
                dishRight.GetComponent<SpriteRenderer>().sprite = input.GetComponent<SpriteRenderer>().sprite;
                dishLeft.SetActive(true);
                dishRight.SetActive(true);
                cloud.SetActive(false);

                // Play SFX
                Instantiate(dishSFX);

                // No more patience loss
                StopCoroutine("PatienceCountdown");
                StartCoroutine("EatDish");

                // Destroy the dish that was used to serve
                Destroy(input);

                return true;
            }
        }

        // On fail, deduct score
        GM.instance.ChangeScore(-10);

        return false;
    }

    // Delay the customer's exit
    IEnumerator EatDish()
    {
        yield return new WaitForSeconds(eatingTime);

        // Play SFX
        Instantiate(paySFX);

        // Give score and reset table
        // When patience is 10, player gains 100 points
        // When patience is 1, player gains 10 points
        GM.instance.ChangeScore(100 * currentPatience / 10);
        ResetTable();
    }

    void ResetTable()
    {
        StopAllCoroutines();
        // Hide everything
        cloud.SetActive(false);
        customerLeft.SetActive(false);
        customerRight.SetActive(false);
        menuLeft.SetActive(false);
        menuRight.SetActive(false);
        dishLeft.SetActive(false);
        dishRight.SetActive(false);
        patienceDisplay.SetActive(false);
        statusDisplay.GetComponent<SpriteRenderer>().sprite = null;

        // Max out patience again and update sprite
        currentPatience = 10;
        UpdatePatienceDisplay();

        // Reset properties
        isOccupied = false;
        hasMenu = false;
        order = null;
    }

    // Changes sprite only; active state handled separately
    void UpdatePatienceDisplay()
    {   
        patienceRenderer.sprite = patienceLevels[currentPatience - 1];
    }

    // Retrieves random dragon sprite
    Sprite RandomDragon()
    {
        int num = Random.Range(0, dragons.Length);
        return dragons[num];
    }

}
