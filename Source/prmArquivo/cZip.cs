using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Compression;
namespace prmArquivo
{

	public class cZip
	{

		public bool ArquivoDescompactar(string pstrArquivoCompactado, string pstrCaminhoDestino, string pstrArquivoDestino)
		{


			try {
				FileStream arquivoOriginal = File.OpenRead(pstrArquivoCompactado);
				FileStream arquivoDestino = File.Create(pstrCaminhoDestino + "\\" + pstrArquivoDestino);
				GZipStream zip = new GZipStream(arquivoOriginal, CompressionMode.Decompress, false);
				byte[] arquivoEmBytes = new byte[(Convert.ToInt32(arquivoOriginal.Length)) + 1];
				Int32 qtdBytes = zip.Read(arquivoEmBytes, 0, arquivoEmBytes.Length);
				arquivoDestino.Write(arquivoEmBytes, 0, qtdBytes);
				zip.Close();
				arquivoDestino.Close();

				return true;


			} catch (System.IO.IOException ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);

				return false;

			}

		}


		public int GetAction(string cFrom)
		{
			return Conversion.Val(GetPiece(cFrom, "|", 2));
		}

		public long GetFileCompressedSize(string cFrom)
		{
			return Conversion.Val(GetPiece(cFrom, "|", 6));
		}

		public int GetFileCompressionRatio(string cFrom)
		{
			return Conversion.Val(GetPiece(cFrom, "|", 7));
		}

		public string GetFileName(string cFrom)
		{
			return GetPiece(cFrom, "|", 4);
		}

		public long GetFileOriginalSize(string cFrom)
		{
			return Conversion.Val(GetPiece(cFrom, "|", 5));
		}

		public int GetPercentComplete(string cFrom)
		{
			return Conversion.Val(GetPiece(cFrom, "|", 7));
		}

		public string GetPiece(string fromX, string delim, int Index)
		{
			string functionReturnValue = null;
			//Tipo de ação retornada pelo arquivo ou compactação
			dynamic TempAux = null;
			int Count = 0;
			int WhereX = 0;

			TempAux = fromX + delim;
			WhereX = Strings.InStr(TempAux, delim);
			Count = 0;
			while ((WhereX > 0)) {
				Count = Count + 1;
				if ((Count == Index)) {
					functionReturnValue = Strings.Left(TempAux, WhereX - 1);
					return functionReturnValue;
				}
				Temp = Strings.Right(TempAux, Strings.Len(TempAux) - WhereX);
				WhereX = Strings.InStr(TempAux, delim);
			}
			if ((Count == 0)) {
				functionReturnValue = fromX;
			} else {
				functionReturnValue = "";
			}
			return functionReturnValue;
		}

		//---------------------------------------------------------------------
		//Rotinas para o addZIP
		//---------------------------------------------------------------------
		public void Compactar(string cArqCompactado, string cArq)
		{
			//Compacta um ou mais arquivos no formato WinZip
			mZip.Z = mZip.addZIP_SetCompressionLevel(mZip.azCOMPRESSION_MAXIMUM);
			//Z = addZIP_SaveStructure(SalvaDir) 'StoreFullPathName - azSTRUCTURE_ABSOLUTE
			mZip.Z = mZip.addZIP_Include(cArq);
			mZip.Z = mZip.addZIP_ArchiveName(cArqCompactado);
			//Z = addZIP_Delete(DeletarOrig)
			mZip.Z = mZip.addZIP();
		}

		//Descrição: Descompacta um ou mais arquivos no formato WinZip
		//Parâmetros:
		//cArqCompactado = arquivo ZIP
		//cNomeArq = nome dos arquivos que deve ser extraidos
		//ExtrairPara = diretório onde os arquivos serão descompactados
		//MontaDir = indica se deve manter a estrutura de diretórios ao descompactar os arquivos
		public void DesCompactar(string cArqCompactado, string cNomeArq, string ExtrairPara, bool MontaDir)
		{
			mZip.Z = mZip.addUNZIP_Overwrite(mZip.azOVERWRITE_ALL);
			mZip.Z = mZip.addUNZIP_ArchiveName(cArqCompactado);
			mZip.Z = mZip.addUNZIP_Include(cNomeArq);
			mZip.Z = mZip.addUNZIP_ExtractTo(ExtrairPara);
			mZip.Z = mZip.addUNZIP_RestoreStructure(MontaDir);
			mZip.Z = mZip.addUNZIP();
		}

		public void ListaConteudoArquivo(string cArquivo)
		{
			//Lista o conteudo de um arquivo zipado.
			mZip.Z = mZip.addZIP_ArchiveName(cArquivo);
			mZip.Z = mZip.addZIP_View(true);
			mZip.Z = mZip.addZIP();
		}

		//Public Sub ZipInicializar(ByVal F As Object, ByVal TextoZip As Object)

		public void ZipInicializar()
		{

			try {
				// Inicializa as bibliotecas do addZIP
				// É necessário um form e um TextBox
				mZip.addZIP_Initialise();
				mZip.addUNZIP_Initialise();
			//Z = addZIP_SetParentWindowHandle(F.Hwnd)
			//Z = addUNZIP_SetParentWindowHandle(F.Hwnd)
			//Z = addZIP_SetWindowHandle(TextoZip.Hwnd)
			//Z = addUNZIP_SetWindowHandle(TextoZip.Hwnd)

			} catch (Exception ex) {
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical);
			}

		}

		public string TipoAcao(long nTipo)
		{
			string functionReturnValue = null;
			switch (nTipo) {
				case mZip.AM_SEARCHING:
					functionReturnValue = "Procurando";
					break;
				case mZip.AM_ZIPCOMMENT:
					functionReturnValue = "Comentário";
					break;
				case mZip.AM_ZIPPING:
					functionReturnValue = "Zipando";
					break;
				case mZip.AM_ZIPPED:
					functionReturnValue = "Zipado";
					break;
				case mZip.AM_UNZIPPING:
					functionReturnValue = "Deszipando";
					break;
				case mZip.AM_UNZIPPED:
					functionReturnValue = "Deszipado";
					break;
				case mZip.AM_TESTING:
					functionReturnValue = "Testando";
					break;
				case mZip.AM_TESTED:
					functionReturnValue = "Testado";
					break;
				case mZip.AM_DELETING:
					functionReturnValue = "Deletando";
					break;
				case mZip.AM_DELETED:
					functionReturnValue = "Deletado";
					break;
				case mZip.AM_DISKCHANGE:
					functionReturnValue = "Troca Disco";
					break;
				case mZip.AM_VIEW:
					functionReturnValue = "Visualizar";
					break;
				case mZip.AM_ERROR:
					functionReturnValue = "Erro";
					break;
				case mZip.AM_WARNING:
					functionReturnValue = "Aviso";
					break;
				case mZip.AM_QUERYOVERWRITE:
					functionReturnValue = "Sobrescrever";
					break;
				case mZip.AM_COPYING:
					functionReturnValue = "Copiando";
					break;
				case mZip.AM_COPIED:
					functionReturnValue = "Copiado";
					break;
				case mZip.AM_ABORT:
					functionReturnValue = "Abortando";
					break;
				default:
					functionReturnValue = "-=Desconhecido=-";
					break;
			}
			return functionReturnValue;

		}



	}
}
