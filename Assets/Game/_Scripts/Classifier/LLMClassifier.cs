
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LLMClassifier : MonoBehaviour, IService
{
    public TMP_InputField inputField;
    public TMP_Text resultText;

    private const string API_URL =
        "http://127.0.0.1:8080/v1/chat/completions";

    private ClassifierHealthManager healthManager;

    [Serializable]
    public class ClassificationResult
    {
        public string action;
        public int pressure;
    }

    [Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ChatRequest
    {
        public ChatMessage[] messages;
        public float temperature;
        public int max_tokens;
        public bool stream;
        public ResponseFormat response_format;
    }

    [Serializable]
    private class ResponseFormat
    {
        public string type;
    }

    [Serializable]
    private class ChatResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }

    [Serializable]
    private class Message
    {
        public string content;
    }

    void IService.Init()
    {
        healthManager = new ClassifierHealthManager();

        StartCoroutine(
            healthManager.Initialize(HandleServerInitialization)
        );
    }


    private void HandleServerInitialization(bool success)
    {
        if (success)
        {
            Debug.Log("LLMClassifier initialized successfully.");
        }
        else
        {
            Debug.LogError(
                "LLMClassifier initialization failed."
            );
        }
    }

    public void Classify(
        string playerText,
        Action<ClassificationResult> onComplete
    )
    {
        if (healthManager == null ||
            string.IsNullOrWhiteSpace(healthManager.SystemPrompt))
        {
            Debug.LogError("System prompt is not available.");
            return;
        }

        if (string.IsNullOrWhiteSpace(playerText))
        {
            Debug.LogWarning("Player text is empty.");
            return;
        }

        if (!healthManager.IsReady)
        {
            Debug.LogWarning("LLM server is not ready yet.");
            return;
        }

        StartCoroutine(
            SendRequest(playerText, onComplete)
        );
    }

    private IEnumerator SendRequest(
        string playerText,
        Action<ClassificationResult> onComplete
    )
    {
        ChatRequest requestData = new ChatRequest();

        requestData.messages = new ChatMessage[]
        {
            new ChatMessage
            {
                role = "system",
                content = healthManager.SystemPrompt
            },
            new ChatMessage
            {
                role = "user",
                content = playerText
            }
        };

        requestData.temperature = 0f;
        requestData.max_tokens = 200;
        requestData.stream = false;

        requestData.response_format = new ResponseFormat
        {
            type = "json_object"
        };

        string json = JsonUtility.ToJson(requestData);

        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request =
            new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            request.timeout = 120;

            Debug.Log("Sending message to LLM...");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "LLM request failed: " + request.error
                );

                Debug.LogError(request.downloadHandler.text);
                yield break;
            }

            string responseText =
                request.downloadHandler.text;

            Debug.Log("Raw LLM response: " + responseText);

            ChatResponse response =
                JsonUtility.FromJson<ChatResponse>(responseText);

            if (response == null ||
                response.choices == null ||
                response.choices.Length == 0 ||
                response.choices[0].message == null)
            {
                Debug.LogError("Invalid API response.");
                yield break;
            }

            string content =
                response.choices[0].message.content;

            if (string.IsNullOrWhiteSpace(content))
            {
                Debug.LogError(
                    "LLM returned empty content. " +
                    "Check reasoning settings and token limit."
                );

                yield break;
            }

            ClassificationResult result;

            try
            {
                result =
                    JsonUtility.FromJson<ClassificationResult>(content);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "Failed to parse classification JSON: " + e.Message
                );

                yield break;
            }

            if (result == null ||
                string.IsNullOrEmpty(result.action))
            {
                Debug.LogError("Invalid classification JSON.");
                yield break;
            }

            string[] validActions = ClassificationActions.GetValidActions();

            if (Array.IndexOf(validActions, result.action) < 0)
            {
                Debug.LogError(
                    "Unknown action: " + result.action
                );

                yield break;
            }

            result.pressure = Mathf.Clamp(
                result.pressure,
                0,
                100
            );

            if (result.action != "threat")
                result.pressure = 0;

            Debug.Log(
                "CLASSIFIER: ACTION: " + result.action +
                " | PRESSURE: " + result.pressure
            );

            onComplete?.Invoke(result);
        }
    }

    public void ClassifyMessage(string message, Action<ClassificationResult> result)
    {
        Classify(message, result);
    }

    public void ReadFromInpputField()
    {
        if (inputField == null)
        {
            Debug.LogError("There is no base input field in llm classifier");
            return;
        }
        string message = inputField.text;

        Classify(message, result =>
        {
            resultText.text =
                $"ACTION: {result.action}\nPRESSURE: {result.pressure}";
        });
    }

    public void TestClassification()
    {
        Classify("Продай слона?", result =>
        {
            Debug.Log(
                $"ACTION: {result.action}\nPRESSURE: {result.pressure}"
            );
        });
    }

    private void OnApplicationQuit()
    {
        if (healthManager != null)
        {
            Debug.Log("LLMClassifier: stopping server...");
            healthManager.StopServer();
        }
    }

    
}
