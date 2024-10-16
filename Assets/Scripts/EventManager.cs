using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Gestiona los eventos del sistema, permitiendo la comunicación desacoplada entre diferentes componentes.
/// </summary>
public static class EventManager
{
    /// <summary>
    /// Diccionario que almacena los eventos y sus listeners.
    /// </summary>
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();

    /// <summary>
    /// Añade un listener a un evento específico.
    /// </summary>
    /// <param name="eventName">Nombre del evento.</param>
    /// <param name="listener">Método a invocar cuando se dispare el evento.</param>
    public static void StartListening(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Elimina un listener de un evento específico.
    /// </summary>
    /// <param name="eventName">Nombre del evento.</param>
    /// <param name="listener">Método a eliminar.</param>
    public static void StopListening(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// Dispara un evento, invocando todos sus listeners.
    /// </summary>
    /// <param name="eventName">Nombre del evento.</param>
    public static void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent?.Invoke();
        }
    }
}
