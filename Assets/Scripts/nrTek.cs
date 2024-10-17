using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class nrTek : MonoBehaviour
{
    public GameObject coins; // Group of coins to display
    public TextMeshProUGUI introText; // Introductory text
    public GameObject answerPanel; // Panel containing answer options
    public float lockoutTime = 3600f; // Lockout time in seconds (e.g., 1 hour)
    public int countdownTime = 5; // Countdown time before the challenge starts

    private int currentAttempts = 0;
    private int totalCoins = 0; // Total coins collected by the player
    private bool isLockedOut = false; // To track if the player is locked out
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI coinCountText; // Text to display total coins
    public TextMeshProUGUI taskText; // Task description text
    public TextMeshProUGUI attemptsText; // Text to display number of attempts left
    public int maxAttempts = 3; // Maximum number of attempts allowed

    public GameObject[] barriersToDestroy;
    public ParticleSystem destructionEffect; // Efekti i shkatërrimit
    public TMP_InputField inputField;


    private void Start()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinCountText.text = "Monedha: " + totalCoins + "$";
        // Set up initial scene
        coins.SetActive(false);
        introText.gameObject.SetActive(false);
        taskText.gameObject.SetActive(false);
        answerPanel.SetActive(false);
        // Start the initial sequence
        StartCoroutine(StartInitialSequence());
    }
    public void Try()
    {
        currentAttempts++;
        if (currentAttempts >= maxAttempts)
        {
            StartCoroutine(LockoutPlayer());
        }
    }

    IEnumerator StartInitialSequence()
    {
        yield return new WaitForSeconds(2);
        introText.gameObject.SetActive(true);

        coins.SetActive(true);
        for (int i = countdownTime; i > 0; i--)
        {
            introText.text = "Keto jane monedhat!                               " +
                             "Sfida fillon në: " + i + " sekonda";
            yield return new WaitForSeconds(1);
        }

        taskText.gameObject.SetActive(true);
        taskText.text = "Task: Shkruaj kodin i cili afishon numrat tek nga 1 ne 7 per te liruar monedhat!";

        introText.gameObject.SetActive(false);
        answerPanel.SetActive(true);
        UpdateAttemptsText();

    }
    public void RemoveAttempt()
    {
        currentAttempts += 1;
        UpdateAttemptsText();
    }
    public void startEvent()
    {
        StartCoroutine(TriggerGameEvent());
    }
    IEnumerator TriggerGameEvent()
    {
        yield return new WaitForSeconds(3);
        answerPanel.SetActive(false);
        StartCoroutine(DestroyBarriers());
    }
    public void DestroyWithEffect()
    {
        
    }
    IEnumerator DestroyBarriers()
    {
        yield return new WaitForSeconds(2);

        foreach (GameObject barrier in barriersToDestroy)
        {
            if (barrier != null)
            {
                // Krijo efektin e shkatërrimit
                Instantiate(destructionEffect, barrier.transform.position, Quaternion.identity);
                destructionEffect.Play();
                // Shkatërro barrierën
                Destroy(barrier);
                introText.gameObject.SetActive(true);
                introText.text = "Mblidhni monedhat!";
            }
        }
    }

    IEnumerator LockoutPlayer()
    {
        isLockedOut = true;
        infoText.text = "Ju keni kaluar limitin e përpjekjeve. Provoni përsëri pas disa orësh.";
        yield return new WaitForSeconds(lockoutTime);
        isLockedOut = false;
        currentAttempts = 0;
        feedbackText.text = "";
        UpdateAttemptsText();
    }

    void UpdateAttemptsText()
    {
        if (maxAttempts - currentAttempts > 0)
        {
            attemptsText.text = "Përpjekje të mbetura: " + (maxAttempts - currentAttempts);
        }
    }

    void UpdateCoinCountText()
    {
        coinCountText.text = "Monedha: " + totalCoins + "$";
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
            infoText.text = "Nuk keni mjaftueshëm monedha për të blerë një përpjekje.";

        }
    }

    public void BuyHint()
    {
        if (totalCoins >= 25)
        {
            totalCoins--;
            PlayerPrefs.SetInt("TotalCoins", totalCoins); // Save the total coins to PlayerPrefs
            PlayerPrefs.Save();
            inputField.text = "#include <iostream>\r\nusing namespace std;\r\n\r\nint main() {\r\n       for (int i = 1; i <=  ; i +=  ) {\r\n        std::cout << i ;\r\n    }\r\n\r\n    return 0;\r\n}\r\n";
            infoText.text = "Ploteso vetem i<= ? dhe i+= ?";

        }
        else
        {
            infoText.text = "Nuk keni mjaftueshëm monedha për të blerë një hint.";
        }
    }
}
