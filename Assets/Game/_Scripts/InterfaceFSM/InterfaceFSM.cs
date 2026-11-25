using UnityEngine;

public class InterfaceFSM : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform inspectionCameraPoint;
    [SerializeField] private Transform dialogueCameraPoint;

    [Header("UI")]
    [SerializeField] private GameObject inspectionUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMPro.TMP_InputField dialogueInput;

    [Header("Interaction")]
    [SerializeField] private GameObject itemInteraction;

    private FSM _fsm;

    private void Awake()
    {
        GameContext context = new GameContext(
            cameraController,
            inspectionCameraPoint,
            dialogueCameraPoint,
            inspectionUI,
            dialogueUI,
            dialogueInput,
            itemInteraction
        );

        _fsm = new FSM();

        var inspectionState = new InspectionState(context, _fsm, InterfaceStatesIds.InspectionState());
        var dialogueState = new DialogueState(context, _fsm, InterfaceStatesIds.DialogueState());

        _fsm.AddState(inspectionState);
        _fsm.AddState(dialogueState);

        // AddState автоматически назначает первое состояние,
        // но не вызывает Enter().
        inspectionState.Enter();
    }

    private void Update()
    {
        _fsm.Update();
    }

    public void OpenDialogue()
    {
        _fsm.SetState("Dialogue");
    }

    public void CloseDialogue()
    {
        _fsm.SetState("Inspection");
    }
}

