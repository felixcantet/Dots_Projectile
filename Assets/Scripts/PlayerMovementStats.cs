using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable][GenerateAuthoringComponent]
public struct PlayerMovementStats : IComponentData
{
    public float MovementSpeed;
    public float RotationSpeed;
    public float MovementSharpness;
    public float RotationSharpness;
    
}
