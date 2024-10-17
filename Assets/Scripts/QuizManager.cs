using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    public GameObject coins; // Group of coins to display
    public TextMeshProUGUI introText; // Introductory text
    public TextMeshProUGUI taskText; // Task description text

    public int lvl;
    public GameObject answerPanel; // Panel containing answer options
    public Button option1;
    public Button option2;
    public Button option3;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI attemptsText; // Text to display number of attempts left
    public TextMeshProUGUI coinCountText; // Text to display total coins
    public int correctOption; // Correct answer option number

    public GameObject objectToDestroy; // Object that blocks the coin
    public ParticleSystem destructionEffect; // Particle effect to play on destruction
    public GameObject[] barriersToDestroy;

    public int maxAttempts = 3; // Maximum number of attempts allowed
    public float lockoutTime = 3600f; // Lockout time in seconds (e.g., 1 hour)
    public int countdownTime = 5; // Countdown time before the challenge starts

    private int currentAttempts = 0;
    private int totalCoins = 0; // Total coins collected by the player
    private bool hasAnsweredCorrectly = false; // To track if the correct answer was given
    private bool isLockedOut = false; // To track if the player is locked out
    private Button previousButton; // To track the previously clicked button

    private void Start()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinCountText.text = "Monedha: " + totalCoins + "$";
        // Set up initial scene
        coins.SetActive(false);
        introText.gameObject.SetActive(false);
        taskText.gameObject.SetActive(false);
        answerPanel.SetActive(false);

        // Set up button listeners
        option1.onClick.AddListener(() => CheckAnswer(option1, 1));
        option2.onClick.AddListener(() => CheckAnswer(option2, 2));
        option3.onClick.AddListener(() => CheckAnswer(option3, 3));

        // Start the initial sequence
        StartCoroutine(StartInitialSequence());
    }

    IEnumerator StartInitialSequence()
    {
        yield return new WaitForSeconds(2);
        introText.gameObject.SetActive(true);

        coins.SetActive(true);
        for (int i = countdownTime; i > 0; i--)
        {
            introText.text = "Keto jane monedhat!                               " +
                             "Sfida fillon n�: " + i + " sekonda";
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(2); // Wait for a few more seconds

        taskText.gameObject.SetActive(true);
        switch (lvl)
        {
            case 1:
                taskText.text = "Task: Gjej kodin i cili afishon vler�n e monedh�s m� t� madhe p�r t� liruar monedh�n.";
                break;
            case 4:
                taskText.text = "Task: Gjej kodin i cili afishon vler�n e monedh�s m� t� vogel p�r t� liruar monedh�n.";

                break;
            case 7:
                taskText.text = "Task: Gjej kodin q� printon numrat nga 1 n� 100. Por p�r numrat q� jan� shum�fish t� 3, printoni \"Fizz\" n� vend t� numrit, dhe p�r numrat q� jan� shum�fish t� 5, printoni \"Buzz\". P�r numrat q� jan� shum�fish t� t� dyve, printoni \"FizzBuzz\".p�r t� liruar monedhat.";

                break;

            default:
                Debug.LogWarning("No script defined for this level.");
                break;
        }
       

        yield return new WaitForSeconds(2); // Wait before showing answer panel

        introText.gameObject.SetActive(false);
        answerPanel.SetActive(true);
        UpdateAttemptsText();
    }

    void CheckAnswer(Button clickedButton, int option)
    {
        if (isLockedOut)
        {
            feedbackText.text = "Jeni bllokuar, provoni p�rs�ri m� von�.";
            return;
        }

        if (hasAnsweredCorrectly) return;

        if (previousButton != null && previousButton != clickedButton)
        {
            ResetButtonColor(previousButton);
        }

        if (option == correctOption)
        {
            switch (lvl)
            {
                case 1:
                    feedbackText.text = "P�rgjigjja e sakt�! Monedha me vleren me te madhe eshte liruar!";
                    break;
                case 4:
                    feedbackText.text = "P�rgjigjja e sakt�! Monedha me vleren me te vogel eshte liruar!";
                    break;
                case 7:
                    feedbackText.text = "P�rgjigjja e sakt�! Monedhat jane liruar!";
                    break;

                default:
                    Debug.LogWarning("No script defined for this level.");
                    break;
            }
            
            SetButtonColor(clickedButton, Color.green);
            StartCoroutine(TriggerGameEvent());
            hasAnsweredCorrectly = true;
        }
        else
        {
            feedbackText.text = "Gabim! Provoni p�rs�ri.";
            SetButtonColor(clickedButton, Color.red);
            currentAttempts++;
            UpdateAttemptsText();

            if (currentAttempts >= maxAttempts)
            {
                StartCoroutine(LockoutPlayer());
            }
        }

        previousButton = clickedButton;
    }

    void SetButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.pressedColor = color;
        cb.highlightedColor = color;
        button.colors = cb;
    }

    void ResetButtonColor(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.selectedColor = Color.white;
        cb.pressedColor = Color.white;
        cb.highlightedColor = Color.white;
        button.colors = cb;
    }

    IEnumerator TriggerGameEvent()
    {
        yield return new WaitForSeconds(3);
        answerPanel.SetActive(false);
        switch (lvl)
        {
            case 1:
                StartCoroutine(DestroyBarrier());
                break;
            case 4:
                StartCoroutine(DestroyBarrier());
                break;
            case 7:
                StartCoroutine(DestroyBarriers());
                break;

            default:
                Debug.LogWarning("No script defined for this level.");
                break;
        }
    }

    IEnumerator DestroyBarrier()
    {
        yield return new WaitForSeconds(2);
        if (objectToDestroy != null)
        {
            Instantiate(destructionEffect, objectToDestroy.transform.position, Quaternion.identity);
            destructionEffect.Play();
            Destroy(objectToDestroy);
            Debug.Log("Ngjarja e loj�s �sht� aktivizuar dhe objekti �sht� shkat�rruar!");
            introText.gameObject.SetActive(true);
            introText.text = "Mblidhni monedhen!";
            yield return new WaitForSeconds(1); // Wait for destruction effect
        }
    }
    IEnumerator DestroyBarriers()
    {
        yield return new WaitForSeconds(2);

        foreach (GameObject barrier in barriersToDestroy)
        {
            if (barrier != null)
            {
                // Krijo efektin e shkat�rrimit
                Instantiate(destructionEffect, barrier.transform.position, Quaternion.identity);
                destructionEffect.Play();
                // Shkat�rro barrier�n
                Destroy(barrier);
                introText.gameObject.SetActive(true);
                introText.text = "Mblidhni monedhat!";
            }
        }
    }

    public void CollectCoin(int coinValue)
    {
        totalCoins += coinValue;
        PlayerPrefs.SetInt("TotalCoins", totalCoins); // Save the total coins to PlayerPrefs
        PlayerPrefs.Save();
        UpdateCoinCountText();
        introText.text = "Monedha �sht� mbledhur! Totali i monedhave: " + totalCoins + "$";
    }

    IEnumerator LockoutPlayer()
    {
        isLockedOut = true;
        feedbackText.text = "Ju keni kaluar limitin e p�rpjekjeve. Provoni p�rs�ri pas disa or�sh.";
        yield return new WaitForSeconds(lockoutTime);
        isLockedOut = false;
        currentAttempts = 0;
        feedbackText.text = "";
        UpdateAttemptsText();
    }

    void UpdateAttemptsText()
    {
        attemptsText.text = "P�rpjekje t� mbetura: " + (maxAttempts - currentAttempts);
    }

    void UpdateCoinCountText()
    {
        coinCountText.text = "Monedha: " + totalCoins +"$";
    }

    public void BuyAttempt()
    {
        if (totalCoins >= 15)
        {
            totalCoins--;
            currentAttempts--;
            PlayerPrefs.SetInt("TotalCoins", totalCoins); // Save the total coins to PlayerPrefs
            PlayerPrefs.Save();
            UpdateCoinCountText();
            UpdateAttemptsText();
        }
        else
        {
            feedbackText.text = "Nuk keni mjaftuesh�m monedha p�r t� bler� nj� p�rpjekje.";
            StartCoroutine(DeactivateFeedText());

        }
    }

    public void BuyHint()
    {
        if (totalCoins >= 25)
        {
            totalCoins--;
            PlayerPrefs.SetInt("TotalCoins", totalCoins); // Save the total coins to PlayerPrefs
            PlayerPrefs.Save();
            UpdateCoinCountText();
            switch (lvl)
            {
                case 1:
                    feedbackText.text = "Opsioni i sakte eshte kodi me 2 kushte 'if'.";
                    break;
                case 4:
                    feedbackText.text = "Ne opsionin e sakte kushti 'if' ben vetem 1 krahasim.";
                    break;
                case 7:
                    feedbackText.text = "Ne opsionin e sakte kushti 'if' eshte me 'and' dhe numrat fillojne nga 1.";
                    break;

                default:
                    Debug.LogWarning("No script defined for this level.");
                    break;
            }
            StartCoroutine(DeactivateFeedText());

        }
        else
        {
            feedbackText.text = "Nuk keni mjaftuesh�m monedha p�r t� bler� nj� hint.";
            StartCoroutine(DeactivateFeedText());
        }
    }
    IEnumerator DeactivateFeedText()
    {
        
        yield return new WaitForSeconds(3);
        feedbackText.text = "";

    }
}
