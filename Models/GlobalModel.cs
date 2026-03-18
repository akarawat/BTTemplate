namespace BTTemplate.Models
{
    // ── Email payload — for sending via internal EmailSender API ───────
    public class SenderMailModel
    {
        public string Addresses { get; set; } = "";  // semicolon-separated
        public string Form      { get; set; } = "";  // sender address
        public string Subject   { get; set; } = "";
        public string Body      { get; set; } = "";  // HTML supported
        public int    Priority  { get; set; } = 1;
    }

    // ── Generic API response wrapper ───────────────────────────────────
    public class ApiResponse<T>
    {
        public bool   Success { get; set; }
        public string Message { get; set; } = "";
        public T?     Data    { get; set; }

        public static ApiResponse<T> Ok(T data, string msg = "")
            => new() { Success = true, Message = msg, Data = data };

        public static ApiResponse<T> Fail(string msg)
            => new() { Success = false, Message = msg };
    }
}
