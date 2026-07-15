using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportActivator : MonoBehaviour
{
    public InputActionProperty teleportActivate;
    public XRRayInteractor teleportRayInteractor;
    public XRRayInteractor rayInteractor;

    private void Start()
    {
        DisableTeleportRay();
    }

    private void OnEnable()
    {
        teleportActivate.action.performed += ActivateTeleport;
        rayInteractor.uiHoverEntered.AddListener(x => DisableTeleportRay());
    }
    private void OnDisable()
    {
        teleportActivate.action.performed -= ActivateTeleport;
    }
    private void ActivateTeleport(InputAction.CallbackContext context)
    {
        if (rayInteractor && rayInteractor.IsOverUIGameObject())
            return;
        teleportRayInteractor.gameObject.SetActive(true);
    }

    private void DisableTeleportRay()
    {
        teleportRayInteractor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (teleportRayInteractor.gameObject.activeSelf && teleportActivate.action.WasReleasedThisFrame())
        {
            DisableTeleportRay();
        }
    }
}
