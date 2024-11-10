using System;

namespace Automate.Interfaces
{
    public interface IWindowService
    {
        DateTime DateSelection { get; set; }
        string Role { get; set; }
        void Close();
    }
}