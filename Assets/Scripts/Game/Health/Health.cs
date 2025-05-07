using UnityEngine;

public class Health : MonoBehaviour { // add this to anything that has health, the bar is separate

    public int maxHealth = 100;
    public int currentHealth = 50;
    public bool isDead;
    public event System.Action OnDeath;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount) {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        //onDamaged?.Invoke();
        Debug.Log(currentHealth);
        if (currentHealth <= 0) {
            //onDied?.Invoke();
            Die(); // me too mate
        }
    }

    public void Heal(int amount) {
        // blood is fuel
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // heal up to full health but dont go over
        //onHealed?.Invoke();
    }

    public void Die() {
        Debug.Log(this.name+ " died!!!");
        isDead = true;
        OnDeath?.Invoke();
        //health.OnDeath += HandleDeath; // to call from another class
        //Destroy(gameObject);
    }

}