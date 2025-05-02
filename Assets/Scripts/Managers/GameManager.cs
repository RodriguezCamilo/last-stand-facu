using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Vector3 playerSpawnPosition = new Vector3(0f, 1f, 0f);


    [Header("Gameplay Settings")]
    public int totalLives = 3;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayerDied()
    {
        totalLives--;

        UIManager.Instance.UpdateLives(totalLives);

        if (totalLives > 0)
        {
            RespawnPlayer();
        }
        else
        {
            GameOver();
        }
    }

    private void RespawnPlayer()
    {
        Debug.Log("Respawn player");
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            StartCoroutine(RespawnRoutine(player));
        }
    }
    
    private IEnumerator RespawnRoutine(Player player)
    {
        yield return new WaitForSeconds(2f);

        player.transform.position = playerSpawnPosition;
        player.gameObject.SetActive(true);
        player.ResetPlayer();
    }


    private void GameOver()
    {
        Debug.Log("Game Over");

        GameOverScreen screen = FindObjectOfType<GameOverScreen>();
        if (screen != null)
        {
            screen.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("No se encontró GameOverScreen en la escena.");
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
