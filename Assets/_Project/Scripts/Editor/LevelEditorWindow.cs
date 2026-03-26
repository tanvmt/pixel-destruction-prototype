using UnityEngine;
using UnityEditor;
using PixelDestruction.Core;
using System.Collections.Generic;

namespace PixelDestruction.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData selectedLevelData;
        private Transform spawnPointMarker;
        private Transform dynamicContainer;
        private Transform staticContainer;

        private SerializedObject serializedLevelData; 
        private Vector2 scrollPos;

        private string newLevelName = "";

        [MenuItem("Tools/Pixel Destruction/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            if (spawnPointMarker == null)
            {
                GameObject spawnObj = GameObject.Find("SpawnMarker");
                if (spawnObj != null) spawnPointMarker = spawnObj.transform;
            }

            if (dynamicContainer == null)
            {
                GameObject dynObj = GameObject.Find("DynamicContainer"); 
                if (dynObj != null) dynamicContainer = dynObj.transform;
            }

            if (staticContainer == null)
            {
                GameObject staObj = GameObject.Find("StaticContainer");
                if (staObj != null) staticContainer = staObj.transform;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Drag & Drop Level Design Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 1. LEVEL DATA SELECTION
            EditorGUI.BeginChangeCheck();
            selectedLevelData = (LevelData)EditorGUILayout.ObjectField("Target Level Data:", selectedLevelData, typeof(LevelData), false);
            if (EditorGUI.EndChangeCheck() && selectedLevelData != null)
            {
                serializedLevelData = new SerializedObject(selectedLevelData);
            }

            EditorGUILayout.Space(5); 

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Create New Level:"); 
            
            newLevelName = EditorGUILayout.TextField(newLevelName); 
            
            if (GUILayout.Button(new GUIContent("Create", "Leave empty to auto-generate name"), GUILayout.Width(80)))
            {
                CreateNewLevelData();
                GUI.FocusControl(null); 
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 2. CONTAINER SETUP
            EditorGUILayout.LabelField("Container Setup", EditorStyles.boldLabel);
            spawnPointMarker = (Transform)EditorGUILayout.ObjectField("Spawn Marker:", spawnPointMarker, typeof(Transform), true);
            dynamicContainer = (Transform)EditorGUILayout.ObjectField("Dynamic Container (Obstacles):", dynamicContainer, typeof(Transform), true);
            staticContainer = (Transform)EditorGUILayout.ObjectField("Static Container (Walls/Floors):", staticContainer, typeof(Transform), true);
            EditorGUILayout.Space();

            // START SCROLL VIEW
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // 3. GAMEPLAY SETTINGS
            if (selectedLevelData != null && serializedLevelData != null)
            {
                EditorGUILayout.LabelField("Gameplay & Spawner Rules", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                
                serializedLevelData.Update();
                EditorGUILayout.PropertyField(serializedLevelData.FindProperty("maxConcurrentObjects"), new GUIContent("Max Concurrent Objects"));
                EditorGUILayout.PropertyField(serializedLevelData.FindProperty("spawnDelay"), new GUIContent("Spawn Delay (s)"));
                EditorGUILayout.PropertyField(serializedLevelData.FindProperty("requiredObjectsToDestroy"), new GUIContent("Target Objects to Destroy"));
                EditorGUILayout.PropertyField(serializedLevelData.FindProperty("allowedWeaponPrefab"), new GUIContent("Allowed Weapon"));
                
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedLevelData.FindProperty("texturesToSpawn"), new GUIContent("Textures to Spawn"), true);
                
                serializedLevelData.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("Please select or create a new Level Data to configure parameters!", MessageType.Warning);
            }

            EditorGUILayout.Space();
            
            // 4. SCENE LAYOUT ACTIONS
            EditorGUILayout.LabelField("Environment Setup (Scene Layout)", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Load Data to Scene"))
            {
                LoadLevelDataToScene();
            }
            
            EditorGUILayout.Space();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("2. Scan Scene & Save to Data", GUILayout.Height(30)))
            {
                SaveSceneToLevelData();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("3. Clear Containers"))
            {
                ClearContainer();
            }
            
            EditorGUILayout.Space();
            
            // HELP BOX
            EditorGUILayout.HelpBox("Level Editor Instructions:\n\n" +
                "1. Assign a Target Level Data, or click 'Create New' to generate one.\n" +
                "2. Assign empty GameObjects from your Scene into the Dynamic and Static Container fields above.\n" +
                "3. Drag and drop Prefabs (Walls, Rocks, Saws) from your Project window into the containers on the Scene and arrange them.\n" +
                "4. Click 'Scan Scene & Save to Data' (Button 2) to save your layout.\n\n" +
                "*IMPORTANT: Always use valid Prefabs from the Project window. Do not create raw 3D Cubes directly in the Scene, as the tool requires Prefab references to save!*", MessageType.Info);

            EditorGUILayout.EndScrollView(); 
        }

        private void SaveSceneToLevelData()
        {
            if (selectedLevelData == null || dynamicContainer == null || staticContainer == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign the Level Data and both Containers!", "OK");
                return;
            }

            // Scan both containers
            if (spawnPointMarker != null)
            {
                selectedLevelData.spawnPosition = spawnPointMarker.position;
            }
            selectedLevelData.obstacles = ScanContainer(dynamicContainer);
            selectedLevelData.walls = ScanContainer(staticContainer);

            EditorUtility.SetDirty(selectedLevelData);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", "Successfully saved all Walls and Obstacles to the Level Data!", "OK");
        }

        private List<LevelData.ObjectData> ScanContainer(Transform container)
        {
            List<LevelData.ObjectData> resultList = new List<LevelData.ObjectData>();
            foreach (Transform child in container)
            {
                GameObject prefabObj = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (prefabObj != null)
                {
                    resultList.Add(new LevelData.ObjectData
                    {
                        position = new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                        scale = new Vector3(child.localScale.x, child.localScale.y, child.localScale.z),
                        rotation = child.localRotation,
                        prefab = prefabObj
                    });
                }
            }
            return resultList;
        }

        private void LoadLevelDataToScene()
        {
            ClearContainer(); 
            SpawnFromList(selectedLevelData.obstacles, dynamicContainer);
            SpawnFromList(selectedLevelData.walls, staticContainer); 
        }

        private void SpawnFromList(List<LevelData.ObjectData> list, Transform container)
        {
            if (list == null || container == null) return;
            foreach (var item in list)
            {
                if (item.prefab != null)
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(item.prefab, container);
                    
                    if (obj != null)
                    {
                        obj.transform.localPosition = new Vector3(item.position.x, item.position.y, item.position.z);
                        obj.transform.localScale = new Vector3(item.scale.x, item.scale.y, item.scale.z);
                        obj.transform.localRotation = item.rotation;
                    }
                }
            }
        }

        private void ClearContainer()
        {
            if (dynamicContainer == null || staticContainer == null) return;

            for (int i = staticContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(staticContainer.GetChild(i).gameObject);
            }

            for (int i = dynamicContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(dynamicContainer.GetChild(i).gameObject);
            }

            Debug.Log("[Level Editor] Containers cleared successfully!");
        }

        private void CreateNewLevelData()
        {
            string folderPath = "Assets/_Project/Data/Levels";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            string fileName = newLevelName.Trim();
            if (string.IsNullOrEmpty(fileName))
            {
                string[] existingLevels = AssetDatabase.FindAssets("t:LevelData");
                fileName = $"Level_{existingLevels.Length + 1}";
            }

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{fileName}.asset");

            LevelData newData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(newData, assetPath);
            AssetDatabase.SaveAssets();

            selectedLevelData = newData;
            serializedLevelData = new SerializedObject(newData);
            newLevelName = "";

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newData;

            Debug.Log($"[Level Editor] Created new LevelData at: {assetPath}");
        }
    }
}