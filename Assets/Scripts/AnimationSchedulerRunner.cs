using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSchedulerRunner : MonoBehaviour {
    public AnimationSchedulerScriptableObject schedulerData;

    private Dictionary<string, Animator> animators = new Dictionary<string, Animator>();
    private readonly List<float> nextTriggers = new List<float>();
    private float startTime;
    private bool running = false;

    void Start() {
        // Collect animators by name
        Debug.Log($"Initializing AnimationSchedulerRunner with {schedulerData.actions.Count} scheduled actions");
        foreach (var action in schedulerData.actions) {
            GameObject obj = GameObject.Find(action.objectName);
            if (obj != null) {
                Debug.Log($"Found GameObject for action '{action.objectName}': {obj.name}");
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null) {
                    animators[action.objectName] = animator;
                    Debug.Log($"Found animator for object '{action.objectName}': {animator}");
                } else {
                    Debug.LogWarning($"Animator not found on GameObject '{action.objectName}'");
                }
            } else {
                Debug.LogError($"GameObject not found for action '{action.objectName}'");
            }
            nextTriggers.Add(action.offset);
        }

        StartCoroutine(Loop());
    }

    IEnumerator Loop() {
        startTime = Time.time;
        running = true;

        while (running) {
            float elapsed = Time.time - startTime;

           
            for (int i = 0; i < schedulerData.actions.Count; i++)
                {
                    var action = schedulerData.actions[i];
                    if (elapsed >= nextTriggers[i])
                    {
                        nextTriggers[i] += action.time;

                    if (animators.TryGetValue(action.objectName, out var animator))
                    {
                        foreach (var trigger in action.actions)
                        {
                            Debug.Log($"Playing animation '{trigger}' on '{action.objectName}' at t={elapsed:0.00}s with speed={action.speed}");
                            animator.speed = action.speed;
                            animator.Play(trigger);
                        }
                            
                        Debug.Log(action.fadeTracks);
                        }
                    else
                    {
                        Debug.LogWarning($"Missing animator for object '{action.objectName}', cannot play actions [{string.Join(", ", action.actions)}]");
                    }
                    }
                }

            yield return null;
        }
    }

    public void Stop() {
        running = false;
    }
}