
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }

    private readonly Dictionary<Type, IService> _services = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Получает зарегистрированный сервис.
    /// </summary>
    public T Get<T>() where T : class, IService
    {
        Type key = typeof(T);

        if (!_services.TryGetValue(key, out IService service))
        {
            Debug.LogError(
                $"Service {key.Name} is not registered in {nameof(ServiceLocator)}."
            );

            throw new InvalidOperationException(
                $"Service {key.Name} is not registered."
            );
        }

        return (T)service;
    }

    /// <summary>
    /// Регистрирует сервис и при необходимости инициализирует его.
    /// </summary>
    public void Register<T>(T service, bool isInited = false)
        where T : class, IService
    {
        if (service == null)
        {
            Debug.LogError("Cannot register a null service.");
            return;
        }

        Type key = typeof(T);

        if (_services.ContainsKey(key))
        {
            Debug.LogError(
                $"Service {key.Name} is already registered."
            );
            return;
        }

        if (!isInited)
            service.Init();

        _services.Add(key, service);
    }

    /// <summary>
    /// Удаляет сервис из ServiceLocator.
    /// </summary>
    public void Unregister<T>() where T : class, IService
    {
        Type key = typeof(T);

        if (!_services.TryGetValue(key, out IService service))
        {
            Debug.LogError(
                $"Service {key.Name} is not registered."
            );
            return;
        }

        if (service is MonoBehaviour behaviour)
        {
            Destroy(behaviour.gameObject);
        }

        _services.Remove(key);
    }
}
