namespace Ecommerce_Apis.Utills
{
    public class ResponseDTO
    {
        public string? Message { get; set; }
        public int Status { get; set; } = 200;
        public bool IsSuccess { get; set; } = true;
        public Dictionary<string, string[]>? Errors { get; set; }
        public dynamic? Data { get; set; } = new List<string>();
        public ResponseDTO(string message = "", int status = 200, bool isSuccess = true)
        {
            Message = message;
            Status = status;
            IsSuccess = isSuccess;
            Errors = null;
            Data = new List<string>();
        }
    }
}
