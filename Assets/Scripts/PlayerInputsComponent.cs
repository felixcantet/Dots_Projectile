using Unity.Entities;
using Unity.Mathematics;
using System;

[GenerateAuthoringComponent]
public struct PlayerInputsComponent : IComponentData
{
    public float2 MovementInput;
    public float2 MousePosition;

    public bool MouseDown;
    public bool MousePress;
    public float3 Orientation;
}
