using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public SceneController rootSceneController;
	public SceneBase toScene;

	void OnGUI()
	{
		if (GUILayout.Button ("TransrateScene")) {
			SceneTransitionUtils.TransrateScene (rootSceneController, toScene);
		}
		//		if (GUILayout.Button ("TransrateSceneFromTo")) {
		//			TransrateSceneFromTo (rootSceneController, fromScene, toScene);
		//		}
		if (GUILayout.Button ("GetCurrentScene")) {
			SceneBase current;
			if (SceneTransitionUtils.GetCurrentScene (rootSceneController, out current)) {
				Debug.Log ("current: "+current);
			}
		}

        if (GUILayout.Button("GetScenePath")) {
            List<SceneTransitionUtils.TransitionInfo> fromPath = SceneTransitionUtils.GetScenePath(rootSceneController, toScene);
            Debug.Log(">>> fromPath");
            for (int i = 0; i < fromPath.Count; i++) {
                Debug.Log(fromPath[i].controller.gameObject.name + " / " + fromPath[i].sceneId);
            }
        }
    }
}
