using System;


public enum ImplantStatus
{
    Safe,
    Stolen,
    Banned
}




[Serializable]
public class ItemData
{
    public ImplantPreset Preset { get; private set; }

    public ImplantStatus Status { get; private set; }

    public string SerialNumber { get; private set; }

    public int OfferedPrice { get; private set; }

    public ItemData(
        ImplantPreset preset,
        ImplantStatus status,
        string serialNumber,
        int offeredPrice)
    {
        if (preset == null)
            throw new ArgumentNullException(nameof(preset));

        Preset = preset;
        Status = status;
        SerialNumber = serialNumber;
        OfferedPrice = offeredPrice;
    }
}
