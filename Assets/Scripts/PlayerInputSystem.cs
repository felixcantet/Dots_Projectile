using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;


public class PlayerInputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var mouseDown = Input.GetKeyDown(KeyCode.Mouse0);
        var mousePress = Input.GetKey(KeyCode.Mouse0);
        var mousePosition = new float2(Input.mousePosition.x, Input.mousePosition.y);
        var movementInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Entities.
        WithAll<PlayerTagComponent>().
            ForEach((ref PlayerInputsComponent inputs) =>
            {
                var i = new PlayerInputsComponent
                {
                    MouseDown = mouseDown,
                    MousePress = mousePress,
                    MousePosition = mousePosition,
                    MovementInput = movementInput
                };
                inputs = i;
            });
    }
}