using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyIoC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UiSupportedViews {
    Start,
    Loading,
    World,
    Game
}

public class UiFacade {
    private Canvas canvas;
    private Dictionary<string, RectTransform> uiElements;
    private GameObject uiRoot;
    private UiService uiService;
    private Dictionary<UiSupportedViews, TextMeshProUGUI> viewToReportMap;

    public UiSupportedViews CurrentView { get; private set; }

    public async Task Start() {
        await Task.Yield();
        uiService = TinyIoCContainer.Current.Resolve<UiService>();
        uiRoot = GameObject.Find("UI");
        canvas = uiRoot.transform.Find("Canvas").GetComponent<Canvas>();

        uiElements = new Dictionary<string, RectTransform>();
        for (var i = 0; i < canvas.transform.childCount; i++) {
            var uiElement = canvas.transform.GetChild(i).gameObject;
            var elementName = uiElement.name;

            if (uiElement.TryGetComponent<RectTransform>(out var rectTransform))
                uiElements.Add(elementName, rectTransform);
            else
                throw new Exception("Missing RectTransform on Canvas Element: " + elementName);
        }

        viewToReportMap = new Dictionary<UiSupportedViews, TextMeshProUGUI>();
        foreach (var state in Enum.GetValues(typeof(UiSupportedViews))) {
            var supportedViewName = state.ToString();
            if (!uiElements.ContainsKey(supportedViewName))
                throw new Exception("Missing Supported View: " + supportedViewName);

            var rect = uiElements[supportedViewName];
            var reportElement = rect.Find("Report")?.GetComponent<TextMeshProUGUI>();
            if (reportElement)
                viewToReportMap[(UiSupportedViews)state] = reportElement;
        }

        SetupInteractions(UiSupportedViews.Start, uiService.StartWorld);
        SetupInteractions(UiSupportedViews.World, uiService.StartGame);
        SetView(UiSupportedViews.Start);
    }

    private void SetupInteractions(UiSupportedViews supportedView, Action callback) {
        var startElement = uiElements[supportedView.ToString()];
        var trigger = startElement.gameObject.AddComponent<EventTrigger>();
        var image = startElement.GetComponent<Image>();
        image.raycastTarget = true;

        var entry = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener(data => {
            callback.Invoke();
            image.raycastTarget = false;
        });
        trigger.triggers.Add(entry);
    }

    public void Report(UiSupportedViews view, string report) {
        if (viewToReportMap.ContainsKey(view))
            viewToReportMap[view].text = report;
    }

    public async Task ToLoading() {
        SetView(UiSupportedViews.Loading);
    }

    public async Task ToWorld() {
        SetView(UiSupportedViews.World);
    }

    public async Task ToGame() {
        SetView(UiSupportedViews.Game);
    }

    private void SetView(UiSupportedViews supportedView) {
        var elementName = supportedView.ToString();

        if (!uiElements.ContainsKey(elementName))
            throw new Exception("Invalid attempt activating UI Element: " + elementName);

        foreach (var kvp in uiElements) {
            var active = kvp.Key == elementName;
            kvp.Value.gameObject.SetActive(active);
        }

        CurrentView = supportedView;
    }
}
