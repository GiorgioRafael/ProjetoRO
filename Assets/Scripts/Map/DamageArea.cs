using UnityEngine;
using TMPro;
using System.Collections;


public class DamageArea : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public float damageCooldown = 1f;

    [Header("UI Warning")]
    public TextMeshProUGUI warningText;
    public string warningMessage = "VOCÊ ESTÁ ANDANDO NA LAVA";
    public float textPersistTime = 1f;  // How long the text stays after leaving area

    private float textHideTimer = 0f;
    private static int activeAreas = 0;  // Track how many areas player is in


    [Header("Shake Settings")]
    public float shakeIntensity = 10f;
    public float shakeSpeed = 10f;

    private float cooldownTimer = 0f;
    private PlayerStats playerInArea;
    private Vector2 textOriginalPosition;
    private bool isShaking = false;

    void Start()
    {
        // Find warning text if not assigned
        if (warningText == null)
        {
            warningText = GameObject.Find("Canvas/Screens/Damage Warning")
                            ?.GetComponent<TextMeshProUGUI>();

            if (warningText == null)
            {
                Debug.LogWarning("DamageArea: Could not find Damage Warning text in Canvas/Screens/Damage Warning");
                return;
            }
        }

        // Store original anchored position and hide warning
        textOriginalPosition = warningText.rectTransform.anchoredPosition;
        warningText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInArea != null)
        {
            if (cooldownTimer <= 0)
            {
                // Deal damage to player
                playerInArea.TakeDamage(damage);
                cooldownTimer = damageCooldown;
            }
            else
            {
                cooldownTimer -= Time.deltaTime;
            }
        }

        if (isShaking && warningText != null)
        {
            float offsetX = Mathf.Sin(Time.unscaledTime * shakeSpeed) * shakeIntensity;
            float offsetY = Mathf.Cos(Time.unscaledTime * shakeSpeed * 1.2f) * shakeIntensity;

            warningText.rectTransform.anchoredPosition = textOriginalPosition + new Vector2(offsetX, offsetY);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            activeAreas++;
            playerInArea = player;
            if (warningText != null)
            {
                StopAllCoroutines(); // Stop any running hide coroutines
                warningText.gameObject.SetActive(true);
                warningText.text = warningMessage;
                warningText.rectTransform.anchoredPosition = textOriginalPosition;
                isShaking = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerStats>(out PlayerStats player) && player == playerInArea)
        {
            activeAreas--;
            playerInArea = null;
            cooldownTimer = 0f;

            // Only hide text if player left all damage areas
            if (activeAreas <= 0)
            {
                activeAreas = 0; // Reset to ensure no negative values
                StartCoroutine(HideTextAfterDelay());
            }
        }
    }
    
        private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(textPersistTime);

        if (warningText != null && activeAreas <= 0)
        {
            isShaking = false;
            warningText.rectTransform.anchoredPosition = textOriginalPosition;
            warningText.gameObject.SetActive(false);
        }
    }
}