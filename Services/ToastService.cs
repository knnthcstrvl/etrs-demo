using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financial.Models;
using Microsoft.Extensions.Logging;

namespace Financial.Services
{
    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;
        event Action<string>? OnHide;

        void Show(ToastMessage message);
        void Hide(string id);
        IEnumerable<ToastMessage> GetAll();
    }

    public class ToastService : IToastService
    {
        private readonly ILogger<ToastService> _logger;
        public ToastService(ILogger<ToastService> logger)
        {
            _logger = logger;
        }

        // instance events (for same DI instance)
        public event Action<ToastMessage>? OnShow;
        public event Action<string>? OnHide;
        private readonly List<ToastMessage> _messages = new();
        private readonly object _lock = new();

        public void Show(ToastMessage message)
        {
            // enforce global default duration of 15s for non-sticky toasts
            if (message.Duration != 0)
            {
                message.Duration = 15000;
            }
            lock (_lock)
            {
                // prevent duplicate messages with same type/title/text already queued
                var exists = _messages.Exists(m => m.Type == message.Type && m.Title == message.Title && m.Message == message.Message);
                if (exists) return;
                _messages.Add(message);
            }
            _logger.LogDebug("ToastService.Show: {Title} - {Message}", message.Title, message.Message);
            try { Console.WriteLine($"ToastService.Show: {message.Title} - {message.Message}"); } catch { }
            OnShow?.Invoke(message);
        }

        public void Hide(string id)
        {
            lock (_lock)
            {
                var existing = _messages.Find(m => m.Id == id);
                if (existing != null) _messages.Remove(existing);
            }
            _logger.LogDebug("ToastService.Hide: {Id}", id);
            OnHide?.Invoke(id);
        }

        public IEnumerable<ToastMessage> GetAll()
        {
            lock (_lock)
            {
                var arr = _messages.ToArray();
                _messages.Clear();
                return arr;
            }
        }
    }
}
