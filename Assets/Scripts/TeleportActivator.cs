using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportActivator : MonoBehaviour
{
    public InputActionProperty teleportActivate;
    public XRRayInteractor rayInteractor;
    private void OnEnable()
    {
        teleportActivate.action.performed += ActivateTeleport;
    }
    private void OnDisable()
    {
        teleportActivate.action.performed -= ActivateTeleport;
    }
    private void ActivateTeleport(InputAction.CallbackContext context)
    {
        rayInteractor.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (rayInteractor.gameObject.activeSelf && teleportActivate.action.WasReleasedThisFrame())
        {
            rayInteractor.gameObject.SetActive(false);
        }
    }
}
