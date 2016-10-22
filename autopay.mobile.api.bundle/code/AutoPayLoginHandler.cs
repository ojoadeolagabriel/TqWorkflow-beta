using app.core.nerve.facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.nerve.dto;
using Newtonsoft.Json;
using autopay.mobile.api.bundle.code.dto;
using System.IO;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;

namespace autopay.mobile.api.bundle.code
{
    public class AutoPayLoginHandler : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var rawMsg = exchange.InMessage.Body.ToString();
            var data = JsonConvert.DeserializeObject<UserLogin>(rawMsg);

            var rsp = JsonConvert.SerializeObject(new UserLogin.UserLoginStatus { ResponseCode = "90000", ResponseMessage = "Success", });
            exchange.InMessage.Body = GetBankSummary();
            base.Process(exchange);
        }

        public string GetBankSummary()
        {
            var summary = new BankSummary
            {
                BankHealthInformation = new List<BankSummary.Bank>
                {
                    new BankSummary.Bank { LastResponseCodes = { "AX0", "00" ,"AX0","00" }, IsProcessorEnabled = true, BankDescription = "Unity Bank", BankId = "10", BankName = "First Bank of Nigeria", LastEventDescription = "90000"  },
                    new BankSummary.Bank { LastResponseCodes = { "00", "00" }, IsProcessorEnabled = true, BankDescription = "FCMB", BankId = "56", BankName = "First Bank of Nigeria", LastEventDescription = "90000"  },
                    new BankSummary.Bank { LastResponseCodes = { "91", "00" }, IsProcessorEnabled = true, BankDescription = "Sterling Bank", BankId = "60", BankName = "First Bank of Nigeria", LastEventDescription = "90000"  },
                    new BankSummary.Bank { LastResponseCodes = { "91", "54", "AX0","00" }, IsProcessorEnabled = true, BankDescription = "First Bank", BankId = "10", BankName = "First Bank of Nigeria", LastEventDescription = "90000"  },
                    new BankSummary.Bank { BankDescription = "Wema Bank", BankId = "16", BankName = "Wema Bank", LastEventDescription = "90000"  },
                    new BankSummary.Bank { BankDescription = "GTB Bank", BankId = "12", BankName = "Guarantey Trust Bank of Nigeria", LastEventDescription = "90000"  },
                     new BankSummary.Bank { LastResponseCodes = { "91", "54" }, BankDescription = "Ecobank", BankId = "49", BankName = "Eco Bank of Nigeria", LastEventDescription = "90000"  },
                    new BankSummary.Bank { BankDescription = "Union Bank", BankId = "13", BankName = "First Bank of Nigeria", LastEventDescription = "90000"  }
                }
            };

            return JsonConvert.SerializeObject(summary);
        }

        private static void EncryptFile(Stream outputStream, string fileName, PgpPublicKey encKey, bool armor, bool withIntegrityCheck)
        {

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            try
            {
                MemoryStream bOut = new MemoryStream();
                PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(
                comData.Open(bOut),
                PgpLiteralData.Binary,
                new FileInfo(fileName));
                comData.Close();
                PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
                SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
                cPk.AddMethod(encKey);
                byte[] bytes = bOut.ToArray();
                Stream cOut = cPk.Open(outputStream, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
                cOut.Close();
                if (armor)
                    outputStream.Close();
            }

            catch (PgpException e)
            {

                Console.Error.WriteLine(e);
                Exception underlyingException = e.InnerException;
                if (underlyingException != null)
                {

                    Console.Error.WriteLine(underlyingException.Message);
                    Console.Error.WriteLine(underlyingException.StackTrace);

                }
            }
        }

        public void EncryptFile(string filePath, string publicKeyFile, string pathToSaveFile)
        {
            Stream keyIn, fos;
            keyIn = File.OpenRead(publicKeyFile);
            string[] fileSplit = filePath.Split('\\');
            string fileName = fileSplit[fileSplit.Length - 1];
            fos = File.Create(pathToSaveFile + fileName + ".asc");
            EncryptFile(fos, filePath, ReadPublicKey(keyIn), true, true);
            keyIn.Close();
            fos.Close();
        }

        private PgpPublicKey ReadPublicKey(Stream keyIn)
        {
            throw new NotImplementedException();
        }
    }
}
