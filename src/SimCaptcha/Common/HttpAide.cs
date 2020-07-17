using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Common
{
    /// <summary>
    /// HTTP助手
    /// </summary>
    public class HttpAide
    {
        #region Http Get
        /// <summary>
        /// HTTP Get请求
        /// </summary>
        /// <param name="url">请求目标URL</param>
        /// <param name="isPost"></param>
        /// <param name="referer"></param>
        /// <param name="cookies"></param>
        /// <param name="ua"></param>
        /// <returns>返回请求回复字符串</returns>
        public static string HttpGet(string url, StringBuilder responseHeadersSb = null, string[] headers = null, WebProxy proxy = null)
        {
            string rtResult = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = false;

                if (headers != null)
                {
                    foreach (string header in headers)
                    {
                        string[] temp = header.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp[0].Equals("Referer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Referer = temp[1];
                        }
                        else if (temp[0].Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.UserAgent = temp[1];
                        }
                        else if (temp[0].Equals("Accept", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Accept = temp[1];
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase) && temp[1].Equals("keep-alive", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = false;
                        }
                        else if (temp[0].Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.ContentType = temp[1];
                        }
                        else
                        {
                            request.Headers.Add(header);
                        }
                    }
                }
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Timeout = 10000;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (responseHeadersSb != null)
                {
                    foreach (string name in response.Headers.AllKeys)
                    {
                        responseHeadersSb.AppendLine(name + ": " + response.Headers[name]);
                    }
                }
                Stream responseStream = response.GetResponseStream();
                //如果http头中接受gzip的话，这里就要判断是否为有压缩，有的话，直接解压缩即可 
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    rtResult = sReader.ReadToEnd();
                }
                responseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtResult;
        }
        #endregion

        #region Http Post
        public static string HttpPost(string url, string postDataStr = "", StringBuilder responseHeadersSb = null, string[] headers = null, WebProxy proxy = null)
        {
            string rtResult = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.KeepAlive = false;
                if (headers != null)
                {
                    foreach (string header in headers)
                    {
                        string[] temp = header.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp[0].Equals("Referer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Referer = temp[1];
                        }
                        else if (temp[0].Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.UserAgent = temp[1];
                        }
                        else if (temp[0].Equals("Accept", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Accept = temp[1];
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase) && temp[1].Equals("keep-alive", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = false;
                        }
                        else if (temp[0].Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.ContentType = temp[1];
                        }
                        else
                        {
                            request.Headers.Add(header);
                        }
                    }
                }
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Timeout = 10000;
                byte[] postBytes = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = postBytes.Length;
                // 写 content-body 一定要在属性设置之后
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (responseHeadersSb != null)
                {
                    foreach (string name in response.Headers.AllKeys)
                    {
                        responseHeadersSb.AppendLine(name + ": " + response.Headers[name]);
                    }
                }
                Stream responseStream = response.GetResponseStream();
                //如果http头中接受gzip的话，这里就要判断是否为有压缩，有的话，直接解压缩即可  
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    rtResult = sReader.ReadToEnd();
                }
                responseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtResult;
        }
        #endregion
    }
}
