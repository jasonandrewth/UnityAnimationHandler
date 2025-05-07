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
        foreach (var action in schedulerData.actions) {
            GameObject obj = GameObject.Find(action.objectName);
            if (obj != null) {
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null) {
                    animators[action.objectName] = animator;
                } else {
                    Debug.LogWarning("Animator not found on: " + action.objectName);
                }
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

            for (int i = 0; i < schedulerData.actions.Count; i++) {
                var action = schedulerData.actions[i];
                if (elapsed >= nextTriggers[i]) {
                    nextTriggers[i] += action.time;

                    if (animators.TryGetValue(action.objectName, out var animator)) {
                        foreach (var trigger in action.actions) {
                            animator.speed = action.speed;
                            animator.Play(trigger);
                        }
                    } else {
                        Debug.LogWarning("Missing animation for " + action.objectName);
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