using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace app.core.nerve.facade
{
    public class CamelValidator
    {
        public class CamelValidatorStatus
        {
            public bool IsValid { get; set; }
            public ValidationStatus Status { get; set; }
            public string Message { get; set; }

            public enum ValidationStatus
            {
                Good,BadXml,InValidNamespace,BadSegment,UnKnown
            }
        }

        public static CamelValidatorStatus IsCamelXmlValid(XElement xml)
        {
            return IsCamelXmlValid(xml.ToString());
        }

        public static CamelValidatorStatus IsCamelXmlValid(String xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml))
                    return new CamelValidatorStatus { IsValid = false, Message = "Failed-CamelNotFound", Status = CamelValidatorStatus.ValidationStatus.BadSegment };

                XElement.Parse(xml);
                return new CamelValidatorStatus { IsValid = true, Message = "Success", Status = CamelValidatorStatus.ValidationStatus.Good };
            }
            catch(Exception ex)
            {
                return new CamelValidatorStatus { IsValid = false, Message = ex.Message, Status = CamelValidatorStatus.ValidationStatus.BadXml };
            }
        }
    }
}
