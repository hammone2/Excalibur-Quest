using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Footsteps Data", menuName = "Audio/Footsteps Data")]
public class TerrainFootstepsData : ScriptableObject
{
    [Header("Terrain Surfaces")]
    public TerrainFootstepSurface[] surfaces;

    [Header("Material Surfaces")]
    public MaterialFootstepSurface[] materialSurfaces;
}

[System.Serializable]
public class TerrainFootstepSurface
{
    [SerializeField] private string surfaceName; //for organization in the inspector
    public TerrainLayer terrainLayer;
    public AudioClip[] footstepSounds;
}

[System.Serializable]
public class MaterialFootstepSurface
{
    [SerializeField] private string surfaceName;
    public Material material;
    public AudioClip[] footstepSounds;
}