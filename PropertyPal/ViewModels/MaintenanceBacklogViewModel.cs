namespace PropertyPal.Mvc.ViewModels
{
    public class MaintenanceBacklogViewModel
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string IssueTitle { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
    }
}