using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.ViewModels
{
    public class AboutViewModel:ReactiveObject
    {
        public delegate void Close();
        public event Close CloseEvent;
        public IReactiveCommand CloseCommand { get; set; }
        public IReactiveCommand OpenGithubCommand { get; set; }
        public IReactiveCommand OpenSiteCommand { get; set; }
        public AboutViewModel()
        {
            CloseCommand = ReactiveCommand.Create(() => CloseEvent?.Invoke());

            OpenGithubCommand = ReactiveCommand.Create(() => 
                Process.Start(@"https://github.com/sPEDwagonProjects/CardsGame"));

            OpenSiteCommand = ReactiveCommand.Create(() => 
                Process.Start(@"https://spedwagon.online/"));
        }
    }
}
