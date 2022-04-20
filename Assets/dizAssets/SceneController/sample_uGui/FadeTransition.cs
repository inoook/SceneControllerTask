using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using Ez;
using System.Threading;

public class FadeTransition
{
    [SerializeField] CanvasGroup canvasGroup = null;
    CancellationTokenSource cancellation = null;

    public FadeTransition(CanvasGroup canvasGroup) {
        this.canvasGroup = canvasGroup;
        this.canvasGroup.alpha = 0;
    }

    public async Task StartFade(float time, float to, System.Action onComplete = null) {
        //var tween = LeanTween.value(canvasGroup.gameObject, canvasGroup.alpha, to, time).setOnUpdate((v) => {
        //    canvasGroup.alpha = v;
        //});
        //await tween;
        //onComplete?.Invoke();

        Cancel();

        await EzTween.TweenCanvasGroupAlpha(canvasGroup, EzEaseType.Linear, to, time, cancellation.Token);
        onComplete?.Invoke();
    }

    void Cancel()
    {
        cancellation?.Cancel();

        cancellation = new CancellationTokenSource();
    }
}

