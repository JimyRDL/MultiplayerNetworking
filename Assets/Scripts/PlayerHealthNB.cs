public class PlayerHealthNB : Health
{
    protected override void Die()
    {
        Destroy(this.gameObject);
    }
}