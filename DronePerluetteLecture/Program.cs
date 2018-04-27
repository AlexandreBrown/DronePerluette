using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

using System.IO;

namespace DronePerluetteLecture
{
   class Program
   {
      
      static void Main(string[] args)
      {
         DisableConsoleQuickEdit.Go();
         StreamReader fichierMessage;
         BinaryMessageFormatter formatter = new BinaryMessageFormatter();

         Console.WriteLine("En attente d'un fichier 'Commande_Mobile.txt'");
         string[] donneesVehicule;
         int ligne = 0;
         
         MessageQueue messageQueue = null;
         if (MessageQueue.Exists(@".\Private$\DronePerluette"))
         {
            messageQueue = new MessageQueue(@".\Private$\DronePerluette");
            messageQueue.Purge();
         }
         else
         {
            // Create the Queue
            MessageQueue.Create(@".\Private$\DronePerluette");
            messageQueue = new MessageQueue(@".\Private$\DronePerluette");
            
            messageQueue.Formatter = formatter;//new XmlMessageFormatter(new Type[] { typeof(Vehicule), typeof(int) });
         }
         while (true)
         {
            do
            {
               try
               {
                  fichierMessage = new StreamReader(File.OpenRead("Commande_Mobile.txt"));
                  messageQueue.Purge();
               }
               catch (Exception)
               {
                  fichierMessage = null;
               }
            } while (fichierMessage == null);
                Console.WriteLine("Traitement du fichier en cours...");
            while (!fichierMessage.EndOfStream)
            {
               donneesVehicule = (fichierMessage.ReadLine()).Split(',');
               ligne++;
               int Temps;
               int ID;
               int X;
               int Y;
               
               // c'est un sleep
               if (donneesVehicule[0].ToLower() == "sleep")
               {
                  if (donneesVehicule.Count() == 1)
                  {
                     Console.WriteLine("Il manque 1 information à la ligne " + ligne);
                  }
                  else
                  {
                     try
                     {
                        Temps = Int32.Parse(donneesVehicule[1]);
                        messageQueue.Send(new Message(Temps, formatter));
                     }
                     catch (Exception)
                     {
                        Console.WriteLine("Le temps pour un sleep doit être un nombre entier.");
                     }
                  }
               }
               else // Pas un sleep
               {
                    if (donneesVehicule[0] == "")
                    {
                        Console.WriteLine("La ligne " + ligne + " ne peut pas être vide.");
                    }
                    else if (donneesVehicule[0].ToLower() != "voiture" && donneesVehicule[0].ToLower() != "moto" && donneesVehicule[0].ToLower() != "camion")
                    {
                        Console.WriteLine("La ligne " + ligne + " est erronée.");
                    }
                    else if (donneesVehicule.Count() == 1 || (donneesVehicule.Count() == 2 && donneesVehicule[1] == "" ))
                    {
                        Console.WriteLine("Il manque 3 informations à la ligne " + ligne);
                    }
                    else if (donneesVehicule.Count() == 2 || (donneesVehicule.Count() == 3 && donneesVehicule[2] == ""))
                    {
                        Console.WriteLine("Il manque 2 informations à la ligne " + ligne);
                    }
                    else if (donneesVehicule.Count() == 3 || (donneesVehicule.Count() == 4 && donneesVehicule[3] == ""))
                    {
                        Console.WriteLine("Il manque 1 information à la ligne " + ligne);
                    }
                    else
                    {
                        try
                        {
                            ID = Int32.Parse(donneesVehicule[1]);
                            try
                            {
                                X = Int32.Parse(donneesVehicule[2]);
                                try
                                {
                                    Y = Int32.Parse(donneesVehicule[3]);
                                    if (donneesVehicule[0].ToLower() == "voiture")
                                    {
                                        messageQueue.Send(new Message(new Voiture(ID, X, Y), formatter));
                                    }
                                    else if (donneesVehicule[0].ToLower() == "moto")
                                    {
                                        messageQueue.Send(new Message(new Moto(ID, X, Y), formatter));
                                    }
                                    else if (donneesVehicule[0].ToLower() == "camion")
                                    {
                                        messageQueue.Send(new Message(new Camion(ID, X, Y), formatter));
                                    }
                                    else
                                    {
                                        Console.WriteLine("Le type " + donneesVehicule[0] + " n'est pas valide à la ligne " + ligne);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Le Y n'est pas valide à la ligne " + ligne);
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Le X n'est pas valide à la ligne " + ligne);
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Le ID n'est pas valide à la ligne " + ligne);
                        }
                    }
               }
            }
            fichierMessage.Close();
            if (System.IO.File.Exists("Commande_Traitee.txt"))
            {
                try
                {
                    System.IO.File.Delete("Commande_Traitee.txt");
                }
                catch (Exception)
                {
                    Console.WriteLine("Vérifier d'avoir les droits sur le dossier et les fichiers concernant le programme");
                }
               
            }
            System.IO.File.Move("Commande_Mobile.txt", "Commande_Traitee.txt");
            Console.WriteLine("En attente d'un fichier 'Commande_Mobile.txt'");
         }
      }

     
   }
}
