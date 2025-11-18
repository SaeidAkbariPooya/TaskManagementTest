using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Core.Entities;
using TaskManagement.UI.Models.Common;
using TaskManagement.UI.Models.TaskItem;
using TaskManagement.UI.Services.Common;
using static System.Net.WebRequestMethods;

namespace TaskManagement.UI.Services.TaskItem
{
    public class TaskService : ITaskService
    {
        private readonly ApiClient _apiClient;
        private readonly HttpClient _httpClient;

        public TaskService(ApiClient apiClient, HttpClient httpClient)
        {
            _apiClient = apiClient;
            _httpClient = httpClient;
        }

        public class ApiResponse
        {
            public IEnumerable<TaskDto> Data { get; set; }
        }
        public async Task<ApiResponse<IEnumerable<TaskDto>>> GetAllAsync()
        {
            var client = await _apiClient.GetAuthenticatedClientAsync();
            var result = await client.GetFromJsonAsync<ApiResponse<IEnumerable<TaskDto>>>($"api/Task/GetAll");
            return result;
            //var client = await _apiClient.GetAuthenticatedClientAsync();
            //var response = await client.GetAsync("api/Task/GetAll");
            //if (response.IsSuccessStatusCode)
            //{
            //    var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<TaskDto>>();
            //}
            //var result = await _httpClient.GetFromJsonAsync<IEnumerable<TaskDto>>($"api/Task/GetAll");
            //return result;
            //try
            //{
            //    var client = await _apiClient.GetAuthenticatedClientAsync();
            //    var response = await client.GetAsync("api/Task/GetAll");

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var options = new JsonSerializerOptions
            //        {
            //            PropertyNameCaseInsensitive = true
            //        };
            //        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>(options);
            //        //return new ApiResponse<List<TaskDto>>
            //        //{
            //        //    IsSuccess = true,
            //        //    Data = tasks ?? new List<TaskDto>()
            //        //};
            //    }

            //    return new ApiResponse<List<TaskDto>>
            //    {
            //        IsSuccess = false,
            //        Message = "Failed to fetch tasks"
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ApiResponse<List<TaskDto>>
            //    {
            //        IsSuccess = false,
            //        Message = "Error fetching tasks",
            //        Errors = new List<string> { ex.Message }
            //    };
            //}
        }

        public async Task<ApiResponse<TaskDto>> GetByIdAsync(Guid id)
        {
            try
            {
                //var client = await _apiClient.GetAuthenticatedClientAsync();
                //var response = await client.GetAsync($"api/Task/GetById/{id}");

                var client = await _apiClient.GetAuthenticatedClientAsync();
                var response = await client.GetFromJsonAsync<ApiResponse<TaskDto>>($"api/Task/GetById/{id}");

                if(response != null) 
                {
                    return response;
                }
                //if (response.IsSuccessStatusCode)
                //{
                //    var task = await response.Content.ReadFromJsonAsync<TaskDto>();
                //    return new ApiResponse<TaskDto>
                //    {
                //        IsSuccess = true,
                //        Data = task
                //    };
                //}

                return new ApiResponse<TaskDto>
                {
                    IsSuccess = false,
                    Message = "Failed to fetch task"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskDto>
                {
                    IsSuccess = false,
                    Message = "Error fetching task",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<Guid>> CreateAsync(CreateTaskRequest request)
        {
            try
            {
                var client = await _apiClient.GetAuthenticatedClientAsync();
                var response = await client.PostAsJsonAsync("api/Task/Create", request);
                //var response = await _httpClient.PostAsJsonAsync("api/Task/Create", new
                //{
                //    Title = request.Title,
                //    Description = request.Description,
                //    Priority = request.Priority,
                //    DueDate = request.DueDate,
                //    UserId = request.UserId
                //});
                if (response.IsSuccessStatusCode)
                {
                    var taskId = await response.Content.ReadFromJsonAsync<Guid>();
                    return new ApiResponse<Guid>
                    {
                        IsSuccess = true
                        //Data = taskId
                    };
                }

                //var response = await _httpClient.PostAsJsonAsync("api/Task/Create", new
                //{
                //    Title = request.Title,
                //    Description = request.Description,
                //    Priority = request.Priority,
                //    DueDate = request.DueDate,
                //    UserId = request.UserId
                //});

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Guid>
                {
                    IsSuccess = false,
                    Message = "Failed to create task",
                    Errors = new List<string> { error }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Guid>
                {
                    IsSuccess = false,
                    Message = "Error creating task",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(UpdateTaskRequest request)
        {
            try
            {
                var client = await _apiClient.GetAuthenticatedClientAsync();
                var response = await client.PutAsJsonAsync($"api/Task/Update", request);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = true,
                        Data = true,
                    };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update task",
                    Errors = new List<string> { error }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Error updating task",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var client = await _apiClient.GetAuthenticatedClientAsync();
                var response = await client.DeleteAsync($"api/Task/Delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = true,
                        Data = true
                    };
                }

                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to delete task"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Error deleting task",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}