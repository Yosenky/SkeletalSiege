using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController instance; // Singleton instance

    private int gold;

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
        gold = 0; // Initialize gold
    }

    public int GetGold()
    {
        return gold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIController.instance.UpdateGoldUI(gold); // Notify UI to update
    }

    public void SpendGold(int amount)
    {
        if (amount <= gold)
        {
            gold -= amount;
            UIController.instance.UpdateGoldUI(gold); // Notify UI to update
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}
