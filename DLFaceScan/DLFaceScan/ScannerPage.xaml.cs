using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLFaceScan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        public ScannerPage(MediaFile file)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            faceImage.Source = ImageSource.FromStream(() =>
            {
                   var stream = file.GetStreamWithImageRotatedForExternalStorage();
                   return stream;
            });
        }

        private void ContinueButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}