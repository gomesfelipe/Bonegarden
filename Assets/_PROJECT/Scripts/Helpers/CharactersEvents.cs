using System;
using UnityEngine;
using UnityEngine.Events;
public class CharacterEvents
{
    //Character damaged and damage value
    public static UnityEvent<GameObject, float> CharacterDamaged;
    //Character healed and amount healed
    public static UnityEvent<GameObject, float> CharacterHealed;

    public static UnityEvent<GameObject> CharacteDeaded;
}
