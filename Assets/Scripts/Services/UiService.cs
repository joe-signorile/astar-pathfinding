using System.Threading.Tasks;
using TinyIoC;

public class UiService {
    private readonly AppLoader appLoader;
    private readonly UiFacade facade;

    public UiService() {
        appLoader = TinyIoCContainer.Current.Resolve<AppLoader>();
        facade = new UiFacade();
    }

    public UiSupportedViews CurrentView => facade.CurrentView;

    public async Task Start() {
        await facade.Start();
    }

    public async void StartWorld() {
        await facade.ToLoading();
        await appLoader.StartWorld();
        await facade.ToWorld();
    }

    public async void StartGame() {
        await appLoader.StartGame();
        await facade.ToGame();
    }
}
