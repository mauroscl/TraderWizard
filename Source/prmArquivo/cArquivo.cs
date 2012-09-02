using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using System.IO;
namespace prmArquivo
{

	public class cArquivo
	{

		private string strNome;
		private string strCaminho;
		private string strCaminhoCompleto;

		private int intNumero;
		public string CaminhoCompleto {
			get { return this.strCaminhoCompleto; }
		}

		//Public Sub New(ByVal pstrArquivoNome As String)
		//    Me.strNome = pstrArquivoNome
		//    Me.intNumero = FileSystem.FreeFile()
		//    Me.strCaminho = CurDir()
		//    Me.strCaminhoCompleto = Me.strCaminho & "\" & Me.strNome
		//End Sub

		public cArquivo(string pstrCaminhoCompleto)
		{
			this.strCaminhoCompleto = pstrCaminhoCompleto;
			this.intNumero = FileSystem.FreeFile();
		}

		public cArquivo()
		{
			this.intNumero = FileSystem.FreeFile();

		}

		/// <summary>
		/// Abre um arquivo para leitura
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool Abrir()
		{
			try {
				//FileOpen(Me.intNumero, Me.strCaminhoCompleto, OpenMode.Append)
				//FileSystem.FileOpen(Me.intNumero, Me.strCaminhoCompleto, OpenMode.Append)
				FileSystem.FileOpen(this.intNumero, this.strCaminhoCompleto, OpenMode.Input);
				return true;
			} catch (IO.IOException e) {
				Interaction.MsgBox(e.Message, MsgBoxStyle.Critical, "Abrir Arquivo");
				return false;
			}
		}

		/// <summary>
		/// Abre um arquivo para escrita
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool AbrirEscrita()
		{


			try {
				FileSystem.FileOpen(this.intNumero, this.strCaminhoCompleto, OpenMode.Output);

				return true;


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Abrir Arquivo para Escrita");
				return false;

			}

		}

		public bool EscreverString(string pstrString)
		{
			try {
				FileSystem.PrintLine(this.intNumero, pstrString);
				return true;
			} catch (System.IO.IOException e) {
				Interaction.MsgBox(e.Message, MsgBoxStyle.Critical, "Escrever em Arquivo");
				return false;
			}
		}

		public bool Fechar()
		{
			try {
				//FileClose(Me.intNumero)
				FileSystem.FileClose(this.intNumero);
				return true;
			} catch (System.IO.IOException e) {
				Interaction.MsgBox(e.Message, MsgBoxStyle.Critical, "Fechar Arquivo");
				return false;
			}
		}

		public bool LerLinha(ref string pstrLinhaRet)
		{
			try {
				//lê uma linha do arquivo
				//LineInput(intNumero)
				pstrLinhaRet = FileSystem.LineInput(this.intNumero);
				return true;
			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
				return false;
			}
		}

		public bool EOF()
		{


			try {
				return FileSystem.EOF(intNumero);


			} catch (System.IO.IOException ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return true;

			}

		}

		public static bool DiretorioCriar(string pstrDiretorio)
		{


			try {
				//cria o diretório no caminho especificado
				FileSystem.MkDir(pstrDiretorio);

				return true;


			} catch (System.IO.IOException ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}

		//Parâmetros:
		//pstrDiretorio = caminho completo do diretório a ser excluido
		public static bool DiretorioExcluir(string pstrDiretorio)
		{


			try {
				FileSystem.Kill(pstrDiretorio);

				return true;


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}


		//Parâmetros:
		//pstrArquivoOrigem =caminho completo do(s) arquivo(s) a ser(em) movido(s)
		//pstrDiretorioDestino = diretório para o qual serão copiados os arquivos
		public static bool ArquivoMover(string pstrArquivoOrigem, string pstrDiretorioDestino, bool pblnArquivoNaoEncontradoMsgExibir = true)
		{

			string strArquivo = null;


			try {
				strArquivo = FileSystem.Dir(pstrArquivoOrigem);


				while (!string.IsNullOrEmpty(strArquivo)) {
					FileSystem.Rename(strArquivo, pstrDiretorioDestino + "\\" + strArquivo);

					//ArquivoExcluir(pstrDiretorioDestino & "\" & strArquivo)

					strArquivo = FileSystem.Dir();

				}

				return true;


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}


		public static bool ArquivoExcluir(string pstrArquivoCaminho)
		{

			try {
				FileSystem.Kill(pstrArquivoCaminho);

				return true;


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}

		//    'caminho completo do arquivo que será excluido
		//    Public Function ArquivoExcluir(ByVal pstrArquivo As String) As Boolean

		//        On Error GoTo TrataErro

		//        Dim objFSO As FileSystemObject
		//        objFSO = New FileSystemObject

		//        objFSO.DeleteFile(pstrArquivo)

		//        ArquivoExcluir = True

		//        objFSO = Nothing

		//        Exit Function

		//TrataErro:

		//        MsgBox(Err.Description, vbCritical, Err.Source)
		//        ArquivoExcluir = False

		//    End Function

		//Parâmetros:
		//pstrDiretorio = diretório que vai ser verificado a existência;
		public static bool DiretorioExistir(string pstrCaminho)
		{

			string strCaminho = null;


			try {
				strCaminho = FileSystem.Dir(pstrCaminho, FileAttribute.Directory);

				return (!string.IsNullOrEmpty(strCaminho));


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}

		/// <summary>
		/// Verifica se um arquivo existe no caminho especificado
		/// </summary>
		/// <param name="pstrCaminho">Caminho completo do arquivo com diretório e nome do arquivo</param>
		/// <returns></returns>
		/// True = o arquivo existe
		/// False = o arquivo não existe        
		/// <remarks></remarks>
		public static bool ArquivoExistir(string pstrCaminho)
		{

			string strCaminho = null;


			try {
				strCaminho = FileSystem.Dir(pstrCaminho, FileAttribute.Archive);

				return (!string.IsNullOrEmpty(strCaminho));


			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}

		//Descrição: Retorna a lista de arquivos de um diretório de acordo com algum critério
		//Retorno:
		//True = existem arquivos na pasta recebida e nos critérios recebidos
		//False = não existem arquivos
		public bool ArquivosListar(string pstrDiretorio, string pstrCriterio = "*.*", IList<string> pcolArquivosRet = null)
		{

			IList<string> colArquivo = new List<string>();

			string strArquivo = null;


			try {
				strArquivo = FileSystem.Dir(pstrDiretorio + "\\" + pstrCriterio);


				while (!string.IsNullOrEmpty(strArquivo)) {
					colArquivo.Add(strArquivo);

					strArquivo = FileSystem.Dir();

				}

				pcolArquivosRet = colArquivo;

				return (colArquivo.Count) > 0;



			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				pcolArquivosRet = null;

				return false;


			} finally {
				colArquivo = null;

			}

		}

		public bool SalvarStream(Stream objStream)
		{
			bool functionReturnValue = false;

			int intBytesRead = 0;
			byte[] arrByte = {
				
			};

			Array.Resize(ref arrByte, 4096);

			FileStream objFileStream = new FileStream(strCaminhoCompleto, FileMode.Create);


			try {
				//loop through the local file reading each data block
				//and writing to the request stream buffer
				intBytesRead = objStream.Read(arrByte, 0, arrByte.Length);
				while ((intBytesRead > 0)) {
					objFileStream.Write(arrByte, 0, intBytesRead);
					intBytesRead = objStream.Read(arrByte, 0, arrByte.Length);
				}

				functionReturnValue = true;

			} catch (Exception ex) {
				Interaction.MsgBox("Erro ao salvar arquivo " + strCaminhoCompleto + ". Mensagem: " + ex.Message);
				functionReturnValue = false;
			} finally {
				objFileStream.Close();
			}
			return functionReturnValue;


		}


	}
}
