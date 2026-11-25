using TMPro;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    public void Awake()
    {
        new GameObject("[COROUTINES]").AddComponent<CoroutineUtility>();
        new GameObject("[INPUT_MANAGER]").AddComponent<PlayerInput>().Init();

        var serviceLocator = new GameObject("[SERVICE_LOCATOR]").AddComponent<ServiceLocator>();
        ServiceLocator.Instance.Register<LLMClassifier>(FindFirstObjectByType<LLMClassifier>());
        ServiceLocator.Instance.Register<DialogueManager>(FindFirstObjectByType<DialogueManager>());
    }
}