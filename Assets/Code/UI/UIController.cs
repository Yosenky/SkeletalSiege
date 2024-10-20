using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance; // Singleton instance

    [SerializeField] private Text goldText; // Reference to the UI Text component

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
        if (goldText == null)
        {
            Debug.LogError("Gold Text component is not assigned!");
        }
        else
        {
            UpdateGoldUI(ResourceController.instance.GetGold()); // Initialize the UI with current gold amount
        }
    }

    public void UpdateGoldUI(int goldAmount)
    {
        goldText.text = "Gold: " + goldAmount.ToString();
    }
}
