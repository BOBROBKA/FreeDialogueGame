using UnityEngine;

class CoroutineUtility : MonoBehaviour
{
    public static CoroutineUtility Instance{get;private set;}

    public void Awake()
    {
        if(Instance!=null) Destroy(gameObject);
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}