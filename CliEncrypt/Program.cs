using ErpCode.Com;
using Library.Core;
using Newtonsoft.Json;
using System;

namespace CliEncrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            //LibAuthenData authenData = new LibAuthenData();
            //authenData.AuthenticateID = Guid.NewGuid().ToString();
            //authenData.Authenticator = "erpcodebyzyy";
            //authenData.ExpirationDT = DateTime.MaxValue;
            //authenData.Password = "123456789";
            //authenData.Validaccounts =999;
            //string datastr = JsonConvert.SerializeObject(authenData);
            //string ss= RSAHelp.EncryptPrivekey(datastr, "<RSAKeyValue><Modulus>oJyjkVjFqjOJNWcR32VOur1Gl+tWK/2DEA6Jhq6UV+d1wZHqZNlwKtK+al0nGdvpq5GjcQcMi7+VS+qrLqVPelVW7XVUGmlUfzXu1rMBCQpQ27+4xvFAgo70hMHBns2tZ97UpYmBUS7aCssJu8S4G+r7KUXt/DDrQArgjOW7gldFMfZL32+fjjp8f0r84AOYZd4VO4BgGOv0N/raj4OGTIeE/xTtbSTdvk2FcL1bDuwyaiDmn69bwZGjSe0S+0IoHlq2Q0kpdcs7KIFsx/r3O90ZDLOFJ5vfiWNzeZ196zGvvp8BHUdagjjkFw6wYSzTLMMcGawsmbHk+Vx9y8KLmQ==</Modulus><Exponent>AQAB</Exponent><P>0mb4s7EwivPF+/xNbZmEUxjDnYJ+KqgUYb7W0629kOIsUOWPh1BsjF6eyffbPVvT+koDG6lTg2e/zN6i0ANMtnokRtENF4v0z89+bFG+XHX88ypcWed5o9US/nRMWFxP8+NKmQmdFiwDj5gvIal979EsqiVmCr/dmyt5TUd4zuM=</P><Q>w2tRoIkQGIndHQOvJP/yIOzTpXHCHTDbzQekLVbcbbq0bmHSMr8obP/uz2JTu2KhurdOxx8Kme9aC/UZvCM4sLLwWTJplgJbR4zHnbthZqwHDRZQgj1eTnaZh4uocLmNklNp63jXimJnOt5D24KzbC/vZLCgcy+gkMGCysLsKFM=</Q><DP>GrX33Ngh0YIhIFxneAKFSgdaziRC4Cd2bvJLU5Q6/Km59osuLX+ISEm63ukYbDG8N4ot2tUkRQpmzxt0j/PbSU84S8U+ZD7gVYGCCz5VbGeONVCraL56MtJaFvOsvYr6m3u/fK47wxTTXudzFFLwKgHkKbWVvsec2pYqUCVCsQE=</DP><DQ>B9VOcVPtLD6ieV5DcAU+bGlx1mjn8gmaVe6fex3HOjLItr//D/+vnCbqKK/1UUaT2wssAjlRUHhSsLYDcRIsXJDfp4OozdWMWSLggYcYUWhF5BVn5sCU/ios300O1G3rqdjkHwc6BJcLMkXkjHEBxi1puCyIUfWvxv2qw3vqwXc=</DQ><InverseQ>H+eQ6cdvsYb6sqloxyz0GsFLYQvsCfxgJWmMGGtj9vhW5l4jL2itsM47LNAMBSkugJ0/I87ztNon0yYhCe1GeG2xehYaZ/Xbzg4wvmY/ysEWzohllMe6gd6TbYhRMhd2fl0ZawX25tOXASzQydvSYSPix8exJAsmKvx9Iv+BMR0=</InverseQ><D>UC6jAhb9Z4EfCMgSBiL/cP65VXKprIB9g/Cd+90ANfpZsW91Qy/Cbb6UTWl/8cr2Fy84F19bhhD5KdNX7ouc6Afew+GFQUJAatm0CP+IjsjVoT9PshITEJrpu7U3Ql6QKYD3TEUfiDYJqMXAcDs/ZsCi7vH4Y6TQbUKA5BZ0zBaW3P5vPcn9ERNjGtU8/UfNVztEE0oOUTMSWR1or47yrGBn6NPP2u/RJXQdGw2uS1k8NqzS0BXBQUnYi7SiO2aYNFu3E9my/ddeAcHMAlkY46acWI8ipTzHHb5LRCdTUAUaTr5jwq9dIjhMVwAkMVkxH1vgpVr7ngEAawtdAXIEeQ==</D></RSAKeyValue>");

            Console.WriteLine("please input ClientId:");
            string clid= Console.ReadLine();
            Console.WriteLine("please input ClientSecret:");
            string secret = Console.ReadLine();
            Console.WriteLine("please input prive key:");
            string key = Console.ReadLine();
            Cliinfo c = new Cliinfo { ClientId = clid, Secret = secret };
            string result1 = DesCryptFactory.EncryptString(clid, AppConstManage ._cliidEncrykey);
            string result = RSAHelp.EncryptPrivekey(JsonConvert.SerializeObject(c), key);
            Console.WriteLine(string.Format("许可证密文：{0}",string.Format("{0};{1}",result1 , result)));
            Console.WriteLine("按任意建退出");
            Console.ReadLine();
        }

        public class Cliinfo
        {
            public string ClientId { get; set; }
            public string Secret { get; set; }
        }
    }
}
