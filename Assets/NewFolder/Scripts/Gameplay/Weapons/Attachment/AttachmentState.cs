public struct AttachmentState {
    public int weaponId;
    public WeaponState weaponState;

    public AttachmentState(int weaponId, WeaponState weaponState) {
        this.weaponId = weaponId;
        this.weaponState = weaponState;
    }
}
