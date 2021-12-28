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
        public MainWindow()
        {
            InitializeComponent();
            config = Config.Instance();
            MainTextBox.FontSize = config.getFontSize();
            MainTextBox.FontStyle = config.getFontStyle();
            MainTextBox.Foreground = config.getFontColor();
            MainTextBox.Background = config.getBackgroundColor();
            deleteAllSamples();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.Show();
        }

        private void StartListenning_Click(object sender, RoutedEventArgs e)
        {
            string outputFIle = @"../../../Samples/sample" + x + ".wav";
            WasapiLoopbackCapture = new WasapiLoopbackCapture();
            WaveFileWriter = new WaveFileWriter(outputFIle, WasapiLoopbackCapture.WaveFormat);

            IamAuthenticator authenticator = new IamAuthenticator(apikey: "h-D6C2eKDZUGDOm7DA6GR8hvjg3DJySmPcNhKk34WyHl");
            SpeechToTextService speechToText = new SpeechToTextService(authenticator);
            speechToText.SetServiceUrl("https://api.eu-gb.speech-to-text.watson.cloud.ibm.com/instances/e6f88697-93d1-4618-b1c3-6bc54d8705d8");
            speechToText.WithHeader("Transfer-Encoding", "chunked");

            WasapiLoopbackCapture.DataAvailable += (s, a) =>
            {
                if (setSecondsBytes == false)
                {
                    perSecond = WasapiLoopbackCapture.WaveFormat.AverageBytesPerSecond * 3;
                }
                else
                {
                    perSecond = perSecondFinal;
                }

                WaveFileWriter.Write(a.Buffer, 0, a.BytesRecorded);

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
                return (string)result["results"][0]["alternatives"][0]["transcript"];
            }
            else
            {
                WasapiLoopbackCapture.StopRecording();
                return "";
            }       
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
                                model: config.getLanguage(), //jaki język (-1) dla nieskończonosci
                                smartFormatting: config.getSmartFormatting()
                            );
                Dispatcher.Invoke(new Action(() => 
                { 
                    MainTextBox.Text += fromJSON(transcribe); 
                }));

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
            DirectoryInfo directoryInfo = new DirectoryInfo(@"../../../Samples/");
            foreach(FileInfo fileInfo in directoryInfo.GetFiles())
            {
                fileInfo.Delete();
            }
        }
    }
}
