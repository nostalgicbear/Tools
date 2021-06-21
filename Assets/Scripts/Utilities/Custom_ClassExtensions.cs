using UnityEngine;

    public static class Custom_ClassExtensions
    {
        /// <summary>
        /// Returns the full path to the object in the Hierarchy (Eg parent/child/object)
        /// </summary>
        /// <param name="t">The transform component</param>
        /// <returns></returns>
        public static string GetHierarchyPath(this Transform t)
        {
            string path = t.gameObject.name;
            Transform parent = t.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }

