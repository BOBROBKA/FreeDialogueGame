using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    private DialogueManager dialogue;

    private void Start()
    {
        _inputField = GetComponent<TMP_InputField>();

        _inputField.onSelect.AddListener(OnInputSelected);
        _inputField.onDeselect.AddListener(OnInputDeselected);
        
        PlayerInput.Instance.Input.UI.Disable();
        PlayerInput.Instance.Input.UI.Enable();

        dialogue = ServiceLocator.Instance.Get<DialogueManager>();
    }

    private void OnInputSelected(string text)
    {

        Debug.Log("Начали вводить текст");
        EnableUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInput.Instance.Input.UI.Confirm.performed += Confirm;
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        dialogue.ProcessMessage(_inputField.text);
        _inputField.DeactivateInputField();
    }
    private void OnInputDeselected(string text)
    {

        Debug.Log("Вышли из InputField");
        EnableGameplay();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerInput.Instance.Input.UI.Confirm.performed -= Confirm;
    }

    private void OnDisable()
    {
        try{
            PlayerInput.Instance.Input.UI.Confirm.performed -= Confirm;
        }
        catch
        {
            Debug.LogError("Tried to disable Confirm but ther's no confirm subscribed");
        }
        EnableGameplay();
    }
    private void EnableGameplay()
    {
        PlayerInput.Instance.Input.UI.Disable();
        PlayerInput.Instance.Input.Gameplay.Enable();
    }

    private void EnableUI()
    {
        PlayerInput.Instance.Input.UI.Enable();
        PlayerInput.Instance.Input.Gameplay.Disable();
    }
}
