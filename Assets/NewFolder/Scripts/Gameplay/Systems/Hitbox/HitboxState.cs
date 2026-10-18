public struct HitboxState {
    public float radius;
    public ContactSurface surface;

    public HitboxState(float radius, ContactSurface surface) {
        this.radius = radius;
        this.surface = surface;
    }
}
