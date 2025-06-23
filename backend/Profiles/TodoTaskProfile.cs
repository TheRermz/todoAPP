using AutoMapper;
using todoApp.Models;
using todoApp.Dtos.TodoTask;

public class TodoTaskProfile : Profile
{
    public TodoTaskProfile()
    {
        CreateMap<TodoTask, TodoTaskCreateDto>();
        CreateMap<TodoTask, TodoTaskReadDto>();
        CreateMap<TodoTask, TodoTaskUpdateDto>();
    }
}
