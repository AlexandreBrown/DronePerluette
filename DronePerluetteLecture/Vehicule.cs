using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DronePerluetteLecture
{
   [Serializable]
   public abstract class Vehicule : IVehicule
    {
      public int ID {get;set;}
      public int X { get; set; }
      public int Y { get; set; }
      protected string PackPath { get => "pack://application:,,,"; }
      protected string BasePath { get => "/Resources/Images/"; }
      protected string UriPath { get; set; } = "";
      public Vehicule()
      {

      }
      public Vehicule(int id, int x, int y)
      {
         ID = id;
         X = x;
         Y = y;
      }
        public abstract BitmapImage ToImage();
    }

   
}
