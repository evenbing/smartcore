﻿using System;
using System.Collections.Generic;
using System.Text;
using COSXML.Network;
using COSXML.Common;
using COSXML.Log;
using COSXML.Utils;
/**
* Copyright (c) 2018 Tencent Cloud. All rights reserved.
* 11/9/2018 4:30:34 PM
* bradyxiao
*/
namespace COSXML.Auth
{

    public delegate void OnGetSign(Request request, string sign);

    /// <summary>
    /// sign caculation
    /// </summary>
    public interface QCloudSigner
    {
        void Sign(Request request, QCloudSignSource qcloudSignSource, QCloudCredentials qcloudCredentials);
    }

    public interface QCloudSignSource
    {
        string Source(Request request);
    }

    public sealed class CosXmlSignSourceProvider : QCloudSignSource
    {
        private HashSet<string> parameterKeys;
        private HashSet<string> headerKeys;
        private string signTime;
        private string headerList;
        private string parameterList;

        private Boolean signAll;

        private String cosHost;

        public OnGetSign onGetSign;

        public CosXmlSignSourceProvider()
        {
            parameterKeys = new HashSet<string>();
            headerKeys = new HashSet<string>();
            this.signAll = true;
        }

        public void setSignAll(Boolean signAll) {
            this.signAll = signAll;
        }

        public void AddParameterKey(string key)
        {
            if (key != null)
            {
                parameterKeys.Add(key);
            }
        }

        public void AddParameterKeys(List<string> keys)
        {
            if (keys != null)
            {
                foreach (string key in keys) {
                    this.parameterKeys.Add(key.ToLower());
                }
            }
        }

        public void AddHeaderKey(string key)
        {
            if(key != null)
            {
                headerKeys.Add(key);
            }
        }

        public void AddHeaderKeys(List<string> keys)
        {
            if (keys != null)
            {
                foreach (string key in keys) {
                    this.headerKeys.Add(key.ToLower());
                }
            }
        }

        public void SetSignTime(string signTime)
        {
            if(signTime != null)
            {
                this.signTime = signTime;
            }
        }

        public void SetSignTime(long signStartTime, long duration)
        {
            this.signTime = String.Format("{0};{1}",signStartTime, signStartTime + duration);
        }

        public string GetSignTime()
        {
            return signTime;
        }

        public string GetHeaderList()
        {
            return headerList;
        }

        public string GetParameterList()
        {
            return parameterList;
        }

        public void setCosHost(string host) {
            cosHost = host;
        }

        public string Source(Request request)
        {
            
            Dictionary<string, string> sourceHeaders = request.Headers;
            Dictionary<string, string> lowerKeySourceHeaders = new Dictionary<string, string>(sourceHeaders.Count);
            foreach (KeyValuePair<string, string> pair in sourceHeaders)
            {
                lowerKeySourceHeaders.Add(pair.Key.ToLower(), pair.Value);
                if (signAll) {
                    headerKeys.Add(pair.Key.ToLower());
                }
            }
            try
            {
                lowerKeySourceHeaders.Add("host", cosHost);
                headerKeys.Add("host");
            }
            catch (Exception)
            {
                
            }
            if (signAll) {
                try
                {
                    long contentLength = 0;
                    if (request.Body != null) {
                        contentLength = request.Body.ContentLength;
                    }
                    if (contentLength > 0) {
                        lowerKeySourceHeaders.Add("content-length", contentLength.ToString());
                        headerKeys.Add("content-length");
                    }
                }
                catch (Exception) {}
            }
            Dictionary<string, string> sourceParameters = request.Url.GetQueryParameters();
            Dictionary<string, string> lowerKeySourceParameters = new Dictionary<string, string>(sourceParameters.Count);
            foreach (KeyValuePair<string, string> pair in sourceParameters)
            {
                lowerKeySourceParameters.Add(pair.Key.ToLower(), pair.Value);
                if (signAll) {
                    parameterKeys.Add(pair.Key.ToLower());
                }
            }
            string path = URLEncodeUtils.Decode(request.Url.Path);
            return GenerateSource(request.Method, path, lowerKeySourceParameters, lowerKeySourceHeaders);
        }

        /// <summary>
        /// $HttpString = [HttpMethod]\n[HttpURI]\n[HttpParameters]\n[HttpHeaders]\n
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="queryParameters"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public string GenerateSource(string method, string path, Dictionary<string, string> queryParameters, Dictionary<string, string> headers)
        {
            StringBuilder formatString = new StringBuilder();
            formatString.Append(method.ToLower()).Append('\n'); // method
            formatString.Append(path).Append('\n'); // path

            //check header and parameter in request
            string headerResult = CheckHeaders(headers);
            string parameterResult = CheckParameters(queryParameters);

            if (parameterResult != null) formatString.Append(parameterResult); // parameters
            formatString.Append('\n');
            if (headerResult != null) formatString.Append(headerResult); // headers
            formatString.Append('\n');
            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append(CosAuthConstants.SHA1).Append('\n');
            stringToSign.Append(signTime).Append('\n');
            stringToSign.Append(DigestUtils.GetSha1ToHexString(formatString.ToString(), Encoding.UTF8)).Append('\n');
            return stringToSign.ToString();
        }

        public string CheckHeaders(Dictionary<string, string> sourceHeaders)
        {
            if (sourceHeaders == null) return null;

            //将指定的headers 小写且排序
            List<String> keys = new List<String>(headerKeys);
            LowerAndSort(keys);

            //计算结果
            string[] result = Calculate(keys, sourceHeaders, true);
            if (result != null)
            {
                headerList = result[1];
                return result[0];
            }
            return null;
        }

        public string CheckParameters(Dictionary<string, string> sourceQueryParameters)
        {
            if (sourceQueryParameters == null) return null;

            //将指定的parameter key 小写 且 排序
            List<String> keys = new List<String>(parameterKeys);
            LowerAndSort(keys);

            //计算结果
            string[] result = Calculate(keys, sourceQueryParameters, false);
            if (result != null)
            {
                parameterList = result[1];
                return result[0];
            }
            return null;
        }

        public string[] Calculate(List<string> keys, Dictionary<string, string> dict, bool isNeedEncode)
        {
            StringBuilder resultBuilder = new StringBuilder();
            StringBuilder keyResultBuilder = new StringBuilder();
            foreach (string key in keys)
            {
                if (!dict.ContainsKey(key)) continue; // 排除一些不可能存在的key
                string value = dict[key];
                if (value != null)
                {
                    if(isNeedEncode)resultBuilder.Append(key).Append('=').Append(URLEncodeUtils.Encode(value)).Append('&');
                    else resultBuilder.Append(key).Append('=').Append(value).Append('&');
                    keyResultBuilder.Append(key).Append(';');
                }
            }
            string result = resultBuilder.ToString();
            string keyResult = keyResultBuilder.ToString();
            if (result.EndsWith("&", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(0, result.Length - 1);
                keyResult = keyResult.Substring(0, keyResult.Length - 1);
                return new string[]{result, keyResult};
            }
            return null;
        }

        /// <summary>
        /// 小写 排序
        /// </summary>
        /// <param name="list"></param>
        public void LowerAndSort(List<string> list)
        {
            if (list != null)
            {
                for (int i = 0, size = list.Count; i < size; i++)
                {
                    list[i] = list[i].ToLower();
                }
                list.Sort(delegate(string strA, string strB)
                {
                   return StringUtils.Compare(strA, strB, false);
                });
            }
        }

    }

    public sealed class CosXmlSigner : QCloudSigner
    {
        public CosXmlSigner() { }

        public void Sign(Request request, QCloudSignSource qcloudSignSource, QCloudCredentials qcloudCredentials)
        {
            if (request == null) throw new ArgumentNullException("Request == null");
            if (qcloudCredentials == null) throw new ArgumentNullException("QCloudCredentials == null");
            if(qcloudSignSource == null || !(qcloudSignSource is CosXmlSignSourceProvider)) throw new ArgumentNullException("CosXmlSourceProvider == null");
            CosXmlSignSourceProvider cosXmlSourceProvider = (CosXmlSignSourceProvider)qcloudSignSource;
           
            string signTime = cosXmlSourceProvider.GetSignTime();
            if (signTime == null)
            {
                signTime = qcloudCredentials.KeyTime;
                cosXmlSourceProvider.SetSignTime(signTime);
            } 
            string signature = DigestUtils.GetHamcSha1ToHexString(cosXmlSourceProvider.Source(request), Encoding.UTF8, qcloudCredentials.SignKey, Encoding.UTF8);
            StringBuilder signBuilder = new StringBuilder();
            
            signBuilder.Append(CosAuthConstants.Q_SIGN_ALGORITHM).Append('=').Append(CosAuthConstants.SHA1).Append('&')
                .Append(CosAuthConstants.Q_AK).Append('=').Append(qcloudCredentials.SecretId).Append('&')
                .Append(CosAuthConstants.Q_SIGN_TIME).Append('=').Append(signTime).Append('&')
                .Append(CosAuthConstants.Q_KEY_TIME).Append('=').Append(qcloudCredentials.KeyTime).Append('&')
                .Append(CosAuthConstants.Q_HEADER_LIST).Append('=').Append(cosXmlSourceProvider.GetHeaderList()).Append('&')
                .Append(CosAuthConstants.Q_URL_PARAM_LIST).Append('=').Append(cosXmlSourceProvider.GetParameterList()).Append('&')
                .Append(CosAuthConstants.Q_SIGNATURE).Append('=').Append(signature);
            string sign = signBuilder.ToString();
            request.AddHeader(CosRequestHeaderKey.AUTHORIZAIION, sign);
            if(qcloudCredentials is SessionQCloudCredentials)
            {
                request.AddHeader(CosRequestHeaderKey.COS_SESSION_TOKEN,((SessionQCloudCredentials)qcloudCredentials).Token);
            }
            if (cosXmlSourceProvider.onGetSign != null)
            {
                cosXmlSourceProvider.onGetSign(request, sign); 
            }
        }

        public static string GenerateSign(string method, string path, Dictionary<string, string> queryParameters, Dictionary<string, string> headers, 
            string signTime, QCloudCredentials qcloudCredentials)
        {
            if (qcloudCredentials == null) throw new ArgumentNullException("QCloudCredentials == null");
            CosXmlSignSourceProvider cosXmlSourceProvider = new CosXmlSignSourceProvider();
            if (signTime == null) signTime = qcloudCredentials.KeyTime;
            cosXmlSourceProvider.SetSignTime(signTime);
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    cosXmlSourceProvider.AddHeaderKey(key);
                }
            }
            if (queryParameters != null)
            {
                foreach (string key in queryParameters.Keys)
                {
                    cosXmlSourceProvider.AddParameterKey(key);
                }
            }
            string signature = DigestUtils.GetHamcSha1ToHexString(cosXmlSourceProvider.GenerateSource(method, path, queryParameters, headers), Encoding.UTF8, 
                qcloudCredentials.SignKey, Encoding.UTF8);

            StringBuilder signBuilder = new StringBuilder();
            signBuilder.Append(CosAuthConstants.Q_SIGN_ALGORITHM).Append('=').Append(CosAuthConstants.SHA1).Append('&')
                .Append(CosAuthConstants.Q_AK).Append('=').Append(qcloudCredentials.SecretId).Append('&')
                .Append(CosAuthConstants.Q_SIGN_TIME).Append('=').Append(cosXmlSourceProvider.GetSignTime()).Append('&')
                .Append(CosAuthConstants.Q_KEY_TIME).Append('=').Append(qcloudCredentials.KeyTime).Append('&')
                .Append(CosAuthConstants.Q_HEADER_LIST).Append('=').Append(cosXmlSourceProvider.GetHeaderList()).Append('&')
                .Append(CosAuthConstants.Q_URL_PARAM_LIST).Append('=').Append(cosXmlSourceProvider.GetParameterList()).Append('&')
                .Append(CosAuthConstants.Q_SIGNATURE).Append('=').Append(signature);

            if (qcloudCredentials is SessionQCloudCredentials)
            {
                signBuilder.Append("&").Append(CosRequestHeaderKey.COS_SESSION_TOKEN).Append("=").Append(((SessionQCloudCredentials)qcloudCredentials).Token);
            }
            return signBuilder.ToString();
        }
    }

    public sealed class CosAuthConstants
    {
        public const string Q_SIGN_ALGORITHM = "q-sign-algorithm";

        public const string Q_AK = "q-ak";

        public const string Q_SIGN_TIME = "q-sign-time";

        public const string Q_KEY_TIME = "q-key-time";

        public const string Q_HEADER_LIST = "q-header-list";

        public const string Q_URL_PARAM_LIST = "q-url-param-list";

        public const string Q_SIGNATURE = "q-signature";

        public const string SHA1 = "sha1";
    }
}