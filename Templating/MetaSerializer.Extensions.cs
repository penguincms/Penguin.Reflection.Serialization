using System.IO;
using System.IO.Compression;
using System.Text;

namespace Penguin.Reflection.Serialization.Templating
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static partial class MetaSerializer
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region Methods

        internal static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        internal static string Unzip(byte[] bytes)
        {
            using (MemoryStream msi = new MemoryStream(bytes))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        internal static byte[] Zip(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            using (MemoryStream msi = new MemoryStream(bytes))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        #endregion Methods
    }
}