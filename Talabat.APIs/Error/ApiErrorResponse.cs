namespace Talabat.APIs.Error
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiErrorResponse(int statusCode , string? message=null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string? GetDefaultMessageForStatusCode( int statusCode)
        {
            return statusCode switch
            {
                400 => "A Bad Request , You Have Made",
                401 => "Authorized , You Are Not",
                404 => "Recourses Not Found",
                505 => "There is Server Error",
                _ => null

            };
        }
    }
}
