
using AutoMapper;
using DAL.Models;
using DataTransferObjects;

namespace BL.Configuration;

public class AutomapperConfiguration
{
    public static MapperConfiguration GetMapperConfiguration()
    {
        return new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<User, UserDto>()
                .ReverseMap();
            configuration.CreateMap<Company, CompanyDto>().ReverseMap();
            configuration.CreateMap<Document, DocumentDto>().ReverseMap();
            configuration.CreateMap<SignOperation, SignOperationDto>()
                .ForMember(c => c.ConfirmationCodes, options => options.ExplicitExpansion())
                .ForMember(c => c.Document, options => options.ExplicitExpansion());

            configuration.CreateMap<SignOperationDto, SignOperation>();

            configuration.CreateMap<ConfirmationCode, ConfirmationCodeDto>().ReverseMap();
        });
    }
    
}