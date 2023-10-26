using System;
using System.Threading.Tasks;
using TinyIoC;

public class GameLoopService {
    private readonly Task delay = Task.Delay(100);
    private DateTime gameStart;
    private UiService uiService;

    public void Start() {
        var current = TinyIoCContainer.Current;
        uiService = current.Resolve<UiService>();
        ObserveGameplay();
    }

    private async void ObserveGameplay() {
        gameStart = DateTime.UtcNow;
        while (true) await Task.Delay(100);
    }

    public TimeSpan GetGameDuration() {
        return DateTime.UtcNow - gameStart;
    }
}
