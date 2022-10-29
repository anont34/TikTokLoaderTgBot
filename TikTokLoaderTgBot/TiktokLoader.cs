using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TikTokLoaderTgBot
{
    public class TiktokLoader
    {
        private string _userAgent = "User-Agent: Mozilla/5.0 (X11; Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0";
        private Regex _regexSearchFullDownloadLink = new Regex(@"https:\/\/[\w.-]+\/[a-z0-9]{32}.*?(\/\?a=)");

        public string URLforDownloadVideo { get; set; } = "[Empty. No changed.]";
        public string Error { get; set; }

        /// <summary>
        /// Checks and gets a link to download the video (clicks on links if necessary)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetURLforDownload(string url)
        {
            // type of links
            // vt.tiktok.com/*********/ - 9 characters - need open link and get next - find https/ link to download

            string response = "";

            using (var webClient = new System.Net.WebClient())
            {
                webClient.Headers.Add(System.Net.HttpRequestHeader.UserAgent, _userAgent);
                
                try
                {
                    response = webClient.DownloadString(url);
                }
                catch (WebException e)
                {
                    Debug.WriteLine(e.Message);
                    Error = e.Message.ToString();
                    return null;
                }

                try
                {
                    response = System.Text.RegularExpressions.Regex.Unescape(response);
                }
                catch (Exception e)
                {
                    if (e.Message.Length > 1000)
                        Error = "I can't find the download link on this page. (perhaps you dropped the link profile) Error message is to long. Sorry. #1";
                    else
                        Error = "I can't find the download link on this page. (perhaps you dropped the link profile) Error #2" + 
                        e.Message.ToString();
                    return null;
                }

                MatchCollection matchedDownloadVideoLink = _regexSearchFullDownloadLink.Matches(response);

                if(matchedDownloadVideoLink.Count == 0)
                {
                    Error = "I can't find the download link on this page. (perhaps you dropped the link profile) Error #2";
                    return null;
                }

                response = matchedDownloadVideoLink[0].Value; 

                response = response.Remove(response.Length - 3); // becouse RegEx +3 symbol like pattern
            }

            return response;
        }

        public Stream DownloadFile(string urlToVideo)
        {
            URLforDownloadVideo = GetURLforDownload(urlToVideo);

            if (URLforDownloadVideo == null)
                return null;

            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.Headers.Add(System.Net.HttpRequestHeader.UserAgent, _userAgent);
                    byte[] data = webClient.DownloadData(URLforDownloadVideo);

                    // Telegram bots limits: send video no more than 50MB

                    if (data.Length > 50000000)
                    {
                        Error = "Tiktok video weighs more than 50 megabytes," +
                                "according to the telegram rules, the bot cannot send such large videos." +
                                "\n\nYou can download it yourself, at the temporary link:\n" + URLforDownloadVideo;
                        return null;
                    }

                    var ms = new System.IO.MemoryStream(data);
                    return ms;
                }
            }
            catch (WebException e)
            {
                Error = e.Message.ToString();
                return null;
            }

            

        }





    }
}
