using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    float currentEventCooldown = 0;

    public EventData[] events;

    [Tooltip("How long to wait before this becomes active.")]
    public float firstTriggerDelay = 180f;

    [Tooltip("How long to wait between each event.")]
    public float triggerInterval = 30f;

    public static EventManager instance;
    [System.Serializable]
    public class Event
    {
        public EventData data;
        public float duration, cooldown = 0;
    }
    List<Event> runningEvents = new List<Event>(); // These are events that have been activated, and are running.

    PlayerStats[] allPlayers;

    // Start is called before the first frame update
    void Start()
    {
        if (instance) Debug.LogWarning("There is more than 1 Spawn Manager in the Scene! Please remove the extras.");
        instance = this;
        currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;
        allPlayers = FindObjectsOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown for adding another event to the slate.
        currentEventCooldown -= Time.deltaTime;
        if (currentEventCooldown <= 0)
        {
            // Get an event and try to execute it.
            EventData e = GetRandomEvent();
            if (e && e.CheckIfWillHappen(allPlayers[Random.Range(0, allPlayers.Length)]))
                runningEvents.Add(new Event
                {
                    data = e,
                    duration = e.duration
                });

            // Set the cooldown for the event.
            currentEventCooldown = triggerInterval;
        }

        // Events that we want to remove.
        List<Event> toRemove = new List<Event>();

        // Cooldown for existing event to see if they should continue running.
        foreach (Event e in runningEvents)
        {
            // Reduce the current duration.
            e.duration -= Time.deltaTime;
            if (e.duration <= 0)
            {
                toRemove.Add(e);
                continue;
            }

            // Reduce the current cooldown.
            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                // Pick a random player to sic this mob on,
                // then reset the cooldown.
                e.data.Activate(allPlayers[Random.Range(0, allPlayers.Length)]);
                e.cooldown = e.data.GetSpawnInterval();
            }
        }

        // Remove all the events that have expired.
        foreach (Event e in toRemove) runningEvents.Remove(e);
    }

    public EventData GetRandomEvent()
    {
        // If no events are assigned, don't return anything.
        if (events.Length <= 0) return null;

        // Create a new list to populate with possible Events
        List<EventData> possibleEvents = new List<EventData>();

        // Add the events in event to the possible events only if the event is active
        foreach (EventData e in events)
        {
            if (e.IsActive())
            {
                possibleEvents.Add(e);
            }
        }
        // Randomly pick an event from the possible events to play
        if (possibleEvents.Count > 0)
        {
            EventData result = possibleEvents[Random.Range(0, possibleEvents.Count)];
            return result;
        }
        return null;
    }
}