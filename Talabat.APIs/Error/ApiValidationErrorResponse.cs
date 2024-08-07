namespace Talabat.APIs.Error
{
    public class ApiValidationErrorResponse:ApiErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse():base(404)
        {
            Errors = new List<string>(); 
        }
    }
}
