using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace CacheUtilities.Editor
{
    [InitializeOnLoad]
    public class ShaderCacheBuildProcessor
    {
        private static ShaderCacheConfig _config;
        
        public int callbackOrder
        {
            get { return 0; }
        }

        private static string _shaderCachePath = "";
        private static string _shaderDBFilePath = "";
        
        static ShaderCacheBuildProcessor()
        {
            Setup();
            EditorApplication.quitting += PerformActionOnQuitEditor;
        }

        private static void Setup()
        {
            _config = ShaderCacheBuildProcessorEditor.FindGlobalConfig();
            _shaderCachePath = Path.Combine(Application.dataPath, "../Library/ShaderCache");
            _shaderDBFilePath = _shaderCachePath + ".db";
        }

        static void PerformActionOnQuitEditor()
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if (_config != null && (_config.clearShaderCacheWhenUnityIsClosed))
            {
                CheckNeedToPurgeShaderCache();
            }
#endif
        }

        private static void CheckNeedToPurgeShaderCache()
        {
            if (Directory.Exists(_shaderCachePath))
            {
                bool deleteDirectory = true;

                if (_config.onlyClearShaderCacheWhenCacheSizeExceeded)
                {
                    if (IsShaderCacheLimitExceeded() == false)
                    {
                        deleteDirectory = false;
                    }
                }

                if (deleteDirectory)
                {
                    ClearShaderCache();
                }
            }
            else
            {
                Debug.Log("ShaderCache not found at " + _shaderCachePath +
                          ". It may have been recently deleted and has not yet auto generated");
            }
        }

        public static void ClearShaderCache()
        {
            if (Directory.Exists(_shaderCachePath)) //Potentially unneccessary check but technically possible someone could manually clear and then call this
            {
                Directory.Delete(_shaderCachePath, true);
            }
            
            //Cant clear DB file too as it will be locked by Unity
        }

        static bool IsShaderCacheLimitExceeded()
        {
            float total = ReturnShaderCacheSizeInMB();

            if (total >= _config.shaderCacheSizeLimitInMB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float ReturnShaderCacheSizeInMB()
        {
            if (Directory.Exists(_shaderCachePath))
            {
                DirectoryInfo di = new DirectoryInfo(_shaderCachePath);
                return ((di.EnumerateFiles(".", SearchOption.AllDirectories).Sum(fi => fi.Length) / 1024f) /
                        1024f); //Convert to MB from bytes
            }
            else
            {
                return 0;
            }
        }
        
        public static float ReturnShaderCacheDBSizeInMB()
        {
            if (File.Exists(_shaderDBFilePath))
            {
                FileInfo fileInfo = new FileInfo(_shaderDBFilePath);
                return ((fileInfo.Length)/1024)/1024;
            }
            else
            {
                return 0;
            }
        }

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if (_config != null && _config.clearShaderCacheAfterBuild)
            {
                CheckNeedToPurgeShaderCache();
            }
#endif
        }
    }
}