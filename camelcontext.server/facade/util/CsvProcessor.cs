using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using app.core.utility;

namespace camelcontext.server.facade.util
{
    public class CsvProcessor
    {
        public class ActivityLog : Blitz<ActivityLog, int>
        {
            public string Body { get; set; }
            public string ExchangeId { get; set; }
        }

        public static void ProcessDb()
        {
            var logger = ActivityLog.Init("AppLogDb");
            var result = logger.ExecuteUniqueSp("usp_FetchLog", new List<SqlParameter> { new SqlParameter("@id", 1) });
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ProcessCsv()
        {
            const string csvFile = @"C:\Users\Adeola Ojo\Desktop\TestData\testcsv.csv";
            const string rootFolder = @"C:\Users\Adeola Ojo\Desktop\TestData\";

            if (!File.Exists(csvFile))
                return;

            var data = File.ReadAllText(csvFile);

            var now = DateTime.Now;
            var csvDao = CsvDao.ParseFromString(data);
            var span = DateTime.Now - now;

            //file name
            var originalFileNameWtExt = Path.GetFileName(csvFile);

            //verify total rows
            var isRowCountGood = csvDao.HasMinimumRowCount(3);
            if (!isRowCountGood)
            {
                csvDao.MarkRowAsBad(BatchHeaderField.BatchRowId, "BF0001");
                csvDao.MarkMultipleRowsAsBad(1);

                var export = csvDao.BadCsvRawData;
                CsvDao.SafeFileWrite(rootFolder, @"SFX\ERROR", originalFileNameWtExt, export);
            }

            //read first row
            string respCode;
            var isBatchRowGood = csvDao.IsRowValid(IsBatchRowGood, BatchHeaderField.BatchRowId, out respCode);

            //if batch header is not good
            if (!isBatchRowGood)
            {
                csvDao.MarkRowAsBad(BatchHeaderField.BatchRowId, respCode);
                var export = csvDao.BadCsvRawData;
                CsvDao.SafeFileWrite(rootFolder, @"SFX\ERROR", originalFileNameWtExt, export);
                return;
            }

            var batchRow = csvDao.FetchSingle(BatchHeaderField.BatchRowId);
            ProcessBatch(batchRow, out respCode);

            //is secure good?
            var isSecureRowGood = csvDao.IsRowValid(IsSecureRowGood, SecureHeaderField.SecureRowId, out respCode);
            if (!isSecureRowGood)
            {
                csvDao.MarkRowAsBad(BatchHeaderField.BatchRowId, "90000");
                csvDao.MarkRowAsBad(SecureHeaderField.SecureRowId, respCode);
                csvDao.MarkMultipleRowsAsBad(2);

                var export = csvDao.BadCsvRawData;
                CsvDao.SafeFileWrite(rootFolder, @"SFX\ERROR", originalFileNameWtExt, export);
                return;
            }
             
            var secureRow = csvDao.FetchSingle(SecureHeaderField.SecureRowId);

            //get payment rows
            var paymentRows = csvDao.FetchAll(2);
            for (var rowIndex = 0; rowIndex <= paymentRows.Count - 1; rowIndex++)
            {
                var currentPaymentRow = paymentRows[rowIndex];
                if (csvDao.IsRowValid(IsPaymentRowGood, rowIndex, out respCode))
                {
                    string errorCode;

                    //if payment successfully created
                    if (BuildPayment(currentPaymentRow, out errorCode))
                    {
                        CsvDao.MarkSelectedRowRecordAsGood(currentPaymentRow, csvDao, csvDaoFunc: (dao) =>
                        {
                            if (csvDao.AnyGoodRow)
                                return;
                            csvDao.MarkRowAsGood(0, "90000");
                        });
                    }
                    else
                    {
                        CsvDao.MarkSelectedRowRecordAsBad(currentPaymentRow, csvDao, csvDaoFunc: (dao) =>
                        {
                            if (csvDao.AnyBadRow)
                                return;
                            csvDao.MarkRowAsBad(BatchHeaderField.BatchRowId, "90000");
                            csvDao.MarkRowAsBad(SecureHeaderField.SecureRowId);
                        }, optionalColumnData: errorCode);
                    }
                }
                else
                {
                    CsvDao.MarkSelectedRowRecordAsBad(currentPaymentRow, csvDao, respCode);
                }
            }

            if (csvDao.AnyBadRow)
            {
                var csv = csvDao.BadCsvRawData;
                CsvDao.SafeFileWrite(rootFolder, @"SFX\ERROR", originalFileNameWtExt, csv);
            }

            if (csvDao.AnyGoodRow)
            {
                var csv = csvDao.GoodCsvRawData;
                CsvDao.SafeFileWrite(rootFolder, @"SFX\OUT", originalFileNameWtExt, csv);
            }

            //allways backup processed files
            CsvDao.SafeFileWrite(rootFolder, @"SFX\BACKUP", originalFileNameWtExt, csvDao.OriginalCsvData);
        }

        public class BatchHeaderField
        {
            public const string BatchReference = "batch-ref";
            public const string BatchDescription = "batch-description";
            public const string TerminalId = "terminal-id";
            public const string ResponseCode = "response-code";
            public const string ResponseDescription = "response-desc";

            public const int BatchReferenceIndex = 0;
            public const int BatchDescriptionIndex = 1;
            public const int TerminalIdIndex = 2;
            public const int ResponseCodeIndex = 3;
            public const int ResponseDescriptionIndex = 4;

            public const int BatchRowId = 0;

            public override string ToString()
            {
                return string.Format("{0},{1},{2},{3},{4}", BatchReference, BatchDescription, TerminalId, ResponseCode, ResponseDescription);
            }
        }

        public class SecureHeaderField
        {
            public const string SecureData = "secure";
            public const string BankCode = "bank";
            public const string ToAccount = "toaccount";
            public const string EncryptedData = "encpin";
            public const string Mac = "mac";

            public const int SecureRowId = 1;

            public override string ToString()
            {
                return string.Format("{0},{1},{2},{3},{4}", SecureData, BankCode, ToAccount, EncryptedData, Mac);
            }
        }

        public class PaymentDataField
        {
            public const int BeneficiaryCode = 0;
            public const int AmountRequested = 1;
            public const int Narration = 2;
            public const int Email = 3;
            public const int IsPrepaid = 4;
            public const int ToAccount = 5;
            public const int ToAccountType = 6;
            public const int BeneficiaryName = 7;
            public const int CurrencyCode = 8;
            public const int PhoneNo = 9;
        }

        private static bool ProcessBatch(List<string> batchRow, out string errorCode)
        {
            try
            {
                errorCode = "90000";
                var batchReference = CsvDao.GetColumnData<string>(BatchHeaderField.BatchReferenceIndex, batchRow, throwOnError: true, errorMessage: "AX0005");
                var batchDesc = CsvDao.GetColumnData<string>(BatchHeaderField.BatchDescriptionIndex, batchRow, throwOnError: true, errorMessage: "AX0005");
                var batchTerminalId = CsvDao.GetColumnData<string>(BatchHeaderField.TerminalIdIndex, batchRow, throwOnError: true, errorMessage: "AX0005");
                return true;
            }
            catch (Exception exception)
            {
                errorCode = exception.Message;
                return false;
            }
        }

        private static bool BuildPayment(List<string> currentPaymentRow, out string errorCode)
        {
            try
            {
                errorCode = "90000";
                var beneficiaryCode = CsvDao.GetColumnData<string>(PaymentDataField.BeneficiaryCode, currentPaymentRow);
                var amount = CsvDao.GetColumnData<int>(PaymentDataField.AmountRequested, currentPaymentRow, throwOnError: true, errorMessage: "AX0004");
                var email = CsvDao.GetColumnData<string>(PaymentDataField.Email, currentPaymentRow);
                var isPrepaid = CsvDao.GetColumnData<bool>(PaymentDataField.IsPrepaid, currentPaymentRow);
                var narration = CsvDao.GetColumnData<string>(PaymentDataField.Narration, currentPaymentRow);
                var phono = CsvDao.GetColumnData<string>(PaymentDataField.PhoneNo, currentPaymentRow);
                var toAccount = CsvDao.GetColumnData<string>(PaymentDataField.ToAccount, currentPaymentRow);
                var toAccountType = CsvDao.GetColumnData<string>(PaymentDataField.ToAccountType, currentPaymentRow);
                var currencyCode = CsvDao.GetColumnData<string>(PaymentDataField.CurrencyCode, currentPaymentRow);
                return true;
            }
            catch (Exception exception)
            {
                errorCode = exception.Message;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool IsSecureRowGood(List<string> arg)
        {
            var secureData = CsvDao.GetColumnData<string>(0, arg, throwOnColumnNotSet: true, errorMessage: "AX0006");
            return true;
        }

        private static bool IsPaymentRowGood(List<string> arg)
        {
            //throw new Exception("AX0003");
            return true;
        }

        private static bool IsBatchRowGood(List<string> arg)
        {
            //throw new Exception("AX0001");
            return true;
        }
    }
}
