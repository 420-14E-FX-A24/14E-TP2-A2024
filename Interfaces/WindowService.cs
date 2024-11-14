using System;
using Automate.Models;

namespace Automate.Interfaces
{
    public interface IWindowService
    {
        DateTime DateSelection { get; set; }
        Role? Role { get; set; }
        void Close();
    }
}