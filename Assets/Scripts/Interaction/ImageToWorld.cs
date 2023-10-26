using TinyIoC;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageToWorld : MonoBehaviour, IPointerDownHandler {
    private Camera mainCamera;
    private Masks masks;
    private PlayerService playerService;

    private void Start() {
        mainCamera = Camera.main;
        masks = new Masks();

        playerService = TinyIoCContainer.Current.Resolve<PlayerService>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        var screenPos = eventData.position;
        var ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 100, masks.Raycast)) {
            var hitLayer = hit.collider.gameObject.layer;
            if ((masks.Ground & (1 << hitLayer)) != 0)
                playerService.MoveTo(hit.point);
        }
    }
}
