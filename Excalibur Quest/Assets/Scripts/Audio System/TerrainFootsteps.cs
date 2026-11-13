using UnityEngine;

public class TerrainFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform groundCheck;

    [Header("Footstep Settings")]
    [SerializeField] private float stepDistance = 2f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float volume = 1f;

    [Header("Terrain Surfaces")]
    [SerializeField] private TerrainFootstepsData terrainFootstepsData;

    [Header("Default Footsteps")]
    [SerializeField] private AudioClip[] defaultFootsteps;

    private Vector3 previousPosition;
    private float distanceTraveled;

    private void Start()
    {
        previousPosition = transform.position;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        // Only count horizontal movement.
        Vector3 movement = currentPosition - previousPosition;
        movement.y = 0f;

        previousPosition = currentPosition;

        // Check whether the player is grounded.
        if (!Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore))
        {
            distanceTraveled = 0f;
            return;
        }

        // Accumulate movement distance.
        distanceTraveled += movement.magnitude;

        if (distanceTraveled >= stepDistance)
        {
            distanceTraveled %= stepDistance;
            PlayFootstep(hit);
        }
    }

    private void PlayFootstep(RaycastHit hit)
    {
        AudioClip[] clips = defaultFootsteps;

        Terrain terrain = hit.collider.GetComponent<Terrain>();

        if (terrain != null)
        {
            TerrainLayer layer = GetDominantTerrainLayer(
                terrain,
                hit.point
            );

            if (layer != null)
            {
                foreach (TerrainFootstepSurface surface in terrainFootstepsData.surfaces)
                {
                    if (surface.terrainLayer == layer)
                    {
                        clips = surface.footstepSounds;
                        break;
                    }
                }
            }
        }
        else
        {
            // Get the material of regular geometry.
            Material material = GetHitMaterial(hit);

            if (material != null && terrainFootstepsData != null)
            {
                foreach (MaterialFootstepSurface surface in terrainFootstepsData.materialSurfaces)
                {
                    if (surface == null)
                        continue;

                    if (surface.material == material)
                    {
                        clips = surface.footstepSounds;
                        break;
                    }
                }
            }
        }

        if (clips == null || clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }

    private Material GetHitMaterial(RaycastHit hit)
    {
        Renderer renderer = hit.collider.GetComponent<Renderer>();

        if (renderer == null)
            return null;

        Material[] materials = renderer.sharedMaterials;

        if (materials.Length == 0)
            return null;

        // Objects with only one material.
        if (materials.Length == 1)
            return materials[0];

        // For multi-material meshes, determine which
        // submesh contains the triangle that was hit.
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (meshCollider == null || meshCollider.sharedMesh == null)
            return materials[0];

        Mesh mesh = meshCollider.sharedMesh;
        int triangleIndex = hit.triangleIndex;

        if (triangleIndex < 0)
            return materials[0];

        int triangleOffset = 0;

        for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
        {
            int indexCount = (int)mesh.GetIndexCount(submesh);
            int triangleCount = indexCount / 3;

            if (triangleIndex < triangleOffset + triangleCount)
            {
                if (submesh < materials.Length)
                    return materials[submesh];

                break;
            }

            triangleOffset += triangleCount;
        }

        return materials[0];
    }

    private TerrainLayer GetDominantTerrainLayer(
        Terrain terrain,
        Vector3 worldPosition)
    {
        TerrainData data = terrain.terrainData;

        Vector3 localPosition =
            worldPosition - terrain.transform.position;

        int x = Mathf.Clamp(
            Mathf.FloorToInt(
                localPosition.x / data.size.x *
                data.alphamapWidth
            ),
            0,
            data.alphamapWidth - 1
        );

        int z = Mathf.Clamp(
            Mathf.FloorToInt(
                localPosition.z / data.size.z *
                data.alphamapHeight
            ),
            0,
            data.alphamapHeight - 1
        );

        float[,,] alphamaps = data.GetAlphamaps(
            x,
            z,
            1,
            1
        );

        int dominantIndex = 0;
        float highestWeight = 0f;

        for (int i = 0; i < data.alphamapLayers; i++)
        {
            float weight = alphamaps[0, 0, i];

            if (weight > highestWeight)
            {
                highestWeight = weight;
                dominantIndex = i;
            }
        }

        if (dominantIndex >= data.terrainLayers.Length)
            return null;

        return data.terrainLayers[dominantIndex];
    }

    public void PlayLandingFootstep()
    {
        if (groundCheck == null)
            return;

        if (Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore))
        {
            PlayFootstep(hit);
        }
    }
}
