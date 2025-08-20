using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Group_Final_Project.Services
{

	// iterface for schedule service
	public interface IScheduleService
	{
		Task<List<Schedule>> GetSchedulesAsync();
		Task<List<Schedule>> GetSchedulesByDateAsync(DateTime date);
		Task<List<Schedule>> GetSchedulesByEmployeeAsync(int employeeId);
		Task<Schedule?> GetScheduleAsync(int id);
		Task AddScheduleAsync(Schedule schedule);
		Task UpdateScheduleAsync(Schedule schedule);
		Task DeleteScheduleAsync(int id);
		Task<bool> ScheduleExistsAsync(int id);
	}
	public class ScheduleService : IScheduleService
	{
		private List<Schedule> _schedules = new();
		private int _nextId = 1;

		public ScheduleService()
		{
			// Initialize with sample data (REMOVE LATER)
			var today = DateTime.Today;
			_schedules = new List<Schedule>
			{
				new() { Id = _nextId++, Date = today, EmployeeId = 1, ShiftType = "Morning" },
				new() { Id = _nextId++, Date = today, EmployeeId = 2, ShiftType = "Afternoon" },
				new() { Id = _nextId++, Date = today.AddDays(1), EmployeeId = 1, ShiftType = "Full" }
			};
		}

		public async Task<List<Schedule>> GetSchedulesAsync()
		{
			return await Task.FromResult(_schedules);
		}

		public async Task<List<Schedule>> GetSchedulesByDateAsync(DateTime date)
		{
			return await Task.FromResult(_schedules.Where(s => s.Date.Date == date.Date).ToList());
		}

		public async Task<List<Schedule>> GetSchedulesByEmployeeAsync(int employeeId)
		{
			return await Task.FromResult(_schedules.Where(s => s.EmployeeId == employeeId).ToList());
		}

		public async Task<Schedule?> GetScheduleAsync(int id)
		{
			return await Task.FromResult(_schedules.FirstOrDefault(s => s.Id == id));
		}

		public async Task AddScheduleAsync(Schedule schedule)
		{
			// Validate schedule data
			schedule.Id = _nextId++;
			_schedules.Add(schedule);
			await Task.CompletedTask;
		}

		public async Task UpdateScheduleAsync(Schedule schedule)
		{
			// validate schedule data
			var existing = _schedules.FirstOrDefault(s => s.Id == schedule.Id);
			// make sure that the schedule exists before updating
			if (existing != null)
			{
				existing.Date = schedule.Date;
				existing.EmployeeId = schedule.EmployeeId;
				existing.ShiftType = schedule.ShiftType;
				existing.Notes = schedule.Notes;
			}
			await Task.CompletedTask;
		}

		public async Task DeleteScheduleAsync(int id)
		{
			// validate that the schedule exists before deleting
			var schedule = _schedules.FirstOrDefault(s => s.Id == id);
			if (schedule != null)
			{
				_schedules.Remove(schedule);
			}
			await Task.CompletedTask;
		}

		public async Task<bool> ScheduleExistsAsync(int id)
		{
			return await Task.FromResult(_schedules.Any(s => s.Id == id));
		}
	}
}
