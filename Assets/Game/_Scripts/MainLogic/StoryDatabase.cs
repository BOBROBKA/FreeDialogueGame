
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StoryDatabase",
    menuName = "Game/NPC/Story Database"
)]
public class StoryDatabase : ScriptableObject
{
    [Header("Story Pools")]

    [SerializeField]
    private List<string> normalStories = new List<string>();

    [SerializeField]
    private List<string> inconsistentStories = new List<string>();

    // Возвращает только обычную историю
    public string GetRandomStory()
    {
        return GetRandomFromPool(normalStories);
    }

    // Возвращает обычную или историю с несоответствиями
    public string GetRandomStory(float inconsistentChance)
    {
        inconsistentChance = Mathf.Clamp01(inconsistentChance);

        bool useInconsistent =
            Random.value < inconsistentChance;

        List<string> selectedPool = useInconsistent
            ? inconsistentStories
            : normalStories;

        // Если выбранный пул пуст, используем другой
        if (selectedPool == null || selectedPool.Count == 0)
        {
            selectedPool = useInconsistent
                ? normalStories
                : inconsistentStories;
        }

        return GetRandomFromPool(selectedPool);
    }

    private string GetRandomFromPool(List<string> pool)
    {
        if (pool == null || pool.Count == 0)
        {
            Debug.LogError("StoryDatabase: выбранный пул историй пуст!");
            return string.Empty;
        }

        int index = Random.Range(0, pool.Count);

        return pool[index];
    }
}
