using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDTO;

namespace prjModelo.Entidades
{

	public abstract class cMediaAbstract
	{

		public cCotacaoAbstract Cotacao;
		public string Tipo;
		public int NumPeriodos;

		public double Valor;
		public cMediaAbstract(cCotacaoAbstract pobjCotacao, string pstrTipo, int pintNumPeriodos, double pdecValor)
		{
			Cotacao = pobjCotacao;
			Valor = pdecValor;
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;
		}

		public override bool Equals(object obj)
		{

			var objMedia = (cMediaAbstract)obj;
			if (Cotacao.Equals(objMedia.Cotacao) & Tipo == objMedia.Tipo & NumPeriodos == objMedia.NumPeriodos) {
				return true;
			} else {
				return false;
			}

		}

		public cMediaDTO ObtemDTO()
		{

			string strDado = null;
			string strTipo = null;

			switch (Tipo) {
				case "MME":
					strTipo = "E";
					strDado = "VALOR";
					break;
				case "MMA":
					strTipo = "A";
					strDado = "VALOR";
					break;
				case "VMA":
					strTipo = "A";
					strDado = "VOLUME";
					break;
				case "IFR2":
					strTipo = "A";
					strDado = "IFR2";
					break;
				default:

					throw new Exception("Tipo não encontrado para conversão: " + Tipo);
			}

			return new cMediaDTO(strTipo, NumPeriodos, strDado);

		}


	}

	public class cMediaDiaria : cMediaAbstract
	{

		public cMediaDiaria(cCotacaoAbstract pobjCotacao, string pstrTipo, int pintNumPeriodos, double pdecValor) : base(pobjCotacao, pstrTipo, pintNumPeriodos, pdecValor)
		{
		}

	}
}
