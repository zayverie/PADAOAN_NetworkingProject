using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerAttack : NetworkBehaviour
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private int damageAmount = 25;
    [SerializeField] private LayerMask playerLayer; // Layer for player detection
    [SerializeField] private KeyCode attackKey = KeyCode.Q; // Key to trigger attack

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner){
            return;
        }
        if(Input.GetKeyDown(attackKey))
        {
            RequestAttackServerRpc();
        }
    }
    [ServerRpc]
    private void RequestAttackServerRpc()
    {
        Debug.Log($"{gameObject.name} is attacking!");
        // Implementation for server-side attack logic
        Vector3 attackCenter = transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange, playerLayer);
        foreach (Collider hit in hits)
        {
            // Check if the hit object has a NetworkPlayerHealth component and is not the attacker
            if (hit.gameObject == gameObject) // Skip self
            {
                continue; // Skip self
            }
            NetworkPlayerHealth targetHealth = hit.GetComponent<NetworkPlayerHealth>();
            if (targetHealth != null)
            {
                Vector3 hitPosition = hit.transform.position; // Get the position of the hit player, this is used to determine where to show the damage popup when the player takes damage
                targetHealth.TakeDamage(damageAmount, hitPosition); // Apply damage to the target player, this reduces the target player's health and triggers the damage popup to show the damage taken
                Debug.Log($"{gameObject.name} attacked {hit.gameObject.name} for {damageAmount} damage.");  
                break; // Only attack one player at a time
            }
        }
    }
}
