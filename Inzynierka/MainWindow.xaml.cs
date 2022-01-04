using System;
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
        private WaveFileWriter WaveFileWriter = null;
        private WasapiLoopbackCapture WasapiLoopbackCapture = null;
        private int x = 1;
        public int perSecond = 0;
        public int perSecondFinal = 0;
        private bool setSecondsBytes = false;
        private SpeechToTextService speechToText;
        private string outputFIle;
        public MainWindow()
        {
            InitializeComponent();
            config = Config.Instance();
            MainTextBox.FontSize = config.getFontSize();
            MainTextBox.FontStyle = config.getFontStyle();
            MainTextBox.Foreground = config.getFontColor();
            MainTextBox.Background = config.getBackgroundColor();
            deleteAllSamples();
            StopListening.IsEnabled = false;

            IamAuthenticator authenticator = new IamAuthenticator(apikey: "h-D6C2eKDZUGDOm7DA6GR8hvjg3DJySmPcNhKk34WyHl");
            speechToText = new SpeechToTextService(authenticator);
            speechToText.SetServiceUrl("https://api.eu-gb.speech-to-text.watson.cloud.ibm.com/instances/e6f88697-93d1-4618-b1c3-6bc54d8705d8");
            speechToText.WithHeader("Transfer-Encoding", "chunked");

            outputFIle = @"../../../Samples/sample" + x + ".wav";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.Show();
        }

        private void StartListenning_Click(object sender, RoutedEventArgs e)
        {
            
            WasapiLoopbackCapture = new WasapiLoopbackCapture();
            WaveFileWriter = new WaveFileWriter(outputFIle, WasapiLoopbackCapture.WaveFormat);

            WasapiLoopbackCapture.DataAvailable += (s, a) =>
            {
                WaveFileWriter.Write(a.Buffer, 0, a.BytesRecorded);
                if (setSecondsBytes == false)
                {
                    perSecond = WasapiLoopbackCapture.WaveFormat.AverageBytesPerSecond * 2;
                }
                else
                {
                    perSecond = perSecondFinal;
                }


                if (WaveFileWriter.Position > perSecond)
                {
                    x++;
                    if (setSecondsBytes == false)
                    {
                        perSecondFinal = perSecond;
                        setSecondsBytes = true;
                    }
                    WaveFileWriter.Dispose();


                    WaveFileWriter = new WaveFileWriter(@"../../../Samples/sample" + x + ".wav", this.WasapiLoopbackCapture.WaveFormat);

                    var task = Task.Run(() =>
                    {
                        TranscriptRecognition(@"../../../Samples/sample" + (x - 1) + ".wav", speechToText);
                    });
                }
            };

            WasapiLoopbackCapture.RecordingStopped += (s, a) =>
            {
                WaveFileWriter.Dispose();
                WaveFileWriter = null;
                WasapiLoopbackCapture.Dispose();
            };

            StartListenning.IsEnabled = false;
            StopListening.IsEnabled = true;
            WasapiLoopbackCapture.StartRecording();
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
                    return (string)result["results"][0]["alternatives"][0]["transcript"];
                }
                catch
                {
                    return " ";
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
            deleteAllSamples();
            MessageBoxResult boxResult = MessageBox.Show("Minął czas nieaktywności.");
        }
        private void InternalServerError()
        {
            WasapiLoopbackCapture.StopRecording();
            StartListenning.IsEnabled = true;
            StopListening.IsEnabled = false;
            deleteAllSamples();
            MessageBoxResult boxResult = MessageBox.Show("Wewnętrzny błąd serwera.");
        }
        private void ServiceUnavailable()
        {
            WasapiLoopbackCapture.StopRecording();
            StartListenning.IsEnabled = true;
            StopListening.IsEnabled = false;
            deleteAllSamples();
            MessageBoxResult boxResult = MessageBox.Show("Serwis tymczasowo niedostępny.");
        }

        private void TranscriptRecognition(string file, SpeechToTextService speechToText)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                using (FileStream fileStream = File.OpenRead(file))
                {
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                }

                File.Delete(file);
                var transcribe = speechToText.Recognize(
                                audio: memoryStream, //skąd
                                contentType: "audio/wav", //jaki typ
                                inactivityTimeout: config.getTimeOut(), //po jakim czasie wyłącza się 
                                model: config.getLanguage(), //jaki język 
                                smartFormatting: config.getSmartFormatting()
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

        private void deleteAllSamples()
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(@"../../../Samples/");
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
            catch (DirectoryNotFoundException e)
            {

            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            deleteAllSamples();
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
    }
}
