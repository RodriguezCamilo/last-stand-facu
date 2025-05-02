using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Player player;

    void Update()
    {
        if (player != null)
        {
            healthSlider.value = player.CurrentHealth / player.MaxHealth;
        }
    }
}
