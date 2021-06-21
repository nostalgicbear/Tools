using UnityEngine;

[CreateAssetMenu(fileName = "ShaderCacheConfig", menuName = "ScriptableObjects/ShaderCache Config", order = 1)]
public class ShaderCacheConfig : ScriptableObject
{
    public bool clearShaderCacheAfterBuild;
    public bool clearShaderCacheWhenUnityIsClosed;
    public bool onlyClearShaderCacheWhenCacheSizeExceeded;
    [Range(1, 10000)]
    public int shaderCacheSizeLimitInMB = 100; //In MB (arbitrary figure)
}
