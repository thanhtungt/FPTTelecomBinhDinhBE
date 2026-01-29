using AutoMapper;
using FPTTelecomBE.DTOs.Package;
using FPTTelecomBE.DTOs.Post;
using FPTTelecomBE.Models;

namespace FPTTelecomBE.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Package, PackageDto>();
        CreateMap<Post, PostDto>();
    }
}