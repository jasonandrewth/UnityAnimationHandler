using UnityEngine;

public class LayeredAnimationTrigger : MonoBehaviour
{
    private Animator animator;
    private int currentLayer = 0;
    private int totalLayers;

    void Start()
    {
        animator = GetComponent<Animator>();
        totalLayers = animator.layerCount;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayNextAnimation();
        }
    }

    void PlayNextAnimation()
    {
        // Disable all layers first
        for (int i = 0; i < totalLayers; i++)
        {
            animator.SetLayerWeight(i, 0);
        }

        // Enable the current layer
        animator.SetLayerWeight(currentLayer, 1);

        // Optionally set a trigger or just rely on entry state
        animator.Play(0, currentLayer, 0); // Plays from the beginning

        // Advance to next layer index
        currentLayer = (currentLayer + 1) % totalLayers;
    }
}