using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
 // ERROR: Not supported in C#: OptionDeclaration
namespace prmArquivo
{

	static class mZip
	{
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

		// Visual Basic declares file for
		//
		//     azip32.dll    addZIP 32-bit compression library
		//

		// Function declarations
		public static extern int addZIP();
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Abort(int bFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_ArchiveName(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_BuildSFX(int iFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_ClearAttributes(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Comment(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Delete(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_DeleteComment(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_DisplayComment(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Encrypt(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Exclude(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_ExcludeListFile(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_GetLastError();
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_GetLastWarning();
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Include(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeArchive(int iFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeDirectoryEntries(int flag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeFilesNewer(string DateVal);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeFilesOlder(string DateVal);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeHidden(int iFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeListFile(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeReadOnly(int iFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_IncludeSystem(int iFlag);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern void addZIP_Initialise();
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_InstallCallback(long cbFunction);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Overwrite(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Recurse(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Register(string lpStr, long Uint32);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SaveAttributes(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SaveRelativeTo(string szPath);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SaveStructure(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SetArchiveDate(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SetCompressionLevel(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SetParentWindowHandle(long Hwnd);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SetTempDrive(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_SetWindowHandle(long Hwnd);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Span(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_Store(string lpStr);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_UseLFN(int Int16);
		[DllImport("azip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addZIP_View(int Int16);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

		// Visual Basic declares file for
		//
		//     aunzip32.dll  addUNZIP 32-bit decompression library
		//

		public static extern long addUNZIP();
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Abort(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_ArchiveName(string filename);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Decrypt(string cPassword);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_DisplayComment(int bFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Exclude(string files);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_ExcludeListFile(string cFile);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_ExtractTo(string cPath);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Freshen(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_GetLastError();
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_GetLastWarning();
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Include(string files);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_IncludeListFile(string cFile);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern void addUNZIP_Initialise();
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_InstallCallback(long fn);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Overwrite(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Register(string cName, long iNumber);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern void addUNZIP_ResetDefaults();
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_RestoreAttributes(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_RestoreStructure(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_SetParentWindowHandle(long Hwnd);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_SetWindowHandle(long Hwnd);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Test(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_ToMemory(string lpStr, long Uint32);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_Update(int iFlag);
		[DllImport("aunzip32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int addUNZIP_View(int bFlag);


		// Visual Basic constants file for
		//
		//     azip32.dll        addZIP 32-bit compression library
		//     aunzip32.dll      addUNZIP 32-bit compression library
		//

		// Function declarations
		//  constants for addZIP_SetCompressionLevel(...)

		public const int azCOMPRESSION_MAXIMUM = 0x3;
		public const int azCOMPRESSION_MINIMUM = 0x1;
		public const int azCOMPRESSION_NONE = 0x0;

		public const int azCOMPRESSION_NORMAL = 0x2;
		// constants for addZIP_SaveStructure(...)
		public const int azSTRUCTURE_ABSOLUTE = 0x2;
		public const int azSTRUCTURE_NONE = 0x0;

		public const int azSTRUCTURE_RELATIVE = 0x1;
		// constants for addZIP_Overwrite(...)
		// constants for addUNZIP_Overwrite(...)
		public const int azOVERWRITE_ALL = 0xb;
		public const int azOVERWRITE_NONE = 0xc;

		public const int  azOVERWRITE_QUERY = 0xa;
		// constants for addZIP_SetArchiveDate()
		public const int DATE_NEWEST = 0x3;
		public const int DATE_OLDEST = 0x2;
		public const int DATE_ORIGINAL = 0x0;

		public const int  DATE_TODAY = 0x1;
		// constants for addZIP_IncludeXXX attribute functions
			// files must never have this attribute set
		public const int  azNEVER = 0x0;
			// files may or may not have this attribute set
		public const int azALWAYS = 0xff;
			// files must always have this attribute set
		public const int azYES = 0x1;

		//  constants for addZIP_ClearAttributes(...)
		// constants for addUNZIP_RestoreAttributes(...)
		public const int  azATTR_NONE = 0;
		public const int azATTR_READONLY = 1;
		public const int  azATTR_HIDDEN = 2;
		public const int azATTR_SYSTEM = 4;
		public const int azATTR_ARCHIVE = 32;

		public const int azATTR_ALL = 39;
		// constants used in messages to identify libraries
		public const int azLIBRARY_ADDZIP = 0;

		public const int azLIBRARY_ADDUNZIP = 1;
		// 'messages' used to provide information to the calling program
		public const int AM_SEARCHING = 0xa;
		public const int AM_ZIPCOMMENT = 0xb;
		public const int AM_ZIPPING = 0xc;
		public const int AM_ZIPPED = 0xd;
		public const int AM_UNZIPPING = 0xe;
		public const int AM_UNZIPPED = 0xf;
		public const int AM_TESTING = 0x10;
		public const int AM_TESTED = 0x11;
		public const int AM_DELETING = 0x12;
		public const int AM_DELETED = 0x13;
		public const int AM_DISKCHANGE = 0x14;
		public const int AM_VIEW = 0x15;
		public const int AM_ERROR = 0x16;
		public const int AM_WARNING = 0x17;
		public const int AM_QUERYOVERWRITE = 0x18;
		public const int AM_COPYING = 0x19;
		public const int AM_COPIED = 0x1a;

		public const int AM_ABORT = 0xff;
		// Constants for whether file is encrypted or not in AM_VIEW
		public const int azFT_ENCRYPTED = 0x1;

		public const int azFT_NOT_ENCRYPTED = 0x0;
		// Constants for whether file is text or binary in AM_VIEW
		public const int azFT_BINARY = 0x1;

		public const int azFT_TEXT = 0x0;
		// Constants for compression method in AM_VIEW
		public const int azCM_DEFLATED_FAST = 0x52;
		public const int azCM_DEFLATED_MAXIMUM = 0x51;
		public const int azCM_DEFLATED_NORMAL = 0x50;
		public const int azCM_DEFLATED_SUPERFAST = 0x53;
		public const int azCM_IMPLODED = 0x3c;
		public const int azCM_NONE = 0x0;
		public const int azCM_REDUCED_1 = 0x14;
		public const int azCM_REDUCED_2 = 0x1e;
		public const int azCM_REDUCED_3 = 0x28;
		public const int  azCM_REDUCED_4 = 0x32;
		public const int azCM_SHRUNK = 0xa;
		public const int azCM_TOKENISED = 0x46;

		public const int azCM_UNKNOWN = 0xff;
		// Constants used in returning from a AM_QUERYOVERWRITE message
		public const int azOW_NO = 0x2;
		public const int azOW_NO_TO_ALL = 0x3;
		public const int azOW_YES = 0x0;
		public const int azOW_YES_TO_ALL = 0x1;
		// Apenas para retorno das funções
		//Public Z As Integer

		public static long Z;




	}
}
