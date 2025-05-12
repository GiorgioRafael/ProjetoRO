using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FrameScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Toggle toggle; // Optional
    public RectTransform frame;
    public Vector3 selectedScale = new Vector3(1.03f, 1.07f, 1f);
    public Vector3 normalScale = Vector3.one;
    public float scaleSpeed = 5f;

    private Vector3 targetScale;
    private bool isHovered = false;
    private bool isSelected = false;

    void Start()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            isSelected = toggle.isOn;
        }

        targetScale = normalScale;
        frame.localScale = normalScale;
    }

    void Update()
    {
        frame.localScale = Vector3.Lerp(frame.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateTargetScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateTargetScale();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        isSelected = isOn;
        UpdateTargetScale();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateTargetScale();
    }

    private void UpdateTargetScale()
    {
        targetScale = (isHovered || isSelected) ? selectedScale : normalScale;
    }
}
