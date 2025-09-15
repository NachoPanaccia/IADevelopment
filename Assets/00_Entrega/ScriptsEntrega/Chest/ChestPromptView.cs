using UnityEngine;
using TMPro;

public class ChestPromptView : MonoBehaviour, IShowPrompt
{
    [Header("Referencia al objeto del prompt (RectTransform con TMP_Text)")]
    [SerializeField] private RectTransform promptRect;

    [Header("Texto")]
    [SerializeField] private string message = "Presioná \"E\" para abrirlo";

    [Header("Colocación")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private bool faceCameraInWorldSpace = true; // solo aplica para Canvas World Space

    Transform _self;
    TMP_Text _label;
    Canvas _canvas;               // canvas que contiene al prompt (si existe)
    RectTransform _canvasRect;    // root rect del canvas
    bool _visible;

    void Reset()
    {
        // Autoasigna si tu hijo se llama "Prompt"
        var child = transform.Find("Prompt");
        if (child) promptRect = child as RectTransform;
    }

    void Awake()
    {
        _self = transform;

        if (!promptRect)
        {
            Debug.LogWarning("[ChestPromptView] Falta asignar 'promptRect'.");
            return;
        }

        _label = promptRect.GetComponentInChildren<TMP_Text>(true);
        if (_label) _label.text = message;

        _canvas = promptRect.GetComponentInParent<Canvas>(true);
        _canvasRect = _canvas ? _canvas.transform as RectTransform : null;

        SetPromptVisible(false);
    }

    void LateUpdate()
    {
        if (!_visible || !promptRect) return;

        Vector3 worldPos = _self.position + worldOffset;

        if (_canvas == null || _canvas.renderMode == RenderMode.WorldSpace)
        {
            // WORLD SPACE: colocación en coordenadas de mundo
            promptRect.position = worldPos;

            if (faceCameraInWorldSpace && Camera.main)
            {
                var cam = Camera.main.transform;
                promptRect.rotation = Quaternion.LookRotation(promptRect.position - cam.position, Vector3.up);
            }
        }
        else if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // OVERLAY: posición en pantalla
            if (Camera.main)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(worldPos);
                promptRect.position = screen; // en Overlay, position es en pixeles de pantalla
            }
        }
        else // ScreenSpaceCamera
        {
            if (Camera.main && _canvasRect)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(worldPos);
                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    screen,
                    _canvas.worldCamera,
                    out local
                );
                promptRect.anchoredPosition = local;
            }
        }
    }

    public void SetPromptVisible(bool visible)
    {
        _visible = visible && promptRect != null;
        if (promptRect) promptRect.gameObject.SetActive(_visible);
    }
}
