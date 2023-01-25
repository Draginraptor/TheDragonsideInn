using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

    public float maxSpeed = 5f;
    public float moveForce = 10f;
    public Transform frontCheck;

    // Display for dishes being held
    public Image rightImage;
    public GameObject rightSelector;
    public Image leftImage;
    public GameObject leftSelector;

    // Dish display images
    public Image baseDishDisplay;
    public Image fireDishDisplay;
    public Image waterDishDisplay;
    public Image windDishDisplay;
    public Image earthDishDisplay;

    public GameObject fireParticles, waterParticles, windParticles, earthParticles;

    private Rigidbody2D rb2d;
    // To cue which sprites are to be used and to rotate the player
    private Animator anim;
    
    // Track what dishes are being held
    private GameObject rightHand;
    private GameObject leftHand;
    
    // Track which hand is active
    private bool isRightActive;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        rightSelector.SetActive(true);
        isRightActive = true;
	}
	
	// Update is called once per frame
	void Update () {
        // Set anim values for orientation of player
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        anim.SetFloat("h", h);
        anim.SetFloat("v", v);

        if (Input.GetButtonDown("Interact"))
        {
            if (isRightActive)
            {
                CheckFront(ref rightHand, rightImage);
                UpdateElementDisplay(ref rightHand);
            }
            else
            {
                CheckFront(ref leftHand, leftImage);
                UpdateElementDisplay(ref leftHand);
            }
        }
        
        // Swap active hand
        if (Input.GetButtonDown("Swap"))
        {
            isRightActive = !isRightActive;
            if (isRightActive)
            {
                rightSelector.SetActive(true);
                leftSelector.SetActive(false);
                UpdateElementDisplay(ref rightHand);
            }
            else
            {
                leftSelector.SetActive(true);
                rightSelector.SetActive(false);
                UpdateElementDisplay(ref leftHand);
            }
        }

        // Combine dishes
        if (Input.GetButtonDown("Combine"))
        {
            if(rightHand != null && leftHand != null)
            {
                // Look through all the combos
                foreach (GameObject combo in GM.instance.dishCombos)
                {
                    DishCombo temp = combo.GetComponent<DishCombo>();
                    if (temp.CanCombine(rightHand, leftHand))
                    {
                        if (isRightActive)
                        {
                            SetHand(ref rightHand, Dish.ConvertDish(rightHand, temp.outputDish), rightImage);
                            UpdateElementDisplay(ref rightHand);
                            SetHand(ref leftHand, null, leftImage);
                        }
                        else
                        {
                            SetHand(ref leftHand, Dish.ConvertDish(leftHand, temp.outputDish), leftImage);
                            UpdateElementDisplay(ref leftHand);
                            SetHand(ref rightHand, null, rightImage);
                        }
                    }
                }
            }
            
        }

        // Check for the 4 element inputs
        CheckElement(Input.GetButtonDown("UseFire"), "fire");
        CheckElement(Input.GetButtonDown("UseWater"), "water");
        CheckElement(Input.GetButtonDown("UseWind"), "wind");
        CheckElement(Input.GetButtonDown("UseEarth"), "earth");

        // Discard a dish
        if (Input.GetButtonDown("Discard"))
        {
            if (isRightActive)
            {
                Destroy(rightHand);
                SetHand(ref rightHand, null, rightImage);
                UpdateElementDisplay(ref rightHand);
            }
            else
            {
                Destroy(leftHand);
                SetHand(ref leftHand, null, leftImage);
                UpdateElementDisplay(ref leftHand);
            }
        }
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

    // Check what the player is facing and take action accordingly
    void CheckFront(ref GameObject activeHand, Image image)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, frontCheck.position, 1 << LayerMask.NameToLayer("Interactables"));
        // Only continues if anything was hit
        if (hit)
        {
            // GameObject hit by the linecast
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Spawner"))
            {
                // Needs an empty hand to be able to pick up a dish
                if (activeHand == null)
                {
                    DishSpawner spawner = hitObject.GetComponent<DishSpawner>();
                    // Create a copy of the dish held in the spawner
                    SetHand(ref activeHand, Instantiate(spawner.dish, transform, true), image);
                }

            }
            else if (hitObject.CompareTag("Holder"))
            {
                DishHolder holder = hitObject.GetComponent<DishHolder>();

                // Action will only occur when one of the 2 is empty, and the other filled, but not when both are empty/filled
                if(activeHand != null && holder.Dish == null)
                {
                    // Place dish into holder
                    holder.Dish = activeHand;
                    SetHand(ref activeHand, null, image);
                }
                else if(activeHand == null && holder.Dish != null)
                {
                    // Retrieve dish from holder
                    SetHand(ref activeHand, holder.Dish, image);
                    holder.Dish = null;
                }
            }
            else if (hitObject.CompareTag("Table"))
            {
                Table table = hitObject.GetComponent<Table>();

                if (table.isOccupied)
                {
                    if (!table.hasMenu)
                    {
                        table.GiveMenu();
                    }
                    else if (table.order != null)
                    {
                        if (table.CheckOrder(activeHand))
                        {
                            SetHand(ref activeHand, null, (isRightActive? rightImage:leftImage));
                        }
                    }
                }
            }
        }
    }

    // For when dishes are being placed in the hand
    // Note: Since the dish is inactive while in the hand, there is no need to manage its position here at the moment
    void SetHand(ref GameObject hand, GameObject dish, Image image)
    {
        if(dish != null)
        {
            // Make dish invisible (but dish remains in its previous position)
            dish.SetActive(false);

            // Update the image on UI to show that a dish has been picked up
            SetImage(image, dish.GetComponent<SpriteRenderer>().sprite);
        }
        else
        {
            // Update image to show that hand is empty
            SetImage(image, null);
        }

        // Assign the dish to the hand
        hand = dish;
    }

    void SetImage(Image image, Sprite sprite)
    {
        if (sprite == null)
        {
            image.sprite = sprite;

            // Make invisible
            image.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            image.sprite = sprite;
            image.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // Check element inputs
    void CheckElement(bool buttonCheck, string element)
    {
        if (buttonCheck)
        {
            if (isRightActive)
            {
                UseElement(ref rightHand, element, rightImage);
            }
            else
            {
                UseElement(ref leftHand, element, leftImage);
            }
        }
    }
    
    void UseElement(ref GameObject hand, string element, Image image)
    {
        // If there is nothing in hand, return
        if(hand == null)
        {
            return;
        }

        Dish currDish = hand.GetComponent<Dish>();

        // Convert dish to corresponding output given on the Dish
        // Emit particles
        // Particles will self-destroy, as set in the Inspector
        switch (element)
        {
            // If not any of the given elements, return
            default:
                return;

            case "fire":
                SetHand(ref hand, Dish.ConvertDish(hand, currDish.fireOutput), image);
                Instantiate(fireParticles, transform, false);
                break;

            case "water":
                SetHand(ref hand, Dish.ConvertDish(hand, currDish.waterOutput), image);
                Instantiate(waterParticles, transform, false);
                break;

            case "wind":
                SetHand(ref hand, Dish.ConvertDish(hand, currDish.windOutput), image);
                Instantiate(windParticles, transform, false);
                break;

            case "earth":
                SetHand(ref hand, Dish.ConvertDish(hand, currDish.earthOutput), image);
                Instantiate(earthParticles, transform, false);
                break;
        }

        UpdateElementDisplay(ref hand);
    }

    void UpdateElementDisplay(ref GameObject hand)
    {
        if (hand != null)
        {
            // Retrieve dishes
            Dish baseDish = hand.GetComponent<Dish>();
            Sprite fireDish = (baseDish.fireOutput != null ? baseDish.fireOutput.GetComponent<SpriteRenderer>().sprite : null);
            Sprite waterDish = (baseDish.waterOutput != null ? baseDish.waterOutput.GetComponent<SpriteRenderer>().sprite : null);
            Sprite windDish = (baseDish.windOutput != null ? baseDish.windOutput.GetComponent<SpriteRenderer>().sprite : null);
            Sprite earthDish = (baseDish.earthOutput != null ? baseDish.earthOutput.GetComponent<SpriteRenderer>().sprite : null);

            // Update images
            SetImage(baseDishDisplay, baseDish.GetComponent<SpriteRenderer>().sprite);
            SetImage(fireDishDisplay, fireDish);
            SetImage(waterDishDisplay, waterDish);
            SetImage(windDishDisplay, windDish);
            SetImage(earthDishDisplay, earthDish);
        }
        else
        {
            SetImage(baseDishDisplay, null);
            SetImage(fireDishDisplay, null);
            SetImage(waterDishDisplay, null);
            SetImage(windDishDisplay, null);
            SetImage(earthDishDisplay, null);
        }
    }
}
