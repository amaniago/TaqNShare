using System;
using System.IO;
using System.ServiceModel;

namespace ServeurTaqnshare
{
    [ServiceContract(Name = "ServiceTaqnshare")]
    public interface IServiceTaqnshare
    {
        [OperationContract]
        string UploadFile(byte[] f, string fileName);

        [OperationContract]
        byte[] GetImageFile(string fileName);
    }

    public class ServiceTaqnshare : IServiceTaqnshare
    {

        public string UploadFile(byte[] f, string fileName)
        {
            try
            {
                MemoryStream ms = new MemoryStream(f);
                //FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName, FileMode.Create);
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + fileName, FileMode.Create);


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
            //return Directory.GetCurrentDirectory();
        }

        public byte[] GetImageFile(string fileName)
        {
            //return System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName);

            return System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + fileName);


        }
    }
}

