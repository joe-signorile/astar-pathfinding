using System.Threading.Tasks;
using TinyIoC;
using UnityEngine;

public class AppLoader : MonoBehaviour {
    private TinyIoCContainer container;

    private async void Start() {
        container = TinyIoCContainer.Current;
        container.Register(this);

        await StartUi();
    }

    private async Task StartUi() {
        Debug.Log("UI Starting");
        var uiService = new UiService();
        container.Register(uiService);
        await uiService.Start();

        Debug.Log("UI Started");
    }

    public async Task StartWorld() {
        Debug.Log("World Starting");
        var worldService = new WorldService();
        container.Register(worldService);
        await worldService.Start();

        var pathfindingService = new PathfindingService();
        container.Register(pathfindingService);
        await pathfindingService.Start();

        Debug.Log("World Started");
    }

    public async Task StartGame() {
        Debug.Log("Game Starting");
        var playerService = new PlayerService();
        container.Register(playerService);
        playerService.Start();

        var cameraService = new CameraService();
        cameraService.Start();
        container.Register(cameraService);

        var gameLoopService = new GameLoopService();
        gameLoopService.Start();
        container.Register(gameLoopService);

        Debug.Log("Game Started");
    }
}
