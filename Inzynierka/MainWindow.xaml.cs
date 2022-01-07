using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using IBM.Watson.SpeechToText.v1;
using IBM.Cloud.SDK.Core.Http.Exceptions;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using Newtonsoft.Json.Linq;

namespace Inzynierka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config config;
        private WasapiLoopbackCapture WasapiLoopbackCapture = null;
        private SpeechToTextService speechToText;
        private int NoTimeOut = 0;
        public MainWindow()
        {
            InitializeComponent();
            config = Config.Instance();
            MainTextBox.FontSize = config.getFontSize();
            MainTextBox.FontStyle = config.getFontStyle();
            MainTextBox.Foreground = config.getFontColor();
            MainTextBox.Background = config.getBackgroundColor();
            StopListening.IsEnabled = false;

            IamAuthenticator authenticator = new IamAuthenticator(apikey: "h-D6C2eKDZUGDOm7DA6GR8hvjg3DJySmPcNhKk34WyHl");
            speechToText = new SpeechToTextService(authenticator);
            speechToText.SetServiceUrl("https://api.eu-gb.speech-to-text.watson.cloud.ibm.com/instances/e6f88697-93d1-4618-b1c3-6bc54d8705d8");
            //speechToText.WithHeader("Transfer-Encoding", "chunked");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.Show();
        }

        private void StartListenning_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("start click");
            WasapiLoopbackCapture = new WasapiLoopbackCapture();

            WasapiLoopbackCapture.StartRecording();
            StartListenning.IsEnabled = false;
            StopListening.IsEnabled = true;
            
            MemoryStream memoryStream = new MemoryStream();

            WasapiLoopbackCapture.DataAvailable += (s, a) =>
            {
                memoryStream.Write(a.Buffer, 0, a.BytesRecorded);

                if (memoryStream.Position > WasapiLoopbackCapture.WaveFormat.AverageBytesPerSecond * 2)
                {
                    MemoryStream stream = new MemoryStream();
                    int memoryLenght = (int)(memoryStream.Length - 8);
                    int numberOfSamples = (int)((memoryStream.Length - 58) / 8);
                    int pureMemoryLenght = (int)(memoryStream.Length - 58);
                    stream = WavHeader(memoryLenght, numberOfSamples, pureMemoryLenght, 384000, 32, 48000);

                    memoryStream.WriteTo(stream);

                    Debug.WriteLine("BEFORE TASK RUN");
                    var task = Task.Run(() =>
                    {
                        Debug.WriteLine("TASK RUN");
                        TranscriptRecognition(stream, speechToText);
                    });
                    memoryStream = new MemoryStream();
                }

                if(NoTimeOut == config.getTimeOut() / 2)
                {
                    WasapiLoopbackCapture.StopRecording();
                    WasapiLoopbackCapture.Dispose();
                    StartListenning.IsEnabled = true;
                    StopListening.IsEnabled = false;
                }
            };

            WasapiLoopbackCapture.RecordingStopped += (s, a) =>
            {
                WasapiLoopbackCapture.Dispose();
            };  
        }

        private void StopListening_Click(object sender, RoutedEventArgs e)
        {
            if (StopListening.IsEnabled == true && WasapiLoopbackCapture != null)
            {
                WasapiLoopbackCapture.StopRecording();
                WasapiLoopbackCapture.Dispose();
                StartListenning.IsEnabled = true;
                StopListening.IsEnabled = false;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Text = "";
        }

        private string fromJSON(IBM.Cloud.SDK.Core.Http.DetailedResponse<IBM.Watson.SpeechToText.v1.Model.SpeechRecognitionResults> detailedResponse)
        {
            var result = JObject.Parse(detailedResponse.Response);
            if (result != null)
            {
                try
                {
                    var text = (string?)result["results"][0]["alternatives"][0]["transcript"];
                    var confidence = (double?)result["results"][0]["alternatives"][0]["confidence"];
                    if (text != null && confidence >= 0.70)
                    {
                        NoTimeOut = 0;
                        return text;
                    }
                    else if(text == null && confidence == null)
                    {
                        NoTimeOut++;
                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "";
                }
                
            }
            else
            {
                WasapiLoopbackCapture.StopRecording();
                WasapiLoopbackCapture.Dispose();
                StartListenning.IsEnabled = true;
                StopListening.IsEnabled = false;
                return "";
            }       
        }

        private void Inactivity()
        {
            WasapiLoopbackCapture.StopRecording();
            StartListenning.IsEnabled = true;
            StopListening.IsEnabled = false;
            MessageBoxResult boxResult = MessageBox.Show("Minął czas nieaktywności.");
        }
        private void InternalServerError()
        {
            WasapiLoopbackCapture.StopRecording();
            StartListenning.IsEnabled = true;
            StopListening.IsEnabled = false;
            MessageBoxResult boxResult = MessageBox.Show("Wewnętrzny błąd serwera.");
        }
        private void ServiceUnavailable()
        {
            WasapiLoopbackCapture.StopRecording();
            StartListenning.IsEnabled = true;
            StopListening.IsEnabled = false;
            MessageBoxResult boxResult = MessageBox.Show("Serwis tymczasowo niedostępny.");
        }

        private void TranscriptRecognition(MemoryStream stream, SpeechToTextService speechToText)
        {
            //float backgroundAudioSuppres = 0.3F;
            //float speechDetectorSens = float.Parse("0.4");
            try
            { 
                var transcribe = speechToText.Recognize(
                                audio: stream, //skąd
                                contentType: "audio/wav", //jaki typ
                                //inactivityTimeout: config.getTimeOut(),
                                model: config.getLanguage(), //jaki język 
                                smartFormatting: config.getSmartFormatting()
                                
                            //backgroundAudioSuppression: backgroundAudioSuppres
                                //speechDetectorSensitivity: speechDetectorSens
                            );
                if (transcribe.StatusCode == 408)
                {
                    //inactivity
                    Inactivity();
                }
                else if(transcribe.StatusCode == 500)
                {
                    //internal server error
                    InternalServerError();
                    
                }
                else if(transcribe.StatusCode == 503)
                {
                    //service unavailable
                    ServiceUnavailable();
                }
                else if(transcribe.StatusCode == 200)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MainTextBox.Text += fromJSON(transcribe);
                        Debug.WriteLine(transcribe.Response);
                    }));
                }
            }
            catch (ServiceResponseException e)
            {
                WasapiLoopbackCapture.StopRecording();
                StartListenning.IsEnabled = true;
                StopListening.IsEnabled = false;
                MessageBoxResult box = MessageBox.Show("Error: " + e.Message);
            }
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(config.getTextMode() == 1)
            {

            }
            else if(config.getTextMode() == 2)
            {
                MainTextBox.Text = resizeText(MainTextBox.Text, 250);
            }
        }

        private string resizeText(string text, int size)
        {
            string sentence = text;
            if (sentence.Length > size)
            {
                sentence = sentence.Substring(text.IndexOf(" ") + 1);
            }
            if (sentence.Length > size)
            {
                resizeText(sentence, size);
            }
            return sentence;
        }

        /// <summary>
        /// Tworzy memorystream z nagłówkiem odpowiadającym dla pliku .wav, do którego można potem dopisać "czyste" dane.
        /// </summary>
        /// <param name="memoryLenght">wielkość pliku od której należy odjąć 8</param>
        /// <param name="numberOfSamples">ilość próbek, jest to wielkość bliku od której należy odjąć 58 oraz wynik ten podzielić przez 8</param>
        /// <param name="pureMemoryLenght">wielkość pliku, bez nagłówka, czyli od wielkości pliku należy odjąć 58 gdyż tyle zamuje nagłówek</param>
        /// <param name="bitsPerSec">ilość bitów na sekunde</param>
        /// <param name="bitsPerSample">ilość bitów na próbkę</param>
        /// <param name="samplePerSec">ilość próbek na sekundę</param>
        /// <returns>memorystream z nagłówkiem do którego potem dołączane są "czyste" dane</returns>
        private MemoryStream WavHeader(int memoryLenght, int numberOfSamples, int pureMemoryLenght, int bitsPerSec, int bitsPerSample, int samplePerSec)
        {
            MemoryStream memory = new MemoryStream();
            memory.Position = 0;

            //nagłówek RIFF
            memory.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            //wielkość pliku - 8
            memory.Write(BitConverter.GetBytes(memoryLenght), 0, 4);

            //format WAVE
            memory.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

            //identyfikator fragmentu "fmt "
            memory.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            //chunk size 18
            memory.Write(BitConverter.GetBytes(18), 0, 4);

            //formatowanie kodu 3
            memory.Write(BitConverter.GetBytes(3), 0, 2);

            //ilość kanałów 2
            memory.Write(BitConverter.GetBytes(2), 0, 2);

            //ilość próbek na sekundę
            memory.Write(BitConverter.GetBytes(samplePerSec), 0, 4);

            //ilość bitów na sekundę
            memory.Write(BitConverter.GetBytes(bitsPerSec), 0, 4);

            //block allign
            memory.Write(BitConverter.GetBytes(8), 0, 2);

            //bity na próbkę
            memory.Write(BitConverter.GetBytes(bitsPerSample), 0, 4);

            //fact
            memory.Write(Encoding.ASCII.GetBytes("fact"), 0, 4);

            //rozmiar, min 4
            memory.Write(BitConverter.GetBytes(4), 0, 4);

            //ilość próbek
            memory.Write(BitConverter.GetBytes(numberOfSamples), 0, 4);

            //data (dane)
            memory.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            //ilość czystych danych, czyli bez nagłówka
            memory.Write(BitConverter.GetBytes(pureMemoryLenght), 0, 4);

            return memory;
        }
    }
}
