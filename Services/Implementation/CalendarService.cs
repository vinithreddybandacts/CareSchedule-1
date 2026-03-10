using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarEventRepository _calendarRepo;

        public CalendarService(ICalendarEventRepository calendarRepo)
        {
            _calendarRepo = calendarRepo;
        }

        public IEnumerable<CalendarEventResponseDto> GetByProvider(int providerId, string date)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CalendarEventResponseDto> GetBySite(int siteId, string date)
        {
            throw new NotImplementedException();
        }
    }
}
