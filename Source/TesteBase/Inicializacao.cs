using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
namespace TesteBase
{

	public abstract class Inicializacao
	{

		public static cConexao objConexao { get; set; }
		public static void Inicializa()
		{
			objConexao = new cConexao();
		}


		public static void Finaliza()
		{
			objConexao.FecharConexao();

		}

	}
}
