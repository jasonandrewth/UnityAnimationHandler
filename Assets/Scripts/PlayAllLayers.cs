using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayAllLayers : MonoBehaviour
{
    public AnimationSchedulerScriptableObject schedulerData;
    private Animator _animator;

    private AnimationAction _animData;

    [Tooltip("Maximum random delay (seconds) before each layer starts")]
    public float maxLayerDelay = 0.7f;

    [Tooltip("Global speed multiplier for all layers")]
    public float globalPlaybackSpeed = 1f;


    void Start()
    {
        _animData = schedulerData.actions[0];
        _animator = GetComponent<Animator>();
        // Apply the global speed multiplier
        _animator.speed = _animData.speed;
        PlayEveryLayer();
    }
    private void PlayEveryLayer()
    {
        // Also play all layers immediately in a simple way
    
        int layers = _animator.layerCount;
        for (int i = 0; i < layers; i++)
        {
            // Start each layer after a random delay
            StartCoroutine(PlayLayerWithDelay(i));
        }
        
    }

    private IEnumerator PlayLayerWithDelay(int layerIndex)
    {
        // Wait for a random delay up to maxLayerDelay
        float delay = _animData.offset * layerIndex;
        yield return new WaitForSeconds(delay);

        // Ensure full weight
        _animator.SetLayerWeight(layerIndex, 1f);

        // Replay the current state from the beginning
        var state = _animator.GetCurrentAnimatorStateInfo(layerIndex);
        _animator.Play(state.fullPathHash, layerIndex, 0f);
    }

}
