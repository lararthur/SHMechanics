using UnityEngine;
using Unity.Cinemachine; // Use 'using Cinemachine;' if you are on an older Unity version

public class CameraZone : MonoBehaviour
{
    // Drag the Virtual Camera for this specific zone into this slot in the Inspector
    public CinemachineCamera zoneCamera;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the Player
        if (other.CompareTag("Player"))
        {
            // Give this camera max priority so Cinemachine snaps to it
            zoneCamera.Priority = 20;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Drop the priority back down when the player leaves this area
            zoneCamera.Priority = 5;
        }
    }
}