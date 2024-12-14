#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HierarchyDesigner_Shared_Operations
    {
        #region General
        private static IEnumerable<GameObject> GetAllGameObjectsInActiveScene()
        {
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            List<GameObject> allGameObjects = new();

            Stack<GameObject> stack = new(rootObjects);
            while (stack.Count > 0)
            {
                GameObject current = stack.Pop();
                allGameObjects.Add(current);
                foreach (Transform child in current.transform) { stack.Push(child.gameObject); }
            }
            return allGameObjects;
        }

        private static bool IsGameObjectActive(GameObject gameObject)
        {
            if (gameObject == null) 
            { 
                return false;
            }
            return gameObject.activeSelf;
        }

        private static bool IsGameObjectParent(GameObject gameObject)
        {
            return gameObject.transform.childCount > 0;
        }

        private static bool IsGameObjectEmpty(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();
            return components.Length == 1 && (components[0] is Transform || components[0] is RectTransform);
        }

        public static bool IsGameObjectLocked(GameObject gameObject)
        {
            if (gameObject == null)
            { 
                return false;
            }
            return (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable;
        }

        private static bool IsGameObjectFolder(GameObject gameObject)
        {
            return gameObject.GetComponent<HierarchyDesignerFolder>();
        }

        private static bool IsGameObjectSeparator(GameObject gameObject)
        {
            return gameObject.CompareTag(HierarchyDesigner_Shared_Constants.SeparatorTag) && gameObject.name.StartsWith(HierarchyDesigner_Shared_Constants.SeparatorPrefix);
        }

        private static bool ShouldIncludeFolderAndSeparator(GameObject gameObject, bool addFolderToCount, bool addSeparatorToCount)
        {
            if (!addFolderToCount && IsGameObjectFolder(gameObject)) 
            {
                return false;
            }

            if (!addSeparatorToCount && IsGameObjectSeparator(gameObject)) 
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Folder
        public static void CreateFolder(string folderName, bool shouldRename)
        {
            GameObject folder = new(folderName);
            folder.AddComponent<HierarchyDesignerFolder>();
            EditorGUIUtility.SetIconForObject(folder, HierarchyDesigner_Shared_Resources.FolderInspectorIcon);
            if (shouldRename) EditorApplication.delayCall += () => BeginRename(folder);
            Undo.RegisterCreatedObjectUndo(folder, $"Create {folderName}");
        }

        public static void BeginRename(GameObject obj)
        {
            Selection.activeObject = obj;
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
        }

        public static bool FolderExists(string folderName)
        {
            #if UNITY_6000_0_OR_NEWER
            Transform[] allTransforms = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            #else
            Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>(true);
            #endif
            foreach (Transform t in allTransforms)
            {
                if (t.gameObject.GetComponent<HierarchyDesignerFolder>() && t.gameObject.name.Equals(folderName))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Separator
        public static void CreateSeparator(string separatorName)
        {
            GameObject separator = new($"{HierarchyDesigner_Shared_Constants.SeparatorPrefix}{separatorName}");
            separator.tag = HierarchyDesigner_Shared_Constants.SeparatorTag;
            SetSeparatorState(separator, false);
            separator.SetActive(false);
            EditorGUIUtility.SetIconForObject(separator, HierarchyDesigner_Shared_Resources.SeparatorInspectorIcon);
            Undo.RegisterCreatedObjectUndo(separator, $"Create {separatorName}");
        }

        public static void SetSeparatorState(GameObject gameObject, bool editable)
        {
            foreach (Component component in gameObject.GetComponents<Component>())
            {
                if (component)
                {
                    component.hideFlags = editable ? HideFlags.None : HideFlags.NotEditable;
                }
            }
            gameObject.hideFlags = editable ? HideFlags.None : HideFlags.NotEditable;
            gameObject.transform.hideFlags = HideFlags.HideInInspector;
            EditorUtility.SetDirty(gameObject);
        }

        public static bool SeparatorExists(string separatorName)
        {
            string fullSeparatorName = HierarchyDesigner_Shared_Constants.SeparatorPrefix + separatorName;
            #if UNITY_6000_0_OR_NEWER
            Transform[] allTransforms = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            #else
            Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>(true);
            #endif
            foreach (Transform t in allTransforms)
            {
                if (t.gameObject.CompareTag(HierarchyDesigner_Shared_Constants.SeparatorTag) && t.gameObject.name.Equals(fullSeparatorName))
                {
                    return true;
                }
            }
            return false;
        }

        public static string StripPrefix(string name)
        {
            if (name.StartsWith(HierarchyDesigner_Shared_Constants.SeparatorPrefix))
            {
                return name[2..].Trim();
            }
            return name.Trim();
        }
        #endregion

        #region GameObject
        public static void RefreshAllGameObjectsData()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectMainIcon &&
                !HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectComponentIcons &&
                !HierarchyDesigner_Configurable_GeneralSettings.EnableHierarchyTree &&
                !HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectTag &&
                !HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectLayer)
            {
                Debug.Log("No GameObject data is enabled for refreshing.");
                return;
            }

            #if UNITY_6000_0_OR_NEWER
            GameObject[] allGameObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            #else
            GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            #endif

            foreach (GameObject gameObject in allGameObjects)
            {
                RefreshGameObjectData(gameObject);
            }
        }

        public static void RefreshSelectedGameObjectsData()
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    RefreshGameObjectData(selectedGameObject);
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to refresh their data.");
            }
        }

        public static void RefreshMainIconForSelectedGameObject()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectMainIcon) return;

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    int instanceID = selectedGameObject.GetInstanceID();
                    if (HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
                    {
                        data.MainIcon = HierarchyDesigner_Manager_GameObject.GetGameObjectMainIcon(selectedGameObject);
                        HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select a GameObject to refresh its main icon.");
            }
        }

        public static void RefreshComponentIconsForSelectedGameObjects()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectComponentIcons) return;

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    int instanceID = selectedGameObject.GetInstanceID();
                    if (HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
                    {
                        data.ComponentIcons = HierarchyDesigner_Manager_GameObject.GetComponentIcons(selectedGameObject);
                        HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to refresh their component icons.");
            }
        }

        public static void RefreshHierarchyTreeIconForSelectedGameObjects()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableHierarchyTree) return;

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    int instanceID = selectedGameObject.GetInstanceID();
                    if (HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
                    {
                        data.HierarchyTreeIcon = HierarchyDesigner_Manager_GameObject.GetOrCreateBranchIcon(selectedGameObject.transform);
                        HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to refresh their hierarchy tree icons.");
            }
        }

        public static void RefreshTagForSelectedGameObjects()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectTag) return;

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    int instanceID = selectedGameObject.GetInstanceID();
                    if (HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
                    {
                        data.Tag = selectedGameObject.tag;
                        HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to refresh their tags.");
            }
        }

        public static void RefreshLayerForSelectedGameObjects()
        {
            if (!HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectLayer) return;

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0)
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    int instanceID = selectedGameObject.GetInstanceID();
                    if (HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
                    {
                        data.Layer = LayerMask.LayerToName(selectedGameObject.layer);
                        HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to refresh their layers.");
            }
        }

        public static void RefreshGameObjectData(GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            if (!HierarchyDesigner_Manager_GameObject.gameObjectDataCache.TryGetValue(instanceID, out HierarchyDesigner_Manager_GameObject.GameObjectData data))
            {
                data = new HierarchyDesigner_Manager_GameObject.GameObjectData();
            }

            if (HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectMainIcon)
            {
                data.MainIcon = HierarchyDesigner_Manager_GameObject.GetGameObjectMainIcon(gameObject);
            }

            if (HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectComponentIcons)
            {
                data.ComponentIcons = HierarchyDesigner_Manager_GameObject.GetComponentIcons(gameObject);
            }

            if (HierarchyDesigner_Configurable_GeneralSettings.EnableHierarchyTree && gameObject.transform.parent != null)
            {
                data.HierarchyTreeIcon = HierarchyDesigner_Manager_GameObject.GetOrCreateBranchIcon(gameObject.transform);
            }

            if (HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectTag)
            {
                data.Tag = gameObject.tag;
            }

            if (HierarchyDesigner_Configurable_GeneralSettings.EnableGameObjectLayer)
            {
                data.Layer = LayerMask.LayerToName(gameObject.layer);
            }

            HierarchyDesigner_Manager_GameObject.gameObjectDataCache[instanceID] = data;
        }
        #endregion

        #region Tools
        #region Activate
        public static void SetActiveState(GameObject gameObject, bool isActive)
        {
            if (IsGameObjectSeparator(gameObject)) return;
            Undo.RegisterCompleteObjectUndo(gameObject, $"{(isActive ? "Activate" : "Deactivate")} GameObject");

            if (gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
                EditorUtility.SetDirty(gameObject);
            }
        }

        public static void Activate_SelectedGameObjects(bool activeState)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length < 1)
            {
                Debug.Log($"No GameObject(s) selected. Please select at least one GameObject to {(activeState ? "activate" : "deactivate")}.");
                return;
            }

            foreach (GameObject gameObject in selectedGameObjects)
            {
                if (IsGameObjectActive(gameObject) != activeState)
                {
                    SetActiveState(gameObject, activeState);
                }
            }
        }

        public static void Activate_AllGameObjects(bool activeState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                SetActiveState(gameObject, activeState);
            }
        }

        public static void Activate_AllParentGameObjects(bool activeState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectParent(gameObject))
                {
                    SetActiveState(gameObject, activeState);
                }
            }
        }

        public static void Activate_AllEmptyGameObjects(bool activeState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectEmpty(gameObject))
                {
                    SetActiveState(gameObject, activeState);
                }
            }
        }

        public static void Activate_AllLockedGameObjects(bool activeState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectLocked(gameObject))
                {
                    SetActiveState(gameObject, activeState);
                }
            }
        }

        public static void Activate_AllFolders(bool activeState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectFolder(gameObject))
                {
                    SetActiveState(gameObject, activeState);
                }
            }
        }

        public static void Activate_AllComponentOfType<T>(bool isActive) where T : Component
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                if (gameObject.GetComponent<T>() != null)
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_All2DSpritesByType(string spriteType, bool isActive)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null && spriteRenderer.sprite.name.Contains(spriteType))
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_AllPhysicsDynamicSprites(bool isActive)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                if (spriteRenderer != null && rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_AllPhysicsStaticSprites(bool isActive)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                if (spriteRenderer != null && rigidbody2D != null && (rigidbody2D.bodyType == RigidbodyType2D.Static || rigidbody2D.bodyType == RigidbodyType2D.Kinematic))
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_All3DObjectsByType(string objectType, bool isActive)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.Contains(objectType))
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_AllHalos(bool isActive)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
                if (halo != null)
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_AllComponentOfType<T>(bool isActive, Func<T, bool> predicate = null) where T : Component
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                T component = gameObject.GetComponent<T>();
                if (component != null && (predicate == null || predicate(component)))
                {
                    SetActiveState(gameObject, isActive);
                }
            }
        }

        public static void Activate_AllTMPComponentIfAvailable<T>(bool isActive) where T : Component
        {
            if (HierarchyDesigner_Shared_Checker.IsTMPAvailable())
            {
                Activate_AllComponentOfType<T>(isActive);
            }
            else
            {
                EditorUtility.DisplayDialog("TMP Not Found", "TMP wasn't found in the project, make sure you have it enabled.", "OK");
            }
        }
        #endregion

        #region Count
        public static void CountGameObjects(string description, Func<GameObject, bool> predicate, bool includeFolders = false, bool includeSeparators = false)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            int count = 0;
            string gameObjectNames = "";
            bool addFolderToCount = includeFolders || !HierarchyDesigner_Configurable_AdvancedSettings.ExcludeFoldersFromCountSelectToolCalculations;
            bool addSeparatorToCount = includeSeparators || !HierarchyDesigner_Configurable_AdvancedSettings.ExcludeSeparatorsFromCountSelectToolCalculations;

            foreach (GameObject gameObject in allGameObjects)
            {
                if (!ShouldIncludeFolderAndSeparator(gameObject, addFolderToCount, addSeparatorToCount)) { continue; }
                if (predicate(gameObject))
                {
                    count++;
                    gameObjectNames += gameObject.name + ", ";
                }
            }
            gameObjectNames = count == 0 ? "none" : gameObjectNames.TrimEnd(',', ' ');
            Debug.Log($"Total <color=#73FF7A>{description}</color> in the scene: <b>{count}</b>\n<i>All {description} found:</i> <b>{gameObjectNames}</b>.\n");
        }

        public static void Count_SelectedGameObjects()
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length < 1)
            {
                Debug.Log($"No GameObject(s) selected. Please select at least one GameObject to count.");
                return;
            }

            int count = selectedGameObjects.Length;
            string selectedGameObjectNames = "";
            for (int i = 0; i < selectedGameObjects.Length; i++)
            {
                selectedGameObjectNames += selectedGameObjects[i].name;
                if (i < selectedGameObjects.Length - 1)
                {
                    selectedGameObjectNames += ", ";
                }
            }
            Debug.Log($"Total <color=#73FF7A>Selected GameObjects</color> in the scene: <b>{count}</b>\n<i>All <Selected GameObjects> found:</i> <b>{selectedGameObjectNames}</b>.\n");
        }

        public static void Count_AllGameObjects()
        {
            CountGameObjects("GameObjects", gameObject => true);
        }

        public static void Count_AllParentGameObjects()
        {
            CountGameObjects("Parent GameObjects", IsGameObjectParent);
        }

        public static void Count_AllEmptyGameObjects()
        {
            CountGameObjects("Empty GameObjects", IsGameObjectEmpty);
        }

        public static void Count_AllLockedGameObjects()
        {
            CountGameObjects("Locked GameObjects", IsGameObjectLocked);
        }

        public static void Count_AllActiveGameObjects()
        {
            CountGameObjects("Active GameObjects", IsGameObjectActive);
        }

        public static void Count_AllInactiveGameObjects()
        {
            CountGameObjects("Inactive GameObjects", gameObject => !IsGameObjectActive(gameObject));
        }

        public static void Count_AllFolders()
        {
            CountGameObjects("Folders", IsGameObjectFolder, includeFolders: true);
        }

        public static void Count_AllSeparators()
        {
            CountGameObjects("Separators", IsGameObjectSeparator, includeSeparators: true);
        }

        public static void Count_AllComponentOfType<T>(string componentName) where T : Component
        {
            CountGameObjects($"{componentName}", gameObject => gameObject.GetComponent<T>() != null);
        }

        public static void Count_All2DSpritesByType(string spriteType)
        {
            CountGameObjects($"2D {spriteType} sprites", gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                return spriteRenderer != null && spriteRenderer.sprite != null && spriteRenderer.sprite.name.Contains(spriteType);
            });
        }

        public static void Count_AllPhysicsDynamicSprites()
        {
            CountGameObjects("2D Dynamic physics sprites", gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return spriteRenderer != null && rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
            });
        }

        public static void Count_AllPhysicsStaticSprites()
        {
            CountGameObjects("2D Static physics sprites", gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return spriteRenderer != null && rigidbody2D != null && (rigidbody2D.bodyType == RigidbodyType2D.Static || rigidbody2D.bodyType == RigidbodyType2D.Kinematic);
            });
        }

        public static void Count_All3DObjectsByType(string objectType)
        {
            CountGameObjects($"3D {objectType} objects", gameObject =>
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                return meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.Contains(objectType);
            });
        }

        public static void Count_AllHalos()
        {
            CountGameObjects("Halos", gameObject =>
            {
                Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
                return halo != null;
            });
        }

        public static void Count_AllComponentOfType<T>(string componentName, Func<T, bool> predicate = null) where T : Component
        {
            CountGameObjects($"{componentName}", gameObject =>
            {
                T component = gameObject.GetComponent<T>();
                return component != null && (predicate == null || predicate(component));
            });
        }

        public static void Count_AllTMPComponentIfAvailable<T>(string componentName) where T : Component
        {
            if (HierarchyDesigner_Shared_Checker.IsTMPAvailable())
            {
                Count_AllComponentOfType<T>(componentName);
            }
            else
            {
                EditorUtility.DisplayDialog("TMP Not Found", "TMP wasn't found in the project, make sure you have it enabled.", "OK");
            }
        }
        #endregion

        #region Lock 
        public static void LockGameObject(GameObject gameObject, bool isLocked)
        {
            SetLockState(gameObject, !isLocked);
        }

        public static void SetLockState(GameObject gameObject, bool editable)
        {
            if (IsGameObjectSeparator(gameObject)) return;
            Undo.RegisterCompleteObjectUndo(gameObject, $"{(editable ? "Unlock" : "Lock")} GameObject");

            HideFlags newFlags = editable ? HideFlags.None : HideFlags.NotEditable;
            if (gameObject.hideFlags != newFlags)
            {
                gameObject.hideFlags = newFlags;
                EditorUtility.SetDirty(gameObject);
            }

            foreach (Component component in gameObject.GetComponents<Component>())
            {
                if (component && component.hideFlags != newFlags)
                {
                    component.hideFlags = newFlags;
                    EditorUtility.SetDirty(component);
                }
            }

            if (editable)
            {
                SceneVisibilityManager.instance.EnablePicking(gameObject, true);
            }
            else
            {
                SceneVisibilityManager.instance.DisablePicking(gameObject, true);
            }

            EditorWindow[] allEditorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (EditorWindow inspector in allEditorWindows)
            {
                if (inspector.GetType().Name == HierarchyDesigner_Shared_Constants.InspectorWindow)
                {
                    inspector.Repaint();
                }
            }
        }

        public static void Lock_SelectedGameObjects(bool lockState)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length < 1)
            {
                Debug.Log($"No GameObject(s) selected. Please select at least one GameObject to {(lockState ? "lock" : "unlock")}.");
                return;
            }

            foreach (GameObject gameObject in selectedGameObjects)
            {
                if (IsGameObjectLocked(gameObject) != lockState)
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllGameObjects(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                SetLockState(gameObject, !lockState);
            }
        }

        public static void Lock_AllParentGameObjects(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectParent(gameObject))
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllEmptyGameObjects(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectEmpty(gameObject))
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllActiveGameObjects(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectActive(gameObject))
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllInactiveGameObjects(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (!IsGameObjectActive(gameObject))
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllFolders(bool lockState)
        {
            foreach (GameObject gameObject in GetAllGameObjectsInActiveScene())
            {
                if (IsGameObjectFolder(gameObject))
                {
                    SetLockState(gameObject, !lockState);
                }
            }
        }

        public static void Lock_AllComponentOfType<T>(bool isLocked) where T : Component
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                if (gameObject.GetComponent<T>() != null)
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_All2DSpritesByType(string spriteType, bool isLocked)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null && spriteRenderer.sprite.name.Contains(spriteType))
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_AllPhysicsDynamicSprites(bool isLocked)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                if (spriteRenderer != null && rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_AllPhysicsStaticSprites(bool isLocked)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                if (spriteRenderer != null && rigidbody2D != null && (rigidbody2D.bodyType == RigidbodyType2D.Static || rigidbody2D.bodyType == RigidbodyType2D.Kinematic))
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_All3DObjectsByType(string objectType, bool isLocked)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.Contains(objectType))
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_AllHalos(bool isLocked)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
                if (halo != null)
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_AllComponentOfType<T>(bool isLocked, Func<T, bool> predicate = null) where T : Component
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            foreach (GameObject gameObject in allGameObjects)
            {
                T component = gameObject.GetComponent<T>();
                if (component != null && (predicate == null || predicate(component)))
                {
                    LockGameObject(gameObject, isLocked);
                }
            }
        }

        public static void Lock_AllTMPComponentIfAvailable<T>(bool isLocked) where T : Component
        {
            if (HierarchyDesigner_Shared_Checker.IsTMPAvailable())
            {
                Lock_AllComponentOfType<T>(isLocked);
            }
            else
            {
                EditorUtility.DisplayDialog("TMP Not Found", "TMP wasn't found in the project, make sure you have it enabled.", "OK");
            }
        }
        #endregion

        #region Rename
        public static void Rename_SelectedGameObjects(string sortingActionDescription, bool autoIndex)
        {
            List<GameObject> selectedGameObjects = new(Selection.gameObjects);
            if (selectedGameObjects.Count < 1)
            {
                Debug.Log($"No GameObject(s) selected. Please select at least one GameObject to {sortingActionDescription}.");
                return;
            }

            HierarchyDesigner_Window_Rename.OpenWindow(selectedGameObjects, autoIndex, 0);
        }

        public static void Rename_OpenRenameToolWindow()
        {
            HierarchyDesigner_Window_Rename.OpenWindow(null, true, 0);
        }
        #endregion

        #region Select
        public static void SelectGameObjects(string description, Func<GameObject, bool> predicate, bool includeFolders = false, bool includeSeparators = false)
        {
            IEnumerable<GameObject> allGameObjects = GetAllGameObjectsInActiveScene();
            List<GameObject> selectedGameObjects = new();
            bool addFolderToSelect = includeFolders || !HierarchyDesigner_Configurable_AdvancedSettings.ExcludeFoldersFromCountSelectToolCalculations;
            bool addSeparatorToSelect = includeSeparators || !HierarchyDesigner_Configurable_AdvancedSettings.ExcludeSeparatorsFromCountSelectToolCalculations;

            foreach (GameObject gameObject in allGameObjects)
            {
                if (!ShouldIncludeFolderAndSeparator(gameObject, addFolderToSelect, addSeparatorToSelect)) continue;

                if (predicate(gameObject))
                {
                    selectedGameObjects.Add(gameObject);
                }
            }

            SelectOrDisplayMessage(selectedGameObjects, description);
        }

        public static void SelectOrDisplayMessage(List<GameObject> gameObjects, string message)
        {
            if (gameObjects.Count > 0)
            {
                FocusHierarchyWindow();
                Selection.objects = gameObjects.ToArray();
            }
            else
            {
                Debug.Log($"No {message} found in the current scene.");
            }
        }

        public static void FocusHierarchyWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        }

        public static void Select_AllGameObjects()
        {
            SelectGameObjects("GameObjects", gameObject => true);
        }

        public static void Select_AllParentGameObjects()
        {
            SelectGameObjects("Parent GameObjects", IsGameObjectParent);
        }

        public static void Select_AllEmptyGameObjects()
        {
            SelectGameObjects("Empty GameObjects", IsGameObjectEmpty);
        }

        public static void Select_AllLockedGameObjects()
        {
            SelectGameObjects("Locked GameObjects", IsGameObjectLocked);
        }

        public static void Select_AllActiveGameObjects()
        {
            SelectGameObjects("Active GameObjects", IsGameObjectActive);
        }

        public static void Select_AllInactiveGameObjects()
        {
            SelectGameObjects("Inactive GameObjects", gameObject => !IsGameObjectActive(gameObject));
        }

        public static void Select_AllFolders()
        {
            SelectGameObjects("Folders", IsGameObjectFolder, includeFolders: true);
        }

        public static void Select_AllSeparators()
        {
            SelectGameObjects("Separators", IsGameObjectSeparator, includeSeparators: true);
        }

        public static void Select_AllComponentOfType<T>() where T : Component
        {
            SelectGameObjects(typeof(T).Name, gameObject => gameObject.GetComponent<T>() != null);
        }

        public static void Select_All2DSpritesByType(string spriteType)
        {
            SelectGameObjects(spriteType, gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                return spriteRenderer != null && spriteRenderer.sprite != null && spriteRenderer.sprite.name.Contains(spriteType);
            });
        }

        public static void Select_AllPhysicsDynamicSprites()
        {
            SelectGameObjects("Dynamic Sprites", gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return spriteRenderer != null && rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
            });
        }

        public static void Select_AllPhysicsStaticSprites()
        {
            SelectGameObjects("Static Sprites", gameObject =>
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return spriteRenderer != null && rigidbody2D != null && (rigidbody2D.bodyType == RigidbodyType2D.Static || rigidbody2D.bodyType == RigidbodyType2D.Kinematic);
            });
        }

        public static void Select_All3DObjectsByType(string objectType)
        {
            SelectGameObjects(objectType, gameObject =>
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                return meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.Contains(objectType);
            });
        }

        public static void Select_AllHalos()
        {
            SelectGameObjects("Halos", gameObject =>
            {
                Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
                return halo != null;
            });
        }

        public static void Select_AllComponentOfType<T>(Func<T, bool> predicate) where T : Component
        {
            SelectGameObjects(typeof(T).Name, gameObject =>
            {
                T component = gameObject.GetComponent<T>();
                return component != null && predicate(component);
            });
        }

        public static void Select_AllTMPComponentIfAvailable<T>() where T : Component
        {
            if (HierarchyDesigner_Shared_Checker.IsTMPAvailable())
            {
                Select_AllComponentOfType<T>();
            }
            else
            {
                EditorUtility.DisplayDialog("TMP Not Found", "TMP wasn't found in the project, make sure you have it enabled.", "OK");
            }
        }
        #endregion

        #region Sort
        public static int AlphanumericComparison(GameObject x, GameObject y)
        {
            string xName = x.name;
            string yName = y.name;

            string[] xParts = System.Text.RegularExpressions.Regex.Split(xName.Replace(" ", ""), "([0-9]+)");
            string[] yParts = System.Text.RegularExpressions.Regex.Split(yName.Replace(" ", ""), "([0-9]+)");

            for (int i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
            {
                if (int.TryParse(xParts[i], out int xPartNum) && int.TryParse(yParts[i], out int yPartNum))
                {
                    if (xPartNum != yPartNum) return xPartNum.CompareTo(yPartNum);
                }
                else
                {
                    int compareResult = xParts[i].CompareTo(yParts[i]);
                    if (compareResult != 0) return compareResult;
                }
            }
            return xParts.Length.CompareTo(yParts.Length);
        }

        public static void Sort_GameObjectChildren(Comparison<GameObject> comparison, string sortingActionDescription)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length == 0)
            {
                Debug.Log($"No Parent GameObjects selected. Please select at least one Parent GameObject to {sortingActionDescription}.");
                return;
            }

            foreach (GameObject selectedGameObject in selectedGameObjects)
            {
                Undo.RegisterCompleteObjectUndo(selectedGameObject.transform, sortingActionDescription);

                List<GameObject> children = new();
                foreach (Transform child in selectedGameObject.transform)
                {
                    children.Add(child.gameObject);
                }

                children.Sort(comparison);
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].transform.SetSiblingIndex(i);
                }
            }
        }

        public static void Sort_GameObjectChildrenByTag(bool ascending, string sortingActionDescription)
        {
            Sort_GameObjectChildren((x, y) =>
            {
                if (ascending)
                {
                    return string.Compare(x.tag, y.tag, StringComparison.Ordinal);
                }
                else
                {
                    return string.Compare(y.tag, x.tag, StringComparison.Ordinal);
                }
            }, sortingActionDescription);
        }

        public static void Sort_GameObjectChildrenByTagListOrder(bool ascending, string sortingActionDescription)
        {
            string[] predefinedOrder = new string[] { "Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController" };

            List<string> allTags = new(UnityEditorInternal.InternalEditorUtility.tags);
            allTags.Sort((x, y) =>
            {
                int indexX = Array.IndexOf(predefinedOrder, x);
                int indexY = Array.IndexOf(predefinedOrder, y);
                indexX = indexX == -1 ? int.MaxValue : indexX;
                indexY = indexY == -1 ? int.MaxValue : indexY;
                return indexX.CompareTo(indexY);
            });

            Sort_GameObjectChildren((x, y) =>
            {
                int indexX = allTags.IndexOf(x.tag);
                int indexY = allTags.IndexOf(y.tag);
                return ascending ? indexX.CompareTo(indexY) : indexY.CompareTo(indexX);
            }, sortingActionDescription);
        }

        public static void Sort_GameObjectChildrenByLayer(bool ascending, string sortingActionDescription)
        {
            Sort_GameObjectChildren((x, y) =>
            {
                string layerX = LayerMask.LayerToName(x.layer);
                string layerY = LayerMask.LayerToName(y.layer);
                return ascending ? string.Compare(layerX, layerY, StringComparison.Ordinal) : string.Compare(layerY, layerX, StringComparison.Ordinal);
            }, sortingActionDescription);
        }

        public static void Sort_GameObjectChildrenByLayerListOrder(bool ascending, string sortingActionDescription)
        {
            List<string> allLayers = new();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    allLayers.Add(layerName);
                }
            }

            Sort_GameObjectChildren((x, y) =>
            {
                int indexX = allLayers.IndexOf(LayerMask.LayerToName(x.layer));
                int indexY = allLayers.IndexOf(LayerMask.LayerToName(y.layer));
                return ascending ? indexX.CompareTo(indexY) : indexY.CompareTo(indexX);
            }, sortingActionDescription);
        }

        public static void Sort_GameObjectChildrenRandomly(string sortingActionDescription)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length == 0)
            {
                Debug.Log($"No Parent GameObjects selected. Please select at least one Parent GameObject to {sortingActionDescription}.");
                return;
            }

            foreach (GameObject selectedGameObject in selectedGameObjects)
            {
                Undo.RegisterCompleteObjectUndo(selectedGameObject.transform, sortingActionDescription);

                List<GameObject> children = new();
                foreach (Transform child in selectedGameObject.transform)
                {
                    children.Add(child.gameObject);
                }

                System.Random rng = new();
                int n = children.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    GameObject value = children[k];
                    children[k] = children[n];
                    children[n] = value;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].transform.SetSiblingIndex(i);
                }
            }
        }
        #endregion
        #endregion

        #region Presets
        public static void ApplyPresetToFolders(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            Dictionary<string, HierarchyDesigner_Configurable_Folders.HierarchyDesigner_FolderData> foldersData = HierarchyDesigner_Configurable_Folders.GetAllFoldersData(false);
            foreach (KeyValuePair<string, HierarchyDesigner_Configurable_Folders.HierarchyDesigner_FolderData> folder in foldersData)
            {
                HierarchyDesigner_Configurable_Folders.SetFolderData(folder.Key, preset.folderTextColor, preset.folderFontSize, preset.folderFontStyle, preset.folderColor, preset.folderImageType);
            }
        }

        public static void ApplyPresetToSeparators(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            Dictionary<string, HierarchyDesigner_Configurable_Separators.HierarchyDesigner_SeparatorData> separatorsData = HierarchyDesigner_Configurable_Separators.GetAllSeparatorsData(false);
            foreach (KeyValuePair<string, HierarchyDesigner_Configurable_Separators.HierarchyDesigner_SeparatorData> separator in separatorsData)
            {
                HierarchyDesigner_Configurable_Separators.SetSeparatorData(separator.Key,
                    preset.separatorTextColor,
                    preset.separatorIsGradientBackground,
                    preset.separatorBackgroundColor,
                    preset.separatorBackgroundGradient,
                    preset.separatorFontSize,
                    preset.separatorFontStyle,
                    preset.separatorTextAlignment,
                    preset.separatorBackgroundImageType);
            }
        }

        public static void ApplyPresetToTag(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.TagColor = preset.tagTextColor;
            HierarchyDesigner_Configurable_DesignSettings.TagFontStyle = preset.tagFontStyle;
            HierarchyDesigner_Configurable_DesignSettings.TagFontSize = preset.tagFontSize;
            HierarchyDesigner_Configurable_DesignSettings.TagTextAnchor = preset.tagTextAnchor;
        }

        public static void ApplyPresetToLayer(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.LayerColor = preset.layerTextColor;
            HierarchyDesigner_Configurable_DesignSettings.LayerFontStyle = preset.layerFontStyle;
            HierarchyDesigner_Configurable_DesignSettings.LayerFontSize = preset.layerFontSize;
            HierarchyDesigner_Configurable_DesignSettings.LayerTextAnchor = preset.layerTextAnchor;
        }

        public static void ApplyPresetToTree(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.HierarchyTreeColor = preset.treeColor;
        }

        public static void ApplyPresetToLines(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.HierarchyLineColor = preset.hierarchyLineColor;
        }

        public static void ApplyPresetToDefaultFolderValues(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.FolderDefaultTextColor = preset.folderTextColor;
            HierarchyDesigner_Configurable_DesignSettings.FolderDefaultFontSize = preset.folderFontSize;
            HierarchyDesigner_Configurable_DesignSettings.FolderDefaultFontStyle = preset.folderFontStyle;
            HierarchyDesigner_Configurable_DesignSettings.FolderDefaultImageColor = preset.folderColor;
            HierarchyDesigner_Configurable_DesignSettings.FolderDefaultImageType = preset.folderImageType;
        }

        public static void ApplyPresetToDefaultSeparatorValues(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultTextColor = preset.separatorTextColor;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultIsGradientBackground = preset.separatorIsGradientBackground;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultBackgroundColor = preset.separatorBackgroundColor;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultBackgroundGradient = preset.separatorBackgroundGradient;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultFontSize = preset.separatorFontSize;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultFontStyle = preset.separatorFontStyle;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultTextAnchor = preset.separatorTextAlignment;
            HierarchyDesigner_Configurable_DesignSettings.SeparatorDefaultImageType = preset.separatorBackgroundImageType;
        }

        public static void ApplyPresetToLockLabel(HierarchyDesigner_Configurable_Presets.HierarchyDesigner_Preset preset)
        {
            HierarchyDesigner_Configurable_DesignSettings.LockColor = preset.lockColor;
            HierarchyDesigner_Configurable_DesignSettings.LockFontSize = preset.lockFontSize;
            HierarchyDesigner_Configurable_DesignSettings.LockFontStyle = preset.lockFontStyle;
            HierarchyDesigner_Configurable_DesignSettings.LockTextAnchor = preset.lockTextAnchor;
        }
        #endregion
    }
}
#endif