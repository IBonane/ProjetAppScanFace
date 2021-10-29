using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DLFaceScan
{
    public partial class MainPage : ContentPage
    {
        public ICommand AnimationClikedCommand { get; set; }
      
        public MainPage()
        {
            AnimationClikedCommand = new Command(() =>
            {
                StartButtonClickedAsync();
            });

            BindingContext = this;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void StartButtonClicked(object sender, EventArgs e)
        {
            StartButtonClickedAsync();
        }

        private async Task StartButtonClickedAsync()
        {
            //tester si on a accès à internet
            var networkAccess = Connectivity.NetworkAccess;

            if (networkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Erreur", "Vous devez être connecter au réseau !", "OK");
                return;
            }

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Erreur", ": La caméra n'est pas disponible !", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
            {
                //await DisplayAlert("Erreur", ": Pas de fichier !", "OK");
                return;
            }


            // await DisplayAlert("Réuissi", file.Path, "OK");

            await Navigation.PushAsync(new ScannerPage(file), false); 
        }

    }
}
