namespace TaxApi
{
    public class CheckResult
    {
        public List<ErrorResult> error { get; set; }
        public List<ErrorResult> warning { get; set; }
        public bool success { get; set; }

    }

    public class ErrorResult
    {
        public string code { get; set; }
        public string message { get; set; }

    }
}
