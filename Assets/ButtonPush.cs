using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPush : MonoBehaviour {

    public SpriteRenderer sr; //This variable will hold out sprite renderer, so we can access it and change the sprite
    public Sprite defaultSprite; //This is our default non pushed button sprite
    public Sprite pressedSprite; //This is our pressed button sprite
    public AudioSource audioSource; //This is the source of which we will play the button sound through
    public AudioClip pressSound; //This is the sound that will play when the button is pressed
    public Test test; //This holds the GameObject of the GameManager so we can send messages more efficiently
    public int id; //The id is the number allocated to the button ranging from 1 to 4 (green = 1, red = 2, blue = 3, yellow = 4)
    public bool canClick; //The canClick bool tells us if we can click on the button (true) or if we can't (false)

    void Awake() //The Awake function is called when the script instance is being loaded
    {
        sr = transform.GetComponent<SpriteRenderer>(); //Here we are getting our SpriteRenderer component in the transform and assigning it to the variable 'sr'
        audioSource = GameObject.Find("Camera").GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("Volume"); //We are setting our volume to the one that we set in the options menu 
    }

    void OnMouseDown() //This function is called whenever you push down on the left mouse button, inside the objects collider
    {
        if (canClick)
        { //The code inside the brackets can only be done if canClick is equal to true you could also write it if(canClick == true)
            
            sr.sprite = pressedSprite; //Here we are setting our transform's sprite to the pushed sprite
            audioSource.PlayOneShot(pressSound); //When we press down on the mouse button the 'pressSound' clip will be played
            test.StartCoroutine("CheckSequenceButton", id); //When we click the button we want to check if it is the correct one. So we send over our id over to the GameManager to see if it is.
            transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f); //When we click on the button we reduce the size just a bit to make it loom like the button is being pushed down
        }
    }

    void OnMouseUp() //This function is called whenever you let go of the left mouse button, while your cursor is inside the collider
    {
        if (canClick)
        {
            sr.sprite = defaultSprite; //Here we are setting our transform's sprite back to its original non pushed sprite
            transform.localScale = new Vector3(0.75f, 0.75f, 0.75f); //When we let go of the button it returns to its original scale
        }
    }

    public IEnumerator ButtonPress() //This is the function that GameLogic calls to press the button and play an audio clip
    {
        sr.sprite = pressedSprite;
        audioSource.PlayOneShot(pressSound);
        transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
        yield return new WaitForSeconds(0.4f); //To make it look like an authentic click, the function waits for 0.4 seconds, then continues with the rest of the code
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        sr.sprite = defaultSprite;
    }

    public void DisableOrEnableButton(bool i) //The message that GameManager sends, telling the button if it can be clicked or not
    {
        canClick = i;
    }
}