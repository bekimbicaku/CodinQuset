using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lvl6 : MonoBehaviour
{
    public GameObject coins; // Group of coins to display
    public TextMeshProUGUI introText; // Introductory text
    public GameObject answerPanel; // Panel containing answer options
    public GameObject objectToDestroy; // Object that blocks the coin
    public ParticleSystem destructionEffect; // Particle effect to play on destruction
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
        taskText.text = "Task: Shkruaj një program në Java që llogarit faktorialin e numrit 5. Nese outputi ka vleren e monedhes monedha do te lirohet";

        introText.gameObject.SetActive(false);
        answerPanel.SetActive(true);
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
        StartCoroutine(DestroyBarrier());
    }

    IEnumerator DestroyBarrier()
    {
        yield return new WaitForSeconds(2);
        if (objectToDestroy != null)
        {
            Instantiate(destructionEffect, objectToDestroy.transform.position, Quaternion.identity);
            destructionEffect.Play();
            Destroy(objectToDestroy);
            Debug.Log("Ngjarja e lojës është aktivizuar dhe objekti është shkatërruar!");
            introText.gameObject.SetActive(true);
            introText.text = "Mblidhni monedhen!";
            yield return new WaitForSeconds(1); // Wait for destruction effect
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
        attemptsText.text = "Përpjekje të mbetura: " + (maxAttempts - currentAttempts);
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
            inputField.text = "import java.math.BigInteger;\r\n\r\npublic class Main {\r\n    public static void main(String[] args) {\r\n        BigInteger factorial = calculateFactorial(5);        \r\n System.out.println(\" factorial);\r\n    }\r\n\r\n    private static BigInteger calculateFactorial(int number) {\r\n        BigInteger result = BigInteger.ONE;\r\n        for (int i = 1; i <= number; i++) {\r\n            result =   \r\n        }\r\n        return result;\r\n    }\r\n}\r\n";
            infoText.text = "Jepi vleren e duhur variablit 'result'";
        }
        else
        {
            infoText.text = "Nuk keni mjaftueshëm monedha për të blerë një hint.";
        }
    }
}
