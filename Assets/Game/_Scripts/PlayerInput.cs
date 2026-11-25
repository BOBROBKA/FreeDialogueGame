using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour, IService
{
    public static PlayerInput Instance{get;private set;}
    public GameInputAction Input{get; private set;}
    public void Init()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);            
        } else
        {
            Instance = this;        
            DontDestroyOnLoad(gameObject);
        }
        Input = new GameInputAction();
        Input.Enable();
    }

    private void OnEnable()
    {
        Input?.Enable();
    }
    private void OnDisable()
    {
        Input?.Disable();
    }
    
    public static bool isSwitchForward() => PlayerInput.Instance.Input.Gameplay.SwitchForward.triggered;
    public static bool isSwitchBack() => PlayerInput.Instance.Input.Gameplay.SwitchBack.triggered;
    public static bool isPause() => PlayerInput.Instance.Input.Gameplay.Pause.triggered;
    public static bool isLeave() => PlayerInput.Instance.Input.UI.Leave.triggered;
     
}
