using System.Collections.Generic;
using System.Linq;
using HoloNetwork.NetworkObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloNetwork.Editor {

  public class SceneObjectIdManager {

    [MenuItem("Holo/HoloNet/Reassign ids in current scene")]
    static void AssignSceneObjectIdsInCurrentScene() {
      var holonetObjects = SceneManager
        .GetActiveScene()
        .GetRootGameObjects()
        .SelectMany(ro => ro.GetComponentsInChildren<HoloNetObject>(true))
        .ToList();
      uint openSceneObjectId = 0;
      foreach (var item in holonetObjects) {
        item.oid = new HoloNetObjectId(0, openSceneObjectId++);
        Debug.Log($"Assigning id ({item.oid}) to {item.gameObject.name} ");
        EditorUtility.SetDirty(item);
      }

      EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
      EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    [MenuItem("Holo/HoloNet/Reassign ids in all scenes")]
    static void AssignSceneObjectIdsAllScenes() {
      string originalScene = SceneManager.GetActiveScene().name;
      foreach (var item in GetAllSceneNames()) {
        EditorSceneManager.OpenScene(item);
        AssignSceneObjectIdsInCurrentScene();
      }

      EditorSceneManager.OpenScene(originalScene);
    }


    private static IEnumerable<string> GetAllSceneNames() {
      var sceneCount = SceneManager.sceneCountInBuildSettings;
      var scenes = new List<string>();
      for (int i = 0; i < sceneCount; i++) {
        scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
      }

      return scenes;
    }

  }

}