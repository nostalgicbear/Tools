using UnityEngine;
using UnityEditor;

namespace CacheUtilities.Editor
{
    public class ShaderCacheBuildProcessorEditor : EditorWindow
    {
        private ShaderCacheConfig _config;
        private float shaderCacheSize;
        private float shaderCacheDBSize;
        
        [MenuItem("Tools/Custom/Cache Manager")]
        static void Init()
        {
            ShaderCacheBuildProcessorEditor window =
                (ShaderCacheBuildProcessorEditor) GetWindow(typeof(ShaderCacheBuildProcessorEditor));
            window.titleContent.text = "Shader Cache Tab";
            window.maxSize = new Vector2(500, 350);
            window.Show();
        }

        private void OnEnable()
        {
            _config = FindGlobalConfig();
            UpdateFileSizeInfo();
        }

        private void UpdateFileSizeInfo()
        {
            shaderCacheSize = ShaderCacheBuildProcessor.ReturnShaderCacheSizeInMB();
            shaderCacheDBSize = ShaderCacheBuildProcessor.ReturnShaderCacheDBSizeInMB();
        }

        public static ShaderCacheConfig FindGlobalConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(ShaderCacheConfig).Name);
            if (guids != null && guids.Length > 0)
            {
                if (guids.Length > 1) //In future, there may be a logical reason to have more than one...
                {
                    Debug.Log(
                        "More than one ShaderCacheConfig scriptable object found. There should really only be one.");
                }

                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                ShaderCacheConfig config = (ShaderCacheConfig) AssetDatabase.LoadAllAssetsAtPath(path)[0];

                return config;
            }

            return null;
        }

        void OnGUI()
        {
            if (_config == null)
            {
                DrawNoConfigFoundWindow();
                return;
            }

            DrawShaderConfigWindow();
        }

        private void DrawNoConfigFoundWindow()
        {
            GUILayout.Label(StringConstants.SHADERCACHE_NO_SHADERCACHE_FOUND, EditorStyles.label);

            if (GUILayout.Button("Create Shader Cache Config"))
            {
                ShaderCacheConfig shaderCacheConfig = CreateInstance<ShaderCacheConfig>();
                string savePath = StringConstants.SHADERCACHE_CONFIG_ASSET_PATH;
                AssetDatabase.CreateAsset(shaderCacheConfig, savePath);
            }

            _config = FindGlobalConfig();
        }

        private void DrawShaderConfigWindow()
        {
            GUILayout.Label("Shader Cache Settings", EditorStyles.boldLabel);

            GUILayout.Space(10);

            GUILayout.Label(StringConstants.SHADERCACHE_INSTRUCTIONS, EditorStyles.label);

            GUILayout.Space(10);
            
            EditorGUILayout.BeginVertical();

            float originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 300;

            _config.clearShaderCacheAfterBuild = EditorGUILayout.Toggle(StringConstants.SHADERCACHE_CLEAR_AFTER_BUILD,
                _config.clearShaderCacheAfterBuild);
            _config.clearShaderCacheWhenUnityIsClosed =
                EditorGUILayout.Toggle("Clear shader cache when Unity is closed: ",
                    _config.clearShaderCacheWhenUnityIsClosed);

            if (_config.clearShaderCacheAfterBuild || _config.clearShaderCacheWhenUnityIsClosed)
            {
                _config.onlyClearShaderCacheWhenCacheSizeExceeded =
                    EditorGUILayout.Toggle("Only delete when above specified file size (MB): ",
                        _config.onlyClearShaderCacheWhenCacheSizeExceeded);

                if (_config.onlyClearShaderCacheWhenCacheSizeExceeded)
                {
                    _config.shaderCacheSizeLimitInMB =
                        Mathf.Clamp(EditorGUILayout.IntField("File size: ", _config.shaderCacheSizeLimitInMB), 1,
                            10000);
                }
            }

            EditorGUIUtility.labelWidth = originalValue;

            
            EditorGUILayout.EndVertical();
            

            GUILayout.FlexibleSpace();
            
            EditorGUILayout.LabelField(StringConstants.SHADERCACHE_DELETE_INSTRUCTION);
            EditorGUILayout.LabelField(StringConstants.SHADERCACHE_CLOSE_WARNING);

            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Shader Cache is currently : " + shaderCacheSize.ToString("F2") +"MB");
            EditorGUILayout.LabelField("Shader Cache DB file is currently : " + shaderCacheDBSize.ToString("F2") +"MB");
            

            if (GUILayout.Button("Clear Shader Cache"))
            {
                if (EditorUtility.DisplayDialog("Are you sure you want to delete the Shader Cache?",
                    "You are about to delete your Shader Cache folder. Unity will auto generate this again when " +
                    "it next compiles", "Yes", "No"))
                {
                    ShaderCacheBuildProcessor.ClearShaderCache();
                    UpdateFileSizeInfo();
                }
            }
        }
    }
}