﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileUploadDemo.Secure
{
 public class RSAClass
 {

 // To generate private key, we can use LinqPad, with following Expression
 // F4 to add reference to System.Security.dll
 // new System.Security.Cryptography.RSACryptoServiceProvider (1024).ToXmlString (true)

   private const string PrivateKey = "<RSAKeyValue><Modulus>s6lpjspk+3o2GOK5TM7JySARhhxE5gB96e9XLSSRuWY2W9F951MfistKRzVtg0cjJTdSk5mnWAVHLfKOEqp8PszpJx9z4IaRCwQ937KJmn2/2VyjcUsCsor+fdbIHOiJpaxBlsuI9N++4MgF/jb0tOVudiUutDqqDut7rhrB/oc=</Modulus><Exponent>AQAB</Exponent><P>3J2+VWMVWcuLjjnLULe5TmSN7ts0n/TPJqe+bg9avuewu1rDsz+OBfP66/+rpYMs5+JolDceZSiOT+ACW2Neuw==</P><Q>0HogL5BnWjj9BlfpILQt8ajJnBHYrCiPaJ4npghdD5n/JYV8BNOiOP1T7u1xmvtr2U4mMObE17rZjNOTa1rQpQ==</Q><DP>jbXh2dVQlKJznUMwf0PUiy96IDC8R/cnzQu4/ddtEe2fj2lJBe3QG7DRwCA1sJZnFPhQ9svFAXOgnlwlB3D4Gw==</DP><DQ>evrP6b8BeNONTySkvUoMoDW1WH+elVAH6OsC8IqWexGY1YV8t0wwsfWegZ9IGOifojzbgpVfIPN0SgK1P+r+kQ==</DQ><InverseQ>LeEoFGI+IOY/J+9SjCPKAKduP280epOTeSKxs115gW1b9CP4glavkUcfQTzkTPe2t21kl1OrnvXEe5Wrzkk8rA==</InverseQ><D>HD0rn0sGtlROPnkcgQsbwmYs+vRki/ZV1DhPboQJ96cuMh5qeLqjAZDUev7V2MWMq6PXceW73OTvfDRcymhLoNvobE4Ekiwc87+TwzS3811mOmt5DJya9SliqU/ro+iEicjO4v3nC+HujdpDh9CVXfUAWebKnd7Vo5p6LwC9nIk=</D></RSAKeyValue>";
   private const string PublicKey = "<RSAKeyValue><Modulus>s6lpjspk+3o2GOK5TM7JySARhhxE5gB96e9XLSSRuWY2W9F951MfistKRzVtg0cjJTdSk5mnWAVHLfKOEqp8PszpJx9z4IaRCwQ937KJmn2/2VyjcUsCsor+fdbIHOiJpaxBlsuI9N++4MgF/jb0tOVudiUutDqqDut7rhrB/oc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";


   private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();

  public static string Decrypt(string data)
  {

    try
    {
    var rsa = new RSACryptoServiceProvider();
    var dataArray = data.Split(new[] { ',' });

    var dataByte = new byte[dataArray.Length];
    for (int i = 0; i < dataArray.Length; i++)
    {
     dataByte[i] = Convert.ToByte(dataArray[i]);
    }

    rsa.FromXmlString(PrivateKey);
    var decryptedByte = rsa.Decrypt(dataByte, false);
    return Encoder.GetString(decryptedByte);
    
   }
   catch (Exception)
   {
    throw new RSAException();
   }

  }

  public static string Encrypt(string data)
  {


   try
   {
    var rsa = new RSACryptoServiceProvider();
    rsa.FromXmlString(PublicKey);
    var dataToEncrypt = Encoder.GetBytes(data);
    var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
    var length = encryptedByteArray.Count();
    var item = 0;
    var sb = new StringBuilder();
    foreach (var x in encryptedByteArray)
    {
     item++;
     sb.Append(x);

     if (item < length)
      sb.Append(",");
    }

    return sb.ToString();
    
   }
   catch (Exception)
   {
    throw new RSAException();
   }
  }

  public class RSAException : Exception {
  
   public RSAException() : base("RSA Encryption Error") {}
  
  }
 }
}