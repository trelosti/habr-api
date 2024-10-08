namespace Habr.Common.DTO.Result
{
    public class GenericResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
