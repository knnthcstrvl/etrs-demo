using System;

namespace Financial.Models
{
    public enum ToastType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public class ToastMessage
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public ToastType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Duration in milliseconds (0 = sticky)
        /// </summary>
        public int Duration { get; set; } = 20000;
    }
}
