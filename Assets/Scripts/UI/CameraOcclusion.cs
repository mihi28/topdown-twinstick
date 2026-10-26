using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Material transparentMaterial; 
    public LayerMask occlusionLayer; 

    [Header("Settings")]
    public float playerHeightOffset = 0f; // Aims the ray at the chest instead of the feet

    private Dictionary<Renderer, Material> currentlyOccluded = new Dictionary<Renderer, Material>();
    private List<Renderer> hitsThisFrame = new List<Renderer>();

    void Update()
    {
        if (player == null) 
        {
            Debug.LogWarning("CameraOcclusion: Player is not assigned!");
            return;
        }

        // Aim slightly above the player's pivot (e.g., at their chest)
        Vector3 targetPosition = player.position + new Vector3(0, playerHeightOffset, 0);
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // DRAW A RED LINE IN THE SCENE VIEW SO YOU CAN SEE THE RAY!
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, occlusionLayer);

        hitsThisFrame.Clear();

        foreach (RaycastHit hit in hits)
        {
            // Use GetComponentsInChildren in case the visual model is a child of the collider
            Renderer hitRenderer = hit.collider.GetComponentInChildren<Renderer>();
            if (hitRenderer != null)
            {
                hitsThisFrame.Add(hitRenderer);

                if (!currentlyOccluded.ContainsKey(hitRenderer))
                {
                    currentlyOccluded.Add(hitRenderer, hitRenderer.material);
                    hitRenderer.material = transparentMaterial;
                }
            }
        }

        List<Renderer> toRestore = new List<Renderer>();
        foreach (var occluded in currentlyOccluded)
        {
            if (!hitsThisFrame.Contains(occluded.Key))
            {
                occluded.Key.material = occluded.Value;
                toRestore.Add(occluded.Key);
            }
        }

        foreach (Renderer r in toRestore)
        {
            currentlyOccluded.Remove(r);
        }
    }
}