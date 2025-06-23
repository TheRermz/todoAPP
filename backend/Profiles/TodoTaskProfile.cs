using AutoMapper;
using todoApp.Models;
using todoApp.Dtos.TodoTask;

public class TodoTaskProfile : Profile
{
    public TodoTaskProfile()
    {
        CreateMap<TodoTaskCreateDto, TodoTask>();
        CreateMap<TodoTask, TodoTaskReadDto>();
        CreateMap<TodoTaskUpdateDto, TodoTask>();
    }
}
