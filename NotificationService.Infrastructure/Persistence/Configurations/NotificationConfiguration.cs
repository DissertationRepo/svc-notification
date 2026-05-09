using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Infrastructure.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations;

public sealed class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.RecipientId)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("recipient_id");

        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .HasColumnName("email");

        builder.Property(x => x.Phone)
            .HasMaxLength(30)
            .HasColumnName("phone");

        builder.Property(x => x.MessageSubject)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("message_subject");

        builder.Property(x => x.MessageBody)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName("message_body");

        builder.Property(x => x.NotificationType)
            .IsRequired()
            .HasColumnName("notification_type");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(x => x.FailureReason)
            .HasColumnType("text")
            .HasColumnName("failure_reason");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at");

        builder.HasIndex(x => x.RecipientId);
    }
}
