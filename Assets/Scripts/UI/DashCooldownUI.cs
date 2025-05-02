using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Player player;

    void Update()
    {
        if (player == null || cooldownImage == null) return;

        float progress = player.DashCooldown();
        cooldownImage.fillAmount = 1f - progress;
    }
}
