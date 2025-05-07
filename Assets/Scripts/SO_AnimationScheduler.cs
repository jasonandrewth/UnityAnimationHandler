using UnityEngine;
using System.Collections.Generic;

// [CreateAssetMenu(fileName = "AnimationAction", menuName = "ScriptableObjects/AnimationActionScriptableObject")]
// public class AnimationActionScriptableObject : ScriptableObject
// {
//     public string objectName;
//     public float time;
//     public float offset;
//     public float speed;
//     public string[] actions;
//     public FadeTrack[] fadeTracks;
// }

[CreateAssetMenu(fileName = "AnimationScheduler", menuName = "ScriptableObjects/AnimationScheduler")]
public class AnimationSchedulerScriptableObject : ScriptableObject {
    public List<AnimationAction> actions = new List<AnimationAction>();
}