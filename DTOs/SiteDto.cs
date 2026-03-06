using System.Text.Json.Serialization;

namespace CareSchedule.DTOs
{
    public class SiteDto
    {
        public int SiteId { get; set; }
        public string Name { get; set; } = default!;
        public string? AddressJson { get; set; }
        public string Timezone { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
    
    public class SiteCreateDto
    {
        public string Name { get; set; } = default!;
        
        [JsonConverter(typeof(StringOrObjectConverter))]
        public string? AddressJson { get; set; }
        
        public string Timezone { get; set; } = "UTC";
    }
    
    public class SiteUpdateDto
    {
        public string? Name { get; set; }
        
        [JsonConverter(typeof(StringOrObjectConverter))]
        public string? AddressJson { get; set; }
        
        public string? Timezone { get; set; }
    }

    public class SiteSearchQuery
    {
        public string? Name { get; set; }
        public string? Status { get; set; } // "Active" | "Inactive" (string-based)

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        public string SortBy { get; set; } = "name"; // name | timezone | status
        public string SortDir { get; set; } = "asc"; // asc | desc
    }
}