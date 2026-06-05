using AutoMapper;
using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Events.DTOs;
using Phantoms.Application.Products.DTOs;
using Phantoms.Domain.Entities;

namespace Phantoms.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO (read)
        CreateMap<Product, ProductDto>();
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                src.Teacher == null ? string.Empty : $"{src.Teacher.FirstName} {src.Teacher.LastName}"));
        CreateMap<Announcement, AnnouncementDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                src.Teacher == null ? string.Empty : $"{src.Teacher.FirstName} {src.Teacher.LastName}"));

        // DTO → Entity (write)
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

        CreateMap<UpdateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

        CreateMap<CreateAnnouncementDto, Announcement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());

        CreateMap<UpdateAnnouncementDto, Announcement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore());
    }
}
