using UnityEngine;

public class DialogueState : FsmState
{
    private GameContext _context;
    private float _zoom;
    public DialogueState(GameContext context, FSM fsm, string id, float zoom = 3f) : base(fsm, id)
    {
        _context = context;
        _zoom = zoom;
    }

    public override void Enter()
    {
        _context.DialogueUI.SetActive(true);
        _context?.InspectionUI.SetActive(false);

        //_context?.ItemInteraction.SetActive(false);

        _context.CameraController.SetTarget(
            _context.DialogueCameraPoint, 
            _zoom
        );

        //_context.DialogueInput.ActivateInputField();
    }

    public override void Exit()
    {
        _context.DialogueUI.SetActive(false);
    }

    public override void Update()
    {
        if(PlayerInput.isSwitchBack())
        {
            base._FSM.SetState(InterfaceStatesIds.InspectionState());
        }
    }
}