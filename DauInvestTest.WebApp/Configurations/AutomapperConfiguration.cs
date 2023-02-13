using DAL.Models;
using DataTransferObjects;
using DauInvestTest.WebApp.Models.RequestModels;

namespace DauInvestTest.WebApp.Configurations;

public static class AutomapperConfiguration
{
    public static IServiceCollection ConfigureAutomapper(this IServiceCollection services)
    {
        return services.AddAutoMapper(configuration =>
        {
            // configuration.CreateMap<User, UserDto>()
            //     .ReverseMap();
            // configuration.CreateMap<Company, CompanyDto>().ReverseMap();
            // configuration.CreateMap<Document, DocumentDto>().ReverseMap();
            // configuration.CreateMap<SignOperation, SignOperationDto>()
            //     .ForMember(c => c.ConfirmationCodes, options => options.ExplicitExpansion())
            //     .ForMember(c => c.Document, options => options.ExplicitExpansion());
            //
            // configuration.CreateMap<SignOperationDto, SignOperation>();
            //
            // configuration.CreateMap<ConfirmationCode, ConfirmationCodeDto>().ReverseMap();
            //
            configuration.CreateMap<DocumentSignRequest, SignOperationDto>();
        });
    }
}