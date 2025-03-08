using AKhderApi.backend.DTOs.SharedDto;
using AKhderApi.DTOs.NotificationDtos;
using AKhderApi.Models;
using AKhderApi.Repositories;
using AutoMapper;
using BlogSystemApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AKhderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var userNotifications = await _unitOfWork.UserNotifications.GetAllAsync
                (
                criteria: un => un.UserId == userId,
                includes: new[] { "Notification" },
                orderBy: un => un.Notification.CreatedAt,
                orderByDirection: OrderByDirection.Descending,
                pageSize: pageSize,
                pageNumber: pageNumber
                );

            if (!userNotifications.Any())
                return Ok(new { message = "No notifications found." });

            var productsPagination = new PaginationDto<ReadNotificationDto>
            {
                TotalCount = await _unitOfWork.UserNotifications.CountAsync(un => un.UserId == userId),
                PageSize = pageSize,
                PageNumber = pageNumber,
                PaginationList = _mapper.Map<IEnumerable<ReadNotificationDto>>(userNotifications)
            };

            return Ok(productsPagination);
        }
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = User.FindFirstValue("uid");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Invalid token or user not authenticated." });

                var unreadCount = await _unitOfWork.UserNotifications.CountAsync(un => un.UserId == userId && !un.IsRead);

                return Ok(new { unreadCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving unread notifications.", error = ex.Message });
            }
        }


        [HttpPost("general-notification")]
        public async Task<IActionResult> SendGeneralNotification(NotificationDto notificationDto)
        {
            if (notificationDto == null)
                return BadRequest(new { message = "Invalid notification data." });

            var notification = _mapper.Map<Notification>(notificationDto);
            notification.IsGeneral = true;
            notification.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            var users = await _unitOfWork.Users.SelectAsync<string>(u => u.Id, u => u.IsActive);
            if (!users.Any())
                return NotFound(new { message = "No active users found to send notifications." });

            var userNotifications = users.Select(userId => new UserNotification
            {
                UserId = userId,
                NotificationId = notification.Id
            }).ToList();

            await _unitOfWork.UserNotifications.AddRangeAsync(userNotifications);

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Notification created and sent successfully.", notificationId = notification.Id });
        }

        [HttpPost("private/{userId}")]
        public async Task<IActionResult> SendPrivateNotification(string userId, NotificationDto notificationDto)
        {
            if (notificationDto == null)
                return BadRequest(new { message = "Invalid notification data." });

            var notification = _mapper.Map<Notification>(notificationDto);
            notification.IsGeneral = false;
            notification.CreatedAt = DateTime.UtcNow;

            if (!await _unitOfWork.Users.Exists(u => u.Id == userId))
                return NotFound(new { message = $"No user was found with ID: {userId}" });

            notification.UserNotifications = new List<UserNotification>
            {
                new UserNotification { UserId = userId }
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Notification created and sent successfully.", notificationId = notification.Id });
        }

        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {

            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var notification = await _unitOfWork.UserNotifications.FindAsync(un => un.NotificationId == notificationId && un.UserId == userId);

            if (notification == null)
                return NotFound(new { message = "Notification not found or does not belong to the user." });

            if (notification.IsRead)
                return Ok(new { message = "Notification is already marked as read." });

            notification.IsRead = true;
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Notification marked as read successfully." });
        }

        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or user not authenticated." });

            var notifications = await _unitOfWork.UserNotifications.FindAllAsync(un => un.UserId == userId && !un.IsRead);

            if (!notifications.Any())
                return Ok(new { message = "All notifications are already read." });

            foreach (var notification in notifications)
                notification.IsRead = true;

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "All notifications marked as read successfully." });
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.FindAsync(n => n.Id == notificationId);
            if (notification == null)
                return NotFound(new { message = $"No notification was found with ID: {notificationId}" });

            _unitOfWork.Notifications.HardDelete(notification);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        
    }
}