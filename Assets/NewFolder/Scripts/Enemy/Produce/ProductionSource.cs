using System;
using System.Linq;

[Serializable]
public struct ProductionSource {
    
    public ProducerVariantSource[] producerVariantSources;

    public readonly ProductionPrototype Build() {
        return new ProductionPrototype (
            producerVariants: producerVariantSources.Select(source => source.Get()).ToArray()
        );
    }

}