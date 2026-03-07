namespace WebsiteAIAssistant
{
    public class SdcaMaximumEntropyOptions
    {
        public float BiasLearningRate { get; set; } = 0.0f; // Learning rate for accuracy
        public int? ConvergenceCheckFrequency { get; set; } = 10; // Check for convergence every 10 iterations
        public int MaximumNumberOfIterations { get; set; } = 500; // More passes for better convergence
        public float ConvergenceTolerance { get; set; } = 1e-5f; // Stricter tolerance
        public float L1Regularization { get; set; } = 1e-4f; // Small L1 penalty
        public float L2Regularization { get; set; } = 1e-4f; // Small L2 penalty
        public bool Shuffle { get; set; } = true;

    }
}
