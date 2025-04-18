using UnityEngine;

namespace Define
{
    public enum PlayerState
    {
        Walk,
        Run,
        Interaction
    }

    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching,
        Stunning,
        Checking
    }

    public enum Target
    {
        None,
        Patient,
        Object,
        Enemy,
    }

    public enum GunType
    {
        BlueGun,
        RedGun,
        Can,
        SmokeBomb
    }
}
