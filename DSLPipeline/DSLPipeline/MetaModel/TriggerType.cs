namespace DSLPipeline.MetaModel
{
    /**
     * TriggerType:
     * Represents range of valid types of Triggers that specifies on
     * when a pipeline should be executed.
     *
     *
     * This class is an alternative to enums.
     * Source: https://ardalis.com/enum-alternatives-in-c
     */
    public class TriggerType
    {
        public static TriggerType Push { get; } = new TriggerType(0, "push");
        public static TriggerType PullRequest { get; } = new TriggerType(1, "pull_request");

        public int Value { get; private set; }
        public string Name { get; private set; }

        public TriggerType(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}