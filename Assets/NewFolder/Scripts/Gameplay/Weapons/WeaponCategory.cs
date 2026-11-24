using System;

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponCategory", menuName = "WeaponCategory", order = 0)]
public class WeaponCategory : ScriptableObject, IEquatable<WeaponCategory> {
    
    public Sprite weaponImage;
    public Sprite bulletImage;

    public bool Equals(WeaponCategory other) {
        return other.weaponImage == weaponImage
            && other.bulletImage == bulletImage;
    }
}