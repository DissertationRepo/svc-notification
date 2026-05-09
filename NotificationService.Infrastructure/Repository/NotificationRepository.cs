using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Abstract_Services;
using NotificationService.Domain.Entities.Aggregates;
using NotificationService.Domain.ValueObjects;
using NotificationService.Infrastructure.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _db;
        private readonly IMapper _mapper;

        public NotificationRepository(NotificationDbContext db, IMapper mapper)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddNotification(Notification notification)
        {
            var entity = _mapper.Map<NotificationEntity>(notification);
            try
            {
                await _db.Notifications.AddAsync(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the notification. Exception Message: {ex.Message}", ex);
            }
        }

        public async Task<Notification?> GetNotificationById(Guid id)
        {
            var entity = await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
            {
                return null;
            }

            return MapToDomain(entity);
        }

        public async Task UpdateNotification(Notification notification)
        {
            var existing = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notification.Id.Value);
            if (existing is null)
            {
                throw new InvalidOperationException($"Notification {notification.Id} not found.");
            }

            existing.Status = (int)notification.Status;
            existing.FailureReason = notification.FailureReason;
            existing.SentAt = notification.SentAt;

            await _db.SaveChangesAsync();
        }

        private static Notification MapToDomain(NotificationEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Email))
            {
                throw new InvalidOperationException($"Notification {entity.Id} is missing an email address.");
            }

            var recipient = new Recipient(entity.RecipientId, new Email(entity.Email!));

            return Notification.Load(
                new NotificationId(entity.Id),
                recipient,
                new MessageSubject(entity.MessageSubject),
                new MessageBody(entity.MessageBody),
                (Domain.ValueObjects.NotificationType)entity.NotificationType,
                (NotificationStatus)entity.Status,
                entity.FailureReason,
                entity.CreatedAt,
                entity.SentAt);
        }
    }
}
