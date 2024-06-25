using JSSATSAPI.DataAccess.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class WarrantyStatusUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); 

        public WarrantyStatusUpdaterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateWarrantyStatuses();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task UpdateWarrantyStatuses()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var warrantyTicketRepository = scope.ServiceProvider.GetRequiredService<IWarrantyTicketRepository>();

                var activeWarrantyTickets = await warrantyTicketRepository.GetActiveWarrantyTicketsAsync();

                foreach (var ticket in activeWarrantyTickets)
                {
                    if (ticket.WarrantyEndDate.HasValue && ticket.WarrantyEndDate.Value <= DateTime.UtcNow)
                    {
                        ticket.Status = "InActive";
                        warrantyTicketRepository.Update(ticket);
                    }
                }

                 warrantyTicketRepository.SaveChanges();
            }
        }
    }
}
