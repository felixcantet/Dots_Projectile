using Unity.Entities;
[GenerateAuthoringComponent]
public struct BulletLifeTimeComponent : IComponentData
{
    public float LifeTime;
    [UnityEngine.HideInInspector]
    public float currentLifeTime;
}
