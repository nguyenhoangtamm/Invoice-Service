namespace Invoice.Domain.DTOs.Responses;

using AutoMapper;
using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;
using AutoMapperProfile = AutoMapper.Profile;

public class VerifyInvoiceResponse : IMapFrom<Invoice>
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public InvoiceResponse OffChainInvoice { get; set; }
    public InvoiceResponse OnChainInvoice { get; set; }

    public void Mapping(AutoMapperProfile profile)
    {
        profile.CreateMap<Invoice, VerifyInvoiceResponse>()
            .ForMember(dest => dest.IsValid, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore())
            .ForMember(dest => dest.OffChainInvoice, opt => opt.Ignore())
            .ForMember(dest => dest.OnChainInvoice, opt => opt.MapFrom(src => src));
    }
}