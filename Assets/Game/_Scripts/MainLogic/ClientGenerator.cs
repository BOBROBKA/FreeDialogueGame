
using UnityEngine;

public class ClientGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Implants")]
    [SerializeField] private ImplantPreset[] implants;

    [Header("Stories")]
    [SerializeField] private StoryDatabase storyDatabase;

    private void Start()
    {
        GenerateClient();
    }

    private void GenerateClient()
    {
        // Проверяем необходимые ссылки
        if (clientPrefab == null || spawnPoint == null ||
            implants == null || implants.Length == 0 ||
            storyDatabase == null)
        {
            Debug.LogError("ClientGenerator: не все поля заполнены!", this);
            return;
        }

        // Выбираем случайный шаблон импланта
        ImplantPreset preset = implants[Random.Range(0, implants.Length)];

        if (preset == null)
        {
            Debug.LogError("ClientGenerator: выбранный ImplantPreset пуст!", this);
            return;
        }

        // Генерируем данные импланта
        ImplantStatus status = (ImplantStatus)Random.Range(0, 3);

        string serialNumber = "IM-" + Random.Range(100000, 999999);

        // Предлагаемая цена — случайная в пределах 60–90% от рыночной
        int offeredPrice = Mathf.RoundToInt(
            preset.BasePrice * Random.Range(0.6f, 0.9f)
        );

        // Получаем случайную историю
        string story = storyDatabase.GetRandomStory();

        // Создаём NPC на точке появления
        GameObject clientObject = Instantiate(
            clientPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Получаем ShopClient с созданного объекта
        ShopClient client = clientObject.GetComponent<ShopClient>();

        if (client == null)
        {
            Debug.LogError("На префабе клиента отсутствует ShopClient!", clientObject);
            Destroy(clientObject);
            return;
        }

        // Инициализируем NPC
        client.Initialize(
            preset,
            status,
            serialNumber,
            offeredPrice,
            story
        );

        Debug.Log($"Создан посетитель с имплантом {preset.ModelName}");
    }
}
