using DLFaceScan.model;
using DLFaceScan.service;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLFaceScan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        bool processing = true;
        FaceDetectResult faceDetectResult = null;
        SpeechOptions speechOptions = null;


        public ScannerPage(MediaFile file)
        {
            InitializeComponent();
            //infoLayout.IsVisible = false;

            NavigationPage.SetHasNavigationBar(this, false);

            faceImage.Source = ImageSource.FromStream(() =>
            {
                   var stream = file.GetStreamWithImageRotatedForExternalStorage();
                   return stream;
            });

            laserAnimationWithSoundAndDisplayResult();

            startDetection(file);

        }

        private async Task laserAnimationWithSoundAndDisplayResult()
        {
            laserAnimation.Opacity = 0;
            await Task.Delay(500);
            await laserAnimation.FadeTo(1, 500);

            playSound("scan.wav");
            await laserAnimation.TranslateTo(0, 360, 1800);
            double y = 0;

            while (processing)
            {
                playCurrentSound();
                await laserAnimation.TranslateTo(0, y, 1800);
                y = (y == 0) ? 360 : 0;
            }

            laserAnimation.IsVisible = false;
            playSound("result.wav");

            if(faceDetectResult == null)
            {
                //cas d'erreur
                await OnAnalysisError();
            }
            else
            {
                await DisplayResults();
                await ResultSpeech();
            }        
        }

        private async Task OnAnalysisError()
        {
            Speak("Humain non détecté");
            await DisplayAlert("Erreur !", "L'analyse n'a pas fonctionné", "OK");           
            await Navigation.PopAsync();
        }

        private async Task startDetection(MediaFile file)
        {
            faceDetectResult = await CognitiveService.FaceDetect(file.GetStreamWithImageRotatedForExternalStorage());
            //await Task.Delay(5000);
            processing = false;
        }

        private async Task DisplayResults()
        {
            if (faceDetectResult == null) return;

            statusLabel.Text = "Analyse Terminée !";

            //on recupère les infos du visages
            ageLabel.Text = faceDetectResult.faceAttributes.age.ToString();
            genderLabel.Text = faceDetectResult.faceAttributes.gender.Substring(0, 1).ToUpper();//substring donne la position et la longueur du mo
            infoLayout.IsVisible = true;
            continueButton.Opacity = 1;
        }

        private void ContinueButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void playSound(String soundName)
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(GetStreamFromFile(soundName));
            player.Play();
        }

        private void playCurrentSound()
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Stop();
            player.Play();
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("DLFaceScan." + filename);
            return stream;
        }

        private async Task InitSpeak()
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            Console.WriteLine("ok " + locales);
            // Grab the first locale
            var locale = locales.Where(o => o.Language.ToLower() == "fr").FirstOrDefault();

            speechOptions = new SpeechOptions()
            {
                Volume = 0.75f,
                Pitch = 0.1f,
                Locale = locale
            };
        }
        private async Task Speak(String text)
        {
            if(speechOptions == null)
            {
                await InitSpeak();
            }
            await TextToSpeech.SpeakAsync(text, speechOptions);
        }

        private async Task ResultSpeech()
        {

            if (faceDetectResult == null) return;

            await Speak("Humain détecté");

            if(faceDetectResult.faceAttributes.gender.ToLower() == "male")
            {
                await Speak("Sexe masculin");
            }

            else
            {
                await Speak("Sexe féminin");
            }
                
            await Speak("âge " + faceDetectResult.faceAttributes.age.ToString() + " ans");
        }
        
    }
}