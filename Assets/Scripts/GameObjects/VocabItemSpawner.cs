public class VocabItemSpawner : SerializedScriptSpawner<VocabItem> {
    // TODO: make list of transform positions that each item can spawn on 
    public override VocabItem Spawn(SerializedScript<VocabItem> serializedScript) {
        return base.Spawn(serializedScript);
    }
}
