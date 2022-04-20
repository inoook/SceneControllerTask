using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionTest2 : MonoBehaviour
{
    [SerializeField] SceneWithSubBase sceneWithSubBase = null;
    public SceneController rootSceneController;
    public SceneBase toScene;

    [SerializeField] Rect drawRect = new Rect(10,10,200,200);

    void OnGUI() {
        rootSceneController = sceneWithSubBase.GetSubSceneController();

        GUILayout.BeginArea(drawRect);
        SceneBase currentScene = null;
        SceneTransitionUtils.GetCurrentScene(rootSceneController, out currentScene);
        GUILayout.Label("CurrentScene: "+currentScene);

        if (GUILayout.Button("TransrateScene")) {
            SceneTransitionUtils.TransrateScene(rootSceneController, toScene);
        }
        //      if (GUILayout.Button ("TransrateSceneFromTo")) {
        //          TransrateSceneFromTo (rootSceneController, fromScene, toScene);
        //      }
        if (GUILayout.Button("GetCurrentScene")) {
            SceneBase current;
            if (SceneTransitionUtils.GetCurrentScene(rootSceneController, out current)) {
                Debug.Log("current: " + current);
            }
        }

        if (GUILayout.Button("GetScenePath")) {
            List<SceneTransitionUtils.TransitionInfo> fromPath = SceneTransitionUtils.GetScenePath(rootSceneController, toScene);
            Debug.Log(">>> fromPath");
            for (int i = 0; i < fromPath.Count; i++) {
                Debug.Log(fromPath[i].controller.gameObject.name + " / " + fromPath[i].sceneId);
            }
        }


        GUILayout.EndArea();
    }
}
