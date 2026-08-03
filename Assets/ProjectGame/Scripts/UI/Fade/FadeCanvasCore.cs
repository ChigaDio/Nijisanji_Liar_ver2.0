using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameCore;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvasCore : BaseSingleton<FadeCanvasCore>
{
    [SerializeField]
    private Image image;

    private CancellationTokenSource cancellationTokenSource;

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public UniTask FadeIn(float duration, Action onComplete = null)
    {
        return FadeAsync(0.0f, 0.5f, duration, onComplete);
    }

    public UniTask FadeOut(float duration, Action onComplete = null)
    {
        return FadeAsync(0.5f, 0.0f, duration, onComplete);
    }

    private async UniTask FadeAsync(float start, float end, float duration, Action onComplete)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();

        cancellationTokenSource = new CancellationTokenSource();

        image.fillAmount = start;

        if (duration <= 0f)
        {
            image.fillAmount = end;
            onComplete?.Invoke();
            return;
        }

        float elapsed = 0f;

        try
        {
            while (elapsed < duration)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(start, end, elapsed / duration);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationTokenSource.Token);
            }

            image.fillAmount = end;
            if(image.fillAmount > 0.0f) image.raycastTarget = true;
            else image.raycastTarget = false;
            onComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
    }
}