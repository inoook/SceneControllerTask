using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(SceneController))]
public class SceneWithSubBase : SceneBase {

	[SerializeField] public SceneController subSceneController = null;

	protected override void OnSetup()
	{
        subSceneController.SetParentScene(this);
        subSceneController.Setup();
    }

    //protected override void OnInitScene()
    //{
    //	base.OnInitScene();
    //}
    protected override void OnInitScene()
    {
        //base.OnInitScene();
        subSceneController.ClearScene();
    }

    protected override async Task OnOpenScene()
	{
		//base.OnOpenScene ();
        //subSceneController.SetInitialScene();
	}

    protected override void OnOpenSceneComplete() {
        subSceneController.SetInitialScene();
    }

    //protected override void OnCloseScene()
    //{
    //	base.OnCloseScene ();
    //}

    //
    public SceneController GetSubSceneController (){
		return subSceneController;
	}
    //
    public void SetScene(int id) {
        subSceneController.SetScene(id);
    }
    public async Task SetSceneAsync(int id)
    {
        await subSceneController.SetSceneAsync(id);
    }

    public void ClearScene() {
        _ = subSceneController.ClearScene();
    }

    public SceneBase[] GetScenes() {
        return subSceneController.GetScenes();
    }
}
