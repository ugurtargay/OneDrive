using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Core.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using up_console;
using Microsoft.Identity.Client;

namespace AdayTedarikciFormu.Controllers
{

    public class PostApiController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/PostApi/SAVE")]
        public HttpResponseMessage SAVE(MainData mainData)
        {
            decimal kontrolHisse = 0;
            string iletisimEmail = "";
            var tumHisse = mainData.IM_HISSE;
            var genelEmail = mainData.IM_GENEL.EMAIL;
            var iletisimEmailList = mainData.IM_SORUMLU;
            var busDevDec = mainData.IM_BUS_DEV;
            var musDec = mainData.IM_MUSTERI;
            var uretimDec = mainData.IM_URETIM;

            #region hisseValidation
            foreach (var item in tumHisse)
            {
                kontrolHisse += item.HISSE_YUZDE1;
            }

            if (kontrolHisse > 100)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "Hisseler toplamı % 100'ü aşmıştır.Lütfen tekrar kontrol ediniz.");
                return response;
            }
            else if (kontrolHisse < 100)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "Hisse yada hisseler toplamı %100 olmalıdır.Lütfen tekrar kontrol ediniz.");
                return response;
            }
            #endregion

            #region emailValidation
            foreach (var item in iletisimEmailList)
            {
                iletisimEmail += item.EMAIL + ",";
            }

            if (!string.IsNullOrEmpty(genelEmail) || !string.IsNullOrEmpty(iletisimEmail))
            {
                var list = iletisimEmail.Split(',');

                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(genelEmail))
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Geçersiz e-mail adresi lütfen geçerli bir e-mail adresi giriniz.");
                    return response;
                }
                foreach (var item2 in list)
                {
                    if ((item2 != null && item2 != "") || item2 != string.Empty || item2 != "")
                    {
                        if (!re.IsMatch(item2))
                        {
                            var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Geçersiz e-mail adresi lütfen geçerli bir e-mail adresi giriniz.");
                            return response;
                        }
                    }
                }
            }
            else
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "E-mail adresi girilmelidir.");
                return response;
            }
            #endregion

            string userName = "usrvoden";
            string password = "1iym!eRT";

            ServiceIntegration.tr.com.tofas.apitest.SI_ADAY_TEDARIKCI_KAYDETService srv = new ServiceIntegration.tr.com.tofas.apitest.SI_ADAY_TEDARIKCI_KAYDETService();
            srv.Credentials = new NetworkCredential(userName, password);
            srv.PreAuthenticate = true;
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_03> IM_BANKA = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_03>();
            foreach (var item in mainData.IM_BANKA)
            {
                IM_BANKA.Add(new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_03
                {
                    BANKA_ADI = item.BANKA_ADI != null ? item.BANKA_ADI : string.Empty,
                    BANKA_ID = item.BANKA_ID != null ? item.BANKA_ID : string.Empty,
                    HESAP_NO = item.HESAP_NO != null ? item.HESAP_NO : string.Empty,
                    IBAN = item.IBAN != null ? item.IBAN : string.Empty,
                    PARA_BIRIMI = item.PARA_BIRIMI != null ? item.PARA_BIRIMI : string.Empty,
                    SUBE_ADI = item.SUBE_ADI != null ? item.SUBE_ADI : string.Empty,
                    SUBE_KODU = item.SUBE_KODU != null ? item.SUBE_KODU : string.Empty
                });
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_08> IM_BUS_DEV = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_08>();
            if (mainData.IM_BUS_DEV != null)
            {
                IM_BUS_DEV = mainData.IM_BUS_DEV;
            }
            ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_09 IM_DIRECT_EK = new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_09();
            if (mainData.IM_DIRECT_EK != null)
            {
                IM_DIRECT_EK = mainData.IM_DIRECT_EK;
            }
            ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_07 IM_END_EK = new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_07
            {
                SENDIKA_ADI = mainData.IM_END_EK.SENDIKA_ADI != null ? mainData.IM_END_EK.SENDIKA_ADI : string.Empty,
                MAVI_YAKA = mainData.IM_END_EK.MAVI_YAKA != null ? mainData.IM_END_EK.MAVI_YAKA : string.Empty,
                BEYAZ_YAKA = mainData.IM_END_EK.BEYAZ_YAKA != null ? mainData.IM_END_EK.BEYAZ_YAKA : string.Empty,
                OY_CIRO = mainData.IM_END_EK.OY_CIRO,
                KUR_SERMAYE = mainData.IM_END_EK.KUR_SERMAYE,
                GUN_SERMAYE = mainData.IM_END_EK.GUN_SERMAYE,
                KAPALI_ALAN = mainData.IM_END_EK.KAPALI_ALAN != null ? mainData.IM_END_EK.KAPALI_ALAN : string.Empty,
                ACIK_ALAN = mainData.IM_END_EK.ACIK_ALAN != null ? mainData.IM_END_EK.ACIK_ALAN : string.Empty,
                PARK_BILGI = mainData.IM_END_EK.PARK_BILGI != null ? mainData.IM_END_EK.PARK_BILGI : string.Empty,
                REF1 = mainData.IM_END_EK.REF1 != null ? mainData.IM_END_EK.REF1 : string.Empty,
                REF2 = mainData.IM_END_EK.REF2 != null ? mainData.IM_END_EK.REF2 : string.Empty,
                REF3 = mainData.IM_END_EK.REF3 != null ? mainData.IM_END_EK.REF3 : string.Empty,
                ODUL = mainData.IM_END_EK.ODUL != null ? mainData.IM_END_EK.ODUL : string.Empty,
                HV_BOLGE = mainData.IM_END_EK.HV_BOLGE != null ? mainData.IM_END_EK.HV_BOLGE : string.Empty,
                FAB_SUBE = mainData.IM_END_EK.FAB_SUBE != null ? mainData.IM_END_EK.FAB_SUBE : string.Empty,
                MU_SATIS = mainData.IM_END_EK.MU_SATIS != null ? mainData.IM_END_EK.MU_SATIS : string.Empty,
                MARKA = mainData.IM_END_EK.MARKA != null ? mainData.IM_END_EK.MARKA : string.Empty,
                ACIKLAMA = mainData.IM_END_EK.ACIKLAMA != null ? mainData.IM_END_EK.ACIKLAMA : string.Empty,
                EY_TOFAS_CALIS = mainData.IM_END_EK.EY_TOFAS_CALIS != null ? mainData.IM_END_EK.EY_TOFAS_CALIS : string.Empty,
                AD_SOYAD = mainData.IM_END_EK.AD_SOYAD != null ? mainData.IM_END_EK.AD_SOYAD : string.Empty,
                YAKIN_CALISAN = mainData.IM_END_EK.YAKIN_CALISAN != null ? mainData.IM_END_EK.YAKIN_CALISAN : string.Empty,
                FIRMA_AD_SOYAD = mainData.IM_END_EK.FIRMA_AD_SOYAD != null ? mainData.IM_END_EK.FIRMA_AD_SOYAD : string.Empty
            };
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_02> IM_FAALIYET = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_02>();
            foreach (var item in mainData.IM_FAALIYET)
            {
                IM_FAALIYET.Add(new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_02
                {
                    ANA_FAL = item.ANA_FAL != null ? item.ANA_FAL : string.Empty,
                    FAALIYET = item.FAALIYET != null ? item.FAALIYET : string.Empty
                });
            }
            ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_01 IM_GENEL = new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_01
            {
                ADRES = mainData.IM_GENEL.ADRES != null ? mainData.IM_GENEL.ADRES : string.Empty,
                SOKAK = mainData.IM_GENEL.SOKAK != null ? mainData.IM_GENEL.SOKAK : string.Empty,
                CADDE = mainData.IM_GENEL.CADDE != null ? mainData.IM_GENEL.CADDE : string.Empty,
                ILC_MAH = mainData.IM_GENEL.ILC_MAH != null ? mainData.IM_GENEL.ILC_MAH : string.Empty,
                ULKE = mainData.IM_GENEL.ULKE != null ? mainData.IM_GENEL.ULKE : string.Empty,
                IL = mainData.IM_GENEL.IL != null ? mainData.IM_GENEL.IL : string.Empty,
                POSTA_KODU = mainData.IM_GENEL.POSTA_KODU != null ? mainData.IM_GENEL.POSTA_KODU : string.Empty,
                EMAIL = mainData.IM_GENEL.EMAIL != null ? mainData.IM_GENEL.EMAIL : string.Empty,
                TEL_NO = mainData.IM_GENEL.TEL_NO != null ? mainData.IM_GENEL.TEL_NO : string.Empty,
                FAX_NO = mainData.IM_GENEL.FAX_NO != null ? mainData.IM_GENEL.FAX_NO : string.Empty,
                MOBIL_NO = mainData.IM_GENEL.MOBIL_NO != null ? mainData.IM_GENEL.MOBIL_NO : string.Empty,
                VERGI_DAIRESI = mainData.IM_GENEL.VERGI_DAIRESI != null ? mainData.IM_GENEL.VERGI_DAIRESI : string.Empty,
                VERGI_NO = mainData.IM_GENEL.VERGI_NO != null ? mainData.IM_GENEL.VERGI_NO : string.Empty,
                DIL_ING = mainData.IM_GENEL.DIL_ING != null ? mainData.IM_GENEL.DIL_ING : string.Empty,
                DIL_ITL = mainData.IM_GENEL.DIL_ITL != null ? mainData.IM_GENEL.DIL_ITL : string.Empty,
                SIRKET_TIPI = mainData.IM_GENEL.SIRKET_TIPI != null ? mainData.IM_GENEL.SIRKET_TIPI : string.Empty
            };
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_04> IM_HISSE = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_04>();
            foreach (var item in mainData.IM_HISSE)
            {
                IM_HISSE.Add(new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_04
                {
                    H_ID = item.H_ID != null ? item.H_ID : string.Empty,
                    AD_SOYAD = item.AD_SOYAD != null ? item.AD_SOYAD : string.Empty,
                    HISSE_YUZDE1 = item.HISSE_YUZDE1,
                    POZISYONU = item.POZISYONU != null ? item.POZISYONU : string.Empty,
                    DIGER_ISI = item.DIGER_ISI != null ? item.DIGER_ISI : string.Empty
                });
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_15> IM_HISSE_DETAY = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_15>();
            foreach (var item in mainData.IM_HISSE_DETAY)
            {
                IM_HISSE_DETAY.Add(new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_15
                {
                    UNVAN = item.UNVAN != null ? item.UNVAN : string.Empty,
                    ADAY_NO = item.ADAY_NO != null ? item.ADAY_NO : string.Empty,
                    HISSE_ORANI = item.HISSE_ORANI,
                    FAAL_KONU = item.FAAL_KONU != null ? item.FAAL_KONU : string.Empty
                });
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_06> IM_KALITE = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_06>();
            foreach (var item in mainData.IM_KALITE)
            {
                IM_KALITE.Add(new ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_06
                {
                    BELGE_ID = item.BELGE_ID != null ? item.BELGE_ID : string.Empty,
                    BELGE_ADI = item.BELGE_ADI != null ? item.BELGE_ADI : string.Empty,
                    VEREN_KURULUS = item.VEREN_KURULUS != null ? item.VEREN_KURULUS : string.Empty,
                    KAPSAM = item.KAPSAM != null ? item.KAPSAM : string.Empty,
                    ENDDA = item.ENDDA != null ? item.ENDDA : string.Empty,
                    HEDEF_TARIH = item.HEDEF_TARIH != null ? item.HEDEF_TARIH : string.Empty
                });
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_10> IM_MUSTERI = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_10>();
            if (mainData.IM_MUSTERI != null)
            {
                IM_MUSTERI = mainData.IM_MUSTERI;
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_05> IM_SORUMLU = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_05>();
            if (mainData.IM_SORUMLU != null)
            {
                IM_SORUMLU = mainData.IM_SORUMLU;
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_16> IM_TPTL = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_16>();
            if (mainData.IM_TPTL != null)
            {
                IM_TPTL = mainData.IM_TPTL;
            }
            IList<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_11> IM_URETIM = new List<ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_11>();
            if (mainData.IM_URETIM != null)
            {
                IM_URETIM = mainData.IM_URETIM;
            }
            ServiceIntegration.tr.com.tofas.apitest.ZRMM_AT_ST_12 zRMM_AT_ST_12 = srv.SI_ADAY_TEDARIKCI_KAYDET(IM_BANKA.ToArray(), IM_BUS_DEV.ToArray(), IM_DIRECT_EK, IM_END_EK, IM_FAALIYET.ToArray(), IM_GENEL, IM_HISSE.ToArray(), IM_HISSE_DETAY.ToArray(), IM_KALITE.ToArray(), IM_MUSTERI.ToArray(), IM_SORUMLU.ToArray(), IM_TPTL.ToArray(), IM_URETIM.ToArray());




            return Request.CreateResponse(HttpStatusCode.OK, "Başarıyla Kaydedildi.");
        }
    }
}