namespace Talabat.APIs.Error
{
    public class ApiExceptionResponse:ApiErrorResponse
    {
        public string? Details { get; set; }
        public ApiExceptionResponse(int statusCode ,string? Details=null , string? message=null)
            :base(statusCode , message)
        {
            this.Details = Details;
        }
    }
}
