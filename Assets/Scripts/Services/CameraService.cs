using TinyIoC;
using UnityEngine;

public class CameraService {
    private PlayerService playerService;

    public void Start() {
        playerService = TinyIoCContainer.Current.Resolve<PlayerService>();
        var cameraRig = GameObject.Find("Camera Rig").GetComponent<CameraRig>();
        cameraRig.SetTarget(playerService.playerTransform);
    }
}
