using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class StaminaBar : MonoBehaviour
{
    // ===== EDIT THESE IN CODE (not shown in the Inspector) =====
    // Layout (pixels, 1920x1080 reference)
    static readonly Vector2 Size   = new Vector2(450f, 45f);
    static readonly Vector2 Margin = new Vector2(40f, 40f);
    const float FillPadding = 3f;

    // Label
    const string LabelText = "Stamina";
    const int LabelFontSize = 35;
    const float LabelGap = 6f;
    static readonly Color LabelColor = Color.white;

    // Colors
    static readonly Color BackColor = new Color(0f, 0f, 0f, 0.6f);
    static readonly Color FullColor = new Color(0.2f, 0.9f, 0.3f, 1f);
    static readonly Color LowColor  = new Color(0.9f, 0.1f, 0.1f, 1f);
    // ===========================================================

    PlayerMovement player;
    RectTransform fillRect;
    Image fillImage;

    void Awake()
    {
        player = GetComponent<PlayerMovement>();
        BuildUI();
    }

    void BuildUI()
    {
        // Canvas
        var canvasGO = new GameObject("StaminaCanvas");
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Background (bottom-left)
        var bgGO = new GameObject("StaminaBG");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bg = bgGO.AddComponent<Image>();
        bg.color = BackColor;
        var bgRect = bg.rectTransform;
        bgRect.anchorMin = bgRect.anchorMax = bgRect.pivot = Vector2.zero;
        bgRect.anchoredPosition = Margin;
        bgRect.sizeDelta = Size;

        // Fill
        var fillGO = new GameObject("StaminaFill");
        fillGO.transform.SetParent(bgGO.transform, false);
        fillImage = fillGO.AddComponent<Image>();
        fillRect = fillImage.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(FillPadding, FillPadding);
        fillRect.offsetMax = new Vector2(-FillPadding, -FillPadding);

        // Label above the bar
        var labelGO = new GameObject("StaminaLabel");
        labelGO.transform.SetParent(canvasGO.transform, false);
        var label = labelGO.AddComponent<Text>();
        label.text = LabelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = LabelFontSize;
        label.color = LabelColor;
        label.alignment = TextAnchor.LowerLeft;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        var labelRect = label.rectTransform;
        labelRect.anchorMin = labelRect.anchorMax = labelRect.pivot = Vector2.zero;
        labelRect.anchoredPosition = Margin + new Vector2(0f, Size.y + LabelGap);
        labelRect.sizeDelta = new Vector2(Size.x, LabelFontSize + 8f);
    }

    void Update()
    {
        float n = Mathf.Clamp01(player.StaminaNormalized);
        fillRect.anchorMax = new Vector2(n, 1f);
        fillImage.color = Color.Lerp(LowColor, FullColor, n);
    }
}