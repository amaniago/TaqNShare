using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CAD;
using System.IO;

namespace WebServiceTaqnshare
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class ServiceTaqnshare : IServiceTaqnshare
    {
        public string GetIdUtilisateur()
        {
            Cad cad = new Cad();

            return cad.GetId();
        }

        public string GetCoucou(byte[] f, string fileName)
        {
            return "coucou";
        }

        public string UploadFile(byte[] f, string fileName)
        {
            // the byte array argument contains the content of the file 
            // the string argument contains the name and extension 
            // of the file passed in the byte array 
            /*try
            {
                // instance a memory stream and pass the 
                // byte array to its constructor 
                MemoryStream ms = new MemoryStream(f);
                // instance a filestream pointing to the 
                // storage folder, use the original file name 
                // to name the resulting file 
                FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath
                            ("~/TransientStorage/") + fileName, FileMode.Create);

                // write the memory stream containing the original 
                // file as a byte array to the filestream 
                ms.WriteTo(fs);
                // clean up 
                ms.Close();
                fs.Close();
                fs.Dispose();
                // return OK if we made it this far 
                return "OK";
            }
            catch (Exception ex)
            {
                // return the error message if the operation fails 
                //return ex.Message.ToString();
                return ("Erreur");
            }*/
            return "coucou";
        }

        public byte[] GetImageFile(string fileName)
        {
            if (System.IO.File.Exists
            (System.Web.Hosting.HostingEnvironment.MapPath
                ("~/TransientStorage/") + fileName))
            {
                return System.IO.File.ReadAllBytes(
                  System.Web.Hosting.HostingEnvironment.MapPath
                    ("~/TransientStorage/") + fileName);
            }
            else
            {
                return new byte[] { 0 };
            }
        }
    }
}
