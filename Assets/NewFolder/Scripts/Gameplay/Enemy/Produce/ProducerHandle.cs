public class ProducerHandle {
    
    public readonly IProducer producer;
    public readonly ProducerActivationConfig activationConfig;

    public ProducerHandle(IProducer producer, ProducerActivationConfig activationConfig) {
        this.producer = producer;
        this.activationConfig = activationConfig;
    }


    public bool IsActivated { get; set; }
}