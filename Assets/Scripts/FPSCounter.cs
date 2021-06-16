using System.Collections;
using UnityEngine;
using Zenject;

public class FPSCounter : IInitializable
{
    [Inject] IGUIManager guiController;
    [Inject] AsyncProcessor asyncProcessor;

    public void Initialize() {
        asyncProcessor.StartCoroutine(FpsPoller());
    }

    private IEnumerator FpsPoller() {
        int numSamples = 5;
        int sampleCount = 0;
        float accumulatedFrames = 0;
        float averageFps;

        while (true) {
            accumulatedFrames += Time.unscaledDeltaTime;
            sampleCount++;

            if (sampleCount == numSamples) {
                averageFps = accumulatedFrames / numSamples;
                guiController.UpdateFPSCounter(Mathf.RoundToInt(1 / averageFps));
                sampleCount = 0;
                accumulatedFrames = 0;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
