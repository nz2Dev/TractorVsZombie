public class BodyDynamic {

    public BodyDynamic(int id) {
        Id = id;
    }

    public int Id { get; }
    public bool Grounded { get; set; }
    public int PhysicsBodyId { get; set; }

}