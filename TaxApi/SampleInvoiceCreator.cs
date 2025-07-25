﻿ using TaxCollectData.Library.Abstraction;
using TaxCollectData.Library.Business;
using TaxCollectData.Library.Dto.Content;

namespace TaxApi;

internal class SampleInvoiceCreator
{
    public List<InvoiceDto> Create(string clientId,InvoiceHeaderDto invoiceHeaderDto, List<InvoiceBodyDto> invoiceBody)
    {
        //Generate Random Serial number
        //var random = new Random();
        //long randomSerialDecimal = random.Next(999999999);
        //var now = new DateTimeOffset(DateTime.Now);
        //var taxId = _taxIdGenerator.GenerateTaxId(clientId, randomSerialDecimal, DateTime.Now);
        //DateTime time2 = new DateTime(2023, 10, 16);
        //var taxId = _taxIdGenerator.GenerateTaxId(clientId, long.Parse(invoiceHeaderDto.Inno), time2);
        //invoiceHeaderDto.Taxid = taxId;
        //invoiceHeaderDto.Irtaxid = taxId;
        //invoiceHeaderDto.Inno = randomSerialDecimal.ToString();
        return new()
        {
            new InvoiceDto
            {
                Body = invoiceBody, // new() { GetInvoiceBodyDto() },
                Header = invoiceHeaderDto,// GetInvoiceHeaderDto(now.ToUnixTimeMilliseconds(), taxId, randomSerialDecimal),
                //Payments = new() { GetPaymentDto() }
            }
        };
    }

    private static PaymentDto GetPaymentDto()
    {
        var payment = new PaymentDto
        {
            Iinn = "1131244211",
            Acn = "2131244212",
            Trmn = "3131244213",
            Trn = "4131244214"
        };
        return payment;
    }

    private static InvoiceBodyDto GetInvoiceBodyDto()
    {
        var body = new InvoiceBodyDto
        {
            Sstid = "1111111111",
            Sstt = "شیر کم چرب پاستوریزه",
            Mu = "006584",
            Am = 2,
            Fee = 500_000,
            Prdis = 500_000,
            Dis = 0,
            Adis = 500_000,
            Vra = 0,
            Vam = 0,
            Tsstam = 1000_000
        };
        return body;
    }

    private static InvoiceHeaderDto GetInvoiceHeaderDto(long now, string taxId, long randomSerialDecimal)
    {

        var header = new InvoiceHeaderDto
        {
            Inty = 1,
            Inp = 1,
            Inno = randomSerialDecimal.ToString(),
            Ins = 1,
            Tins = "14003778990",
            Tprdis = 1000_000,
            Tadis = 1000,
            Tdis = 0,
            Tvam = 0,
            Todam = 0,
            Tbill = 1000_000,
            Setm = 1,
            Cap = 1000_000,
            Insp = 1000_000,
            Tvop = 0,
            Tax17 = 0,
            Indatim = now,
            Indati2m = now,
            Taxid = taxId
        };
        return header;
    }


}