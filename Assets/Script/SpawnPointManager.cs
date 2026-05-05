using UnityEngine;
using Unity.Netcode;

public class SpawnPointManager : NetworkBehaviour
{
    //Stores which spawn point to use next, this is used to ensure that players spawn at different locations when they join the game
    //Static means all player object share the same variable, so when one player changes it, it changes for all players, this is useful for keeping track of the next spawn point across all players
    private static int nextSpawnIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn() //called when the player object is spawned on the network, this is where we assign the spawn point to the player
    {
        if (!IsServer) //only the server should assign spawn points, this ensures that all players get assigned a spawn point in a consistent manner
        {
            return; //if this is not the server, we return early and do not assign a spawn point, this is because the server will handle assigning spawn points for all players
        }
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint"); //find all game objects with the tag "SpawnPoint", this is how we get the list of available spawn points in the scene
        if(spawnPointObjects.Length == 0) //if there are no spawn points in the scene, we log an error and return early, this is important to prevent errors when trying to assign a spawn point
        {
            Debug.LogError("No spawn point detected"); //log an error if there are no spawn points in the scene, this helps with debugging and ensures that we know why players are not spawning correctly
            return; //return early if there are no spawn points, this prevents errors when trying to assign a spawn point and allows us to handle the situation gracefully
        }
        Transform selectedSpawnPoint = spawnPointObjects[nextSpawnIndex].transform; //select the next spawn point from the list using the nextSpawnIndex, this ensures that players spawn at different locations when they join the game
        CharacterController characterController = GetComponent<CharacterController>(); //get the CharacterController component of the player object, this is used to move the player to the spawn point
        if(characterController != null) //if the player object has a CharacterController component, we move the player to the spawn point using the CharacterController, this is important for ensuring that the player is moved correctly and does not get stuck in the environment
        {
            characterController.enabled = false; //disable the CharacterController before moving the player, this is necessary to prevent physics issues when moving the player
        }
        transform.position = selectedSpawnPoint.position; //move the player to the spawn point, this is done by setting the player's position to the position of the selected spawn point
        transform.rotation = selectedSpawnPoint.rotation; //set the player's rotation to match the spawn point's rotation, this ensures that the player is facing the correct direction when they spawn

        if(characterController != null) //if the player object has a CharacterController component, we re-enable it after moving the player, this is necessary to allow the player to move and interact with the environment after spawning
        {
            characterController.enabled = true; //re-enable the CharacterController after moving the player, this allows the player to move and interact with the environment after spawning
        }
        nextSpawnIndex++; //increment the nextSpawnIndex to ensure that the next player spawns at a different location, this is important for ensuring that players do not spawn on top of each other and have a better gameplay experience
        if(nextSpawnIndex >= spawnPointObjects.Length) //if the nextSpawnIndex exceeds the number of available spawn points, we reset it to 0 to loop back to the first spawn point, this ensures that we can reuse spawn points when there are more players than spawn points available
        {
            nextSpawnIndex = 0; //reset the nextSpawnIndex to 0 if it exceeds the number of available spawn points, this allows us to reuse spawn points when there are more players than spawn points available
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
