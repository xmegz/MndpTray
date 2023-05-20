/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core.Update
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Github API release data.
    /// </summary>
    [DataContract]
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "<Pending>")]
    public class release
    {
        [DataMember]
        public string url { get; set; }

        [DataMember]
        public string assets_url { get; set; }

        [DataMember]
        public string upload_url { get; set; }

        [DataMember]
        public string html_url { get; set; }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string node_id { get; set; }

        [DataMember]
        public string tag_name { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public bool draft { get; set; }

        [DataMember]
        public List<release_asset> assets { get; set; }
    }
}