namespace WebsiteAIAssistant
{
    public class SdcaMaximumEntropyOptions
    {
        public int MaximumNumberOfIterations { get; set; } = 500; // More passes for better convergence
        public float ConvergenceTolerance { get; set; } = 1e-5f; // Stricter tolerance
        public float L1Regularization { get; set; } = 1e-4f; // Small L1 penalty
        public float L2Regularization { get; set; } = 1e-4f; // Small L2 penalty
        public bool Shuffle { get; set; } = true;

    }
}
