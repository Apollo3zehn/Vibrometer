namespace Vibrometer.BaseTypes
{
    public static class SystemParameters
    {
        public const int CLOCK_RATE = 125000000;
        public const int BUFFER_COUNT = 4;
        public const int BYTE_COUNT = 4;
        public const int FFT_LENGTH = 256;

        public const int DATA_BASE = 0x1E00_0000;
        public const int DATA_SIZE = 0x0100_0000;

        public const int SWITCH_BASE = 0x43C0_0000;
        public const int SWITCH_SIZE = 0x0001_0000;

        public const int GPIO_BASE = 0x4120_0000;
        public const int GPIO_REG_COUNT = 6;
        public const int GPIO_REG_SIZE = 0x0001_0000;
        public const int GPIO_SIZE = GPIO_REG_COUNT * GPIO_REG_SIZE;

        public const int GPIO_SIGNAL_GENERATOR = 0x0000_0000;
        public const int GPIO_DATA_ACQUISITION = 0x0001_0000;
        public const int GPIO_POSITION_TRACKER = 0x0002_0000;
        public const int GPIO_FILTER = 0x0003_0000;
        public const int GPIO_FOURIER_TRANSFORM = 0x0004_0000;
        public const int GPIO_RAM_WRITER = 0x0005_0000;
    }
}
