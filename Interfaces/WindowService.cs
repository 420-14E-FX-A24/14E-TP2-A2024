using System;
using System.Collections.ObjectModel;
using Automate.Models;

namespace Automate.Interfaces
{
    public interface IWindowService
    {
        DateTime DateSelection { get; set; }
        ObservableCollection<Task> Tasks { get; set; }
		bool IsAdmin { get; set; }
        void Close();
    }
}