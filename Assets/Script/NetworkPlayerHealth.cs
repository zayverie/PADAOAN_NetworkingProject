using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.InputSystem;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject damagePopupPrefab; // Prefab for the damage popup, this is used to display the amount of damage taken when the player is hit
    
    // NetworkVariable to synchronize health across clients
    public NetworkVariable<int> currentHealth = new(
        100, 
        NetworkVariableReadPermission.Everyone, // Allow all clients to read health
        NetworkVariableWritePermission.Server // Only the server can modify health
    );

    public override void OnNetworkSpawn() 
    {
        // Initialize health on the server when the player spawns
        base.OnNetworkSpawn();
        if (IsServer) 
        {
            currentHealth.Value = maxHealth; // Set initial health on the server
        }
        currentHealth.OnValueChanged += OnHealthChange; // Subscribe to health change events for all clients

    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChange; // Unsubscribe from health change events when the player despawns
    }
    // Callback method to handle health changes
    private void OnHealthChange(int oldValue, int newValue) 
    {
        Debug.Log($"{gameObject.name} health change from {oldValue} -> {newValue}");
    }
    // Method to handle player respawn
    private void Respawn() 
    {
        // Reset health and reposition the player at a random spawn point
        currentHealth.Value = maxHealth;
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Player");
        int randomIndex = Random.Range(0, spawns.Length);
        Transform selectedSpawn = spawns[randomIndex].transform;
        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }
        transform.position = selectedSpawn.position;
        transform.rotation = selectedSpawn.rotation;
        
        if (controller != null){
            controller.enabled = true;
        }
            
    }

    public void TakeDamage(int damageAmount, Vector3 hitPosition)
    {
        if (!IsServer) return;

        currentHealth.Value -= damageAmount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth); // Ensure health does not go below 0s

        ShowDamagePopupClientRpc(damageAmount, hitPosition); // Show damage popup on all clients when damage is taken
        Debug.Log($"TakeDamage on {gameObject.name}: {damageAmount}");
        if (currentHealth.Value <= 0) 
        {
            StartCoroutine(HandleDeath()); // Start the death handling coroutine when health reaches zero
        }
    }
    IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before respawning, this gives players a moment to see that they have been defeated before they respawn
        Respawn(); // Call the Respawn method to reset health and reposition the player
    }
    
    [ClientRpc]
    private void ShowDamagePopupClientRpc(int damageAmount, Vector3 position) 
    {
        Debug.Log($"ShowDamagePopupClientRpc called. Prefab null? {damagePopupPrefab == null}");
        Debug.Log($"Popup spawned at {position + Vector3.up * 1.5f} for player at {position}");
        if (damagePopupPrefab != null) 
        {
            GameObject popup = Instantiate(damagePopupPrefab, position + Vector3.up * 1.5f, Quaternion.identity); // Instantiate the damage popup at the specified position, this creates a visual effect to show the damage taken when the player is hit
            popup.GetComponent<DamagePopup>().Setup(damageAmount); // Set up the damage popup with the damage amount, this allows the popup to display the correct damage value to the player
        }
    }
}