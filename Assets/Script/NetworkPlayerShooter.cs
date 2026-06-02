using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerShooter : NetworkBehaviour
{
    [SerializeField] GameObject projectilePrefab; //Projectile Prefab
    [SerializeField] Transform firePoint; //Where does te projectile spawn
    [SerializeField] float fireCooldown = 0.25f; //fire rate / attack cooldown
    [SerializeField] KeyCode fireButton = KeyCode.Mouse0;
    private float nextFireTime; //Time when the player can fire again, this is used to implement the fire cooldown and prevent the player from firing too rapidly in the game
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
        if(Input.GetKeyDown(fireButton) && Time.time >= nextFireTime){
            nextFireTime = Time.time + fireCooldown; // Update the next fire time based on the current time and the fire cooldown, this ensures that the player can only fire again after the cooldown period has passed
            RequestShootServerRpc (firePoint.position, firePoint.forward); // Call the RequestShootServerRpc method to request that the server spawns a projectile at the fire point position and in the direction the fire point is facing, this allows the player to shoot projectiles in the game while ensuring that the projectile spawning is handled on the server for consistency and security
        }
    }

    [ServerRpc]
    private void RequestShootServerRpc(Vector3 spawnPosition, Vector3 shootDirection){
        //Instantiate = Create a new instance of the projectile prefab at the specified spawn position and with the specified rotation, this is used to spawn the projectile in the game when the player shoots
       GameObject projectileInstantiate = Instantiate(
              projectilePrefab, 
              spawnPosition, 
              Quaternion.LookRotation(shootDirection) // Rotate the projectile to face the shoot direction, this ensures that the projectile is oriented correctly when it is spawned in the game
       );
        //Spawn = Spawn the projectile on the network, this allows all clients to see the projectile and ensures that it is synchronized across the game session, this is necessary for multiplayer functionality so that all players can see the projectile and its movement in the game
       NetworkObject networkObject = projectileInstantiate.GetComponent<NetworkObject>(); // Get the NetworkObject component from the instantiated projectile, this is necessary to spawn the projectile on the network and synchronize it across all clients
       networkObject.Spawn(); // Spawn the projectile on the network, this allows all clients to see the projectile and ensures that it is synchronized across the game session
    }
}
