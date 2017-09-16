using System.Collections.Generic;
using Ionic.Zip;
using Services;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ArquivoTextoService
    {
        public ICollection<string> LerLinhas(string pstrCaminho, string pstrArquivoZip, string pstrArquivoTexto)
        {
            var zipFile = new ZipFile(pstrCaminho + "\\" + pstrArquivoZip);

            zipFile.ExtractAll(pstrCaminho);

            IFileService fileService = new FileService();

            //o conteúdo do arquivo zipado é um arquivo chamado BDIN, sem extensão
            //abre o arquivo para leitura
            var linhas = fileService.ReadAllLines(pstrCaminho + "\\" + pstrArquivoTexto);

            //apaga o arquivo BDIN que foi extraído do zip.
            fileService.Delete(pstrCaminho + "\\" + pstrArquivoTexto);

            return linhas;

        }
    }
}
