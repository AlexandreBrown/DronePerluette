using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DronePerluetteLecture
{
    [Serializable]
    public class Moto : Vehicule
   {
      public Moto()
      {

      }
      public Moto(int id, int x, int y) : base(id, x, y)
      {
      }

        public override BitmapImage ToImage()
        {
            UriPath = PackPath + BasePath + "Moto.png";
            return new BitmapImage(new Uri(UriPath, UriKind.Absolute));
        }
    }
}
