using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable][GenerateAuthoringComponent]
public struct WeaponDataComponent : IComponentData
{
    public Entity shootPosition;
    public Entity BulletPrefab;
    public float ShootRate;
    [UnityEngine.HideInInspector]
    public float ShootTimer;
    
}
