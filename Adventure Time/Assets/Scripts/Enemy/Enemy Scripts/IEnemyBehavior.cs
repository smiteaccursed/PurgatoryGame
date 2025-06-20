using System;
public interface IEnemyBehavior
{
    void Execute(EnemyAI enemy);
    string GetName();
    void OnDeath(EnemyAI enemy);
    void OnDamage(EnemyAI enemy);
    void OnHurt(EnemyAI enemy);
    void OnHit(EnemyAI enemy);

    Action<EnemyAI, bool> OnNightChange { get; }
}
