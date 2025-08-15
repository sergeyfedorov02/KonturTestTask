namespace KonturTestTask.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string errorMessage, Exception ex = null) : base(errorMessage, ex) { }
    }
}
