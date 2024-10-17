using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int totalCoins; // Total coins collected by the player
    public TextMeshProUGUI coinCountText; // Text to display total coins
    public TextMeshProUGUI introText; // Introductory text

    private void Update()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinCountText.text = "Monedha: " + totalCoins + "$";
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Coin"))
        {
            Coin coinValue = other.GetComponent<Coin>();
            if (coinValue != null)
            {
                CollectCoin(coinValue.coinValue);
                Destroy(other.gameObject); // Destroy the coin
            }            
            introText.text = "Monedha është mbledhur! Totali i monedhave: " + totalCoins + "$";

        }
    }
    private void CollectCoin(int value)
    {
        totalCoins += value;
        PlayerPrefs.SetInt("TotalCoins", totalCoins); // Save the total coins to PlayerPrefs
        PlayerPrefs.Save();
        UpdateCoinCountText();
    }
    void UpdateCoinCountText()
    {
        coinCountText.text = "Monedha: " + totalCoins + "$";
    }
}
