namespace HiredIn.Backend.Contracts.DTOs.AdminDTOs
{
    public class AdminStatisticsDTO
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int BlockedUsers { get; set; }
        public int DeletedUsers { get; set; }

        public int TotalCompanies { get; set; }
        public int PendingCompanies { get; set; }
        public int ActiveCompanies { get; set; }
        public int BlockedCompanies { get; set; }
        public int ArchivedCompanies { get; set; }

        public int TotalVacancies { get; set; }
        public int DraftVacancies { get; set; }
        public int PublishedVacancies { get; set; }
        public int ArchivedVacancies { get; set; }
        public int ClosedVacancies { get; set; }

        public int TotalApplications { get; set; }
        public int TotalCompanyRatings { get; set; }
    }
}
