namespace DSLPipeline.MetaModel.Configuration
{
    /**
     * OperatingSystem:
     * Represents range of valid types of operating systems that specifies on
     * which system a pipeline must be executed.
     *
     *
     * This class is an alternative to enums.
     * Source: https://ardalis.com/enum-alternatives-in-c
     */
    public class OperatingSystem
    {
        public static OperatingSystem UbuntuLatest { get; } = new OperatingSystem(0, "ubuntu-latest");
        public static OperatingSystem Ubuntu1804 { get; } = new OperatingSystem(0, "ubuntu-18.04");
        public static OperatingSystem Ubuntu1604 { get; } = new OperatingSystem(0, "ubuntu-16.04");

        public int Value { get; private set; }
        public string Name { get; private set; }

        public OperatingSystem(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}