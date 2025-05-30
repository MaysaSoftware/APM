

using Microsoft.Extensions.DependencyInjection;
using TaxCollectData.Library.Abstraction;
using TaxCollectData.Library.Business;
using TaxCollectData.Library.Dto.Config;
using TaxCollectData.Library.Dto.Content;
using TaxCollectData.Library.Dto.Properties;
using TaxCollectData.Library.Enums;
using TaxCollectData.Library.Extensions;

namespace TaxApi
{

    internal class TaxService
    { 
        private string MEMORY_ID { get; set; }
        private string PRIVATE_KEY { get; set; }
        private string Url { get; set; }
        private InvoiceHeaderDto _InvoiceHeaderDto { get; set; }
        private List<InvoiceBodyDto> _InvoiceBody { get; set; }

        private readonly ITaxApis _api;

        private readonly SampleInvoiceCreator _sampleInvoiceCreator = new();

        private string uid;

        private string referenceNumber;
        private SQLDataBase DataBase { get; set; }
        private long RowID { get; set; }
        private string TableName { get; set; }

        public TaxService(string MEMORY_ID,
                          string PRIVATE_KEY,
                          string Url,
                          InvoiceHeaderDto invoiceHeaderDto,
                          List<InvoiceBodyDto> invoiceBody,
                          SQLDataBase DataBase,
                          long RowID,
                          string TableName)
        { 
            TaxApiService.Instance.Init(MEMORY_ID,
                new SignatoryConfig(PRIVATE_KEY, null), 
                new NormalProperties(ClientType.SELF_TSP), Url);
            _api = TaxApiService.Instance.TaxApis;
            this.MEMORY_ID=MEMORY_ID;
            this.PRIVATE_KEY=PRIVATE_KEY;
            this.Url=Url;    
            this._InvoiceHeaderDto = invoiceHeaderDto;
            this._InvoiceBody=invoiceBody;
            this.DataBase = DataBase;
            this.RowID = RowID;
            this.TableName = TableName;
        }

        public async Task RunAsync()
        {
            _api.GetServerInformation();
            GetToken();
            GetFiscalInformation();
            await SendInvoicesAsync().ConfigureAwait(false);
            Thread.Sleep(2000);
            InquiryByReferenceId();
            //InquiryByTime();
            //InquiryByTimeRange();
            //Thread.Sleep(6000);
            //GetFiscalInformation();
            //InquiryByReferenceId();
            //GetEconomicCodeInformation();
        }

        public void GetServerInformation()
        {
            var serverInformation = _api.GetServerInformation();
            //DataBase.Execute("Update صورتحساب set  ارتباط_با_سرور = 1 where شناسه = " + RowID); 
        }

        private void GetToken()
        {
            var token = _api.RequestToken();
            if (token?.Token != null && token.Token.Length > 0)
            {
                //DataBase.Execute("Update صورتحساب set  دریافت_توکن = 1 where شناسه = " + RowID);  
            }
            else
            {
                DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'خطا در دریافت توکن' where شناسه = " + RowID); 
            }
        }

        private async Task SendInvoicesAsync()
        {
            var invoices = _sampleInvoiceCreator.Create(MEMORY_ID,_InvoiceHeaderDto,_InvoiceBody);
            var responseModel = await _api.SendInvoicesAsync(invoices, null).ConfigureAwait(false);
            if (responseModel?.Body?.Result != null && responseModel.Body.Result.Any())
            {
                DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'" + string.Join(",", responseModel.Body.Result).ToString() + "' where شناسه = " + RowID);
                Console.WriteLine("success send invoice, response" + responseModel.Body.Result);
                var packetResponse = responseModel.Body.Result.First();
                uid = packetResponse.Uid;
                referenceNumber = packetResponse.ReferenceNumber; 

            }
            else
            {
                DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'خطا در ارسال اطلاعات' where شناسه = " + RowID);
                Console.WriteLine(responseModel?.Body?.Errors);
            }
        }

        private void InquiryByUid()
        {
            var uidAndFiscalId = new UidAndFiscalId(uid, MEMORY_ID);
            var inquiryResultModels = _api.InquiryByUidAndFiscalId(new() { uidAndFiscalId });

            if (inquiryResultModels[0].Data == null)
            {
                DataBase.Execute("Update "+ TableName + " set وضعیت_ارسال_به_کارپوشه=0,   شماره_منحصر_بفرد_مالیاتی=N'" + _InvoiceHeaderDto.Taxid.ToString() + "',پیغام_کارپوشه=N'مشخص نیست' where شناسه = " + RowID);
            }
            else
            {
                CheckResult checkResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckResult>(inquiryResultModels[0].Data.ToString());
                string Error = "";
                foreach (ErrorResult error in checkResult.error)
                {
                    Error += error.message;
                }
                DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'" + string.Join(", ", Error) + "' where شناسه = " + RowID);

                if (checkResult.error.Count == 0)
                {
                    DataBase.Execute("Update "+ TableName + " set  شماره_منحصر_بفرد_مالیاتی=N'" + _InvoiceHeaderDto.Taxid.ToString() + "',پیغام_کارپوشه=N'با موفقیت ارسال شد' where شناسه = " + RowID);

                }
            }
        }

        private void InquiryByTime()
        {
            var inquiryResultModels = _api.InquiryByTime("14010725"); 
            Console.WriteLine("inquiry By Time result: " + string.Join(", ", inquiryResultModels));
        }

        private void InquiryByTimeRange()
        {
            var inquiryResultModels = _api.InquiryByTimeRange("14030401", "14030531"); 
            Console.WriteLine("inquiry By Time Range result: " + string.Join(", ", inquiryResultModels));
        }

        private void InquiryByReferenceId()
        {
            var inquiryResultModels = _api.InquiryByReferenceId(new() { referenceNumber });
            if(inquiryResultModels[0].Status == "IN_PROGRESS")
                DataBase.Execute("Update "+ TableName + " set وضعیت_ارسال_به_کارپوشه=0,  پیغام_کارپوشه=N'در حال پردازش اطلاعات' , شماره_منحصر_بفرد_مالیاتی =N'" + _InvoiceHeaderDto.Taxid.ToString() + "'  where شناسه = " + RowID);
            else if (inquiryResultModels[0].Status != "FAILED")
            {  
                DataBase.Execute("Update "+ TableName + " set پیغام_کارپوشه=N'ارسال با موفقیت انجام شد' , شماره_منحصر_بفرد_مالیاتی =N'" + _InvoiceHeaderDto.Taxid.ToString() + "' where شناسه = " + RowID);
            }
            else
                InquiryByUid();
            Console.WriteLine("inquiry By Reference Id result: " + string.Join(", ", inquiryResultModels));
        }

        private void GetFiscalInformation()
        {
            var fiscalInformation = _api.GetFiscalInformation(MEMORY_ID);
            DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'" + fiscalInformation + "' where شناسه = " + RowID);
            DataBase.Execute("Update "+ TableName + " set  پیغام_کارپوشه=N'" + fiscalInformation + "' where شناسه = " + RowID);
            Console.WriteLine("Fiscal Information: " + fiscalInformation);
        }

        private void GetServiceStuffList()
        {
            var searchDto = new SearchDto(1, 10);
            var serviceStuffList = _api.GetServiceStuffList(searchDto);
            Console.WriteLine("Stuff List: " + string.Join(", ", serviceStuffList?.Result));
        }

        private void GetEconomicCodeInformation()
        {
            var economicCodeInformation = _api.GetEconomicCodeInformation("10980030972");
            Console.WriteLine("Economic Code Information: " + economicCodeInformation);
        }
    }
}

 
