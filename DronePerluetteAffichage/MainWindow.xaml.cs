using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DronePerluetteLecture;

namespace DronePerluetteAffichage
{
   /// <summary>
   /// Logique d'interaction pour MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public int SleepTime { get; set; } = 25;
      public List<Vehicule> LstVehicules { get; set; }
      public List<Image> LstImagesVehicules { get; set; }
      public int sizeImage = 200;
      BinaryMessageFormatter formatter = new BinaryMessageFormatter();


      public MainWindow()
      {
        InitializeComponent();
        LstVehicules = new List<Vehicule>();
        LstImagesVehicules = new List<Image>();
        
            // Vérifie si MessageQueue est installé
        List<ServiceController> services = ServiceController.GetServices().ToList();
        ServiceController msQue = services.Find(o => o.ServiceName == "MSMQ");
        if (msQue != null)
        {
            if (msQue.Status == ServiceControllerStatus.Running)
            {
                MonitorPerluetteQueueAsync();
            }
        }
        else
        {
            MessageBox.Show("MessageQueue n'est pas installé sur cet ordinateur");
            MessageBox.Show("Lien pour télécharger MessageQueue : https://docs.microsoft.com/en-us/dotnet/framework/wcf/samples/installing-message-queuing-msmq");
        }
                
      }

      private async void MonitorPerluetteQueueAsync()
      {
        MessageQueue dronePerluetteQueue = new MessageQueue(@".\Private$\DronePerluette")
        {
            Formatter = formatter
        };

            await Task.Run(async () =>
            {
                while (true)
                 {
                    try
                    {
                       if (dronePerluetteQueue != null){
                            // Read message
                            if (MessageQueue.Exists(@".\Private$\DronePerluette"))
                            {
                              Message[] messages = await ReceiveMessagesAsync(dronePerluetteQueue);
                                foreach (Message message in messages)
                                {
                                    if (message != null)
                                    {
                                        if (message.Body.GetType().Name != null)
                                        {
                                            if (message.Body.GetType().Name == typeof(int).Name)
                                            {
                                                SleepTime = ((int)message.Body);
                                            }
                                            else
                                            {
                                                await UpdateVehiculeLocation(((Vehicule)message.Body));
                                            }
                                        }
                                    }
                                    await Task.Run(() => { Thread.Sleep(SleepTime); });
                                }
                            }
                       }
                    }
                    catch
                    {
                        // Code bug free pls
                    }
                 }
            });
      }

        private async Task UpdateVehiculeLocation(Vehicule vehiculeRetrieved)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(async () =>
                {
                    if (VehiculeIsNew(vehiculeRetrieved))
                    {
                        LstVehicules.Add(vehiculeRetrieved);
                        await UpdateDisplayAsync();
                    }
                    else
                    {
                        if (GetVehiculeFromId(vehiculeRetrieved.ID).X != vehiculeRetrieved.X || GetVehiculeFromId(vehiculeRetrieved.ID).Y != vehiculeRetrieved.Y)
                        {
                            LstVehicules[LstVehicules.IndexOf(GetVehiculeFromId(vehiculeRetrieved.ID))] = vehiculeRetrieved;
                            await UpdateDisplayAsync();
                        }
                    }
                }));
            });
        }

        private Vehicule GetVehiculeFromId(int id)
        {
            foreach (Vehicule v in LstVehicules)
            {
                if(v.ID == id)
                {
                    return v;
                }
            }
            throw new Exception("Aucun vehicule pour l'id " + id);
        }

      private async Task<Message[]> ReceiveMessagesAsync(MessageQueue queue){
            Message[] msgs = null;
            await Task.Run(() => {
                msgs = queue.GetAllMessages();
            });
            return msgs;
      }

      private bool VehiculeIsNew(Vehicule vehiculeGiven)
      {
         foreach (Vehicule vehiculeFound in LstVehicules)
         {
            if (vehiculeGiven.ID == vehiculeFound.ID)
            {
               return false;
            }
         }
         return true;
      }

        private async Task UpdateDisplayAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    cnvAffichage.Children.Clear();
                    for (int i = 0; i < LstVehicules.Count; i++)
                    {
                        Image img = new Image
                        {
                            Source = LstVehicules[i].ToImage(),
                            Height = sizeImage,
                            Width = sizeImage
                        };
                        Canvas.SetLeft(img, LstVehicules[i].X);
                        Canvas.SetTop(img, LstVehicules[i].Y);
                        cnvAffichage.Children.Add(img);
                    }
                }));
            });
        }

      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }

      private void TopBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         DragMove();
      }

      private void Minimize_Click(object sender, RoutedEventArgs e)
      {
         WindowState = WindowState.Minimized;
      }

      private void Maximize_Click(object sender, RoutedEventArgs e)
      {
         if (WindowState == WindowState.Maximized)
         {
            WindowState = WindowState.Normal;
         }
         else
         {
            WindowState = WindowState.Maximized;
         }
      }
   }
}