using System;
using UnityEngine;

/// <summary>
/// Attack Theif Behavior factory
/// Troll will attack the theif if they are on the same tile
/// </summary>
public class AttackTheifBehavior : ABehaviourFactory
{
    private Action _attackTheif;
    public AttackTheifBehavior(DungeonGrid dungeonGrid, Transform transform, Action attackTheif) : base(dungeonGrid, transform)
    {
        _attackTheif = attackTheif;
    }

    protected override float Delay => TrollSettings.TrollSpeed;

    public override bool Condition()
    {
        // attack if theif is on our tile
        var trollTile = GetCurrentTile();
        var theifTile = _dungeonGrid.FirstOrDefault(tile => tile.GetGlobalPosition() == _dungeonGrid.Theif.transform.position);
        return trollTile == theifTile;
    }

    public override void PerformAction()
    {
        _attackTheif();
    }
}