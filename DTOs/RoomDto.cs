using System.Text.Json.Serialization;

namespace CareSchedule.DTOs
{
    public sealed class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = "";
        public string RoomType { get; set; } = "";
        public int SiteId { get; set; }
        public string? AttributesJson { get; set; }
        public string Status { get; set; } = "Active";
    }

    public sealed class RoomCreateDto
    {
        public string RoomName { get; set; } = "";
        public int SiteId { get; set; }
        public string RoomType { get; set; } = "";
        public string? AttributesJson { get; set; }
    }

    public sealed class RoomUpdateDto
    {
        public string? RoomName { get; set; }
        public string? RoomType { get; set; }
        public int? SiteId { get; set; }
        public string? AttributesJson { get; set; }
    }

    public sealed class RoomSearchQuery
    {
        public string? RoomName { get; set; }
        public string? RoomType { get; set; }
        public string? Status { get; set; } // Active/Inactive
        public int? SiteId { get; set; }
        public int Page { get; set; } = 1;       // optional
        public int PageSize { get; set; } = 25;  // optional
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
    }
}