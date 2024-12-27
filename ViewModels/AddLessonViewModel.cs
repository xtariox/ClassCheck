using System.Windows.Input;
using MauiApp1.Models;
using ClassCheck.Services;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels
{
    public class AddLessonViewModel : BaseStudentViewModel
    {
        private readonly DatabaseService _databaseService;

        // Propriétés pour le binding
        private string _courseName;
        public string CourseName
        {
            get => _courseName;
            set => SetProperty(ref _courseName, value);
        }

        private string _professor;
        public string Professor
        {
            get => _professor;
            set => SetProperty(ref _professor, value);
        }

        private string _schedule;
        public string Schedule
        {
            get => _schedule;
            set => SetProperty(ref _schedule, value);
        }

        private string _major;
        public string Major
        {
            get => _major;
            set => SetProperty(ref _major, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        // Commandes pour Ajouter et Annuler
        public ICommand AddLessonCommand { get; }
        public ICommand CancelCommand { get; }

        public AddLessonViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Initialisation des commandes
            AddLessonCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Add button clicked");
                await AddLessonAsync();
            });

            CancelCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");
                Shell.Current.GoToAsync("///home");
            });
        }

        // Méthode pour ajouter une leçon
        private async Task AddLessonAsync()
        {
            if (ValidateInput())
            {
                // Création de la leçon
                var newLesson = new Lesson
                {
                    CourseName = CourseName,
                    Professor = Professor,
                    Schedule = Schedule,
                    Major = Major
                };

                // Ajout dans la base de données
                var result = await _databaseService.Insert(newLesson);

                // Mise à jour du message de succès ou d'erreur
                if (result > 0)
                {
                    SuccessMessage = "Lesson added successfully!";
                    ClearFields();
                }
                else
                {
                    SuccessMessage = "Failed to add the lesson.";
                }
            }
            else
            {
                SuccessMessage = "Please fill in all the required fields.";
            }
        }

        // Validation des champs
        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(CourseName) &&
                   !string.IsNullOrWhiteSpace(Professor) &&
                   !string.IsNullOrWhiteSpace(Schedule) &&
                   !string.IsNullOrWhiteSpace(Major);
        }

        // Réinitialiser les champs
        private void ClearFields()
        {
            CourseName = string.Empty;
            Professor = string.Empty;
            Schedule = string.Empty;
            Major = string.Empty;
        }
    }
}
