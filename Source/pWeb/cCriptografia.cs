using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace pWeb
{

	public class cCriptografia
	{

		private RSAParameters objPrivateParameters;

		private RSAParameters objPublicParameters;

		/// <summary>
		/// Criptografa uma string, retornando outra string criptografada utilizando o algoritmo RSA
		/// </summary>
		/// <param name="pstrTexto">Texto que será criptografado</param>
		/// <returns>Texto criptografado</returns>
		/// <remarks></remarks>
		/// 

		public string Criptografar(string pstrTexto)
		{

			string strAux = "";

		    try {
				//Create a UnicodeEncoder to convert between byte array and string.
				UnicodeEncoding ByteConverter = new UnicodeEncoding();

				//Create byte arrays to hold original, encrypted, and decrypted data.
				byte[] dataToEncrypt = ByteConverter.GetBytes(pstrTexto);
				byte[] encryptedData;

				//Create a new instance of RSACryptoServiceProvider to generate
				//public and private key data.
				//RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

				//Pass the data to ENCRYPT, the public key information 
				//(using RSACryptoServiceProvider.ExportParameters(false),
				//and a boolean flag specifying no OAEP padding.
				encryptedData = RSAEncrypt(dataToEncrypt, objPublicParameters, false);


			    for (int intI = 0; intI <= encryptedData.Length - 1; intI++) {
					//converte o byte para hexadecimal. Quando a conversão retornar apenas um caracter
					//coloca um zero à esquerda, usando a função PadLeft;
					strAux = strAux + Conversion.Hex(encryptedData[intI]).PadLeft(2, '0');
				}



				//return the decrypted plaintext in hexadecimal format
				return strAux;

			} catch (ArgumentNullException) {
				return string.Empty;
			}

		}

		/// <summary>
		/// Descriptografa uma string que foi criptograda utilizando o algoritmo RSA.
		/// </summary>
		/// <param name="pstrTexto">Texto criptografado</param>
		/// <returns>String contendo o texto descriptografado</returns>
		/// <remarks></remarks>
		public string Descriptograr(string pstrTexto)
		{
		    byte[] dataToDecrypt = {
				
			};

			Array.Resize(ref dataToDecrypt, 128);

			int intArrayPosition = 0;

			try {
				//Create a UnicodeEncoder to convert between byte array and string.
				UnicodeEncoding ByteConverter = new UnicodeEncoding();

				//Create byte arrays to hold original, encrypted, and decrypted data.
				//Dim dataToDecrypt As Byte() = ByteConverter.GetBytes(pstrTexto)

				//percorre a string de caracteres dois a dois, que é um conjunto de caracteres em hexadecimal.
				//Converte cada número de hexadecimal para byte

				for (int intI = 0; intI < pstrTexto.Length; intI += 2) {
					//usa base 16 (hexadecimal) para converter de hexadecimal para byte.
                    dataToDecrypt[intArrayPosition] = Convert.ToByte(pstrTexto.Substring(intI, 2), 16);

					intArrayPosition = intArrayPosition + 1;

				}

			    //Create a new instance of RSACryptoServiceProvider to generate
				//public and private key data.
				//RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

				//Pass the data to DECRYPT, the private key information 
				//(using RSACryptoServiceProvider.ExportParameters(true),
				//and a boolean flag specifying no OAEP padding.
				byte[] decryptedData = RSADecrypt(dataToDecrypt, objPrivateParameters, false);

				return ByteConverter.GetString(decryptedData);


			} catch (Exception) {
				return string.Empty;

			}

		}

		private static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
		{
			try {
				//Create a new instance of RSACryptoServiceProvider.
				RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

				//Import the RSA Key information. This only needs
				//toinclude the public key information.
				RSA.ImportParameters(RSAKeyInfo);

				//Encrypt the passed byte array and specify OAEP padding.  
				//OAEP padding is only available on Microsoft Windows XP or
				//later.  
				return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
			//Catch and display a CryptographicException  
			//to the console.
			} catch (CryptographicException e) {
                MessageBox.Show("Erro ao criptografar dados. Erro: " + e.Message, "Criptografia", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}


		private static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
		{
			try {
				//Create a new instance of RSACryptoServiceProvider.
				RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

				//Import the RSA Key information. This needs
				//to include the private key information.
				RSA.ImportParameters(RSAKeyInfo);

				//Decrypt the passed byte array and specify OAEP padding.  
				//OAEP padding is only available on Microsoft Windows XP or
				//later.  
				return RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
			//Catch and display a CryptographicException  
			//to the console.

			} catch (CryptographicException e) {
                MessageBox.Show("Erro ao criptografar dados. Erro: " + e.Message, "Criptografia", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return null;
			}
		}


		public cCriptografia()
		{
			objPublicParameters = new RSAParameters();

			byte[] arrExponent = {
				1,
				0,
				1
			};

			objPublicParameters.Exponent = arrExponent;

			byte[] arrModulus = {
				218,
				131,
				253,
				142,
				16,
				182,
				31,
				9,
				182,
				9,
				127,
				137,
				155,
				169,
				48,
				43,
				255,
				52,
				137,
				100,
				246,
				181,
				127,
				250,
				138,
				2,
				253,
				108,
				222,
				35,
				94,
				0,
				140,
				46,
				169,
				29,
				228,
				99,
				208,
				149,
				101,
				249,
				194,
				25,
				13,
				83,
				156,
				97,
				53,
				231,
				125,
				154,
				152,
				219,
				28,
				41,
				83,
				29,
				161,
				185,
				89,
				253,
				144,
				63,
				24,
				218,
				94,
				182,
				213,
				249,
				59,
				12,
				18,
				43,
				74,
				121,
				232,
				104,
				237,
				233,
				124,
				80,
				251,
				74,
				69,
				67,
				191,
				73,
				108,
				130,
				93,
				170,
				122,
				191,
				119,
				25,
				226,
				10,
				202,
				242,
				209,
				13,
				181,
				96,
				176,
				5,
				60,
				100,
				127,
				64,
				204,
				244,
				73,
				86,
				130,
				85,
				21,
				57,
				213,
				187,
				84,
				190,
				164,
				48,
				127,
				138,
				154,
				57
			};

			objPublicParameters.Modulus = arrModulus;

			objPrivateParameters = new RSAParameters();

			byte[] arrD = {
				132,
				243,
				216,
				27,
				79,
				57,
				176,
				74,
				213,
				82,
				148,
				33,
				226,
				239,
				31,
				27,
				53,
				236,
				254,
				71,
				203,
				0,
				5,
				189,
				39,
				169,
				200,
				14,
				44,
				94,
				114,
				124,
				124,
				206,
				139,
				33,
				95,
				236,
				7,
				102,
				79,
				36,
				150,
				159,
				109,
				135,
				88,
				215,
				160,
				215,
				151,
				137,
				175,
				197,
				105,
				46,
				15,
				159,
				48,
				222,
				56,
				205,
				217,
				172,
				26,
				107,
				175,
				30,
				37,
				75,
				169,
				64,
				156,
				167,
				31,
				11,
				219,
				50,
				69,
				115,
				176,
				36,
				144,
				200,
				83,
				21,
				55,
				57,
				248,
				245,
				108,
				7,
				35,
				174,
				134,
				213,
				84,
				9,
				43,
				40,
				52,
				111,
				152,
				249,
				43,
				47,
				77,
				235,
				149,
				74,
				253,
				142,
				42,
				247,
				61,
				62,
				185,
				132,
				36,
				131,
				192,
				30,
				136,
				79,
				147,
				147,
				181,
				173
			};

			objPrivateParameters.D = arrD;

			byte[] arrDP = {
				57,
				94,
				218,
				190,
				182,
				202,
				167,
				128,
				149,
				31,
				0,
				67,
				93,
				252,
				189,
				224,
				69,
				246,
				169,
				225,
				155,
				187,
				185,
				245,
				16,
				247,
				187,
				208,
				191,
				227,
				147,
				168,
				68,
				218,
				103,
				212,
				248,
				253,
				170,
				100,
				42,
				252,
				38,
				163,
				244,
				25,
				45,
				96,
				42,
				126,
				149,
				175,
				246,
				28,
				131,
				149,
				209,
				79,
				192,
				143,
				168,
				58,
				84,
				151
			};

			objPrivateParameters.DP = arrDP;

			byte[] arrDQ = {
				104,
				210,
				105,
				206,
				209,
				46,
				238,
				62,
				194,
				109,
				18,
				236,
				92,
				159,
				80,
				138,
				10,
				138,
				25,
				153,
				35,
				80,
				28,
				151,
				167,
				212,
				71,
				77,
				21,
				10,
				52,
				229,
				109,
				117,
				207,
				48,
				38,
				17,
				94,
				153,
				107,
				105,
				187,
				223,
				255,
				19,
				14,
				118,
				82,
				30,
				243,
				227,
				58,
				150,
				145,
				166,
				103,
				107,
				252,
				128,
				78,
				197,
				194,
				195
			};

			objPrivateParameters.DQ = arrDQ;

			objPrivateParameters.Exponent = arrExponent;

			byte[] arrInverseQ = {
				168,
				201,
				42,
				71,
				201,
				133,
				110,
				196,
				85,
				226,
				107,
				64,
				253,
				50,
				47,
				81,
				7,
				218,
				249,
				143,
				202,
				131,
				38,
				180,
				33,
				213,
				251,
				199,
				230,
				192,
				134,
				139,
				2,
				165,
				196,
				91,
				26,
				109,
				61,
				117,
				53,
				127,
				103,
				108,
				254,
				26,
				47,
				216,
				14,
				138,
				220,
				77,
				16,
				49,
				29,
				113,
				106,
				180,
				117,
				82,
				91,
				71,
				191,
				170
			};

			objPrivateParameters.InverseQ = arrInverseQ;

			objPrivateParameters.Modulus = arrModulus;

			byte[] arrP = {
				245,
				145,
				106,
				114,
				245,
				129,
				136,
				232,
				72,
				228,
				107,
				29,
				80,
				121,
				167,
				8,
				121,
				111,
				180,
				41,
				160,
				89,
				182,
				225,
				68,
				114,
				199,
				122,
				77,
				73,
				10,
				200,
				37,
				161,
				156,
				60,
				241,
				3,
				72,
				148,
				41,
				137,
				174,
				215,
				196,
				109,
				198,
				86,
				164,
				17,
				185,
				235,
				58,
				250,
				186,
				102,
				113,
				22,
				64,
				144,
				18,
				160,
				57,
				139
			};

			objPrivateParameters.P = arrP;

			byte[] arrQ = {
				227,
				204,
				96,
				52,
				237,
				162,
				231,
				225,
				96,
				178,
				27,
				214,
				189,
				115,
				228,
				86,
				113,
				118,
				196,
				252,
				180,
				158,
				224,
				170,
				91,
				230,
				125,
				239,
				158,
				138,
				146,
				136,
				77,
				9,
				242,
				46,
				0,
				233,
				167,
				132,
				176,
				178,
				171,
				34,
				84,
				38,
				167,
				144,
				213,
				142,
				202,
				192,
				40,
				177,
				177,
				28,
				194,
				210,
				225,
				124,
				32,
				140,
				11,
				203
			};

			objPrivateParameters.Q = arrQ;

		}

	}
}
