using UnityEngine;
[System.Serializable]

public class Action
{
    public PlayerState playerState;
    public PlayerType player;
    public int id;
    public string name;
    public CardType type;
    public int initialTime;
    public int windUpTime;
    public int timeOfEffect;
    public int returnTime;
    public GameObject icon;

    //public Action(PlayerState _playerState, string _name, int _windUpTime, PlayerType player)
    //{
    //    playerState = _playerState;
    //    name = _name;
    //    windUpTime = _windUpTime;
    //}

    public Action(PlayerType _player)
    {
        player = _player;
    }

    public Action(PlayerType _player, PlayerState _playerState, int _windUpTime)
    {
        player = _player;
        playerState = _playerState;
        windUpTime = _windUpTime;
    }

    public Action(int _id, PlayerState _playerState, string _name, int _windUpTime, int _returnTime, PlayerType _player)
    {
        id = _id;
        playerState = _playerState;
        name = _name;
        windUpTime = _windUpTime;
        returnTime = _returnTime;
        player = _player;
    }
}

public class AttackAction : Action
{
    public int damage;
    public int knockBack;
    public AttackType attack;

    public AttackAction(int _id, PlayerState _playerState, string _name, AttackType _attack, int _windUpTime, int _returnTime, int _damage, int _knockBack, PlayerType _player)
        : base(_id, _playerState, _name, _windUpTime, _returnTime, _player)
    {
        type = CardType.Attack;
        attack = _attack;
        damage = _damage;
        knockBack = _knockBack;
        player = _player;
    }
}

public class DefenceAction : Action
{
    public DefenceType defence;

    public DefenceAction(int _id, PlayerState _playerState, string _name, DefenceType _defence, int _windUpTime, int _returnTime, PlayerType _player)
        : base(_id, _playerState, _name, _windUpTime, _returnTime, _player)
    {
        type = CardType.Defence;
        defence = _defence;
    }
}

public enum PlayerType { Player, Opponent }

public enum CardType { Attack, Defence, Special }

public enum PlayerState { Null, Stunned, Idle, Block, Slip, Bob, Jab, Cross, Hook, Uppercut }

public enum AttackType { Jab, Cross, LeadHook, RearHook, LeadUppercut, RearUppercut }

public enum DefenceType { None, Block, Slip, Bob }

public enum Effect { Blocked, Hit, Missed }