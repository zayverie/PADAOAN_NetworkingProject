using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private NetworkPlayerHealth localPlayer;

    void Update()
    {
        // Find the local player once
        if (localPlayer == null)
        {
            foreach (var player in FindObjectsOfType<NetworkPlayerHealth>()) // Loop through all NetworkPlayerHealth components in the scene to find the one that belongs to the local player, this is necessary to ensure that we are updating the health UI for the correct player character
            {
                if (player.IsOwner)
                {
                    localPlayer = player;

                    localPlayer.currentHealth.OnValueChanged += UpdateUI;

                    UpdateUI(0, localPlayer.currentHealth.Value);
                    break;
                }
            }
        }
    }

    private void UpdateUI(int oldValue, int newValue)
    {
        healthBar.value = newValue;
    }
}