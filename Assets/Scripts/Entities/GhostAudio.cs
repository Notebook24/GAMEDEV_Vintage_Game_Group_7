using UnityEngine;

public class GhostAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource footstep;
    private GhostAI ai;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ai = GetComponent<GhostAI>();
        footstep.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive)
        {
            if (footstep != null && footstep.isPlaying)
            {
                footstep.Stop();
            }
            return;
        }

        if (!ai.isMoving())
        {
            if (footstep.isPlaying) footstep.Pause();
            return;
        }
        else
        {
            if (!footstep.isPlaying) footstep.Play();
        }

        if (ai.getCurrentState() == GhostAI.State.NonAggressiveStalking)
        {
            footstep.volume = 0.2f;
            footstep.pitch = 0.5f;
        }
        else
        {
            footstep.volume = 1;
            footstep.pitch = 1;
        }
    }

}
