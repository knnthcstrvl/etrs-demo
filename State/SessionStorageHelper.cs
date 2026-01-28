using Microsoft.JSInterop;

namespace Financial.State
{
    public class SessionStorageHelper
    {
        private readonly IJSRuntime _jSRuntime;

        public SessionStorageHelper(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task StoreTokenAsync(string token)
        {
            await _jSRuntime.InvokeVoidAsync("sessionStorageHelper.setItem", "authToken", token);
        }

        public async Task<string> RetrieveTokenAsync()
        {
            return await _jSRuntime.InvokeAsync<string>("sessionStorageHelper.getItem", "authToken");
        }

        public async Task RemoveTokenAsync()
        {
            await _jSRuntime.InvokeVoidAsync("sessionStorageHelper.removeItem", "authToken");
        }
    }
}
