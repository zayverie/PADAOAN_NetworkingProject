using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f; // Speed at which the player moves, this is set in the Unity editor and can be adjusted to change how fast the player moves in the game
    [SerializeField] float gravity = -9.81f; // Gravity force applied to the player, this is set in the Unity editor and can be adjusted to change how strong the gravity is in the game
    [SerializeField] float groundedGravity = -2f; // Gravity force applied to the player when they are grounded, this is set in the Unity editor and can be adjusted to change how the player behaves when they are on the ground
    
    private CharacterController characterController; // Reference to the CharacterController component, this is used to move the player and handle collisions with the environment
    private float verticalVelocity; // Velocity of the player, this is used to apply gravity and move the player in the game
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component attached to the player object, this is necessary for moving the player and handling collisions with the environment
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner){
            return; // If this is not the owner of the player object, we return early and do not process input or move the player, this is important for ensuring that only the player who owns the object can control it and prevents conflicts between multiple players trying to control the same object
        }
        float horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input from the player, this is used to determine how the player should move in the game
        float verticalInput = Input.GetAxis("Vertical"); // Get vertical input from the player, this is used to determine how the player should move in the game
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput); // Create a Vector2 to represent the input direction based on the horizontal and vertical input, this is used to calculate the movement direction for the player

        if(IsServer){ // If this is the server, we can move the player directly without needing to send an RPC, this is because the server has authority over the player object and can control its movement directly
            MovePlayer(inputDirection); // Call the MovePlayer method to process the movement input and move the player in the game, this is done on the server to ensure that all players see consistent movement and to prevent cheating by allowing clients to control their own movement
        } else {
            MovePlayerRpc(inputDirection); // If this is not the server, we send an RPC to the server to request that it moves the player based on the input direction, this allows clients to control their own movement while still ensuring that all movement is processed on the server for consistency and security
        }
    }
    [Rpc(SendTo.Server)] // This attribute indicates that this method is a Remote Procedure Call (RPC) that should be executed on the server, this is used to allow the client to send input data to the server for processing and movement of the player object
    private void MovePlayerRpc(Vector2 movementInput){
        MovePlayer(movementInput); // Call the MovePlayer method to process the movement input and move the player in the game, this is done on the server to ensure that all players see consistent movement and to prevent cheating by allowing clients to control their own movement
    }
    private void MovePlayer(Vector2 movementInput){
        if(characterController.isGrounded){ // Check if the player is grounded, this is important for applying the correct gravity and movement behavior when the player is on the ground
            verticalVelocity = groundedGravity; // If the player is grounded, we set the vertical velocity to the grounded gravity value, this allows the player to stay on the ground and prevents them from floating or falling through the environment
        } else {
            verticalVelocity += gravity * Time.deltaTime; // If the player is not grounded, we apply gravity to the vertical velocity, this allows the player to fall and simulates realistic movement in the game
        }
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized; // Create a Vector3 to represent the movement direction based on the input direction, this is used to calculate the movement for the player in the game
        Vector3 horizontalMovement = moveDirection * moveSpeed; // Calculate the horizontal movement based on the movement direction and move speed, this is used to determine how far the player should move in the horizontal plane
        Vector3 verticalMovement = new Vector3(0, verticalVelocity, 0); // Create a Vector3 to represent the vertical movement based on the vertical velocity, this is used to apply gravity and move the player vertically in the game
        Vector3 finalMovement = horizontalMovement + verticalMovement; // Combine the horizontal and vertical movement to get the final movement vector, this is used to move the player in the game
        characterController.Move(finalMovement * Time.deltaTime); // Move the player using the CharacterController
    }
    void Awake()
    {
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component attached to the player object, this is necessary for moving the player and handling collisions with the environment
    }
}
