
using UnityEngine;

public class ShopClient : MonoBehaviour
{
    [Header("Item")]
    public ItemData Item { get; private set; }

    [Header("Story")]
    public string Story { get; private set; }

    
    private int irritation;
    private int pressure;

    public void Initialize(
        ImplantPreset preset,
        ImplantStatus status,
        string serialNumber,
        int offeredPrice,
        string story)
    {
        // Создаём конкретный экземпляр импланта
        Item = new ItemData(
            preset,
            status,
            serialNumber,
            offeredPrice
        );

        // Заполняем данные посетителя
        Story = story;
    }

    public ImplantStatus GetStatus()
    {
        if (Item == null)
        {
            Debug.LogError(
                "ShopClient has not been initialized!",
                this
            );

            return ImplantStatus.Safe;
        }

        return Item.Status;
    }
}
