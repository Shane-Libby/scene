using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public int[] sequence; //The sequence int array will hold our sequence order
    public int sequenceLength; //The sequenceLength is how long a sequence will be. Everytime the player completes a sequence it goes up one
    public int sequencesPlayedCom; //The sequencesPlayedCom is to measure how many times the computer has pressed the buttons in the current sequence
    public int sequencesPlayedPlayer; //The sequencesPlayedPlayer is to measure how many times the player has pressed the buttons in the current sequence

    //Buttons
    public GameObject greenButton; //This holds the GameObject of our green button, so that the script can send messages over to it
    public GameObject redButton; //This holds the GameObject of our red button, so that the script can send messages over to it
    public GameObject blueButton; //This holds the GameObject of our blue button, so that the script can send messages over to it
    public GameObject yellowButton; //This holds the GameObject of our yellow button, so that the script can send messages over to it

    //Sounds
    public AudioSource audioSource; //This is the source of which we will play the sequence fail and complete sounds through
    //public AudioClip sequenceCompleteSound;
    //public AudioClip sequenceFailSound;
    public AudioClip buttonClickSound;
   


    public GameObject youWinText;
    public GameObject youFailedText;

    public string nextLevel = "Level2";
    public int levelToUnlock = 2;

    public string levelTag;
    
    List<Vector3> buttonPositions = new List<Vector3>();


    void Start() //The Start function is called on the frame that the script becomes active
    {
        // save all of the current button positions
        buttonPositions.Add(greenButton.transform.position);
        buttonPositions.Add(redButton.transform.position);
        buttonPositions.Add(blueButton.transform.position);
        buttonPositions.Add(yellowButton.transform.position);

        audioSource = GameObject.Find("Camera").GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("Volume"); //We are setting our volume to the one that we set in the options menu
        sequence = new int[2]; //Here we are defining the sequence array's size
        StartCoroutine(GenerateNextSequence()); //We call the GenerateNextSequence function which randomly selects the next number for the sequence


    }
    void Update()
    {
        
    }


    IEnumerator GenerateNextSequence()
    {
            sequencesPlayedCom = 0; //Set the number of times that the computer has clicked a button to 0
            sequencesPlayedPlayer = 0; //Set the number of times that the player has clicked a button to 0

            {
                for (int i = 0; i < sequenceLength; i++)
                {
                    sequence[i] = Random.Range(1, 5);
                }
                yield return new WaitForSeconds(1);
                StartCoroutine(PlaySequence()); //Now that we have our random sequence, we can have the computer play it
            }
    }
    

    IEnumerator PlaySequence() //Plays the sequence
    {
        CanClickButton(false); //We call a function that disables the buttons while the computer is playing the sequence

        while (sequencesPlayedCom < sequenceLength)
        { //A while loop. Here we have it continuously do what is inside the brackets as long as sequencesPlayedCom < sequenceLength
            GameObject buttonToPress = null; //We set this temporary GameObject variable to hold the button so its ButtonPress function can be called

            if (sequence[sequencesPlayedCom] == 1)
            { //If the next element in the int array is 1 then it will be a green button
                buttonToPress = greenButton;
            }
            else if (sequence[sequencesPlayedCom] == 2)
            {
                buttonToPress = redButton;
            }
            else if (sequence[sequencesPlayedCom] == 3)
            {
                buttonToPress = blueButton;
            }
            else if (sequence[sequencesPlayedCom] == 4)
            {
                buttonToPress = yellowButton;
            }

            buttonToPress.SendMessage("ButtonPress"); //We call the ButtonPress function on the button so the player can see and hear it
            yield return new WaitForSeconds(0.7f); //Wait 0.7 seconds until we continue to the next colour in the sequence
            sequencesPlayedCom++; //Adding one to the sequencesPlayedCom so we know that computer just clicked a button
        }

        if (sequencesPlayedCom == sequenceLength)
        {
            CanClickButton(true); //After the computer has pressed all the buttons in the sequence, the player can now click on the buttons
        }
        
    }

    IEnumerator CheckSequenceButton(int id)
    {
        if (sequence[sequencesPlayedPlayer] == id)
        { //We check to see if the next color of the sequence is the one that the player pressed
            sequencesPlayedPlayer++;

        }
        else
        {
            youFailedText.SetActive(true);
            yield return new WaitForSeconds(0.8f); //A delay after the player fails 
            Application.LoadLevel(3); //Since the player we load up the menu

        }

        if (sequencesPlayedPlayer == sequenceLength)
        { //If the player has completed the sequence
            CanClickButton(true);
            yield return new WaitForSeconds(0.3f); //We have a small delay after the player completes the sequence
                                                   //audioSource.PlayOneShot(sequenceCompleteSound);
                                                   // sequenceLength++; //The sequence length is one added to it
                                                   //StartCoroutine(GenerateNextSequence());//We continue on with this loop, increasing the sequence one colour at a time

            if (sequenceLength == 2) // load the next level here...

            {
                CanClickButton(false);
                youWinText.SetActive(true);
                int currentUnlock = PlayerPrefs.GetInt("levelReached");

                if (currentUnlock < levelToUnlock) 

                PlayerPrefs.SetInt("levelReached", 2);
                PlayerPrefs.SetInt(levelTag, 1);
                yield return new WaitForSeconds(0.3f);
                Application.LoadLevel(0);
            }

            else
            {
                sequenceLength++; //The sequence length is one added to it

                // wait
                yield return new WaitForSeconds(0.5f);
                // shuffle a few times
                for (int i = 0; i < 5; ++i)
                {
                    ShuffleButtons();
                    yield return new WaitForSeconds(0.1f);
                }
                // wait
                yield return new WaitForSeconds(0.5f);

                StartCoroutine(GenerateNextSequence()); //We continue on with this loop, increasing the sequence one colour at a time
            }
        }
    }

    void ShuffleButtons()
    {
        // make a list of the button transforms
        List<Transform> buttons = new List<Transform>();
        buttons.Add(greenButton.transform);
        buttons.Add(redButton.transform);
        buttons.Add(blueButton.transform);
        buttons.Add(yellowButton.transform);

        // for each position
        foreach (var position in buttonPositions)
        {
            // select a random button
            Transform randomButton = buttons[Random.Range(0, buttons.Count)];
            // move the random button to that location
            randomButton.transform.position = position;
            // remove the button from the list
            buttons.Remove(randomButton);
        }
    }


    public void GoToMenu() //Function used to get back to the main menu	
    {
        audioSource.PlayOneShot(buttonClickSound);
        Application.LoadLevel(0);
    }


    void CanClickButton(bool i) //Tells the buttons if they can be clicked or not
    {
        greenButton.SendMessage("DisableOrEnableButton", i);
        redButton.SendMessage("DisableOrEnableButton", i);
        blueButton.SendMessage("DisableOrEnableButton", i);
        yellowButton.SendMessage("DisableOrEnableButton", i);
    }
}
