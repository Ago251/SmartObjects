using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum StatType
{
    Fuel,
    Energy,
    Hunger,
    Health,
    Stress
}

[Serializable]
public class Stat
{
    [SerializeField]
    private StatType _type;
    public StatType Type
    {
        get { return _type; }
    }

    [SerializeField]
    private int _currentLevel;
    public int CurrentLevel
    {
        get { return _currentLevel; }
    }

    [SerializeField]
    private int _maxLevel;

    [SerializeField]
    private int _decay = 1;

    public int Decay
    {
        get { return _decay; }
    }

    public void Increase(int amount)
    {
        _currentLevel = math.clamp(_currentLevel + amount, 0, _maxLevel);
    }

    public void Decrease(int amount)
    {
        _currentLevel = math.clamp(_currentLevel - amount, 0, _maxLevel);
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public void SetCurrentLevel(int amount)
    {
        _currentLevel = amount;
    }

    public int GetMaxLevel()
    {
        return _maxLevel;
    }
}
