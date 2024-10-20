using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance; // Singleton instance

    [SerializeField] private TMP_Text player1GoldText;
    [SerializeField] private TMP_Text player2GoldText;
    [SerializeField] private GameObject player1Panel;
    [SerializeField] private GameObject player2Panel;

    private int currentPlayer = 1; // Use int instead of enum

    void Awake()
    {
        // Ensure that there is only one instance of UIController
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (player1GoldText == null || player2GoldText == null || player1Panel == null || player2Panel == null)
        {
            Debug.LogError("Gold Text components or Panels are not assigned!");
        }
        else
        {
            UpdateGoldUI(1, ResourceController.instance.GetGold(1));
            UpdateGoldUI(2, ResourceController.instance.GetGold(2));
            UpdateActivePanel();
        }
    }

    public void UpdateGoldUI(int player, int goldAmount)
    {
        switch (player)
        {
            case 1:
                if (player1GoldText != null)
                {
                    player1GoldText.text = "Gold: " + goldAmount;
                }
                break;
            case 2:
                if (player2GoldText != null)
                {
                    player2GoldText.text = "Gold: " + goldAmount;
                }
                break;
        }
    }

    public void SwitchPlayer()
    {
        // Switch the current player
        currentPlayer = currentPlayer == 1 ? 2 : 1;

        // Notify the CameraController to switch the player view
        RTSGameController.instance.team = currentPlayer;

        // Update the visible gold UI for the current player
        UpdateGoldUI(currentPlayer, ResourceController.instance.GetGold(currentPlayer));

        // Update the active panel based on the switched player
        UpdateActivePanel();
    }

    private void UpdateActivePanel()
    {
        if (currentPlayer == 1)
        {
            player1Panel.SetActive(true);
            player2Panel.SetActive(false);
        }
        else if (currentPlayer == 2)
        {
            player1Panel.SetActive(false);
            player2Panel.SetActive(true);
        }
    }
}