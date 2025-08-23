using Exiled.API.Features;
using System;
using System.Net.Http;
using System.Text;

namespace RpCommands.Services
{
    public static class WebhookService
    {
        private static readonly HttpClient HttpClient = new();

        public static async void SendWebhookAsync(string url, string message)
        {
            if (string.IsNullOrEmpty(url) || url.Contains("your_webhook_url"))
            {
                Log.Warn("Webhook URL is not configured. Skipping webhook message.");
                return;
            }

            try
            {
                string jsonPayload = $"{{\"content\": \"{message.Replace("\"", "\\\"")}\"}}";
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await HttpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Failed to send webhook. Status: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An exception occurred while sending the webhook: {ex}");
            }
        }
    }
}