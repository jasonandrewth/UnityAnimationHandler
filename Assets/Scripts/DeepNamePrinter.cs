using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class DeepNamePrinter : MonoBehaviour
{
    public AnimationSchedulerScriptableObject schedulerData;

    // Keep references to all clips you’ve created
    private List<AnimationClip> _clips = new List<AnimationClip>();

    private float startTime;
    private bool running = false;

    private Animation _animation;

    private AnimationAction _animData;

    void Start()
    {
        // Logs the root itself
        Debug.Log($"Root: {gameObject.name}");

        _animData = schedulerData.actions[0];

        // Kick off the recursive print
        PrintAllChildNames(transform);
    }

    private void PrintAllChildNames(Transform parent)
    {
        // Recursively traverses the scene and does the opacity tracks
        foreach (Transform child in parent)
        {
            AddAnimationTrack(child);
            // Recurse into grandchildren, etc.
            PrintAllChildNames(child);
        }
    }

    private void AddAnimationTrack(Transform child)
    {
        var fadeTracks = _animData.fadeTracks;

        // Find the FadeTrack whose clipName matches this child's name
        var track = System.Array.Find(fadeTracks, t => t.clipName == child.name);
        if (track != null)
        {
            Debug.Log($"Found FadeTrack for clip '{child.name}'");
            // You can now use 'track' here...
            // Create a legacy AnimationClip:
        
            // 3) Build an AnimationCurve from keyTimes / keyValues:
            var curve = new AnimationCurve();
            for (int i = 0; i < track.times.Length && i < track.weights.Length; i++)
            {
                curve.AddKey(track.times[i], track.weights[i]);
            }
            // Log the generated curve contents
            // Debug.Log($"AnimationCurve for '{child.name}' contains {curve.length} keyframes");
            // foreach (var key in curve.keys)
            // {
            //     Debug.Log($"Keyframe: time={key.time:F3}, value={key.value:F3}");
            // }
            // var anim = child.GetComponent<Animation>() ?? gameObject.AddComponent<Animation>();
         
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer)
            {
                Material mat = renderer.material;

                SetupTransparentMaterial(mat);

                StartCoroutine(FadeMaterial(mat, curve, track.blendDuration));

            }
        }
        else
        {
            Debug.LogWarning($"No FadeTrack found for clip '{child.name}'");
            return;
        }
    }

    private IEnumerator FadeMaterial(Material mat, AnimationCurve curve, float duration)
{
    float t = 0f;
    Color c = mat.color;

    while (t < duration)
    {
        float a = curve.Evaluate(t);
        c.a = a;
        mat.color = c;          // update alpha
        t += Time.deltaTime;
        yield return null;
    }

    // ensure final frame
    c.a = curve.Evaluate(duration);
    mat.color = c;
}



    
    private void SetupTransparentMaterial(Material mat)
    {
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

}