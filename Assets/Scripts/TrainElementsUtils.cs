public static class TrainElementsUtils {

    public static TrainElement FindLastTail(TrainElement trainElement) {
        var lastCheckedElement = trainElement;
        
        while (lastCheckedElement.Tail != null) {
            lastCheckedElement = lastCheckedElement.Tail;
        }

        return lastCheckedElement;
    }

}