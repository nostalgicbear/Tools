using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomTools.MeshBakerUtilities
{
    public static class Custom_MeshBakerUtility
    {
#if UNITY_EDITOR
        /// <summary>
        /// Removes gameobjects that are part of LODGroups that do not meet a set criteria
        /// </summary>
        /// <param name="objToMesh"></param>
        /// <returns></returns>
        public static List<GameObject> RemoveObjectsWithIncorrectLODs(List<GameObject> objToMesh)
        {
            foreach (var go in objToMesh.ToList())
            {
                bool incorrectLOD = IsLODLevelTooHigh(go) || IsLODLevelZeroMissing(go);

                if (incorrectLOD) { objToMesh.Remove(go); }
            }

            return objToMesh;
        }

        /// <summary>
        /// Checks to see if the object is part of a LODGroup that has more LOD levels than the targetLODLevel
        /// </summary>
        /// <param name="obj">The gameobject being checked</param>
        /// <param name="_targetLODLevel">The LOD level to check against</param>
        /// <returns></returns>
        public static bool IsLODLevelTooHigh(GameObject obj, int _targetLODLevel = 3)
        {
            if (obj.transform.parent != null && obj.transform.parent.GetComponent<LODGroup>() != null &&
                obj.transform.parent.GetComponent<LODGroup>().lodCount > _targetLODLevel)
            {
                Debug.Log("Mesh Baker Message: Removed <color=yellow>" + obj.transform.GetHierarchyPath() + "</color> from selected objects as its parent : " + obj.transform.parent.GetHierarchyPath() + " had a LOD group entry higher than " + _targetLODLevel, obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a missing LOD0 entry.
        /// </summary>
        /// <param name="obj">The gameobject being checked</param>
        /// <returns></returns>
        private static bool IsLODLevelZeroMissing(GameObject obj) //Note : This func doubles up as a check to see if this object is in a LOD group or not. 
        {
            if (obj.transform.parent == null || obj.transform.parent.GetComponent<LODGroup>() == null)
            {
                Debug.Log("Mesh Baker Message: Removed <color=yellow>" + obj.transform.GetHierarchyPath() +
                          "</color> from selected objects as it was not a LODded object");
                return true;
            }
            
            var lod0 = obj.transform.parent.GetComponent<LODGroup>().GetLODs()[0];
            if (lod0.renderers.Length == 0)
            {
                Debug.Log("Mesh Baker Message: Removed <color=yellow>" + obj.transform.GetHierarchyPath() + "</color> from selected objects as its parent  " + obj.transform.parent.GetHierarchyPath()+ " has no renderer for LOD 0 ", obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies extra LOD levels to the lod 
        /// </summary>
        /// <param name="obj"></param>
        public static List<GameObject> ApplyMissingLODLevel(List<GameObject> objs, int _expectedLODLevel = 3)
        {
            foreach (GameObject obj in objs.ToList())
            {
                if (obj.transform.parent == null || obj.transform.parent.GetComponent<LODGroup>() == null)
                {
                    continue;
                }
                
                LODGroup lodGroup = obj.transform.parent.GetComponent<LODGroup>();
                List<LOD> lods = new List<LOD>(lodGroup.GetLODs());
                
                for (int i = lods.Count; i < _expectedLODLevel; i++)
                {
                    List<Renderer> renderers = new List<Renderer>(lodGroup.GetLODs()[i - 1].renderers);
                    
                    lods.Add(new LOD(lods[i - 1].screenRelativeTransitionHeight / 2.0f, renderers.ToArray())); 

                    lodGroup.SetLODs(lods.ToArray());
                    Debug.Log("Mesh Baker Message: Added LOD level <color=yellow>" + i + "</color> to " +  "<color=yellow>" + obj.transform.parent.GetHierarchyPath() + "</color> as it did not have a LOD entry for level " + i, obj);
                }
                
                SerializedObject sObj = new SerializedObject(lodGroup);
                for (int j = 0; j < _expectedLODLevel; j++)
                {
                    SerializedProperty sHeight = sObj.FindProperty("m_LODs.Array.data[" + j.ToString() + "].screenRelativeHeight");
                    
                    if (j == 0) { sHeight.doubleValue = 0.1f; } //Unlikely to want to change these. Could always expose in editor at a later date.
                    if (j == 1) { sHeight.doubleValue = 0.05; }
                    if (j == 2) { sHeight.doubleValue = 0.02; }
                }
                
                sObj.ApplyModifiedProperties();
                lodGroup.RecalculateBounds();
            }
            
            return objs;
        }
        
        public static void ListObjectsWithNonUniformScale(List<GameObject> listOfObjects) //TODO : Move this out of this class
        {
            foreach (var go in listOfObjects)
            {
                if (go.transform.localScale != Vector3.one)
                {
                    //TODO: Consolodate debug strings
                    Debug.Log("<color=yellow>" + go + "</color> : does not have a scale of 1,1,1. Scale is :" + go.transform.localScale, go);
                }
            }
        }
#endif
    }
}