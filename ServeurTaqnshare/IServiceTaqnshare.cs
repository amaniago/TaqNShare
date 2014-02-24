using System;
using System.IO;
using System.ServiceModel;

namespace ServeurTaqnshare
{
    [ServiceContract(Name = "ServiceTaqnshare")]
    public interface IServiceTaqnshare
    {
        [OperationContract]
        string EnvoyerImage(byte[] imageByte, string nomImage);

        [OperationContract]
        byte[] RecupererImage(string nomImage);
    }

    public class ServiceTaqnshare : IServiceTaqnshare
    {
        public string EnvoyerImage(byte[] imageByte, string nomImage)
        {
            try
            {
                MemoryStream ms = new MemoryStream(imageByte);
                //FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName, FileMode.Create);
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + nomImage, FileMode.Create);

                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                return "OK";
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
        }

        public byte[] RecupererImage(string nomImage)
        {
            //return File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName);
            return File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + nomImage);
        }
    }
}

