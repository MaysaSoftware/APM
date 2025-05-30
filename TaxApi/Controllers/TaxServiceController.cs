using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.Json;
using TaxCollectData.Library.Abstraction;
using TaxCollectData.Library.Business;
using TaxCollectData.Library.Dto.Content;

namespace TaxApi.Controllers
{ 

    [ApiController]
    [Route("[controller]")]
    public class TaxServiceController : Controller
    {

        [HttpGet(Name = "GetTaxService")]
        public async Task<string> SendTax(long RowID,
                                      string Customers,
                                      string Project,
                                      string Url
            )
        {
            try
            {
                string DBUser = "sa";
                string DBPassword = "maysasys";
                string Source = ".";
                string DataBaseName = string.Empty;

                switch (Customers)
                {
                    case "CloudLand":
                        {
                            DBUser = "sa";
                            DBPassword = "1iSd2i0_7";
                            Source = "10.1.0.16";
                            DataBaseName = "CloudLand";
                            break;
                        }
                    case "MaysaSoftware":
                        {
                            Source = "185.55.224.4";
                            DBUser = "maysaso1_sa";
                            DBPassword = "1iSd2i0_7";
                            DataBaseName = "maysaso1_MSCOfficial";

                            break;
                        }
                }
                DataBaseName += "Data";
                SQLDataBase sQLDataBase = new SQLDataBase(Source, DataBaseName, DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);

                Record record = new Record(sQLDataBase, "SELECT " + " شناسه_یکتای_حافظه" +
                 ",کلید_خصوصی" +
                 ", فاکتور.سریال_صورتحساب" +
                 ", فاکتور.وضعیت_صورتحساب" +
                 ", فاکتور.شماره_منحصر_بفرد_مالیاتی" +
                 "\n, فاکتور.شرایط_و_نحوه_فروش" +
                 ", درخواست_گواهی_امضا_الکترونیکی.شناسه_ملی as شناسه_ملی_فروشنده" +
                 ", مشتری.نوع_مشتری" +
                 "\n, مشتری.شناسه_ملی " +
                 "\n, مشتری.کد_اقتصادی " +
                 ", مشتری.کد_پستی " +
                 "\n, فاکتور.ساعت_ثبت" +
                 ", لیست_تاریخ.تاریخ_میلادی as تاریخ_صدور_میلادی" +
                 ", فاکتور.مبلغ_پرداختی_نقدی" +
                 ", فاکتور.مبلغ_پرداختی_نسیه" +
                 //", صورتحساب.شناسه_یکتای_ثبت_قرارداد_فروشنده" +
                 ", فاکتور.مجموع_مبلغ_قبل_از_كسر_تخفیف" +
                 ", فاکتور.مجموع_تخفیفات" +
                 ", فاکتور.مجموع_مبلغ_پس_از_كسر_تخفیف" +
                 "\n, فاکتور.مجموع_مالیات_بر_ارزش_افزوده" +
                 "\n, فاکتور.مبلغ as مجموع_صورتحساب" +
                 "\n  FROM  فاکتور inner join درخواست_گواهی_امضا_الکترونیکی on فاکتور.گواهی_امضا_الکترونیکی = درخواست_گواهی_امضا_الکترونیکی.شناسه inner join مشتری on مشتری.شناسه = فاکتور.مشتری  inner join لیست_تاریخ on فاکتور.تاریخ =لیست_تاریخ.تاریخ_شمسی " +
                 "\n  Where فاکتور.شناسه = " + RowID);

                string MEMORY_ID = record.Field("شناسه_یکتای_حافظه", "").ToString();
                string PRIVATE_KEY = record.Field("کلید_خصوصی").ToString();
                DateTime time1 = new DateTime(int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(0, 4)),
                                                                            int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(5, 2)),
                                                                            int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(8, 2)),
                                                                            int.Parse(record.Field("ساعت_ثبت").ToString().Substring(0, 2)),
                                                                            int.Parse(record.Field("ساعت_ثبت").ToString().Substring(3, 2)),
                                                                            int.Parse(record.Field("ساعت_ثبت").ToString().Substring(6, 2)));

                List<InvoiceBodyDto> InvoiceBodyDtolist = new List<InvoiceBodyDto>();

                InvoiceHeaderDto invoiceHeaderDto = new InvoiceHeaderDto();

                ITaxIdGenerator _taxIdGenerator = new TaxIdGenerator(new VerhoffProvider());
                var taxId = _taxIdGenerator.GenerateTaxId(MEMORY_ID, long.Parse(record.Field("سریال_صورتحساب", "").ToString()), time1);
                invoiceHeaderDto.Taxid = taxId;
                invoiceHeaderDto.Indatim = (new DateTimeOffset(time1)).ToUnixTimeMilliseconds();
                invoiceHeaderDto.Indati2m = (new DateTimeOffset(time1)).ToUnixTimeMilliseconds();
                invoiceHeaderDto.Inno = record.Field("سریال_صورتحساب", "").ToString();
                invoiceHeaderDto.Ins = int.Parse(record.Field("وضعیت_صورتحساب", 0).ToString());
                invoiceHeaderDto.Tins = record.Field("شناسه_ملی_فروشنده", "").ToString();
                if (invoiceHeaderDto.Ins > 1)
                    invoiceHeaderDto.Irtaxid = record.Field("شماره_منحصر_بفرد_مالیاتی", "").ToString();
                if (invoiceHeaderDto.Ins != 3)
                {
                    invoiceHeaderDto.Inty = 1;
                    invoiceHeaderDto.Inp = 1;

                    if (invoiceHeaderDto.Inty == 1)
                        invoiceHeaderDto.Tob = int.Parse(record.Field("نوع_مشتری", 0).ToString());

                    if (record.Field("شناسه_ملی", "").ToString() != "")
                        invoiceHeaderDto.Bid = invoiceHeaderDto.Tob == 1 ? record.Field("شناسه_ملی", "").ToString().Substring(0, 10) : record.Field("شناسه_ملی", "").ToString();

                    if (record.Field("کد_اقتصادی", "").ToString() != "")
                        invoiceHeaderDto.Tinb = record.Field("کد_اقتصادی", "").ToString();

                    if (!string.IsNullOrEmpty(record.Field("کد_پستی", "").ToString()))
                        invoiceHeaderDto.Bpc = record.Field("کد_پستی", "").ToString();

                    if (!string.IsNullOrEmpty(record.Field("شناسه_یکتای_ثبت_قرارداد_فروشنده", "").ToString()))
                        invoiceHeaderDto.Crn = record.Field("شناسه_یکتای_ثبت_قرارداد_فروشنده", "").ToString();

                    invoiceHeaderDto.Tprdis = decimal.Parse(record.Field("مجموع_مبلغ_قبل_از_كسر_تخفیف", 0).ToString());
                    invoiceHeaderDto.Tdis = decimal.Parse(record.Field("مجموع_تخفیفات", 0).ToString());
                    invoiceHeaderDto.Tadis = decimal.Parse(record.Field("مجموع_مبلغ_پس_از_كسر_تخفیف", 0).ToString());
                    invoiceHeaderDto.Tvam = decimal.Parse(record.Field("مجموع_مالیات_بر_ارزش_افزوده", 0).ToString());
                    invoiceHeaderDto.Todam = 0;
                    invoiceHeaderDto.Tbill = decimal.Parse(record.Field("مجموع_صورتحساب", 0).ToString());
                    invoiceHeaderDto.Setm = int.Parse(record.Field("شرایط_و_نحوه_فروش", 0).ToString());
                    invoiceHeaderDto.Cap = decimal.Parse(record.Field("مبلغ_پرداختی_نقدی", 0).ToString());
                    invoiceHeaderDto.Insp = decimal.Parse(record.Field("مبلغ_پرداختی_نسیه", 0).ToString());
                    invoiceHeaderDto.Tvop = decimal.Parse(record.Field("مجموع_سهم_مالیات_بر_ارزش_افزوده_از_پرداخت", 0).ToString());

                    DataTable Data = sQLDataBase.SelectDataTable("SELECT شناسه_مالیاتی_کالا , تعداد, مبلغ_واحد, مبلغ_کل, تخفیف , مبلغ_کل_پس_از_تخفیف, جمع_مبلغ_کل_مالیات_و_عوارض , مبلغ_نهایی, شرح_کالا , شناسه_مالیاتی_واحد_اندازه_گیری, مالیات_و_عوارض " +
                                                                 "\n  FROM اقلام_فاکتور  where فاکتور = " + RowID);

                    foreach (DataRow row in Data.Rows)
                    {
                        InvoiceBodyDto invoiceBody = new InvoiceBodyDto();
                        invoiceBody.Sstid = row["شناسه_مالیاتی_کالا"].ToString();
                        invoiceBody.Sstt = row["شرح_کالا"].ToString();
                        invoiceBody.Mu = row["شناسه_مالیاتی_واحد_اندازه_گیری"].ToString();
                        invoiceBody.Am = decimal.Parse(row["تعداد"].ToString());
                        invoiceBody.Fee = decimal.Parse(row["مبلغ_واحد"].ToString());

                        invoiceBody.Prdis = decimal.Parse(row["مبلغ_کل"].ToString() == "" ? "0" : row["مبلغ_کل"].ToString());
                        invoiceBody.Dis = decimal.Parse(row["تخفیف"].ToString() == "" ? "0" : row["تخفیف"].ToString());
                        invoiceBody.Adis = decimal.Parse(row["مبلغ_کل_پس_از_تخفیف"].ToString() == "" ? "0" : row["مبلغ_کل_پس_از_تخفیف"].ToString());
                        invoiceBody.Vra = decimal.Parse(row["مالیات_و_عوارض"].ToString() == "" ? "0" : row["مالیات_و_عوارض"].ToString());
                        invoiceBody.Vam = decimal.Parse(row["جمع_مبلغ_کل_مالیات_و_عوارض"].ToString() == "" ? "0" : row["جمع_مبلغ_کل_مالیات_و_عوارض"].ToString());

                        if (decimal.Parse(row["مبلغ_نهایی"].ToString() == "" ? "0" : row["مبلغ_نهایی"].ToString()) != 0)
                            invoiceBody.Tsstam = decimal.Parse(row["مبلغ_نهایی"].ToString());



                        //if (decimal.Parse(row["میزان_ارز"].ToString() == "" ? "0" : row["میزان_ارز"].ToString()) != 0)
                        //    invoiceBody.Cfee = decimal.Parse(row["میزان_ارز"].ToString());

                        //if (!string.IsNullOrEmpty(row["نوع_ارز"].ToString()))
                        //    invoiceBody.Cut = row["نوع_ارز"].ToString();

                        //if (decimal.Parse(row["نرخ_برابری_ارز_با_ریال"].ToString() == "" ? "0" : row["نرخ_برابری_ارز_با_ریال"].ToString()) != 0)
                        //    invoiceBody.Exr = decimal.Parse(row["نرخ_برابری_ارز_با_ریال"].ToString());

                        //if (!string.IsNullOrEmpty(row["موضوع_سایر_مالیات_و_عوارض"].ToString()))
                        //    invoiceBody.Odt = row["موضوع_سایر_مالیات_و_عوارض"].ToString();

                        //if (decimal.Parse(row["نرخ_سایر_مالیات_و_عوارض"].ToString() == "" ? "0" : row["نرخ_سایر_مالیات_و_عوارض"].ToString()) != 0)
                        //    invoiceBody.Odr = decimal.Parse(row["نرخ_سایر_مالیات_و_عوارض"].ToString());

                        //if (decimal.Parse(row["مبلغ_سایر_مالیات_و_عوارض"].ToString() == "" ? "0" : row["مبلغ_سایر_مالیات_و_عوارض"].ToString()) != 0)
                        //    invoiceBody.Odam = decimal.Parse(row["مبلغ_سایر_مالیات_و_عوارض"].ToString());

                        //if (!string.IsNullOrEmpty(row["موضوع_سایر_وجوه_قانونی"].ToString()))
                        //    invoiceBody.Olt = row["موضوع_سایر_وجوه_قانونی"].ToString();

                        //if (decimal.Parse(row["نرخ_سایر_وجوه_قانونی"].ToString() == "" ? "0" : row["نرخ_سایر_وجوه_قانونی"].ToString()) != 0)
                        //    invoiceBody.Olr = decimal.Parse(row["نرخ_سایر_وجوه_قانونی"].ToString());

                        //if (decimal.Parse(row["مبلغ_سایر_وجوه_قانونی"].ToString() == "" ? "0" : row["مبلغ_سایر_وجوه_قانونی"].ToString()) != 0)
                        //    invoiceBody.Olam = decimal.Parse(row["مبلغ_سایر_وجوه_قانونی"].ToString());

                        //if (decimal.Parse(row["اجرت_ساخت"].ToString() == "" ? "0" : row["اجرت_ساخت"].ToString()) != 0)
                        //    invoiceBody.Consfee = decimal.Parse(row["اجرت_ساخت"].ToString());

                        //if (decimal.Parse(row["سود_فروشنده"].ToString() == "" ? "0" : row["سود_فروشنده"].ToString()) != 0)
                        //    invoiceBody.Spro = decimal.Parse(row["سود_فروشنده"].ToString());

                        //if (decimal.Parse(row["حق_العمل"].ToString() == "" ? "0" : row["حق_العمل"].ToString()) != 0)
                        //    invoiceBody.Bros = decimal.Parse(row["حق_العمل"].ToString());

                        //if (decimal.Parse(row["جمع_کل_حق_العمل_و_سود"].ToString() == "" ? "0" : row["جمع_کل_حق_العمل_و_سود"].ToString()) != 0)
                        //    invoiceBody.Tcpbs = decimal.Parse(row["جمع_کل_حق_العمل_و_سود"].ToString());

                        //if (decimal.Parse(row["سهم_نقدی_از_پرداخت"].ToString() == "" ? "0" : row["سهم_نقدی_از_پرداخت"].ToString()) != 0)
                        //    invoiceBody.Cop = decimal.Parse(row["سهم_نقدی_از_پرداخت"].ToString());

                        //if (decimal.Parse(row["سهم_ارزش_افزوده_از_پرداخت"].ToString() == "" ? "0" : row["سهم_ارزش_افزوده_از_پرداخت"].ToString()) != 0)
                        //    invoiceBody.Vop = decimal.Parse(row["سهم_ارزش_افزوده_از_پرداخت"].ToString());

                        //if (!string.IsNullOrEmpty(row["شناسه_یکتای_ثبت_قرارداد_حق_العمل_کاری"].ToString()))
                        //    invoiceBody.Bsrn = row["شناسه_یکتای_ثبت_قرارداد_حق_العمل_کاری"].ToString();

                        //if (decimal.Parse(row["وزن_خالص"].ToString() == "" ? "0" : row["وزن_خالص"].ToString()) != 0)
                        //    invoiceBody.Nw = decimal.Parse(row["وزن_خالص"].ToString());

                        //if (decimal.Parse(row["ارزش_ریالی_کالا"].ToString() == "" ? "0" : row["ارزش_ریالی_کالا"].ToString()) != 0)
                        //    invoiceBody.Ssrv = decimal.Parse(row["ارزش_ریالی_کالا"].ToString());

                        //if (decimal.Parse(row["ارزش_ارزی_کالا"].ToString() == "" ? "0" : row["ارزش_ارزی_کالا"].ToString()) != 0)
                        //    invoiceBody.Sscv = decimal.Parse(row["ارزش_ارزی_کالا"].ToString());

                        InvoiceBodyDtolist.Add(invoiceBody);
                    }
                }
                PRIVATE_KEY = PRIVATE_KEY.Replace("\n", "");
                await new TaxService(MEMORY_ID, PRIVATE_KEY, Url, invoiceHeaderDto, InvoiceBodyDtolist, sQLDataBase, RowID, "فاکتور").RunAsync().ConfigureAwait(false);

                sQLDataBase.Execute("Update صورتحساب set وضعیت_ارسال_به_کارپوشه= case when(پیغام_کارپوشه = N'ارسال با موفقیت انجام شد') then 1 else 0 end where شناسه = " + RowID);
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        //[HttpGet(Name = "GetTaxService")]
        //public async Task<string> Get(long RowID,
        //                              string ServerName,
        //                              string DatabaseName,
        //                              string UserName,
        //                              string Password,
        //                              string Url)
        //{
        //    try
        //    {
        //        Password = Encoding.ASCII.GetString(JsonSerializer.Deserialize<byte[]>(Password));
        //        SQLDataBase sQLDataBase = new SQLDataBase(ServerName, DatabaseName, Password, UserName, SQLDataBase.SQLVersions.SQL2008);

        //        Record record = new Record(sQLDataBase, "SELECT شناسه_کلید_امضا" +
        //         ",شناسه_یکتای_حافظه" +
        //         ",لینک_سرویس_مایسا" +
        //         ",لینک_سرویس_سازمان_مالیات" +
        //         ",کلید_خصوصی   " +
        //         ",صورتحساب.نوع_صورتحساب" +
        //         ", صورتحساب.سریال_صورتحساب" +
        //         ", صورتحساب.ماهیت_صورتحساب" +
        //         ", صورتحساب.وضعیت_صورتحساب" +
        //         ", صورتحساب.شماره_منحصر_بفرد_مالیاتی" +
        //         ", صورتحساب.نوع_پرداخت" +
        //         ", صورتحساب.نوع_فروشنده" +
        //         ", صورتحساب.شناسه_ملی_فروشنده" +
        //         ", صورتحساب.کد_پستی_فروشنده" +
        //         ", صورتحساب.مبلغ_نهایی_صورتحساب" +
        //         ", صورتحساب.نوع_مشتری" +
        //         ", صورتحساب.شناسه_ملی_مشتری" +
        //         ", صورتحساب.کد_پستی_مشتری" +
        //         ", صورتحساب.ساعت_ثبت" +
        //         ", صورتحساب.تاریخ_ثبت_میلادی" +
        //         ", صورتحساب.تاریخ_صدور_میلادی" +
        //         ", صورتحساب.موضوع_صورتحساب" +
        //         ", صورتحساب.مبلغ_پرداختی_نقدی" +
        //         ", صورتحساب.مبلغ_پرداختی_نسیه" +
        //         ", صورتحساب.مجموع_سهم_مالیات_بر_ارزش_افزوده_از_پرداخت" +
        //         ", صورتحساب.کد_شعبه_فروشنده" +
        //         ", صورتحساب.کد_شعبه_مشتری" +
        //         ", صورتحساب.نوع_پرواز" +
        //         ", صورتحساب.شماره_گذرنامه_مشتری" +
        //         ", صورتحساب.شماره_پروانه_گمركی" +
        //         ", صورتحساب.كد_گمرک_محل_اظهار" +
        //         ", صورتحساب.شناسه_یکتای_ثبت_قرارداد_فروشنده" +
        //         ", صورتحساب.شناسه_قبض_بهره_بردار" +
        //         ", صورتحساب.مجموع_مبلغ_قبل_از_كسر_تخفیف" +
        //         ", صورتحساب.مجموع_تخفیفات" +
        //         ", صورتحساب.مجموع_مبلغ_پس_از_كسر_تخفیف" +
        //         ", صورتحساب.مجموع_مالیات_بر_ارزش_افزوده" +
        //         ", صورتحساب.مجموع_سایر_مالیات_عوارض_و_وجوه_قانونی" +
        //         ", صورتحساب.مجموع_صورتحساب" +
        //         ", صورتحساب.شماره_کوتاژ_اظهارنامه_گمرکی" +
        //         ", صورتحساب.تاریخ_میلادی_کوتاژ_اظهارنامه_گمرکی" +
        //         ", صورتحساب.مجموع_وزن_خالص" +
        //         ", صورتحساب.مجموع_ارزش_ریالی" +
        //         ", صورتحساب.مجموع_ارزش_ارزی " +
        //         "  FROM  صورتحساب inner join درخواست_گواهی_امضا_الکترونیکی on صورتحساب.درخواست_گواهی = درخواست_گواهی_امضا_الکترونیکی.شناسه" +
        //         "  Where صورتحساب.شناسه = " + RowID);

        //        string MEMORY_ID = record.Field("شناسه_یکتای_حافظه", "").ToString();
        //        string PRIVATE_KEY = record.Field("کلید_خصوصی").ToString();
        //        DateTime time1 = new DateTime(int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(0, 4)),
        //                                                                    int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(5, 2)),
        //                                                                    int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(8, 2)),
        //                                                                    int.Parse(record.Field("ساعت_ثبت").ToString().Substring(0, 2)),
        //                                                                    int.Parse(record.Field("ساعت_ثبت").ToString().Substring(3, 2)),
        //                                                                    int.Parse(record.Field("ساعت_ثبت").ToString().Substring(6, 2)));

        //        //DateTime time2 = new DateTime(int.Parse(record.Field("تاریخ_ثبت_میلادی").ToString().Substring(0, 4)),
        //        //                                                            int.Parse(record.Field("تاریخ_ثبت_میلادی").ToString().Substring(5, 2)),
        //        //                                                            int.Parse(record.Field("تاریخ_ثبت_میلادی").ToString().Substring(8, 2)));
        //        List<InvoiceBodyDto> InvoiceBodyDtolist = new List<InvoiceBodyDto>();

        //        InvoiceHeaderDto invoiceHeaderDto = new InvoiceHeaderDto();

        //        ITaxIdGenerator _taxIdGenerator = new TaxIdGenerator(new VerhoffProvider());
        //        var taxId = _taxIdGenerator.GenerateTaxId(MEMORY_ID, long.Parse(record.Field("سریال_صورتحساب", "").ToString()), time1);
        //        invoiceHeaderDto.Taxid = taxId;
        //        invoiceHeaderDto.Indatim = (new DateTimeOffset(time1)).ToUnixTimeMilliseconds();
        //        invoiceHeaderDto.Indati2m = (new DateTimeOffset(time1)).ToUnixTimeMilliseconds();
        //        invoiceHeaderDto.Inno = record.Field("سریال_صورتحساب", "").ToString();
        //        invoiceHeaderDto.Ins = int.Parse(record.Field("وضعیت_صورتحساب", 0).ToString());
        //        invoiceHeaderDto.Tins = record.Field("شناسه_ملی_فروشنده", "").ToString();
        //        if (invoiceHeaderDto.Ins > 1)
        //            invoiceHeaderDto.Irtaxid = record.Field("شماره_منحصر_بفرد_مالیاتی", "").ToString();
        //        if (invoiceHeaderDto.Ins != 3)
        //        {
        //            invoiceHeaderDto.Inty = int.Parse(record.Field("نوع_صورتحساب", 0).ToString());
        //            invoiceHeaderDto.Inp = int.Parse(record.Field("ماهیت_صورتحساب", 0).ToString());
                    
        //            if (invoiceHeaderDto.Inp != 3)
        //            {
        //                if (invoiceHeaderDto.Inty == 1)
        //                    invoiceHeaderDto.Tob = int.Parse(record.Field("نوع_مشتری", 0).ToString());

        //                if (record.Field("شناسه_ملی_مشتری", "").ToString() != "")
        //                    invoiceHeaderDto.Bid = invoiceHeaderDto.Tob == 1 ? record.Field("شناسه_ملی_مشتری", "").ToString().Substring(0, 10) : record.Field("شناسه_ملی_مشتری", "").ToString();

        //                if (record.Field("شناسه_ملی_مشتری", "").ToString() != "")
        //                    invoiceHeaderDto.Tinb = record.Field("شناسه_ملی_مشتری", "").ToString();

        //                if (!string.IsNullOrEmpty(record.Field("کد_شعبه_فروشنده", "").ToString()))
        //                    invoiceHeaderDto.Sbc = record.Field("کد_شعبه_فروشنده", "").ToString();


        //                if (!string.IsNullOrEmpty(record.Field("کد_پستی_مشتری", "").ToString()))
        //                    invoiceHeaderDto.Bpc = record.Field("کد_پستی_مشتری", "").ToString();

        //                if (!string.IsNullOrEmpty(record.Field("کد_شعبه_مشتری", "").ToString()))
        //                    invoiceHeaderDto.Bbc = record.Field("کد_شعبه_مشتری", "").ToString();


        //                if (!string.IsNullOrEmpty(record.Field("شماره_گذرنامه_مشتری", "").ToString()))
        //                    invoiceHeaderDto.Bpn = record.Field("شماره_گذرنامه_مشتری", "").ToString();
        //            }

        //            if (int.Parse(record.Field("نوع_پرواز", 0).ToString()) != 0)
        //                invoiceHeaderDto.Ft = int.Parse(record.Field("نوع_پرواز", 0).ToString());

        //            if (!string.IsNullOrEmpty(record.Field("شماره_پروانه_گمركی", "").ToString()))
        //                invoiceHeaderDto.Scln = record.Field("شماره_پروانه_گمركی", "").ToString();

        //            if (!string.IsNullOrEmpty(record.Field("كد_گمرک_محل_اظهار", "").ToString()))
        //                invoiceHeaderDto.Scc = record.Field("كد_گمرک_محل_اظهار", "").ToString();

        //            if (!string.IsNullOrEmpty(record.Field("شناسه_یکتای_ثبت_قرارداد_فروشنده", "").ToString()))
        //                invoiceHeaderDto.Crn = record.Field("شناسه_یکتای_ثبت_قرارداد_فروشنده", "").ToString();


        //            if (!string.IsNullOrEmpty(record.Field("شناسه_قبض_بهره_بردار", "").ToString()))
        //                invoiceHeaderDto.Billid = record.Field("شناسه_قبض_بهره_بردار", "").ToString();

        //            invoiceHeaderDto.Tprdis = decimal.Parse(record.Field("مجموع_مبلغ_قبل_از_كسر_تخفیف", 0).ToString());
        //            invoiceHeaderDto.Tdis = decimal.Parse(record.Field("مجموع_تخفیفات", 0).ToString());
        //            invoiceHeaderDto.Tadis = decimal.Parse(record.Field("مجموع_مبلغ_پس_از_كسر_تخفیف", 0).ToString());
        //            invoiceHeaderDto.Tvam = decimal.Parse(record.Field("مجموع_مالیات_بر_ارزش_افزوده", 0).ToString());
        //            invoiceHeaderDto.Todam = decimal.Parse(record.Field("مجموع_سایر_مالیات_عوارض_و_وجوه_قانونی", 0).ToString());
        //            invoiceHeaderDto.Tbill = decimal.Parse(record.Field("مجموع_صورتحساب", 0).ToString());
        //            invoiceHeaderDto.Setm = int.Parse(record.Field("نوع_پرداخت", 0).ToString());
        //            invoiceHeaderDto.Cap = decimal.Parse(record.Field("مبلغ_پرداختی_نقدی", 0).ToString());
        //            invoiceHeaderDto.Insp = decimal.Parse(record.Field("مبلغ_پرداختی_نسیه", 0).ToString());
        //            invoiceHeaderDto.Tvop = decimal.Parse(record.Field("مجموع_سهم_مالیات_بر_ارزش_افزوده_از_پرداخت", 0).ToString());

        //            if (!string.IsNullOrEmpty(record.Field("شماره_کوتاژ_اظهارنامه_گمرکی", "").ToString()))
        //                invoiceHeaderDto.Cdcn = record.Field("شماره_کوتاژ_اظهارنامه_گمرکی", "").ToString();
        //            //invoiceHeaderDto.Cdcd = (new DateTimeOffset(new DateTime(int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(1, 4)),
        //            //                                                        int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(6, 2)),
        //            //                                                        int.Parse(record.Field("تاریخ_صدور_میلادی").ToString().Substring(8, 2))))).ToUnixTimeMilliseconds();

        //            if (decimal.Parse(record.Field("مجموع_وزن_خالص", 0).ToString()) != 0)
        //                invoiceHeaderDto.Tonw = decimal.Parse(record.Field("مجموع_وزن_خالص", 0).ToString());

        //            if (decimal.Parse(record.Field("مجموع_ارزش_ریالی", 0).ToString()) != 0)
        //                invoiceHeaderDto.Torv = decimal.Parse(record.Field("مجموع_ارزش_ریالی", 0).ToString());

        //            if (decimal.Parse(record.Field("مجموع_ارزش_ارزی", 0).ToString()) != 0)
        //                invoiceHeaderDto.Tocv = decimal.Parse(record.Field("مجموع_ارزش_ارزی", 0).ToString());

        //            DataTable Data = sQLDataBase.SelectDataTable("SELECT شناسه_یکتا_کالا , تعداد, قیمت_پایه, مبلغ_ناخالص, مبلغ_تخفیف , مبلغ_کل_پس_از_تخفیف, مبلغ_مالیات_و_عوارض , مبلغ_نهایی, عنوان_خدمات , واحد_اندازه_گیری, میزان_ارز , نوع_ارز" +
        //                                                            ", نرخ_برابری_ارز_با_ریال, نرخ_مالیات_بر_ارزش_افزوده, موضوع_سایر_مالیات_و_عوارض, نرخ_سایر_مالیات_و_عوارض, مبلغ_سایر_مالیات_و_عوارض, موضوع_سایر_وجوه_قانونی" +
        //                                                            ", نرخ_سایر_وجوه_قانونی, مبلغ_سایر_وجوه_قانونی, اجرت_ساخت, سود_فروشنده, حق_العمل, جمع_کل_حق_العمل_و_سود, سهم_نقدی_از_پرداخت, سهم_ارزش_افزوده_از_پرداخت, شناسه_یکتای_ثبت_قرارداد_حق_العمل_کاری" +
        //                                                            ", وزن_خالص, ارزش_ریالی_کالا, ارزش_ارزی_کالا  FROM اقلام_صورتحساب  where صورتحساب = " + RowID);

        //            foreach (DataRow row in Data.Rows)
        //            {
        //                InvoiceBodyDto invoiceBody = new InvoiceBodyDto();
        //                invoiceBody.Sstid = row["شناسه_یکتا_کالا"].ToString();
        //                invoiceBody.Sstt = row["عنوان_خدمات"].ToString();
        //                invoiceBody.Mu = row["واحد_اندازه_گیری"].ToString();
        //                invoiceBody.Am = decimal.Parse(row["تعداد"].ToString());
        //                invoiceBody.Fee = decimal.Parse(row["قیمت_پایه"].ToString());

        //                if (decimal.Parse(row["میزان_ارز"].ToString() == "" ? "0" : row["میزان_ارز"].ToString()) != 0)
        //                    invoiceBody.Cfee = decimal.Parse(row["میزان_ارز"].ToString());

        //                if (!string.IsNullOrEmpty(row["نوع_ارز"].ToString()))
        //                    invoiceBody.Cut = row["نوع_ارز"].ToString();

        //                if (decimal.Parse(row["نرخ_برابری_ارز_با_ریال"].ToString() == "" ? "0" : row["نرخ_برابری_ارز_با_ریال"].ToString()) != 0)
        //                    invoiceBody.Exr = decimal.Parse(row["نرخ_برابری_ارز_با_ریال"].ToString());

        //                invoiceBody.Prdis = decimal.Parse(row["مبلغ_ناخالص"].ToString() == "" ? "0" : row["مبلغ_ناخالص"].ToString());
        //                invoiceBody.Dis = decimal.Parse(row["مبلغ_تخفیف"].ToString() == "" ? "0" : row["مبلغ_تخفیف"].ToString());
        //                invoiceBody.Adis = decimal.Parse(row["مبلغ_کل_پس_از_تخفیف"].ToString() == "" ? "0" : row["مبلغ_کل_پس_از_تخفیف"].ToString());
        //                invoiceBody.Vra = decimal.Parse(row["نرخ_مالیات_بر_ارزش_افزوده"].ToString() == "" ? "0" : row["نرخ_مالیات_بر_ارزش_افزوده"].ToString());
        //                invoiceBody.Vam = decimal.Parse(row["مبلغ_مالیات_و_عوارض"].ToString() == "" ? "0" : row["مبلغ_مالیات_و_عوارض"].ToString());

        //                if (!string.IsNullOrEmpty(row["موضوع_سایر_مالیات_و_عوارض"].ToString()))
        //                    invoiceBody.Odt = row["موضوع_سایر_مالیات_و_عوارض"].ToString();

        //                if (decimal.Parse(row["نرخ_سایر_مالیات_و_عوارض"].ToString() == "" ? "0" : row["نرخ_سایر_مالیات_و_عوارض"].ToString()) != 0)
        //                    invoiceBody.Odr = decimal.Parse(row["نرخ_سایر_مالیات_و_عوارض"].ToString());

        //                if (decimal.Parse(row["مبلغ_سایر_مالیات_و_عوارض"].ToString() == "" ? "0" : row["مبلغ_سایر_مالیات_و_عوارض"].ToString()) != 0)
        //                    invoiceBody.Odam = decimal.Parse(row["مبلغ_سایر_مالیات_و_عوارض"].ToString());

        //                if (!string.IsNullOrEmpty(row["موضوع_سایر_وجوه_قانونی"].ToString()))
        //                    invoiceBody.Olt = row["موضوع_سایر_وجوه_قانونی"].ToString();

        //                if (decimal.Parse(row["نرخ_سایر_وجوه_قانونی"].ToString() == "" ? "0" : row["نرخ_سایر_وجوه_قانونی"].ToString()) != 0)
        //                    invoiceBody.Olr = decimal.Parse(row["نرخ_سایر_وجوه_قانونی"].ToString());

        //                if (decimal.Parse(row["مبلغ_سایر_وجوه_قانونی"].ToString() == "" ? "0" : row["مبلغ_سایر_وجوه_قانونی"].ToString()) != 0)
        //                    invoiceBody.Olam = decimal.Parse(row["مبلغ_سایر_وجوه_قانونی"].ToString());

        //                if (decimal.Parse(row["اجرت_ساخت"].ToString() == "" ? "0" : row["اجرت_ساخت"].ToString()) != 0)
        //                    invoiceBody.Consfee = decimal.Parse(row["اجرت_ساخت"].ToString());

        //                if (decimal.Parse(row["سود_فروشنده"].ToString() == "" ? "0" : row["سود_فروشنده"].ToString()) != 0)
        //                    invoiceBody.Spro = decimal.Parse(row["سود_فروشنده"].ToString());

        //                if (decimal.Parse(row["حق_العمل"].ToString() == "" ? "0" : row["حق_العمل"].ToString()) != 0)
        //                    invoiceBody.Bros = decimal.Parse(row["حق_العمل"].ToString());

        //                if (decimal.Parse(row["جمع_کل_حق_العمل_و_سود"].ToString() == "" ? "0" : row["جمع_کل_حق_العمل_و_سود"].ToString()) != 0)
        //                    invoiceBody.Tcpbs = decimal.Parse(row["جمع_کل_حق_العمل_و_سود"].ToString());

        //                if (decimal.Parse(row["سهم_نقدی_از_پرداخت"].ToString() == "" ? "0" : row["سهم_نقدی_از_پرداخت"].ToString()) != 0)
        //                    invoiceBody.Cop = decimal.Parse(row["سهم_نقدی_از_پرداخت"].ToString());

        //                if (decimal.Parse(row["سهم_ارزش_افزوده_از_پرداخت"].ToString() == "" ? "0" : row["سهم_ارزش_افزوده_از_پرداخت"].ToString()) != 0)
        //                    invoiceBody.Vop = decimal.Parse(row["سهم_ارزش_افزوده_از_پرداخت"].ToString());

        //                if (!string.IsNullOrEmpty(row["شناسه_یکتای_ثبت_قرارداد_حق_العمل_کاری"].ToString()))
        //                    invoiceBody.Bsrn = row["شناسه_یکتای_ثبت_قرارداد_حق_العمل_کاری"].ToString();

        //                if (decimal.Parse(row["مبلغ_نهایی"].ToString() == "" ? "0" : row["مبلغ_نهایی"].ToString()) != 0)
        //                    invoiceBody.Tsstam = decimal.Parse(row["مبلغ_نهایی"].ToString());

        //                if (decimal.Parse(row["وزن_خالص"].ToString() == "" ? "0" : row["وزن_خالص"].ToString()) != 0)
        //                    invoiceBody.Nw = decimal.Parse(row["وزن_خالص"].ToString());

        //                if (decimal.Parse(row["ارزش_ریالی_کالا"].ToString() == "" ? "0" : row["ارزش_ریالی_کالا"].ToString()) != 0)
        //                    invoiceBody.Ssrv = decimal.Parse(row["ارزش_ریالی_کالا"].ToString());

        //                if (decimal.Parse(row["ارزش_ارزی_کالا"].ToString() == "" ? "0" : row["ارزش_ارزی_کالا"].ToString()) != 0)
        //                    invoiceBody.Sscv = decimal.Parse(row["ارزش_ارزی_کالا"].ToString());

        //                InvoiceBodyDtolist.Add(invoiceBody);
        //            }
        //        }
        //        PRIVATE_KEY = PRIVATE_KEY.Replace("\n", "");
        //        await new TaxService(MEMORY_ID, PRIVATE_KEY, Url, invoiceHeaderDto, InvoiceBodyDtolist, sQLDataBase, RowID, "صورتحساب").RunAsync().ConfigureAwait(false);

        //        sQLDataBase.Execute("Update صورتحساب set وضعیت_ارسال_به_کارپوشه= case when(پیغام_کارپوشه = N'ارسال با موفقیت انجام شد') then 1 else 0 end where شناسه = " + RowID);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return HttpContext.Session.GetString("TaxServiceMessage");
        //}


    }
}
