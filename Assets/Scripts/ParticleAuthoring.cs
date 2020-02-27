using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ParticleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public VisualEffect effect;
    public Transform tr;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        conversionSystem.AddHybridComponent(effect);
        conversionSystem.AddHybridComponent(tr);
    }
}
