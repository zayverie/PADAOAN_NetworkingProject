using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText; // Reference to the TextMeshPro component for displaying damage amount
    [SerializeField] private float floatUpSpeed = 1f; // Speed at which the popup floats upwards
    // [SerializeField] private float fadeOutSpeed = 1f; // Speed at which the popup fades out
    [SerializeField] private float lifetime = 1f; // Total lifetime of the popup before it is destroyed
    [SerializeField] private Color textColor; // Color of the damage text, this can be set in the Unity editor to customize the appearance of the damage popup
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void Awake(){
        textColor = damageText.color; // Store the initial color of the text, this is used to maintain the original color while applying fading effects
    }
    public void Setup(int damageAmount) // Method to set up the damage popup with the specified damage amount, this is called when the player takes damage and allows the popup to display the correct damage value
    {
        damageText.text = damageAmount.ToString(); // Set the text of the damage popup to the damage amount, this converts the integer damage amount to a string for display
        Debug.Log("Damage Popup Setup: " + damageAmount); // Log the damage amount for debugging purposes, this helps verify that the correct damage value is being passed to the popup
    }
    private void Update()
    {
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime; // Move the popup upwards over time, this creates a floating effect as the popup rises above the player when damage is taken
        textColor.a -= Time.deltaTime / lifetime; // Decrease the alpha channel of the text color over time, creating a fading effect
        damageText.color = textColor; // Apply the updated color to the text component

        if(textColor.a <= 0) // If the alpha channel of the text color is fully transparent, we destroy the popup, this ensures that the popup is removed from the scene after it has faded out completely
        {
            Destroy(gameObject); // Destroy the damage popup GameObject, this removes it from the scene and frees up resources after it has served its purpose of displaying damage information to the player
        }
    }
}
