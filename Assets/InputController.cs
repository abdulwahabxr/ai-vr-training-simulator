using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public Animator animator;
    public InputActionProperty grip, trigger;

    private void Update()
    {
        float gripValue = grip.action.ReadValue<float>();
        float triggerValue = trigger.action.ReadValue<float>();
        animator.SetFloat("Grip", gripValue);
        animator.SetFloat("Trigger", triggerValue);
    }
}
