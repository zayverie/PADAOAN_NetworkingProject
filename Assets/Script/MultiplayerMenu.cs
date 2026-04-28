using UnityEngine;
using Unity.Netcode;

public class MultiplayerMenu : MonoBehaviour
{
    public void StartHost() //starts the game as both server and client, allowing the player to host a game and play on it
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() //starts the game as a client, allowing the player to join a game hosted by another player
    {
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer() //starts the game as a server, allowing the player to host a game but not play on it, useful for dedicated servers
    {
        NetworkManager.Singleton.StartServer();
    }
}
