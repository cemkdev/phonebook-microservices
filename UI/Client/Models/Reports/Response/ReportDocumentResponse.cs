using Shared.Contracts.Enums;

namespace Client.Models.Reports.Response
{
    public class ReportDocumentResponse
    {
        public Guid Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public ReportStatus Status { get; set; }
        public List<ReportItemResponse> Items { get; set; }
        public string Error { get; set; }
    }
}
