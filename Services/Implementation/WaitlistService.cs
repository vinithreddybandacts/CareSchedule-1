using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class WaitlistService : IWaitlistService
    {
        private readonly IWaitlistRepository _waitlistRepo;
        private readonly IBookingService _bookingService;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public WaitlistService(
            IWaitlistRepository waitlistRepo,
            IBookingService bookingService,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _waitlistRepo = waitlistRepo;
            _bookingService = bookingService;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        public WaitlistResponseDto Add(CreateWaitlistRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public void Remove(int waitId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WaitlistResponseDto> Search(WaitlistSearchDto dto)
        {
            throw new NotImplementedException();
        }

        public WaitlistResponseDto Fill(int waitId, FillWaitlistRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
