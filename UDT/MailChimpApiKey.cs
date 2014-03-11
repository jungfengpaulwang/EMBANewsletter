using System;
using FISCA.UDT;

namespace Newsletter.UDT
{
    /// <summary>
    /// MailChimp APIKey
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.newsletter.mailchimp_apikey")]
    public class MailChimpAPIKey : ActiveRecord
    {
        /// <summary>
        /// API Key
        /// </summary>
        [Field(Field = "apikey", Indexed = false, Caption = "APIKey")]
        public string APIKey { get; set; }

        /// <summary>
        /// 淺層複製物件
        /// </summary>
        /// <returns></returns>
        public MailChimpAPIKey Clone()
        {
            return this.MemberwiseClone() as MailChimpAPIKey;
        }
    }
}
