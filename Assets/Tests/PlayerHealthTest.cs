using NUnit.Framework;

[TestFixture]
public class PlayerHealthTests
{
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        //var health = new PlayerHealthBar();
        var health = new Health();
        health.TakeDamage(30);
        Assert.AreEqual(70, health.currentHealth);
    }

    [Test]
    public void TakeDamage_DoesNotGoBelowZero()
    {
        var health = new Health();
        health.TakeDamage(150);
        Assert.AreEqual(0, health.currentHealth);
        Assert.IsTrue(health.isDead);
    }
}
