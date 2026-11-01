public struct TruckState {
    
    public FactionId factionId;
    public FactionState factionState;
    public AttachmentState attachmentState;

    public TruckState(AttachmentState attachmentState, FactionState factionState, FactionId factionId) {
        this.attachmentState = attachmentState;
        this.factionState = factionState;
        this.factionId = factionId;
    }
}