using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace LibraryBuddy.Mobile.ViewModels
{
    public class CameraViewModel : BaseViewModel
    {
        public CameraViewModel()
        {
            Title = "Camera";

            TakePictureCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand TakePictureCommand { get; }
    }
}