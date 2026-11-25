using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;

    [Header("Typing")]
    [SerializeField] private float characterDelay = 0.035f;
    [SerializeField] private float punctuationDelay = 0.15f;


    private Coroutine typingCoroutine;
    private bool isTyping;

    public void ShowText(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;

        text.text = message;
        text.maxVisibleCharacters = 0;

        text.ForceMeshUpdate();

        int characterCount = text.textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            text.maxVisibleCharacters = i + 1;

            if (typingSound != null &&
                !char.IsWhiteSpace(message[i]))
            {
                audioSource.PlayOneShot(typingSound);
            }

            float delay = characterDelay;

            char currentChar = message[i];

            if (currentChar == '.' ||
                currentChar == ',' ||
                currentChar == '!' ||
                currentChar == '?')
            {
                delay += punctuationDelay;
            }

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    public void SkipText()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);

        text.maxVisibleCharacters = text.textInfo.characterCount;

        isTyping = false;
        typingCoroutine = null;
    }
}