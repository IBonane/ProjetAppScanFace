using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLFaceScan
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void StartButtonClicked(object sender, EventArgs e)
        {
            StartButtonClickedAsync();
        }


        private async Task StartButtonClickedAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Erreur", ": La caméra n'est pas disponible !", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
            {
                await DisplayAlert("Erreur", ": Pas de fichier !", "OK");
                return;
            }


            // await DisplayAlert("Réuissi", file.Path, "OK");

            await Navigation.PushAsync(new ScannerPage(file)); 
        }

    }
}
