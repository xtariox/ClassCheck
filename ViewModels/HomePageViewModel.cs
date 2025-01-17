﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClassCheck.ViewModels
{

    public class HomePageViewModel
    {
        public ICommand NavigateToAbsenceCommand { get; }
        public ICommand NavigateToAddStudentCommand { get; }
        public ICommand NavigateToNewLessonCommand { get; }
        public ICommand NavigateToSearchCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand NavigateToListOfStudentsCommand { get; }

        public HomePageViewModel()
        {
            NavigateToAbsenceCommand = new Command(async () =>
            {
                
                await Shell.Current.GoToAsync("///attendance");
            });

            NavigateToListOfStudentsCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("///student");
            });

            NavigateToAddStudentCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("///addstudent");
            });

            NavigateToNewLessonCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("///addlesson");
            });

            NavigateToSearchCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("///search");
            });

            LogoutCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("///login");
            });
        }

    }
}
