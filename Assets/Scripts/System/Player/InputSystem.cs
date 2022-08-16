using Unity.Entities;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// A system that handles the player input and sets the player MoveData.
/// </summary>
public partial class InputSystem : SystemBase
{
    private InputActions inputActions;

    private float3 movementInput;
    private float3 lookInput;

    protected override void OnCreate()
    {
        // Subscribes for movement and look inputs.
        inputActions = new InputActions();
        inputActions.Player.Move.performed += Move_performed;
        inputActions.Player.Move.canceled += Move_performed;
        inputActions.Player.Look.performed += Look_performed;

        inputActions.Player.Enable();
        inputActions.Player.Move.Enable();
        inputActions.Player.Look.Enable();
    }

    protected override void OnDestroy()
    {
        inputActions.Player.Move.performed -= Move_performed;
        inputActions.Player.Move.canceled -= Move_performed;
        inputActions.Player.Look.performed -= Look_performed;

        inputActions.Player.Disable();
        base.OnDestroy();
    }

    // Movement input performed.
    private void Move_performed(InputAction.CallbackContext obj) => movementInput = 
        V2ToFloat3(obj.ReadValue<Vector2>());

    // Look input performed.
    private void Look_performed(InputAction.CallbackContext obj) => lookInput = 
        V2ToFloat3(Camera.main.ScreenToWorldPoint(obj.ReadValue<Vector2>()));

    // Converts the Vector2 to float3.
    private float3 V2ToFloat3(Vector2 value) => new float3(value.x, value.y, 0);

    protected override void OnUpdate()
    {
        float3 look = lookInput;
        float3 move = movementInput;
        float offset = 1.57079637f;         // 90 degrees to radians

        // Loops throug all the players and sets the movement data.
        Entities.WithAll<PlayerTag>().ForEach((ref MoveData moveData, in Translation translation) =>
        {
            float3 diff = look - translation.Value;
            diff = math.normalizesafe(diff);
            float angle = math.atan2(diff.y, diff.x) - offset;

            moveData.rotationAngle = angle;
            moveData.movement = move;
        }).Run();
    }

    // Enables inputs on start.
    protected override void OnStartRunning() => inputActions.Enable();

    // Disables inputs on stop.
    protected override void OnStopRunning() => inputActions.Disable();
}
