using System.Threading.Tasks;
using ClassCheck.Models;
using ClassCheck.Services;

namespace ClassCheck.ViewModels
{
    public class SessionViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public SessionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

       
    }
}
