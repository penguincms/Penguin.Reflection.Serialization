using Penguin.Reflection.Serialization.Constructors;
using Penguin.Reflection.Serialization.Objects;
using Penguin.Reflection.Serialization.Templating;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Penguin.Reflection.Serialization
{
    /// <summary>
    /// Methods for constructing and serializing objects in a Zipped format
    /// </summary>
    public static class GZip
    {
        #region Methods

        /// <summary>
        /// Creates a serialized and gzipped version of a MetaObject
        /// </summary>
        /// <param name="o">The object to serialize and zip</param>
        /// <param name="c">The optional constructor to use</param>
        /// <returns>A byte[] containing the gzipped string from the serialized object</returns>
        public static byte[] MetaZip(object o, MetaConstructor c = null)
        {
            if (o is null)
            {
                return null;
            }

            c = c ?? new MetaConstructor();

            StringBuilder target = new StringBuilder();

            new MetaObject(o, c).Serialize(target);

            return Zip(target.ToString());
        }

        /// <summary>
        /// Unzips a gziped byte[] to its original string
        /// </summary>
        /// <param name="bytes">The byte[] to unzip</param>
        /// <returns>The original string</returns>
        public static string Unzip(byte[] bytes)
        {
            if (bytes is null)
            {
                return null;
            }

            using (MemoryStream msi = new MemoryStream(bytes))
            {
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
        }

        internal static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        internal static byte[] Zip(string str)
        {
            if (str is null)
            {
                return null;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            using (MemoryStream msi = new MemoryStream(bytes))
            {
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
        }

        #endregion Methods
    }
}