using UnityEngine;
using UnityEngine.Playables;
[System.Serializable]

public class Action
{
    public PlayerState playerState;
    public PlayerType playerType;
    public int id;
    public string name;
    public CardType type;
    public int initialIndex;
    public int endIndex;
    public int initialTime;
    public int windUpTime;
    public int timeOfEffect;
    public int returnTime;
    public string description;
    public GameObject icon;

    protected Player player;

    //public Action(PlayerState _playerState, string _name, int _windUpTime, PlayerType player)
    //{
    //    playerState = _playerState;
    //    name = _name;
    //    windUpTime = _windUpTime;
    //}

    public Action(PlayerType _player)
    {
        playerType = _player;
    }

    public Action(PlayerType _player, PlayerState _playerState, int _windUpTime)
    {
        playerType = _player;
        playerState = _playerState;
        windUpTime = _windUpTime;
    }

    //public Action(int id, PlayerState _playerState, string name, int _windUpTime)
    //{
    //    player = _player;
    //    playerState = _playerState;
    //    windUpTime = _windUpTime;
    //}

    public Action(int _id, PlayerState _playerState, CardType _type, string _name, int _windUpTime, int _returnTime, PlayerType _player)
    {
        id = _id;
        playerState = _playerState;
        type = _type;
        name = _name;
        windUpTime = _windUpTime;
        returnTime = _returnTime;
        playerType = _player;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }
}

public class AttackAction : Action
{
    public int damage;
    public int acurracy;
    public int knockBack;
    public AttackType attack;
    public AttackRange range;
    public Hand hand;

    public AttackAction(int _id, PlayerState _playerState, string _name, AttackType _attack, Hand _hand, AttackRange range, int _windUpTime, int _returnTime, int _damage, int _acurracy, int _knockBack, PlayerType _player)
        : base(_id, _playerState, CardType.Attack, _name, _windUpTime, _returnTime, _player)
    {
        type = CardType.Attack;
        hand = _hand;
        attack = _attack;
        damage = _damage;
        acurracy = _acurracy;
        knockBack = _knockBack;
        playerType = _player;
    }
}

public class DefenceAction : Action
{
    public DefenceType defence;
    public Hand synergisingHand;

    public DefenceAction(int _id, PlayerState _playerState, string _name, DefenceType _defence, int _windUpTime, int _returnTime, PlayerType _player)
        : base(_id, _playerState, CardType.Defence, _name, _windUpTime, _returnTime, _player)
    {
        type = CardType.Defence;
        defence = _defence;
    }
}

public class SpecialAction : Action
{
    public SpecialAction(int _id, string _name, string _description, PlayerType _player)
        : base(_id, PlayerState.Null, CardType.Special, _name, 0, 0, _player)
    {
        id = _id;
        name = _name;
        playerType = _player;
        description = _description;
    }

    public void Effect() {}
}

public enum PlayerType { Player, Opponent }

public enum CardType { Attack, Defence, Special, Breathing }

public enum PlayerState { Null, Stunned, Idle, Breathing, Feint, Block, Slip, Bob, Jab, Cross, Hook, Uppercut, Returning }

public enum AttackType { Feint, Jab, Cross, LeadHook, RearHook, LeadUppercut, RearUppercut }

public enum DefenceType { None, Block, Slip, Bob }

public enum Hand { Lead, Rear }

public enum AttackRange { Outside, Ranged, Mid, Pocket }

public enum Effect { Blocked, Hit, Missed }