using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour {

    [HideInInspector]
    public static GM instance = null;
    
    // Used when a game is ongoing
    public Text timerText;
    public Text scoreText;

    // When timer reaches zero
    public GameObject levelComplete;
    public Text completeScore;
    public Text customersLost;
    public Text finalScore;

    // Various inspector exposed variables
    public int noCustomerLossBonus = 500;
    public int timeLimitInSec = 120;
    public float customerSpawnDelay = 20f;
    public float patienceCountdownDelay = 30f;
    public float menuReadTime = 10f;
    public float eatingTime = 3f;

    // Tables in the scene that can be used
    public GameObject[] tables;
    // Dishes available on the level
    public GameObject[] dishes;
    // Combos avaialble on the level
    public GameObject[] dishCombos;

    // Accessed by Table
    [HideInInspector]
    public int lostCustomers = 0;

    private int score = 0;
    private int timeLeft;

    private void Awake()
    {
        // Set the GM instance and prevent any others
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        // Initialise timer text
        timeLeft = timeLimitInSec;
        timerText.text = "Time: " + SecToMinAndSec(timeLeft);

        // Initialise score text
        ChangeScore(0);
        
        StartCoroutine("SpawnCustomer");
        StartCoroutine("TimerTick");
	}

    // A coroutine that randomly selects a table, and if empty, spawns a customer
    IEnumerator SpawnCustomer()
    {
        while (true)
        {
            int num = Random.Range(0, tables.Length);

            // Get the Table at num position
            Table table = tables[num].GetComponent<Table>();

            if (!table.isOccupied)
            {
                table.SpawnCustomer();
            }

            yield return new WaitForSeconds(customerSpawnDelay);
        }
    }

    // Updates score and score text
    public void ChangeScore(int scoreDelta)
    {
        score += scoreDelta;
        scoreText.text = "Score: " + score;
    }
    
    IEnumerator TimerTick()
    {
        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
            timerText.text = "Time: " + SecToMinAndSec(timeLeft);
        }

        // After time runs out
        timerText.text = "Time: 00:00";

        // Stop everything
        Time.timeScale = 0;

        // Show results
        levelComplete.SetActive(true);
        completeScore.text = "Score: " + score;
        if(lostCustomers == 0)
        {
            customersLost.text = "No Customers Lost: " + noCustomerLossBonus;
            score += noCustomerLossBonus;
        }
        else if(lostCustomers == 1)
        {
            customersLost.text = lostCustomers + " Customer Lost";
        }
        else
        {
            customersLost.text = lostCustomers + " Customers Lost";
        }
        finalScore.text = "Final Score: " + score;
    }

    string SecToMinAndSec(int seconds)
    {
        int sec = seconds % 60;
        int min = (seconds - sec) / 60;
        if(sec < 10)
        {
            return min + ":0" + sec;
        }
        else
        {
            return min + ":" + sec;
        }
    }
}
