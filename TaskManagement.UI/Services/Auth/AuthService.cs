using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.UI.Auth;
using TaskManagement.UI.Models.Auth;
using TaskManagement.UI.Models.Common;
using TaskManagement.UI.Services.Auth;

namespace TaskManagement.Blazor.Services.Auth
{
    public class AuthService : IAuthService
    {

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly AuthStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager, AuthStateProvider authStateProvider = null)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {

                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new
                {
                    Email = request.Email,
                    Password = request.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse != null)
                    {
                        //await _localStorage.SetItemAsync("authToken", authResponse.Token);
                        //((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(authResponse.Token);
                        //return authResponse;
                        await StoreAuthDataAsync(authResponse);
                        return new ApiResponse<AuthResponse>
                        {
                            IsSuccess = true,
                            Data = authResponse,
                            Message = "Login successful"
                        };
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ApiResponse<AuthResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password"
                    };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<AuthResponse>
                {
                    IsSuccess = false,
                    Message = "Login failed",
                    Errors = new List<string> { error }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
                {
                    IsSuccess = false,
                    Message = "Login failed",
                    Errors = new List<string> { ex.Message
    }
                };
            }
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            //try
            //{
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", new
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            });
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (authResponse != null)
                {
                    //await StoreAuthDataAsync(authResponse);
                    return new ApiResponse<AuthResponse>
                    {
                        IsSuccess = true,
                        Data = authResponse,
                        Message = "Registration successful"
                    };
                }
            }

            var error = await response.Content.ReadAsStringAsync();
            return new ApiResponse<AuthResponse>
            {
                IsSuccess = false,
                Message = "Registration failed",
                Errors = new List<string> { error }
            };
            //}
            //catch (Exception ex)
            //{
            //    return new ApiResponse<AuthResponse>
            //    {
            //        IsSuccess = false,
            //        Message = "Registration failed",
            //        Errors = new List<string> { ex.Message }
            //    };
            //}
        }

        public async Task LogoutAsync()
        {
            await ClearAuthDataAsync();

            //await _localStorage.RemoveItemAsync("authToken");
            //((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            //_navigationManager.NavigateTo("/login", true);
            //await ClearAuthDataAsync();
            //_navigationManager.NavigateTo("/login", true);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        public async Task<string> GetFullNameAsync()
        {
            var FullName = await _localStorage.GetItemAsync<string>("userFullName");
            return FullName;
        }

        public async Task<Guid?> GetUserIdAsync()
        {
            return await _localStorage.GetItemAsync<Guid?>("userId");
        }

        private async Task StoreAuthDataAsync(AuthResponse authResponse)
        {
            await _localStorage.SetItemAsync("authToken", authResponse.Token);
            await _localStorage.SetItemAsync("userId", authResponse.UserId);
            await _localStorage.SetItemAsync("userEmail", authResponse.Email);
            await _localStorage.SetItemAsync("userFullName", authResponse.FullName);

            await GetFullNameAsync();
        }

        private async Task ClearAuthDataAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userId");
            await _localStorage.RemoveItemAsync("userEmail");
            await _localStorage.RemoveItemAsync("userFirstName");
            await _localStorage.RemoveItemAsync("userLastName");
            await _localStorage.RemoveItemAsync("userFullName");
        }
    }
}