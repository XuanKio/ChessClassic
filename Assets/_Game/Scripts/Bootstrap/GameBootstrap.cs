using UnityEngine;
using UnityEngine.SceneManagement;

// Runs in Bootstrap.unity (build index 0) and hands off to the first game scene.
// Game-wide services are initialized here, before the handoff, once a real one exists.
public sealed class GameBootstrap : MonoBehaviour
{
    [SerializeField, BuildScene] private string firstSceneName = "SampleScene";

    private void Start()
    {
        if (!Application.CanStreamedLevelBeLoaded(firstSceneName))
        {
            Debug.LogError($"GameBootstrap: scene '{firstSceneName}' is not in the build scene list.", this);
            return;
        }

        SceneManager.LoadScene(firstSceneName, LoadSceneMode.Single);
    }
}
