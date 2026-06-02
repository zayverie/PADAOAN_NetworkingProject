using UnityEngine;
using Unity.Netcode;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] float speed = 12.5f; // Speed of the projectile, this is set in the Unity editor and can be adjusted to change how fast the projectile moves in the game
    [SerializeField] float lifetime = 10.0f; // Lifetime of the projectile, this is set in the Unity editor and can be adjusted to change how long the projectile exists in the game before it is destroyed
    private float despawnTime; // Time when the projectile should be despawned, this is calculated based on the current time and the lifetime of the projectile to determine when it should be removed from the game

    public override void OnNetworkSpawn() // Called when the projectile is spawned on the network, this is where we initialize the despawn time for the projectile based on its lifetime
    {
       if(IsServer){ // Only the server should handle the despawn time and movement of the projectile, this ensures that all clients see consistent behavior for the projectile and prevents conflicts between multiple clients trying to control the same projectile
            despawnTime = Time.time + lifetime; // Set the despawn time based on the current time and the lifetime of the projectile, this allows us to determine when the projectile should be removed from the game
       }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer){
            return; // If this is not the server, we return early and do not process movement or despawning for the projectile, this is important for ensuring that only the server controls the behavior of the projectile and prevents conflicts between multiple clients trying to control the same projectile
        }
        transform.position += transform.forward * speed * Time.deltaTime; // Move the projectile forward based on its speed and the time since the last frame, this allows the projectile to move through the game world in a consistent manner
        if(Time.time >= despawnTime){ // Check if the current time has reached or exceeded the despawn time for the projectile, this is used to determine when the projectile should be removed from the game after its lifetime has expired
            NetworkObject.Despawn(); // Despawn the projectile on the network, this allows us to remove the projectile from all clients and ensures that it no longer exists in the game after its lifetime has expired
        }
    }
    private void OnTriggerEnter(Collider other) // Called when the projectile collides with another object, this is used to handle interactions between the projectile and other objects in the game, such as damaging players or destroying the projectile on impact
    {
        if(!IsServer){
            return; // If this is not the server, we return early and do not process collisions for the projectile, this is important for ensuring that only the server controls the behavior of the projectile and prevents conflicts between multiple clients trying to control the same projectile
        }
        if(other.CompareTag("Player")){ // Check if the projectile collided with an object tagged as "Player", this is used to determine if the projectile should damage a player in the game
            Debug.Log($"Projectile hit {other.gameObject.name}"); // Log a message indicating that the projectile hit a player, this is useful for debugging and understanding when the projectile interacts with players in the game
            NetworkObject.Despawn();
        }
    }
}
