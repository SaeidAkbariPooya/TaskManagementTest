using AutoMapper;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Features.Auth.Commands.Register;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Core.Entities;

namespace TaskManagement.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<TaskItem, TaskResponse>().ReverseMap();
            CreateMap<CreateTaskCommand, TaskItem>();
            CreateMap<TaskItem, TaskResponse>();
            CreateMap<UpdateTaskCommand, TaskItem>();
            CreateMap<RegisterCommand, User>();
        }
    }
}
