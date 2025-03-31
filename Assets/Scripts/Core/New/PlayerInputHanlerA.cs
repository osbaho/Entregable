// PlayerInputHandlerA.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandlerA : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string aim = "Aim";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string dash = "Dash";
    [SerializeField] private string basicAttack = "Basic Attack";
    [SerializeField] private string specialPower = "Special Power";

    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction dashAction;
    private InputAction basicAttackAction;
    private InputAction specialPowerAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 AimInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool BasicAttackInput { get; private set; }
    public bool SpecialPowerInput { get; private set; }

    public static PlayerInputHandlerA Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Exit early to prevent further execution
        }

        if (playerControls == null)
        {
            Debug.LogError("Player Controls Input Action Asset is not assigned!");
            return;
        }

        InputActionMap actionMap = playerControls.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"Action Map '{actionMapName}' not found in Player Controls!");
            return;
        }

        moveAction = actionMap.FindAction(move);
        aimAction = actionMap.FindAction(aim);
        jumpAction = actionMap.FindAction(jump);
        sprintAction = actionMap.FindAction(sprint);
        dashAction = actionMap.FindAction(dash);
        basicAttackAction = actionMap.FindAction(basicAttack);
        specialPowerAction = actionMap.FindAction(specialPower);

        if (moveAction == null || aimAction == null || jumpAction == null || sprintAction == null || dashAction == null || basicAttackAction == null || specialPowerAction == null)
        {
            Debug.LogError("One or more actions not found in the action map!");
            return;
        }
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        aimAction.performed += ctx => AimInput = ctx.ReadValue<Vector2>();
        aimAction.canceled += ctx => AimInput = Vector2.zero;

        jumpAction.performed += ctx => JumpTriggered = true;
        jumpAction.canceled += ctx => JumpTriggered = false;

        sprintAction.performed += ctx => SprintInput = true;
        sprintAction.canceled += ctx => SprintInput = false;

        dashAction.performed += ctx => DashInput = true;
        dashAction.canceled += ctx => DashInput = false;

        basicAttackAction.performed += ctx => BasicAttackInput = true;
        basicAttackAction.canceled += ctx => BasicAttackInput = false;

        specialPowerAction.performed += ctx => SpecialPowerInput = true;
        specialPowerAction.canceled += ctx => SpecialPowerInput = false;
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void EnableInputActions()
    {
        if (moveAction != null) moveAction.Enable();
        if (aimAction != null) aimAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (sprintAction != null) sprintAction.Enable();
        if (dashAction != null) dashAction.Enable();
        if (basicAttackAction != null) basicAttackAction.Enable();
        if (specialPowerAction != null) specialPowerAction.Enable();
    }

    private void DisableInputActions()
    {
        if (moveAction != null) moveAction.Disable();
        if (aimAction != null) aimAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (sprintAction != null) sprintAction.Disable();
        if (dashAction != null) dashAction.Disable();
        if (basicAttackAction != null) basicAttackAction.Disable();
        if (specialPowerAction != null) specialPowerAction.Disable();
    }
}
