
/// <summary>
/// A static class for storing strings. This example here uses it to store 
/// </summary>
public static class StringConstants
{
    #region Popular Tags
    
    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_MAIN_CAMERA = "MainCamera";
    public const string TAG_SPAWN_POINT = "Spawn";
    
    #endregion
    
    #region ShaderCache strings
    public const string SHADERCACHE_CONFIG_ASSET_PATH = "Assets/ShaderCache Config.asset";
    
    public const string SHADERCACHE_INSTRUCTIONS =
        "Use the settings below to set when you want to have your Shader Cache" + "\n" +
        "automatically checked and reduced in size.";
    
    public const string SHADERCACHE_CLEAR_AFTER_BUILD = "Clear shader cache after build: ";

    public const string SHADERCACHE_NO_SHADERCACHE_FOUND =
        "No ShaderCache Config file found. Press the button to create one.";

    public const string SHADERCACHE_DELETE_INSTRUCTION =
        "If ShaderCache.db file is above a few MB in size, delete it from your Library folder";
    
    public const string SHADERCACHE_CLOSE_WARNING = "However it can only be deleted when Unity is closed";

    #endregion


}
