class Weapon
{
    private readonly int _damage;
    private int _bulletsCount;

    public Weapon(int damage, int bulletsCount)
    {
        if (damage <= 0)
            throw new ArgumentOutOfRangeException(nameof(damage));

        if (bulletsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(bulletsCount));

        _damage = damage;
        _bulletsCount = bulletsCount;
    }

    private bool CanFire => _bulletsCount >= 0;

    public void Fire(Player player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        if (CanFire)
        {
            player.TakeDamage(_damage);
            _bulletsCount--;
        }
    }
}

class Player
{
    private int _health;

    public Player(int health)
    {
        if (_health <= 0)
            throw new ArgumentOutOfRangeException(nameof(health));

        _health = health;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            throw new ArgumentOutOfRangeException(nameof(damage));

        _health -= damage;
    }
}

class Bot
{
    private readonly Weapon _weapon;

    public Bot(Weapon weapon) =>
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));

    public void OnSeePlayer(Player player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        _weapon.Fire(player);
    }
}