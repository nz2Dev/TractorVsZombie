using System;

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponCategory", menuName = "WeaponCategory", order = 0)]
public class WeaponCategory : ScriptableObject, IEquatable<WeaponCategory> {
    public Sprite categoryImage;

    public bool Equals(WeaponCategory other) {
        return other.categoryImage == categoryImage;
    }
}