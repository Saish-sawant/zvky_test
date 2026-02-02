using System.Collections;
using UnityEngine;

public class AssetLoader
{
    public float Progress { get; private set; }

    public IEnumerator LoadAllAssets()
    {
        Progress = 0f;

        // Example asset groups
        yield return LoadGroup(0.3f); // UI
        yield return LoadGroup(0.3f); // Gameplay
        yield return LoadGroup(0.4f); // Audio / VFX

        Progress = 1f;
    }

    IEnumerator LoadGroup(float weight)
    {
        float start = Progress;
        float target = Progress + weight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            Progress = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }
}
