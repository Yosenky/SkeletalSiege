using UnityEngine;
using TMPro; // If you want to update UI with health

public class BaseScript : MonoBehaviour
{
    public int startingHealth = 500; // Initial health of the base
    private int currentHealth;

    [SerializeField] private TMP_Text healthText; // Reference to the TMP_Text component for displaying health

    void Start()
    {
        currentHealth = startingHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyBase();
        }

        UpdateHealthUI();
    }

    private void DestroyBase()
    {
        // Handle base destruction, e.g., disable the GameObject, play an animation, etc.
        Debug.Log(gameObject.name + " has been destroyed!");
        Destroy(gameObject); // Destroy the base GameObject
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString();
        }
    }
}