using UnityEngine;

[System.Serializable]
public class AnimationAction
{
    public string objectName;
    public float time;
    public float offset;
    public string[] actions;
    public float speed = 1f;
    public FadeTrack[] fadeTracks;
}