using UnityEngine;
using UnityEngine.InputSystem;

public class GhostAITester : MonoBehaviour
{
    [SerializeField]
    private GhostAI ghost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            ghost.increaseAgression();
        } else if (Mouse.current.leftButton.wasPressedThisFrame) {
            ghost.Startle();
        }
    }
}
