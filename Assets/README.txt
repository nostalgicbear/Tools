Topics covered in these examples below 
- Custom assembly definitions
- Editor scripting
- Hooking into Post build events
- Serialization
- Unity lightmapping shortcomings
- Custom LOD management

Stephen Long - PERSONAL CHANGELOG FOR MESH BAKER
This project contains Mesh Baker, an asset used for combining meshes in the interest of optimization. Mesh Baker has an assembly in its Editor folder (MeshBaker/Editor). I have created a custom assembly (Custom_Utilities) for interfacing
with this. Generally, I will separate out code into assemblies where possible in the interest of both decoupling, and reduced compile times. 

This is a custom version of Mesh Baker created to support LODing and the transfer of lightmap data between objects. Mesh Baker does not work properly with LODs. It creates non-unified objects in the hierarchy, with incorrectly generated
LODs that are not at all what is expected. This custom version allows you to easily combine meshes together, generating the result with LOD groups that are grouped neatly in the hierarchy. 
Another issue this navigates is how Unity stores lightmap data. When you duplicate an object, Unity does not transfer the the lightmap data from the original mesh to the duplicate. This handles that too and allows us to specify what 
mesh a given mesh should take its lightmap data from. This is useful in large scenes where you want to duplicate objects, but do not want to wait for a new lightmap to be generated. 

Listed below are some issues, and the implemented solutions.  


MESH BAKER TEXTURE ARRAY ISSUES
- When using texture arrays rather than a texture atlas, MB created a single slice containing all material data. Modified it so there is an option to create individual slice entries for each material.
- There was no way of toggling the Consider UV option for all elements in the slices array. Added that functionality.

MESH BAKER (AND UNITY IN GENERAL) LIGHTMAP ISSUES
- The preserve_current_lightmapping option for the Lightmapping UVs section on a MeshBaker component does not work in versions of Unity beyond Unity 5. Created aCustomManageGeneratedMeshs() function in  MB3_MeshBakerEditorFunctions.cs to handle this, allowing the ability to have lightmaps passed from source to baked object. (Inc adding a custom LightMapDataHandler component to the resulting combined mesh)
- Mesh Baker allowed you to select objects based on lightmap index, but not based on whether an object was lightmapped or not
- Selecting an object by lightmap index was awkward and not user friendly. Changed it to use a more user friendly UI that does not involve a 256 element long dropdown

MESH BAKER LOD UPDATES
- Mesh Baker creates individual LOD, ungrouped objects when clustering by LODs meaning the resulting LOD 0, 1, 2 etc are in no way related to each other. This amends the resulting LOD clusters, groups them under a new object, applies a LOD Group to the object, and applies the resulting combined meshes to each LOD level of the parents LOD Group. It also updates the original set of Mesh Bakers to have the correct references to the updated objects. 
- Added custom button to TextureBakerEditorInternal to apply changes to the list of objects provided with regard to incorrect/missing LODs (filter not covered in detail here) to avoid needing to do it manually on a per object basis. 
- Added a menu function that creates a TextureBaker+MeshBaker object with desired settings already set to avoid manually iterating over 40+ settings.


DETAILED SCRIPT CHANGES 
MeshBaker\Editor\MB_TextureBakerEditorConfigureTextureArrays.cs
- Added button to allow creating slices array with an individual entry for each material (Button name - Custom : Build Texture Array Slices From Objects To Be Combined)
- Added boolean (bool separateSlices) to ConfigureTextureArraysFromObsToCombine which allows us to specify if slices texture array should contain individual materials
- Body of ConfigureTextureArraysFromObsToCombine() function ammended to populate Slice array based on the above criteria
- Added button for toggling Mesh UVs on entries in Slice array (Button name - Toggle Consider Mesh UVs)

MeshBaker/Editor/MB3_MeshBakerEditorWindowAddObjectsTab.cs 
- Replaced lightmap int array with enum (DONT_FILTER_BY_LIGHTMAP, NOT_LIGHTMAPPED, HAS_LIGHTMAP, HAS_SPECIFIC_LIGHTMAP_INDEX)
- Added switch statement that determines whether an object is added to the selected objects list based on the above criteria
- Replaced the 256 element long lightmap index list with a slider to more easily select the lightmap index
- Updated the OnGUI() function to only show the slider if HAS_SPECIFIC_LIGHTMAP is selected before hand

MeshBaker/Editor/core/MB3_MeshBakerEditorFunctions.cs
- CustomManageGeneratedCombinedMeshes() added to MB3_MeshBakerEditorFunctions.cs for doing the vast majority of changes to the generated MeshBaker combined objects (not covered in detail here as its an encapsulated in the function).
- Ammended _BakeIntoCombinedSceneObject() function in MB3_MeshBakerEditorFunctions.cs to call our custom function if correct settings are set (eg must be clustering by LOD and triggered through a MeshBakerGrouper as opposed to MeshBaker object)

TextureBakerEditorInternal.cs
- OnGUI() now has a button titled "Custom: Filter incorrect LODs" that a call to our custom Utilities class for filtering

MB3_MeshBakerEditor.cs
- Added CreateNewMeshBaker() function to MB3_MeshBakerEditor to have menu item that calls function that creates the MeshBaker object with desired settings.
- Updated the MeshBaker/Editor assembly so it knows about our custom Custom_Utilities assembly.


--------------- CACHE MANAGEMENT --------------
Unity has many a downfall, and one of them is the fact that a lot of actions generate a lot of garbage. Its Library/ShaerCache directory, and its Library/shadercache.db file can
get abnormally large (> 12 GB). For example, after doing an Android build, these can become massively inflated. This leads to massive slowdown when working with materials and shaders. Eg if you select a material and want to change its shader, it can cause 
delays of upward of 20 -30 seconds per change. I have created a config that allows you to manage this. It tracks the size of the above, and allows you to specify when to delete 
them (they auto generate when needed). I hook into the post build events to allow you to delete after a build, or you can specify to have it auto delete when closing Unity.
I have also given the option to auto delete they go past a specified size. In future I will probably write a batch script to do the above before launching.  

------------- Lightmaps --------------
Unity handles lightmapping on a per scene basis, meaning that duplicating objects means their lightmap data is not also duplicated. LightmapDataHolder.cs allows us to take the 
lightmap data from a Renderer and apply that to the renderer of another object. Why is that useful? Its useful for the purposes of fast iteration and prototyping. Waiting on Unity
to generate a lightmap for a new scene takes a long time (depending on the size of the scene, the selected lightmapper etc). 