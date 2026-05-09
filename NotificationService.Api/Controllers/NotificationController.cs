using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Models;
using NotificationService.Application.Abstract_Services;
using NotificationService.Application.Models;

namespace NotificationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("send")]
        //[Authorize]
        public async Task<IActionResult> SendNotification([FromBody] SendNotification request, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<NotificationModel>(request);
            var notification = await _notificationService.CreateAndSendAsync(model, cancellationToken);
            var response = _mapper.Map<NotificationResponse>(notification);
            return Ok(response);
        }

        [HttpGet("notification/{id}")]
        [Authorize]
        public async Task<IActionResult> GetNotificationById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid notification ID format. Please provide a valid GUID.");
            }

            var notification = await _notificationService.GetNotificationByIdAsync(guid);
            if (notification is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<NotificationResponse>(notification);
            return Ok(response);
        }
    }
}
