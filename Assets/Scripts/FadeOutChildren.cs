using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeOutChildren : MonoBehaviour
{
    private Animator animator;
    private float fadeDuration = 1f;
    private float lastNormalizedTime = 0f;
    private List<Material> childMaterials = new List<Material>();
    private string lastClipName = "";


    void Start()
    {
        animator = GetComponent<Animator>();

        // Try to get current clip duration
        if (animator.runtimeAnimatorController != null)
        {
            var currentClip = animator.GetCurrentAnimatorClipInfo(0);
            if (currentClip.Length > 0)
            {
                string clipName = currentClip[0].clip.name;
                Debug.Log("Currently playing clip: " + clipName);
                fadeDuration = currentClip[0].clip.length;
            }
        }

        foreach (Transform child in transform)
        {
            // print(child.GetComponent<Renderer>().material);
            // StartCoroutine(FadeOut(child.gameObject));
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer)
            {
                Material mat = renderer.material;

                SetupTransparentMaterial(mat);

                childMaterials.Add(mat);
                StartCoroutine(FadeOut(mat));
            }
        }
    }

    void Update()
    {
        if (!animator) return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            string currentClip = clipInfo[0].clip.name;

            if (currentClip != lastClipName)
            {
                Debug.Log("Animation changed to: " + currentClip);
                lastClipName = currentClip;
            }
        }

        // Check for loop reset
        float normalizedTime = state.normalizedTime % 1f;

        if (normalizedTime < lastNormalizedTime)
        {
            // Animation looped — reset opacity
            foreach (var mat in childMaterials)
            {
                Color c = mat.color;
                mat.color = new Color(c.r, c.g, c.b, 1f);
            }

            // Start fading again
            foreach (var mat in childMaterials)
            {
                StartCoroutine(FadeOut(mat));
            }
        }

        lastNormalizedTime = normalizedTime;
    }

    private IEnumerator FadeOut(Material mat)
    {
        Color originalColor = mat.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
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