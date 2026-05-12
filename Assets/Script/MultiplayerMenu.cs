using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MultiplayerMenu : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text playerCountText;

    [Header("Relay Settings")]
    [SerializeField] private int maxConnections = 4; // Maximum number of players that can join the session, this is set in the Unity editor and can be adjusted to change how many players can play together in the game
    //it tells Unity to use web sockets for network communication, which is necessary for WebGL builds as they do not support traditional TCP connections, this ensures that the game can communicate properly over the network when running in a web browser
    //if the transport does not use web sockets, the game will not be able to connect to the Relay service and players will not be able to host or join game sessions, this is crucial for enabling multiplayer functionality in WebGL builds and ensuring that players can connect to each other successfully
    private const string WebGLConnectionType = "wss"; // WebSocket connection type for WebGL builds, this is used to ensure that the correct connection type is used when running the game in a web browser, as WebGL does not support traditional TCP connections

    void Update()
    {
        if (NetworkManager.Singleton != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            playerCountText.text = "Players Connected: " + playerCount; // Update the player count text to show how many players are currently connected to the game session, this provides feedback to the players about the current state of the multiplayer session and can help with debugging by confirming that players are connecting successfully
        }
    }

    
    private async void Start()
    {
        await InitializeUnityServices(); // Initialize Unity Services when the game starts, this is necessary to use features like Relay and Authentication for multiplayer functionality, and ensures that the services are ready before players try to host or join a game session
    }

    private async System.Threading.Tasks.Task InitializeUnityServices() 
    {
        try // Try to initialize Unity Services, this is important for enabling multiplayer features and ensuring that the services are ready before players try to host or join a game session
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) // Check if Unity Services are not already initialized, this prevents unnecessary initialization and ensures that we only initialize the services once when the game starts
            {
                await UnityServices.InitializeAsync(); // Initialize Unity Services asynchronously, this allows the game to continue running while the services are being initialized, and ensures that we do not block the main thread during initialization
            }

            if (!AuthenticationService.Instance.IsSignedIn) // Check if the player is not already signed in, this prevents unnecessary sign-in attempts and ensures that we only sign in once when the game starts
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Sign in anonymously to Unity Services, this allows the player to use multiplayer features without needing to create an account or sign in with a specific identity, and is useful for quick testing and casual multiplayer games where user accounts are not necessary
            }

            SetStatus("Unity Services ready."); // Set the status text to indicate that Unity Services are ready, this provides feedback to the player that they can now host or join a game session, and helps with debugging by confirming that the services were initialized successfully
        }
        catch (Exception exception) // Catch any exceptions that occur during initialization, this is important for handling errors gracefully and providing feedback to the player if something goes wrong during initialization, such as network issues or service outages
        {
            SetStatus("Unity Services failed to initialize."); // Set the status text to indicate that Unity Services failed to initialize, this provides feedback to the player that there was an issue with initialization and that multiplayer features may not work, and helps with debugging by confirming that there was an error during initialization
            Debug.LogError(exception);
        }
    }

    public async void StartHost()
    {
        //Start hosting a game session, this allows the player to create a game session that others can join through a code, and is essential for enabling multiplayer functionality in the game, as it allows one player to act as the host and manage the game session while other players connect as clients
        try // Try to start hosting a game session, this is important for enabling multiplayer functionality and allowing players to create a game session that others can join, and ensures that we handle any errors that may occur during the hosting process gracefully
        {
            SetStatus("Creating host session..."); // Set the status text to indicate that we are creating a host session, this provides feedback to the player that the hosting process has started and helps with debugging by confirming that we are attempting to create a host session

            await InitializeUnityServices();

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.UseWebSockets = true;

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, WebGLConnectionType)
            );

            bool started = NetworkManager.Singleton.StartHost();

            if (started)
            {
                if (joinCodeText != null)
                {
                    joinCodeText.text = "Join Code: " + joinCode;
                }

                SetStatus("Host started. Join Code: " + joinCode);
                HideMenu();
            }
            else
            {
                SetStatus("Failed to start Host.");
            }
        }
        catch (Exception exception) // Catch any exceptions that occur during the hosting process, this is important for handling errors gracefully and providing feedback to the player if something goes wrong during hosting
        {
            SetStatus("Host failed. Check Console.");
            Debug.LogError(exception);
        }
    }

    public async void StartClient()
    {
        try
        {
            SetStatus("Joining session...");

            await InitializeUnityServices();

            if (joinCodeInput == null)
            {
                SetStatus("Join Code Input is missing.");
                return;
            }

            string joinCode = joinCodeInput.text.Trim().ToUpper();

            if (string.IsNullOrEmpty(joinCode))
            {
                SetStatus("Please enter a join code.");
                return;
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.UseWebSockets = true;

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(joinAllocation, WebGLConnectionType)
            );

            bool started = NetworkManager.Singleton.StartClient();

            if (started)
            {
                SetStatus("Client started.");
                HideMenu();
            }
            else
            {
                SetStatus("Failed to start Client.");
            }
        }
        catch (Exception exception)
        {
            SetStatus("Client failed. Check join code and Console.");
            Debug.LogError(exception);
        }
    }

    public void StartServer()
    {
        SetStatus("Dedicated Server is not recommended for Unity Play WebGL.");
        Debug.LogWarning("StartServer is disabled for Unity Play WebGL. Use StartHost or StartClient instead.");
    }

    private void HideMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}