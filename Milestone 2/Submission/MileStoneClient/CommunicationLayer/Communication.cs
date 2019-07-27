using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace MileStoneClient.CommunicationLayer
{

    public sealed class Communication // singleTon
    { //sealed modifier prevents other classes from inheriting from it

        #region Implement Singleton Design
        private static Communication instance = null;
        private static readonly object padlock = new object();

        //private constructor for singleton
        private Communication()
        {

        }

        public static Communication Instance
        {
            get
            {   //only if there is no instance lock object, otherwise return instance
                if (instance == null)
                {
                    lock (padlock) // senario: n threads in here,
                    {              //locking the first and others going to sleep till the first get new Instance
                        if (instance == null)  // rest n-1 threads no need new instance because its not null anymore.
                        {
                            instance = new Communication();
                        }
                    }
                }
                return instance;
            }
        }


        #endregion

        #region API for Users
        /// <summary>
        /// Send method: send request to server with HttpClient and returned updated guid of currect message
        /// </summary>
        /// <param name="url">url of the server</param>
        /// <param name="msg">CommunicationMessage message content</param>
        /// <returns>Guid from server back to client.</returns>
        public IMessage Send(string url, string gourpID, string nickName, string messageContent)
        {
            return SimpleHTTPClient.SendPostRequest(url, new Request(new Guid(), gourpID, nickName, messageContent, 0, "1"));
        }

        /// <summary>
        /// GetTenMessages method: send request to server with HttpClient and returned list of last ten messages
        /// </summary>
        /// <param name="url">url of the server</param>
        /// <returns>List of last ten CommunicationMessage</returns>
        public List<IMessage> GetTenMessages(string url)
        {
            List<IMessage> retVal = SimpleHTTPClient.GetListRequest(url, "2");
            return retVal;
        }
        #endregion

        #region HttpRequest
        //inner class that represent the Http Client request/response
        private class SimpleHTTPClient
        {

            internal static CommunicationMessage SendPostRequest(string url, Request item)
            {
                JObject jsonItem = JObject.FromObject(item);
                StringContent content = new StringContent(jsonItem.ToString());
                using (var client = new HttpClient())
                {
                    var result = client.PostAsync(url, content).Result;
                    var responseContent = result?.Content?.ReadAsStringAsync().Result;
                    return getMessage(JObject.Parse(responseContent));
                }
            }

            internal static List<IMessage> GetListRequest(string url, string messageType)
            {
                List<IMessage> res = new List<IMessage>();
                JObject jsonItem = new JObject();
                JArray jsonArr = new JArray();
                jsonItem["messageType"] = messageType;
                StringContent content = new StringContent(jsonItem.ToString());
                using (var client = new HttpClient())
                {
                    var result = client.PostAsync(url, content).Result;
                    var responseContent = result?.Content?.ReadAsStringAsync().Result;
                    
                    jsonArr = JArray.Parse(responseContent);
                    for (int i = 0; i < jsonArr.Count; i++)
                    {
                        res.Add(getMessage(jsonArr[i]));
                    }
                    return res;
                }
            }

            private static CommunicationMessage getMessage(JToken jToken)
            {
                return new CommunicationMessage(
                    new Guid(jToken["messageGuid"].ToString()),
                    jToken["groupID"].ToString(),
                    jToken["userName"].ToString(),
                    jToken["messageContent"].ToString(),
                    Convert.ToInt64(jToken["msgDate"])    
                );
            }
            
        }
        #endregion

        #region Private Class 

        /// <summary>
        /// class that represent the Message without anyrequest
        /// </summary>
        private sealed class CommunicationMessage : IMessage
        {
            public Guid Id { get; }
            public string UserName { get; }
            public DateTime Date { get; }
            public string MessageContent { get; }
            public string GroupID { get; }
            public CommunicationMessage(Guid id = new Guid(), string groupId = "", string userName = "", string messageContent = "", long utcTime = 0)
            {
                this.Id = id;
                this.UserName = userName;
                this.Date = TimeFromUnixTimestamp(utcTime);
                this.MessageContent = messageContent;
                this.GroupID = groupId;
            }

            public override string ToString()
            {
                return String.Format("Message ID:{0}\n" +
                    "UserName:{1}\n" +
                    "DateTime:{2}\n" +
                    "MessageContect:{3}\n" +
                    "GroupId:{4}\n"
                    , Id, UserName, Date.ToString(), MessageContent, GroupID);
            }

            private static DateTime TimeFromUnixTimestamp(long unixTimestamp)
            {
                unixTimestamp /= 1000;
                DateTime unixYear0 = new DateTime(1970, 1, 1);
                long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
                DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
                return dtUnix;
            }

        }

        /// <summary>
        /// inner class that represent the request object from server
        /// </summary>
        private class Request
        {
            public Guid messageGuid;
            public string userName;
            public long msgDate;
            public string messageContent;
            public string groupID;
            public string messageType;

            public Request(Guid messageGuid, string groupID, string nickName, string messageContent, long msgDate, string messageType)
            {
                this.messageType = messageType;
                this.messageGuid = messageGuid;
                this.userName = nickName;
                this.msgDate = msgDate;
                this.messageContent = messageContent;
                this.groupID = groupID;
            }

        }

        #endregion
    }
}
