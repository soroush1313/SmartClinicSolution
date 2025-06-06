using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsHandler
    {
        private readonly IAppointmentRepository _repository;
        private readonly ICacheService _cache;

        public GetAvailableSlotsHandler(IAppointmentRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<TimeSlotDto>> Handle(GetAvailableSlotsQuery query)
        {
            string cacheKey = $"doctor:{query.DoctorId}:slots:{query.Date:yyyyMMdd}";

            // Try to get from Redis
            var cached = await _cache.GetAsync<List<TimeSlotDto>>(cacheKey);
            if (cached is not null)
                return cached;

            // Define working hours (08:00 - 16:00)
            var startOfDay = query.Date.Date.AddHours(8);
            var endOfDay = query.Date.Date.AddHours(16);
            var slotDuration = TimeSpan.FromMinutes(30);

            // Get all appointments for doctor in that date
            var appointments = await _repository.GetAppointmentsByDoctorAndDate(query.DoctorId, query.Date);

            var availableSlots = new List<TimeSlotDto>();

            for (var slotStart = startOfDay; slotStart < endOfDay; slotStart += slotDuration)
            {
                var slotEnd = slotStart + slotDuration;

                bool isTaken = appointments.Any(a =>
                    a.StartTime < slotEnd &&
                    a.EndTime > slotStart &&
                    a.Status != "Cancelled");

                if (!isTaken)
                {
                    availableSlots.Add(new TimeSlotDto
                    {
                        Start = slotStart,
                        End = slotEnd
                    });
                }
            }

            // Cache the result in Redis
            await _cache.SetAsync(cacheKey, availableSlots, TimeSpan.FromMinutes(10));

            return availableSlots;
        }
    }
}
