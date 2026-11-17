using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    EnemyController[] enemies;
    public UIHandler uiHandler;
    private bool gameEnding;
    private int enemiesFixed;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(
            FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
            enemy.OnFixed += HandleEnemyFixed;

        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }

    private void HandleEnemyFixed()
    {
        enemiesFixed++;
        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }

    void Update()
    {
        if (gameEnding)
        {
            return;
        }

        if (player.health <= 0)
        {
            gameEnding = true;
            uiHandler.DisplayLoseScreen();
        }
        else if (AllEnemiesFixed())
        {
            gameEnding = true;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.questComplete,
                    AudioManager.Instance.questCompleteVolume);
            }
            uiHandler.DisplayWinScreen();
        }
    }

    bool AllEnemiesFixed()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isBroken) return false;
        }
        return true;
    }
}
