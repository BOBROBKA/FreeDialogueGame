using UnityEngine;
using TMPro;

public class GameContext
{
    public CameraController CameraController;

    public Transform InspectionCameraPoint;
    public Transform DialogueCameraPoint;

    public GameObject InspectionUI;
    public GameObject DialogueUI;

    public TMP_InputField DialogueInput;

    public GameObject ItemInteraction;

    public GameContext(
        CameraController cameraController,
        Transform inspectionCameraPoint,
        Transform dialogueCameraPoint,
        GameObject inspectionUI,
        GameObject dialogueUI,
        TMP_InputField dialogueInput,
        GameObject itemInteraction)
    {
        CameraController = cameraController;
        InspectionCameraPoint = inspectionCameraPoint;
        DialogueCameraPoint = dialogueCameraPoint;
        InspectionUI = inspectionUI;
        DialogueUI = dialogueUI;
        DialogueInput = dialogueInput;
        ItemInteraction = itemInteraction;
    }
}