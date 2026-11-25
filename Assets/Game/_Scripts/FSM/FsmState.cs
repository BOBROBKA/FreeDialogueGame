public abstract class FsmState
{
    protected readonly FSM _FSM;
    protected readonly string _stateID;

    public FsmState(FSM fsm, string id)
    {
        _FSM = fsm;
        _stateID = id;
    }
    public string GetStateID() => _stateID;

    public virtual void Enter<T>(T data) where T:IFSMEnterStateData{}
    public virtual void Enter(){}
    public virtual void Exit(){}
    public virtual void Update(){}
    public virtual void FixedUpdate(){}
}