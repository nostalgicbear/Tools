using System;
using UnityEngine;

/// <summary>
/// A class for holding lightmap data and transfering it to a mesh renderer (neccessary for transfering lightmap data between objects)
/// </summary>
[ExecuteInEditMode]
[Serializable]
public class LightmapDataHolder : MonoBehaviour
{
    /// <summary>
    /// Small class for holding lightmap data
    /// </summary>
    [Serializable]
    public class MeshRendererData
    {
        public int lightmapIndex = 0;
        public Vector4 tilingAndOffset = Vector4.one;
        public float scaleInLightmap = 0;
        public int lightmapResolution = 0;

        public MeshRendererData(MeshRenderer r) //For now I am only passing in lightmap index as other data seems to be retained
        {
            lightmapIndex = r.lightmapIndex;
            tilingAndOffset = r.lightmapScaleOffset;
#if UNITY_EDITOR
            scaleInLightmap = r.scaleInLightmap;
#endif
        }

        public void ApplyData(MeshRenderer r)
        {
            lightmapIndex = r.lightmapIndex;
            tilingAndOffset = r.lightmapScaleOffset;
#if UNITY_EDITOR
            scaleInLightmap = r.scaleInLightmap;
#endif
        }
    }
    
    [SerializeField] 
    private MeshRenderer _rendererToCopy;
    private MeshRenderer _meshRenderer;
    [SerializeField]
    public MeshRendererData meshRendererData;
   
    private void Awake()
    {
        ApplyLightmapData();
    }
    
    /// <summary>
    /// Applies lightmap to a mesh based on the MeshRendererData data. Overrides the meshrendererdata with data from another mesh if one is specified
    /// </summary>
    void ApplyLightmapData()
    {
        if (_meshRenderer == null) { _meshRenderer = GetComponent<MeshRenderer>(); }

        if (_rendererToCopy != null) { meshRendererData.ApplyData(_rendererToCopy); }

        if (meshRendererData != null)
        {
            _meshRenderer.lightmapIndex = meshRendererData.lightmapIndex;
            _meshRenderer.lightmapScaleOffset = meshRendererData.tilingAndOffset;

#if UNITY_EDITOR
            _meshRenderer.scaleInLightmap = meshRendererData.scaleInLightmap;
#endif
        }
    }
}


