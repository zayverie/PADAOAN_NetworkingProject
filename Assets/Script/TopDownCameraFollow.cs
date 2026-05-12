using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0, 10f, -8f); // Offset from the player, this is set in the Unity editor and can be adjusted to change how the camera follows the player in the game
    [SerializeField] float followSpeed = 10f; // Speed at which the camera follows the player, this is set in the Unity editor and can be adjusted to change how quickly the camera moves to follow the player in the game

    private Transform target; // Reference to the player's transform, this is used to determine where the camera should be positioned in relation to the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetTarget(Transform newTarget) // Method to set the target for the camera to follow, this is called when the player object is spawned and allows the camera to start following the player
    {
        target = newTarget; // Set the target to the new target transform, this allows the camera to follow the specified target in the game
    }
    private void LateUpdate() // LateUpdate is called after all Update methods have been called, this is used to ensure that the camera follows the player after the player has moved in the current frame, providing smoother camera movement and preventing jittering
    {
        if (target == null) // If there is no target set for the camera to follow, we return early and do not update the camera's position, this prevents errors and ensures that the camera does not try to follow a non-existent target
        {
            return; // Return early if there is no target, this prevents errors and ensures that the camera does not try to follow a non-existent target
        }
        Vector3 desiredPosition = target.position + offset; // Calculate the desired position for the camera based on the target's position and the specified offset, this determines where the camera should be positioned in relation to the player
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime); // Smoothly move the camera towards the desired position using linear interpolation (Lerp), this creates smooth camera movement as it gradually moves towards the target position based on the follow speed and delta time
        transform.LookAt(target.position); // Make the camera look at the target's position, this ensures that the camera is always oriented towards the player as it follows them in the game, providing a consistent view of the player from above
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
