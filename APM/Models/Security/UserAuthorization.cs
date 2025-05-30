using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace APM.Models.Security
{
    public class UserAuthorization
    {
        public static bool CanUserVisit()
        {
            //Referral.UserAccount.GetUserNotification();
            if(Referral.CoreObjects!=null)
                return true;
            return false;
        }

        public static bool CanSettingVisit()
        {
            return (CanUserVisit() && Referral.UserAccount.IsAdmin);
        }

        public static bool CanFormVisit(int _CoreObjectID)
        {
            return (CanUserVisit() && CoreObject.Find(_CoreObjectID) != null && CoreObject.Find(_CoreObjectID).Permission(Referral.UserAccount.Permition).IsAllow);
        }

        public static string ExclusionURL()
        {
            return "~/Signin";
        }
    }

    public static class Codec
    {
        public static string Base64Encoding(string toEncode)
        {
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(toEncode);
            string toReturn = System.Convert.ToBase64String(bytes);
            return toReturn;
        }

        public static string Base64Decoding(string toDecode)
        {
            string base64Decoded;
            byte[] data = System.Convert.FromBase64String(toDecode);
            base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);
            return base64Decoded;
        }
    }

    public struct reqCodec
    {
        public string RecordID, InnerID;

        public reqCodec(string req)
        {
            RecordID = "0";
            InnerID = "0";

            if (req != null)
            {
                string DecodeLayer = Codec.Base64Decoding(req);
                string[] Splited = DecodeLayer.Split(new[] { ".", ":" }, StringSplitOptions.None);
                //RecordID = Convertor.ToDecrypt(Splited[1], ProjectInfo.User.CoreObjectID.ToString());
                InnerID = Splited[3];
            }
        }
    }
}