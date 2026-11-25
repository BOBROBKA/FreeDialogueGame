using UnityEngine;

public class InspectionState : FsmState
{
    private GameContext _context;
    private float _zoom;
    public InspectionState(GameContext context, FSM fsm, string id, float zoom = 7.3f) : base(fsm, id)
    {
        _context = context;
        _zoom = zoom;
    }

    public override void Enter()
    {
        _context?.InspectionUI.SetActive(true);
        _context?.ItemInteraction.SetActive(true);

        _context?.DialogueUI.SetActive(false);

        _context.CameraController.SetTarget(
            _context.InspectionCameraPoint,
            _zoom
        );
    }

    public override void Exit()
    {
        _context?.InspectionUI.SetActive(false);
        _context?.ItemInteraction.SetActive(false);
    }

    public override void Update()
    {
        if(PlayerInput.isSwitchForward())
        {
            base._FSM.SetState(InterfaceStatesIds.DialogueState());
        }
    }
}