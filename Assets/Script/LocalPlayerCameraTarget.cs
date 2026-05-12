using UnityEngine;
using Unity.Netcode;

public class LocalPlayerCameraTarget : NetworkBehaviour
{
    public override void OnNetworkSpawn() // Called when the player object is spawned on the network, this is where we set the camera's target to follow this player, ensuring that each player has their own camera that follows them in the game
    //override means we are overriding the base implementation of the OnNetworkSpawn method from the NetworkBehaviour class, this allows us to add our own functionality for setting the camera target when the player object is spawned on the network
    {
        if (!IsOwner) // Check if this player object belongs to the local player, this is important for ensuring that we only set the camera target for the local player's character and not for other players' characters
        {
            return; // If this is not the local player's object, we return early and do not set the camera target, this prevents conflicts between multiple players trying to set the camera target for their own character and ensures that each player has a camera that follows only their own character
        }
        TopDownCameraFollow cameraFollow = Camera.main.GetComponent<TopDownCameraFollow>(); // Get the TopDownCameraFollow component from the main camera, this is used to set the target for the camera to follow
        
        if (cameraFollow != null) // If the camera has a TopDownCameraFollow component, we set the target to this player's transform, allowing the camera to follow this player in the game
        {
            cameraFollow.SetTarget(transform); // Set the camera's target to this player's transform, this allows the camera to follow this player in the game and provides a consistent view of the player from above
        }
        
    }
}
