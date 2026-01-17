using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationEditManager : MonoBehaviour
{
    private const string EVENT_DATA_PATH = "Assets/Data/AnimationEvents";

    [Header("References")]
    public Animator animator;
    public Canvas canvas;

    private TMP_Dropdown motionDropdown;
    private Slider timelineSlider;
    private TMP_Text timeText;
    private Button playButton;
    private Button stopButton;
    private Button prevFrameButton;
    private Button nextFrameButton;
    private TMP_Text playButtonText;
    private TMP_InputField eventNameInput;
    private Button addEventButton;
    private Transform eventListContent;

    private AnimationClip[] clips;
    private AnimationClip selectedClip;
    private ePlayerMotionType selectedMotion;
    private AnimationEventData currentEventData;

    private float normalizedTime;
    private bool isPlaying;

    private void Start()
    {
        CreateUI();
        SetupMotionDropdown();
        SetupButtons();

        if (animator != null)
        {
            animator.speed = 0;
            if (animator.runtimeAnimatorController != null)
                LoadClips();
        }

        timelineSlider.onValueChanged.AddListener(OnTimelineChanged);
    }

    private void Update()
    {
        if (!isPlaying) return;

        normalizedTime += Time.deltaTime / GetClipLength();
        if (normalizedTime >= 1f)
            normalizedTime = 0f;

        timelineSlider.SetValueWithoutNotify(normalizedTime);
        UpdateTimeText();
        UpdateAnimation();
    }

    private float GetClipLength()
    {
        if (selectedClip != null) return selectedClip.length;
        return 1f;
    }

    private void CreateUI()
    {
        var panel = CreatePanel("MainPanel", canvas.transform, new Vector2(800, 1000), new Vector2(-40, 0));

        float y = -20;

        CreateLabel("Motion", panel.transform, new Vector2(360, 40), new Vector2(0, y));
        motionDropdown = CreateDropdown("MotionDropdown", panel.transform, new Vector2(360, 50), new Vector2(0, y - 40));
        y -= 100;

        CreateLabel("Timeline", panel.transform, new Vector2(360, 40), new Vector2(0, y));
        timelineSlider = CreateSlider("TimelineSlider", panel.transform, new Vector2(360, 30), new Vector2(0, y - 45));
        timeText = CreateLabel("0.000 (0.000s)", panel.transform, new Vector2(360, 30), new Vector2(0, y - 80));
        y -= 120;

        var buttonPanel = new GameObject("ButtonPanel", typeof(RectTransform));
        buttonPanel.transform.SetParent(panel.transform, false);
        var buttonPanelRt = buttonPanel.GetComponent<RectTransform>();
        buttonPanelRt.sizeDelta = new Vector2(320, 60);
        buttonPanelRt.anchorMin = new Vector2(0.5f, 1);
        buttonPanelRt.anchorMax = new Vector2(0.5f, 1);
        buttonPanelRt.pivot = new Vector2(0.5f, 1);
        buttonPanelRt.anchoredPosition = new Vector2(0, y);

        playButton = CreateButton("Play", buttonPanel.transform, new Vector2(80, 50), new Vector2(-120, 0));
        playButtonText = playButton.GetComponentInChildren<TMP_Text>();
        stopButton = CreateButton("Stop", buttonPanel.transform, new Vector2(80, 50), new Vector2(-35, 0));
        prevFrameButton = CreateButton("<", buttonPanel.transform, new Vector2(50, 50), new Vector2(35, 0));
        nextFrameButton = CreateButton(">", buttonPanel.transform, new Vector2(50, 50), new Vector2(95, 0));
        y -= 80;

        CreateLabel("Event Name", panel.transform, new Vector2(360, 40), new Vector2(0, y));
        eventNameInput = CreateInputField("EventNameInput", panel.transform, new Vector2(260, 50), new Vector2(-50, y - 45));
        addEventButton = CreateButton("+", panel.transform, new Vector2(70, 50), new Vector2(145, y - 45));
        y -= 110;

        CreateLabel("Events", panel.transform, new Vector2(360, 40), new Vector2(0, y));
        eventListContent = CreateScrollView("EventList", panel.transform, new Vector2(360, 350), new Vector2(0, y - 50));
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(1, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot = new Vector2(1, 0.5f);
        rt.anchoredPosition = pos;
        go.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        return go;
    }

    private TMP_Text CreateLabel(string text, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private TMP_Dropdown CreateDropdown(string name, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_Dropdown));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);

        var label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(go.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(5, 0);
        labelRt.offsetMax = new Vector2(-35, 0);
        var labelTmp = label.GetComponent<TextMeshProUGUI>();
        labelTmp.fontSize = 20;
        labelTmp.alignment = TextAlignmentOptions.Center;

        var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
        arrow.transform.SetParent(go.transform, false);
        var arrowRt = arrow.GetComponent<RectTransform>();
        arrowRt.sizeDelta = new Vector2(25, 25);
        arrowRt.anchorMin = new Vector2(1, 0.5f);
        arrowRt.anchorMax = new Vector2(1, 0.5f);
        arrowRt.pivot = new Vector2(1, 0.5f);
        arrowRt.anchoredPosition = new Vector2(-10, 0);

        var template = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect), typeof(Canvas), typeof(UnityEngine.UI.GraphicRaycaster));
        template.transform.SetParent(go.transform, false);
        var templateRt = template.GetComponent<RectTransform>();
        templateRt.sizeDelta = new Vector2(size.x, 300);
        templateRt.anchorMin = new Vector2(0, 0);
        templateRt.anchorMax = new Vector2(1, 0);
        templateRt.pivot = new Vector2(0.5f, 1);
        templateRt.anchoredPosition = new Vector2(0, 0);
        template.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);
        template.GetComponent<Canvas>().overrideSorting = true;
        template.GetComponent<Canvas>().sortingOrder = 100;
        template.SetActive(false);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
        viewport.transform.SetParent(template.transform, false);
        var viewportRt = viewport.GetComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.GetComponent<RectTransform>();
        contentRt.sizeDelta = new Vector2(size.x, 56);
        contentRt.anchorMin = new Vector2(0, 1);
        contentRt.anchorMax = new Vector2(1, 1);
        contentRt.pivot = new Vector2(0.5f, 1);

        var item = new GameObject("Item", typeof(RectTransform), typeof(Toggle));
        item.transform.SetParent(content.transform, false);
        var itemRt = item.GetComponent<RectTransform>();
        itemRt.sizeDelta = new Vector2(size.x, 56);
        itemRt.anchorMin = new Vector2(0, 0.5f);
        itemRt.anchorMax = new Vector2(1, 0.5f);
        itemRt.pivot = new Vector2(0.5f, 0.5f);

        var itemBg = new GameObject("Item Background", typeof(RectTransform), typeof(Image));
        itemBg.transform.SetParent(item.transform, false);
        var itemBgRt = itemBg.GetComponent<RectTransform>();
        itemBgRt.anchorMin = Vector2.zero;
        itemBgRt.anchorMax = Vector2.one;
        itemBgRt.offsetMin = Vector2.zero;
        itemBgRt.offsetMax = Vector2.zero;
        itemBg.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

        var itemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        itemLabel.transform.SetParent(item.transform, false);
        var itemLabelRt = itemLabel.GetComponent<RectTransform>();
        itemLabelRt.anchorMin = Vector2.zero;
        itemLabelRt.anchorMax = Vector2.one;
        itemLabelRt.offsetMin = new Vector2(5, 0);
        itemLabelRt.offsetMax = new Vector2(-5, 0);
        var itemLabelTmp = itemLabel.GetComponent<TextMeshProUGUI>();
        itemLabelTmp.fontSize = 20;
        itemLabelTmp.alignment = TextAlignmentOptions.Center;

        var toggle = item.GetComponent<Toggle>();
        toggle.targetGraphic = itemBg.GetComponent<Image>();

        var scrollRect = template.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRt;
        scrollRect.content = contentRt;

        var dropdown = go.GetComponent<TMP_Dropdown>();
        dropdown.targetGraphic = go.GetComponent<Image>();
        dropdown.template = templateRt;
        dropdown.captionText = labelTmp;
        dropdown.itemText = itemLabelTmp;

        return dropdown;
    }

    private Slider CreateSlider(string name, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;

        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(go.transform, false);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        var fillAreaRt = fillArea.GetComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = new Vector2(10, 0);
        fillAreaRt.offsetMax = new Vector2(-10, 0);

        var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fill.GetComponent<Image>().color = new Color(0.3f, 0.6f, 1f);

        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(go.transform, false);
        var handleAreaRt = handleArea.GetComponent<RectTransform>();
        handleAreaRt.anchorMin = Vector2.zero;
        handleAreaRt.anchorMax = Vector2.one;
        handleAreaRt.offsetMin = new Vector2(20, 0);
        handleAreaRt.offsetMax = new Vector2(-20, 0);

        var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        var handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(20, 0);
        handle.GetComponent<Image>().color = Color.white;

        var slider = go.GetComponent<Slider>();
        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.minValue = 0;
        slider.maxValue = 1;

        return slider;
    }

    private Button CreateButton(string text, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        go.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);

        var label = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(go.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;
        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();

        return btn;
    }

    private TMP_InputField CreateInputField(string name, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
        go.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

        var textArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
        textArea.transform.SetParent(go.transform, false);
        var textAreaRt = textArea.GetComponent<RectTransform>();
        textAreaRt.anchorMin = Vector2.zero;
        textAreaRt.anchorMax = Vector2.one;
        textAreaRt.offsetMin = new Vector2(10, 0);
        textAreaRt.offsetMax = new Vector2(-10, 0);

        var placeholder = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
        placeholder.transform.SetParent(textArea.transform, false);
        var placeholderRt = placeholder.GetComponent<RectTransform>();
        placeholderRt.anchorMin = Vector2.zero;
        placeholderRt.anchorMax = Vector2.one;
        placeholderRt.offsetMin = Vector2.zero;
        placeholderRt.offsetMax = Vector2.zero;
        var placeholderTmp = placeholder.GetComponent<TextMeshProUGUI>();
        placeholderTmp.text = "Enter event name...";
        placeholderTmp.fontSize = 20;
        placeholderTmp.fontStyle = FontStyles.Italic;
        placeholderTmp.color = new Color(0.7f, 0.7f, 0.7f);
        placeholderTmp.alignment = TextAlignmentOptions.Left;

        var textComponent = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textComponent.transform.SetParent(textArea.transform, false);
        var textRt = textComponent.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
        var tmp = textComponent.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = 20;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;

        var inputField = go.GetComponent<TMP_InputField>();
        inputField.textViewport = textAreaRt;
        inputField.textComponent = tmp;
        inputField.placeholder = placeholderTmp;

        return inputField;
    }

    private Transform CreateScrollView(string name, Transform parent, Vector2 size, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
        go.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
        viewport.transform.SetParent(go.transform, false);
        var viewportRt = viewport.GetComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 1);
        contentRt.anchorMax = new Vector2(1, 1);
        contentRt.pivot = new Vector2(0.5f, 1);
        contentRt.sizeDelta = new Vector2(0, 0);

        var vlg = content.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(10, 10, 10, 10);

        var csf = content.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scrollRect = go.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRt;
        scrollRect.content = contentRt;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        return content.transform;
    }

    private GameObject CreateEventItem(AnimEventInfo evt, int index)
    {
        var go = new GameObject("EventItem", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
        go.GetComponent<LayoutElement>().preferredHeight = 60;

        var text = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        text.transform.SetParent(go.transform, false);
        var textRt = text.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10, 0);
        textRt.offsetMax = new Vector2(-120, 0);
        var tmp = text.GetComponent<TextMeshProUGUI>();
        tmp.text = $"{evt.eventName} @ {evt.normalizedTime:F3}";
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Left;

        var selectBtn = CreateButton(">", go.transform, new Vector2(50, 50), new Vector2(-70, 0));
        selectBtn.name = "SelectButton";
        selectBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        selectBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        float capturedTime = evt.normalizedTime;
        selectBtn.onClick.AddListener(() =>
        {
            normalizedTime = capturedTime;
            timelineSlider.SetValueWithoutNotify(normalizedTime);
            UpdateTimeText();
        });

        var deleteBtn = CreateButton("X", go.transform, new Vector2(50, 50), new Vector2(-14, 0));
        deleteBtn.name = "DeleteButton";
        deleteBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        deleteBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        deleteBtn.GetComponent<Image>().color = new Color(0.6f, 0.2f, 0.2f);
        int capturedIndex = index;
        deleteBtn.onClick.AddListener(() => DeleteEvent(capturedIndex));

        return go;
    }

    private void SetupMotionDropdown()
    {
        motionDropdown.ClearOptions();
        var options = System.Enum.GetNames(typeof(ePlayerMotionType)).ToList();
        motionDropdown.AddOptions(options);
        motionDropdown.onValueChanged.AddListener(OnMotionChanged);

        selectedMotion = (ePlayerMotionType)0;
        LoadEventData();
    }

    private void LoadClips()
    {
        clips = animator.runtimeAnimatorController.animationClips;
        UpdateSelectedClip();
    }

    private void UpdateSelectedClip()
    {
        string motionName = selectedMotion.ToString();
        selectedClip = clips?.FirstOrDefault(c => c.name.Contains(motionName));
    }

    private void SetupButtons()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        prevFrameButton.onClick.AddListener(OnPrevFrame);
        nextFrameButton.onClick.AddListener(OnNextFrame);
        addEventButton.onClick.AddListener(OnAddEvent);
    }

    private void OnMotionChanged(int index)
    {
        selectedMotion = (ePlayerMotionType)index;
        normalizedTime = 0f;
        timelineSlider.SetValueWithoutNotify(0f);
        UpdateSelectedClip();
        LoadEventData();
        RefreshEventList();
        UpdateAnimation();
    }

    private void OnTimelineChanged(float value)
    {
        normalizedTime = value;
        isPlaying = false;
        UpdatePlayButtonText();
        UpdateTimeText();
        UpdateAnimation();
    }

    private void OnPlayClicked()
    {
        isPlaying = !isPlaying;
        UpdatePlayButtonText();
    }

    private void OnStopClicked()
    {
        isPlaying = false;
        normalizedTime = 0f;
        timelineSlider.SetValueWithoutNotify(0f);
        UpdatePlayButtonText();
        UpdateTimeText();
        UpdateAnimation();
    }

    private void OnPrevFrame()
    {
        float frameStep = 1f / 60f / GetClipLength();
        normalizedTime = Mathf.Max(0f, normalizedTime - frameStep);
        timelineSlider.SetValueWithoutNotify(normalizedTime);
        UpdateTimeText();
        UpdateAnimation();
    }

    private void OnNextFrame()
    {
        float frameStep = 1f / 60f / GetClipLength();
        normalizedTime = Mathf.Min(1f, normalizedTime + frameStep);
        timelineSlider.SetValueWithoutNotify(normalizedTime);
        UpdateTimeText();
        UpdateAnimation();
    }

    private void UpdatePlayButtonText()
    {
        playButtonText.text = isPlaying ? "||" : "Play";
    }

    private void UpdateTimeText()
    {
        if (selectedClip != null)
            timeText.text = $"{normalizedTime:F3} ({normalizedTime * selectedClip.length:F3}s)";
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.Play(selectedMotion.ToString(), 0, normalizedTime);
        animator.Update(0f);
    }

    private void LoadEventData()
    {
#if UNITY_EDITOR
        string path = $"{EVENT_DATA_PATH}/{selectedMotion}.asset";
        currentEventData = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationEventData>(path);
#endif
    }

    private AnimationEventData GetOrCreateEventData()
    {
#if UNITY_EDITOR
        if (currentEventData != null && currentEventData.motion == selectedMotion)
            return currentEventData;

        string path = $"{EVENT_DATA_PATH}/{selectedMotion}.asset";
        currentEventData = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationEventData>(path);

        if (currentEventData == null)
        {
            currentEventData = ScriptableObject.CreateInstance<AnimationEventData>();
            currentEventData.motion = selectedMotion;
            currentEventData.events = new List<AnimEventInfo>();

            if (!Directory.Exists(EVENT_DATA_PATH))
                Directory.CreateDirectory(EVENT_DATA_PATH);

            UnityEditor.AssetDatabase.CreateAsset(currentEventData, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        return currentEventData;
#else
        return null;
#endif
    }

    private void OnAddEvent()
    {
#if UNITY_EDITOR
        string eventName = eventNameInput.text;
        if (string.IsNullOrEmpty(eventName)) return;

        var data = GetOrCreateEventData();
        data.events.Add(new AnimEventInfo
        {
            eventName = eventName,
            normalizedTime = normalizedTime
        });

        UnityEditor.EditorUtility.SetDirty(data);
        UnityEditor.AssetDatabase.SaveAssets();

        eventNameInput.text = "";
        RefreshEventList();
#endif
    }

    private void RefreshEventList()
    {
        foreach (Transform child in eventListContent)
            Destroy(child.gameObject);

        if (currentEventData == null) return;

        for (int i = 0; i < currentEventData.events.Count; i++)
        {
            var item = CreateEventItem(currentEventData.events[i], i);
            item.transform.SetParent(eventListContent, false);
        }
    }

    private void DeleteEvent(int index)
    {
#if UNITY_EDITOR
        if (currentEventData == null || index >= currentEventData.events.Count) return;

        currentEventData.events.RemoveAt(index);
        UnityEditor.EditorUtility.SetDirty(currentEventData);
        UnityEditor.AssetDatabase.SaveAssets();
        RefreshEventList();
#endif
    }
}
