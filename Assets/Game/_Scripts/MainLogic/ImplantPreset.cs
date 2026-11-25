using UnityEngine;

public enum ImplantType
{
    Arm,
    Leg,
    Optics,
    Spine,
    Neural,
    Other
}


[CreateAssetMenu(
    fileName = "New Implant Preset",
    menuName = "Game/Items/Implant Preset"
)]
public class ImplantPreset : ScriptableObject
{
    [Header("Visual")]
    [SerializeField] private Sprite icon;

    [Header("Model")]
    [SerializeField] private string modelName;
    [SerializeField] private ImplantType implantType;

    [Header("Economy")]
    [SerializeField] private int basePrice;

    public Sprite Icon => icon;
    public string ModelName => modelName;
    public ImplantType Type => implantType;
    public int BasePrice => basePrice;
}
