namespace MndpTray.Update
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// Update methods.
    /// </summary>
    public static class Methods
    {
        #region Public

        /// <summary>
        /// Download binary content from web.
        /// </summary>
        /// <param name="url">target url.</param>
        /// <returns>target content.</returns>
        public static byte[] DownloadBinary(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.UserAgent = "download";

            MemoryStream ms = new MemoryStream();
            WebResponse response = webRequest.GetResponse();

            using (Stream stream = response.GetResponseStream())
            {
                stream.CopyTo(ms);
            }

            return ms.GetBuffer();
        }

        /// <summary>
        /// Get next version download url if available.
        /// </summary>
        /// <param name="author">Github author.</param>
        /// <param name="repositoryName">Github repository name.</param>
        /// <param name="assetName">Github asset name.</param>
        /// <param name="curVersion">Current assembly version.</param>
        /// <returns>Next release download url is available, else null.</returns>
        public static string GetNextVersionDownloadUrl(string author, string repositoryName, string assetName, Version curVersion)
        {
            if (assetName == null)
            {
                assetName = repositoryName;
            }

            string releaseStr = GetReleasesFromApi(author, repositoryName);

            List<release> list = DeserializeResponse<List<release>>(releaseStr);

            release max = GetMaxRelease(list);

            if (ReleaseIsNewer(max, curVersion))
            {
                return GetReleaseDownloadUrl(max, assetName);
            }

            return null;
        }

        /// <summary>
        /// Update method to running programs.
        /// </summary>
        /// <param name="fileName">Program file name.</param>
        /// <param name="data">New program file content.</param>
        public static void UpdateProgram(string fileName, byte[] data)
        {
            string curName = fileName;
            string oldName = fileName + ".old";
            string newName = fileName + ".new";

            if (File.Exists(newName))
            {
                File.Delete(newName);
            }

            if (File.Exists(oldName))
            {
                File.Delete(oldName);
            }

            File.WriteAllBytes(newName, data);
            File.Move(curName, oldName);
            File.Move(newName, curName);
        }

        #endregion

        #region Private

        private static T DeserializeResponse<T>(string data)
        {
            var instance = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(data)))
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        private static release GetMaxRelease(List<release> list)
        {
            if (list == null)
            {
                return null;
            }

            release ret = null;

            foreach (var j in list)
            {
                if (ret == null)
                {
                    ret = j;
                }
                else
                {
                    var max = GetReleaseVersion(ret);
                    var cur = GetReleaseVersion(j);

                    for (int i = 0; (i < max.Count) && (i < cur.Count); i++)
                    {
                        if (cur[i] < max[i])
                        {
                            break;
                        }

                        if (cur[i] > max[i])
                        {
                            ret = j;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        private static string GetReleaseDownloadUrl(release data, string assetName)
        {
            if (data == null)
            {
                return null;
            }

            if (data.assets == null)
            {
                return null;
            }

            foreach (var i in data.assets)
            {
                if (i.content_type == "application/x-msdownload")
                {
                    if (i.name.StartsWith(assetName))
                    {
                        return i.browser_download_url;
                    }
                }
            }

            return null;
        }

        private static string GetReleasesFromApi(string author, string repositoryName)
        {
            string url = "https://api.github.com/repos/" + author + "/" + repositoryName + "/releases";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.UserAgent = repositoryName;
            webRequest.ServicePoint.Expect100Continue = true;

            string data = null;
            using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                data = responseReader.ReadToEnd();
            }

            return data;
        }

        private static List<int> GetReleaseVersion(release data)
        {
            List<int> ret = new List<int>();

            if (data == null)
            {
                return ret;
            }

            if (data.tag_name == null)
            {
                return ret;
            }

            string[] parts = data.tag_name.ToLower().TrimStart('v').Split('.');

            foreach (var i in parts)
            {
                if (!int.TryParse(i, out int j))
                {
                    break;
                }

                ret.Add(j);
            }

            return ret;
        }

        private static bool ReleaseIsNewer(release data, Version curVersion)
        {
            if (data == null)
            {
                return false;
            }

            var nextVersion = GetReleaseVersion(data);

            if (nextVersion.Count < 3)
            {
                return false;
            }

            if (nextVersion[0] > curVersion.Major)
            {
                return true;
            }

            if (nextVersion[0] == curVersion.Major)
            {
                if (nextVersion[1] > curVersion.Minor)
                {
                    return true;
                }

                if (nextVersion[1] == curVersion.Minor)
                {
                    if (nextVersion[2] > curVersion.Build)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}