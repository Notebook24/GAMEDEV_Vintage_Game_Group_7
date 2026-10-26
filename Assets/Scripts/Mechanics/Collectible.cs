using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10;
    private AudioSource sound;

    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sound.pitch = Random.Range(0.85f, 1.15f);
            sound.Play();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            other.GetComponentInChildren<FlashLight>().IncreaseBattery(0.5f);
            Destroy(gameObject, 0.5f);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }
        }
    }
}
