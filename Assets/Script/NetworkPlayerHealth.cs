using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    
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

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        currentHealth.Value -= amount;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth); // Ensure health does not go below 0s

        if (currentHealth.Value <= 0) 
        {
            Respawn();
        }
    }
}