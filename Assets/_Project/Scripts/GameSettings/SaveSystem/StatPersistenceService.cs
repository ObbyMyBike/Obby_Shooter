using UnityEngine;

public class StatPersistenceService : IStatPersistenceService
{
    private const string KEY_SPEED = "STAT_SPEED";
    private const string KEY_HEALTH = "STAT_HEALTH";
    private const string KEY_DAMAGE = "STAT_DAMAGE";

    public float LoadSpeed() => PlayerPrefs.GetFloat(KEY_SPEED, float.NaN);
    
    public float LoadHealth() => PlayerPrefs.GetFloat(KEY_HEALTH, float.NaN);
    
    public float LoadDamage() => PlayerPrefs.GetFloat(KEY_DAMAGE, float.NaN);

    public void SaveSpeed(float value)
    {
        PlayerPrefs.SetFloat(KEY_SPEED, value);
        PlayerPrefs.Save();
    }

    public void SaveHealth(float value)
    {
        PlayerPrefs.SetFloat(KEY_HEALTH, value);
        PlayerPrefs.Save();
    }

    public void SaveDamage(float value)
    {
        PlayerPrefs.SetFloat(KEY_DAMAGE, value);
        PlayerPrefs.Save();
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_SPEED);
        PlayerPrefs.DeleteKey(KEY_HEALTH);
        PlayerPrefs.DeleteKey(KEY_DAMAGE);
        PlayerPrefs.Save();
    }
}