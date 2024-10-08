using AutoMapper;
using Habr.Common.DTO.Comments;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentGetDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.ParentCommentId, opt => opt.MapFrom(src => src.ParentCommentId ?? 0))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
