using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstract_Services;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Repository;
using NotificationService.Infrastructure.Senders;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, Application.Services.NotificationService>();

            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));

            services.AddScoped<INotificationSender, EmailNotificationSender>();
            services.AddScoped<INotificationSender, SmsNotificationSender>();

            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            services.AddHostedService<MessageSentKafkaConsumer>();

            return services;
        }
    }
}
