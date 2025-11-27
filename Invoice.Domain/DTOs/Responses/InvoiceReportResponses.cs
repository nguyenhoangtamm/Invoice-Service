using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Enums;

namespace Invoice.Domain.DTOs.Responses;

public record InvoiceReportResponse : IMapFrom<Invoice.Domain.Entities.InvoiceReport>
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ReportedByUserId { get; set; }
    public InvoiceReportReason Reason { get; set; }
    public string? Description { get; set; }
    public InvoiceReportStatus Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public record InvoiceReportDetailResponse : IMapFrom<Invoice.Domain.Entities.InvoiceReport>
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ReportedByUserId { get; set; }
    public string? ReportedByUserName { get; set; }
    public InvoiceReportReason Reason { get; set; }
    public string? Description { get; set; }
    public InvoiceReportStatus Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    
    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Invoice.Domain.Entities.InvoiceReport, InvoiceReportDetailResponse>()
            .ForMember(dest => dest.ReportedByUserName,
                opt => opt.MapFrom(src => src.ReportedByUser.FullName ?? src.ReportedByUser.UserName));
    }
}
