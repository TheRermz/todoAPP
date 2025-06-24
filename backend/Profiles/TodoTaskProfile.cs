using AutoMapper;
using todoApp.Models;
using todoApp.Dtos.TodoTask;

public class TodoTaskProfile : Profile
{
    public TodoTaskProfile()
    {
        CreateMap<TodoTaskCreateDto, TodoTask>();
        CreateMap<TodoTask, TodoTaskReadDto>()
           .ForMember(dest => dest.TagList,
                        opt => opt.MapFrom(src => src.TaskTags.Select(tt => tt.Tag)));
        CreateMap<TodoTaskUpdateDto, TodoTask>();
    }
}
