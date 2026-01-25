using AutoMapper;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;

namespace CarServiceBG.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Car, CarDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Chat, ChatDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Comment, CommentResponseDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Shop, ShopDto>().ReverseMap();
            CreateMap<Worker, WorkerDto>().ReverseMap();
            CreateMap<AuctionProduct, AuctionProductResponseDto>()
     .ForMember(d => d.StartTime, m => m.MapFrom(s => s.StartTime))
     .ForMember(d => d.EndTime, m => m.MapFrom(s => s.EndTime));

        }
    }
}
