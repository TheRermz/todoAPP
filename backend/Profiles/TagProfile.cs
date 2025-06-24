using AutoMapper;
using todoApp.Dtos.Tag;
using todoApp.Models;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Tag, TagReadDto>();
        CreateMap<TagCreateDto, Tag>();
    }
}
