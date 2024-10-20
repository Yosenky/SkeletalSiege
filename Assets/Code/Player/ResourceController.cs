using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController instance; // Singleton instance

    private Dictionary<int, int> playerGold = new Dictionary<int, int>();

    void Awake()
    {
        // Ensure that there is only one instance of ResourceController
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize gold for each player
        playerGold[1] = 100; // Player 1
        playerGold[2] = 100; // Player 2
    }

    public int GetGold(int player)
    {
        if (playerGold.ContainsKey(player))
        {
            return playerGold[player];
        }
        return 0;
    }

    public void AddGold(int player, int amount)
    {
        if (playerGold.ContainsKey(player))
        {
            playerGold[player] += amount;
            UIController.instance.UpdateGoldUI(player, playerGold[player]); // Notify UI to update
        }
    }

    public void SpendGold(int player, int amount)
    {
        if (playerGold.ContainsKey(player) && playerGold[player] >= amount)
        {
            playerGold[player] -= amount;
            UIController.instance.UpdateGoldUI(player, playerGold[player]); // Notify UI to update
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}