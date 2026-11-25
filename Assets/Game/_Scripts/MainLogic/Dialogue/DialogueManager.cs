using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour, IService
{
    public TMP_InputField InputField;
    public DialogueText text;

    private LLMClassifier classifier;
    
    public void RecieveMessageFromInputField()
    {
        if(InputField == null){
            Debug.LogError("No inputfield found");
            return;
        }
        ProcessMessage(InputField.text);
    }
    public void ProcessMessage(string message)
    {
        classifier.ClassifyMessage(message, ProcessAction);
    }

    private void ProcessAction(LLMClassifier.ClassificationResult result)
    {
        if (result.action == ClassificationActions.Leave())
        {
            text.ShowText("Я ухожу");
        }
        else if (result.action == ClassificationActions.Insult())
        {
            text.ShowText("А, да ты быдло");
        }
    }

    public void Init()
    {
        classifier = ServiceLocator.Instance.Get<LLMClassifier>();
    }
}
