using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class NeweggServices : INeweggServices
    {
        private readonly Random _rnd = new Random();
        
        public Task<string> GenerateRandomString(int length)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length]; // 36

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_rnd.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return Task.FromResult(finalString);
        }

        public Task<string> GenerateRandomDigitString(int length)
        {
            var chars = "0123456789";
            var stringChars = new char[length]; //21

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_rnd.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return Task.FromResult(finalString);
        }
        
        public async Task<AccertifyJsonClass> GetPayload(string signupUrl)
        {
            var pageId = await NextLong(_rnd, 1000000000000000, 9999999999999999);
            var sessionId = await GenerateRandomString(36);
            var events = await AccertifyEvents(signupUrl, pageId, sessionId);

            var accertifyClass = new AccertifyJsonClass
            {
                eventSource = "web",
                deviceTransactionID = "NEWEGG" + await GenerateRandomDigitString(21),
                uBAID = await GenerateRandomString(36),
                uBAEvents = events,
                uBASessionID = sessionId,
                pageID = pageId
            };

            return accertifyClass;
        }
        
        public Task<string> AccertifyEvents(string signupUrl, long pageId, string sessionId)
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int startTime = (int) Math.Round(t.TotalMilliseconds - _rnd.Next(30000,60000));

            var t2 = startTime + _rnd.Next(3000, 10000);
            var t3 = t2 + _rnd.Next(3000, 10000);
            
            NeweggEvents.Root[] eventArray = new NeweggEvents.Root[3];
            var event1 = new NeweggEvents.Root
            {
                loc = signupUrl,
                pid = pageId,
                sid = null,
                bsid = sessionId,
                ts = startTime,
                type = "mtrk",
                pay = new NeweggEvents.Pay
                {
                    t = startTime,
                    fd = 1044.27,
                    sd = 1557.69,
                    bb = new List<int>
                    {
                        _rnd.Next(500, 800),
                        _rnd.Next(100,300),
                        _rnd.Next(1000,1400),
                        _rnd.Next(100,300)
                    },
                    s = new List<NeweggEvents.S>
                    {
                        new NeweggEvents.S
                        {
                            t = 0,
                            x = 1324,
                            y = 274,
                            fd = 945.72,
                            sd = 829.03,
                            c = 15,
                            a = 9139.97,
                            mx = 119066.74,
                            mn = 62.5
                        },
                        new NeweggEvents.S
                        {
                            t = 260,
                            x = 771,
                            y = 613,
                            fd = 652.69,
                            sd = 648.64,
                            c = 24,
                            a = 2455.57,
                            mx = 8766.41,
                            mn = 90.91
                        },
                        new NeweggEvents.S
                        {
                            t = 551,
                            x = 769,
                            y = 693,
                            fd = 80.32,
                            sd = 80.02,
                            c = 22,
                            a = 330.8,
                            mx = 1004.99,
                            mn = 83.33
                        }
                    },
                    c = 50,
                    sc = 3
                }
            };

            var m1 = new
            {
                t = 0,
                b = 0,
                x = _rnd.Next(500, 800),
                y = _rnd.Next(500, 800)
            };
            var event2 = new NeweggEvents.Root
            {
                loc = signupUrl,
                pid = pageId,
                sid = null,
                bsid = sessionId,
                ts = t2,
                type = "mclk",
                pay = new NeweggEvents.Pay
                {
                    t = t2,
                    m = new NeweggEvents.M
                    {
                        _ = new List<object>
                        {
                            m1
                        }
                    },
                    c = 1
                }
            };
            
            var event3 = new NeweggEvents.Root
            {
                loc = signupUrl,
                pid = pageId,
                sid = null,
                bsid = sessionId,
                ts = t3,
                type = "meta",
                pay = new NeweggEvents.Pay
                {
                    t = t3,
                    m = new NeweggEvents.M
                    {
                        _ = new List<object>
                        {

                        }
                    }
                }
            };
            eventArray[0] = event1;
            eventArray[1] = event2;
            eventArray[2] = event3;
            
            var jsonEncoded = JsonConvert.SerializeObject(eventArray);
            var urlEncodedJson = HttpUtility.UrlEncode(jsonEncoded, Encoding.UTF8);
            
            var t23 = DateTime.UtcNow - new DateTime(1970, 1, 1);

            var semi = new {ts = t23.TotalMilliseconds, pay = urlEncodedJson};
            var semiJson = JsonConvert.SerializeObject(semi);
            var semiJsonBytes = Encoding.UTF8.GetBytes(semiJson);
            var semiB64 = Convert.ToBase64String(semiJsonBytes).Replace('+', '-').Replace('/', '_');
            
            var t232 = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var semiFinal = new SemiFinal
            {
                ts = Math.Round(t232.TotalMilliseconds),
                pays = new List<string>
                {
                    semiB64,
                    null,
                    null
                }
            };
            
            var semiFinalJson = JsonConvert.SerializeObject(semiFinal);
            var semiFinalBytes = Encoding.UTF8.GetBytes(semiFinalJson);
           
            return Task.FromResult(Convert.ToBase64String(semiFinalBytes).Replace('+', '-').Replace('/', '_')); //URL safe b64 encoding, https://github.com/neosmart/UrlBase64/blob/master/UrlBase64/UrlBase64.cs
        }


        public Task<string> EncryptPayload(string stringDataToEncrypt, string stringPublicKey)
        {
            //from https://stackoverflow.com/questions/37413837/c-sharp-rsa-encrypting-text-using-a-given-pkcs1-public-key
            Asn1Object obj = Asn1Object.FromByteArray(Convert.FromBase64String(stringPublicKey));
            
            DerSequence publicKeySequence = (DerSequence)obj;

            DerBitString encodedPublicKey = (DerBitString)publicKeySequence[1];
            DerSequence publicKey = (DerSequence)Asn1Object.FromByteArray(encodedPublicKey.GetBytes());

            DerInteger modulus = (DerInteger) publicKey[0];
            DerInteger exponent = (DerInteger) publicKey[1];
            
            RsaKeyParameters keyParameters = new RsaKeyParameters(false, modulus.PositiveValue, exponent.PositiveValue);
            
            RSAParameters parameters = DotNetUtilities.ToRSAParameters(keyParameters);
            
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);
            
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(stringDataToEncrypt);
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false); //false for pkcs#1 1.5
            return Task.FromResult(Convert.ToBase64String(encryptedData));
        }
        
        public Task<long> NextLong(Random random, long min, long max)
        {
            //from https://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            //Working with ulong so that modulo works correctly with values > long.MaxValue
            ulong uRange = (ulong)(max - min);

            //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return Task.FromResult((long)(ulongRand % uRange) + min);
        }
    }
    
    public class NeweggEvents
    {
        public class S
        {
            public int t { get; set; }
            public int x { get; set; } 
            public int y { get; set; }
            public double fd { get; set; }
            public double sd { get; set; }
            public int c { get; set; }
            public double a { get; set; }
            public double mx { get; set; }
            public double mn { get; set; }
        }

        public class M
        { 
            public object _ { get; set; }
        }

        public class Pay
        {
            public int t { get; set; }
            public double fd { get; set; }
            public double sd { get; set; }
            public List<int> bb { get; set; }
            public List<S> s { get; set; }
            public int c { get; set; }
            public int sc { get; set; }
            public M m { get; set; }
        }

        public class Root 
        { 
            public string loc { get; set; }
            public long pid { get; set; }
            public object sid { get; set; }
            public string bsid { get; set; } 
            public int ts { get; set; }
            public string type { get; set; } 
            public Pay pay { get; set; }
        }
    }
    
    public class SemiFinal
    {
        public double ts { get; set; } 
        public List<string> pays { get; set; }
    }
    
    public class AccertifyJsonClass
    {
        public string eventSource { get; set; }
        public string deviceTransactionID { get; set; }
        public string uBAID { get; set; }
        public string uBAEvents { get; set; }
        public string uBASessionID { get; set; }
        public long pageID { get; set; }
    }
}