using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class SignalRHub : Hub
    {
        public async Task SendMessageNewPetrolStation()
        {
            await Clients.All.SendAsync("NewStation");
        }
    }
}
