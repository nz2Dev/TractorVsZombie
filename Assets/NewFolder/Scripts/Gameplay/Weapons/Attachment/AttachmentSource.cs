using System;
using UnityEngine;

[Serializable]
public struct AttachmentSource {
    
    [NonNull, Local, SerializeField] private WeaponSource weaponSource;

    public AttachmentPrototype Get() {
        var weaponPrototype = weaponSource.GetPrototype();
        return new AttachmentPrototype {
            weaponPrototype = weaponPrototype,
            weaponPlacementOffset = weaponSource.transform.localPosition,
        };
    }
}
