using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro; 

public class MultiplayerMenu : NetworkBehaviour
{
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private TextMeshProUGUI playerCountText; 
    void Awake()
    {
        // Limits CPU usage by capping frames per second
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        if(IsServer)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            UpdatePlayerCountRpc(playerCount); 
        }
    }
    [Rpc(SendTo.Everyone)] // This attribute indicates that this method is a Remote Procedure Call (RPC) that should be executed on all clients, this is used to update the player count UI on all clients whenever there is a change in the number of connected players, ensuring that all players have accurate information about the current player count in the game
    private void UpdatePlayerCountRpc(int playerCount)
    {
        if(NetworkManager.Singleton != null)
        {
            playerCountText.text = $"Players Count: {playerCount}";
        }
    }
    public void StartHost() 
    {
        NetworkManager.Singleton.StartHost();
        HideMenu();    
    }

    public void StartClient() 
    {
        NetworkManager.Singleton.StartClient();
        HideMenu(); 
    }
    public void StartServer() 
    {
        NetworkManager.Singleton.StartServer();
        HideMenu(); 
    }
    public void HideMenu() 
    {
        menuPanel.SetActive(false);
    }
}
