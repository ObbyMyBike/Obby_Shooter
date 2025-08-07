public interface IStatPersistenceService
{
    public float LoadSpeed();
    
    public float LoadHealth();
    
    public float LoadDamage();
    
    public void SaveSpeed(float value);
    
    public void SaveHealth(float value);
    
    public void SaveDamage(float value);
    
    public void ResetAll();
}