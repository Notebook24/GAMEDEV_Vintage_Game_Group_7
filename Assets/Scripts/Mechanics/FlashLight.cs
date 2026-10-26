using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class FlashLight : MonoBehaviour
{
    [SerializeField]
    private List<Transform> ghosts;
    [SerializeField]
    private float flashDistance = 30;
    [SerializeField]
    private float flashDuration = 0.05f;
    [SerializeField]
    private float flashFade = 2f;
    [SerializeField]
    private float flashIntensity = 100;
    [SerializeField]
    private float flashDrainBatteryAmount = 10f; //HOW MUCH EACH FLASH DRAINS BATTERY

    private Light light;
    private Camera camera;
    private float FOV = 90;
    private float flashBattery = 100f;
    private AudioSource flashAudio;

    // Add these public getters to FlashLight.cs
    public float FlashBattery => flashBattery;
    public float MaxBattery => 100f; // Matches initial flashBattery value

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // UIManager.Instance.UpdateFlashlightBattery(100f, 100f);
        GameObject[] ghostObj = GameObject.FindGameObjectsWithTag("Ghost");
        if (ghostObj.Length != 0)
        {
            foreach (GameObject obj in ghostObj) {
                ghosts.Add(obj.transform);
            }
        }

        camera = GetComponentInChildren<Camera>();
        if (camera == null)
        {
            Debug.LogError("put dis in camera");
            return;
        }
        FOV = camera.fieldOfView;

        light = GetComponentInChildren<Light>();
        if (light == null)
        {
            Debug.LogError("NEED LIGHT COMPONENT IN CAMERA");
            return;
        }

        flashAudio = GetComponent<AudioSource>();

        light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (flashBattery <= 0)
        {
            Debug.Log("OUT OF BATTERY!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(flashLight());
        foreach(Transform ghost in ghosts)
            detectMonster(ghost);
        flashBattery -= flashDrainBatteryAmount;
        flashBattery = Mathf.Max(flashBattery, 0f);

        flashAudio.PlayOneShot(flashAudio.clip);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateFlashlightBattery(flashBattery, 100f);
        }
        Debug.Log("FLASHLIGHT BATTERY: " + flashBattery);
    }


    private void detectMonster(Transform ghost)
    {
        Vector3 ghostPos = new Vector3(ghost.position.x, 1, ghost.position.z);

        if (Vector3.Distance(ghostPos, transform.position) > flashDistance)
            return;

        Vector3 lineToMonster = ghostPos - transform.position;
        if (Vector3.Angle(transform.forward, lineToMonster) > FOV / 2) return;

        RaycastHit hit;
        LayerMask layer = LayerMask.GetMask("Default");

        if (Physics.Raycast(transform.position, lineToMonster.normalized, out hit, flashDistance, layer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 2.0f);
            GhostAI ghostAI = hit.collider.transform.root.GetComponent<GhostAI>();
            if (ghostAI == null) return;


            Debug.Log("GHOST DETECTED");
            ghostAI.Startle();
        }
    }

    private IEnumerator flashLight()
    {
        light.intensity = flashIntensity;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0;
        while (elapsed < flashFade)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(flashIntensity, 0, elapsed / flashDuration);
            yield return null;
        }


        light.intensity = 0;
    }

    public void IncreaseBattery(float amount) {
        flashBattery = Mathf.Min(flashBattery + amount, 100f);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateFlashlightBattery(flashBattery, 100f);
        }
    }
}
