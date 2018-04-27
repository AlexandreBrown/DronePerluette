using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DronePerluetteLecture
{
    [Serializable]
    public class Camion : Vehicule
   {
      public Camion()
      {

      }
      public Camion(int id, int x, int y) : base(id, x, y)
      {
      }

        public override BitmapImage ToImage()
        {
            UriPath = PackPath + BasePath + "Camion.png";
            return new BitmapImage(new Uri(UriPath, UriKind.Absolute));
        }
    }
}
