/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core.Update
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Github API release asset data.
    /// </summary>
    [DataContract]
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "<Pending>")]
    public class release_asset
    {
        [DataMember]
        public string url { get; set; }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string node_id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string state { get; set; }

        [DataMember]
        public string content_type { get; set; }

        [DataMember]
        public int size { get; set; }

        [DataMember]
        public string browser_download_url { get; set; }
    }
}