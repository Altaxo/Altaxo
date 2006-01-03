#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Main
{
  /// <summary>
  /// Summary description for AboutDialog.
  /// </summary>
  public class AboutDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Button m_btOK;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.LinkLabel m_LinkLabel;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public AboutDialog()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      // Create a new link using the Add method of the LinkCollection class.
      int len = m_LinkLabel.Text.Length;
      int pos = m_LinkLabel.Text.IndexOf("http://");
      m_LinkLabel.Links.Add(pos,len-pos,"http://sourceforge.net/projects/altaxo");


    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_btOK = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.m_LinkLabel = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // m_btOK
      // 
      this.m_btOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btOK.Location = new System.Drawing.Point(216, 432);
      this.m_btOK.Name = "m_btOK";
      this.m_btOK.TabIndex = 0;
      this.m_btOK.Text = "OK";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.label1.Location = new System.Drawing.Point(192, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(104, 32);
      this.label1.TabIndex = 1;
      this.label1.Text = "Altaxo";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.label2.Location = new System.Drawing.Point(64, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(432, 32);
      this.label2.TabIndex = 2;
      this.label2.Text = "data processing / data plotting program";
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(8, 112);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(496, 312);
      this.textBox1.TabIndex = 3;
      this.textBox1.Text = 
        "\t\t    ACKNOWLEDGEMENTS\r\n" + 
        "\r\n" +
        "    This projects would not be possible without the great contributions of the open\r\n" + 
        "    source community. I want to thank the authors of the projects, that are included\r\n" +
        "    directly or indirectly in this project:\r\n" +
        "\r\n" +
        "- SharpDevelop, (www.icsharpcode.net), from which the GUI and the cool code completion\r\n"+
        "  and syntax highlighting is adopted.\r\n" +
        "- The authors of the projects that are included in SharpDevelop and therefore also in Altaxo\r\n"+
        "\r\n" +
        "- Matpack (B.Gammel, www.matpack.de). A lot of the function library was adopted from the\r\n" +
        "  C++ sources of Matpack.\r\n" +
        "\r\n" +
        "- Exocortex DSP (exocortexdsp.sourceforge.net) contributes the complex library.\r\n" +
        "\r\n" +
        "- dnAnalytics (www.dnAnalytics.net) for part of the linear algebra and optimization functions.\r\n" +
        "\r\n" +
        "- The authors of the projects that I have forgotten (sorry!), and the authors of the\r\n"+
        "  countless support tools that are neccessary to manage such a project (for instance\r\n"+
        "  Subversion, WinMerge, NUnit, NDoc).\r\n" +
        "\r\n" +
        "- My wife for for her support, new ideas and critism.\r\n" +
        "\r\n" +
        "                 Dr. D. Lellinger\r\n" +
        "\r\n" +
        "\t\t    GNU GENERAL PUBLIC LICENSE\r\n\t\t       Version 2, June 1991\r\n\r\n Copyright (C)" +
        " 1989, 1991 Free Software Foundation, Inc.\r\n                       59 Temple Pla" +
        "ce, Suite 330, Boston, MA  02111-1307  USA\r\n Everyone is permitted to copy and d" +
        "istribute verbatim copies\r\n of this license document, but changing it is not all" +
        "owed.\r\n\r\n\t\t\t    Preamble\r\n\r\n  The licenses for most software are designed to tak" +
        "e away your\r\nfreedom to share and change it.  By contrast, the GNU General Publi" +
        "c\r\nLicense is intended to guarantee your freedom to share and change free\r\nsoftw" +
        "are--to make sure the software is free for all its users.  This\r\nGeneral Public " +
        "License applies to most of the Free Software\r\nFoundation\'s software and to any o" +
        "ther program whose authors commit to\r\nusing it.  (Some other Free Software Found" +
        "ation software is covered by\r\nthe GNU Library General Public License instead.)  " +
        "You can apply it to\r\nyour programs, too.\r\n\r\n  When we speak of free software, we" +
        " are referring to freedom, not\r\nprice.  Our General Public Licenses are designed" +
        " to make sure that you\r\nhave the freedom to distribute copies of free software (" +
        "and charge for\r\nthis service if you wish), that you receive source code or can g" +
        "et it\r\nif you want it, that you can change the software or use pieces of it\r\nin " +
        "new free programs; and that you know you can do these things.\r\n\r\n  To protect yo" +
        "ur rights, we need to make restrictions that forbid\r\nanyone to deny you these ri" +
        "ghts or to ask you to surrender the rights.\r\nThese restrictions translate to cer" +
        "tain responsibilities for you if you\r\ndistribute copies of the software, or if y" +
        "ou modify it.\r\n\r\n  For example, if you distribute copies of such a program, whet" +
        "her\r\ngratis or for a fee, you must give the recipients all the rights that\r\nyou " +
        "have.  You must make sure that they, too, receive or can get the\r\nsource code.  " +
        "And you must show them these terms so they know their\r\nrights.\r\n\r\n  We protect y" +
        "our rights with two steps: (1) copyright the software, and\r\n(2) offer you this l" +
        "icense which gives you legal permission to copy,\r\ndistribute and/or modify the s" +
        "oftware.\r\n\r\n  Also, for each author\'s protection and ours, we want to make certa" +
        "in\r\nthat everyone understands that there is no warranty for this free\r\nsoftware." +
        "  If the software is modified by someone else and passed on, we\r\nwant its recipi" +
        "ents to know that what they have is not the original, so\r\nthat any problems intr" +
        "oduced by others will not reflect on the original\r\nauthors\' reputations.\r\n\r\n  Fi" +
        "nally, any free program is threatened constantly by software\r\npatents.  We wish " +
        "to avoid the danger that redistributors of a free\r\nprogram will individually obt" +
        "ain patent licenses, in effect making the\r\nprogram proprietary.  To prevent this" +
        ", we have made it clear that any\r\npatent must be licensed for everyone\'s free us" +
        "e or not licensed at all.\r\n\r\n  The precise terms and conditions for copying, dis" +
        "tribution and\r\nmodification follow.\r\n\r\n\t\t    GNU GENERAL PUBLIC LICENSE\r\n   TERM" +
        "S AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION\r\n\r\n  0. This License" +
        " applies to any program or other work which contains\r\na notice placed by the cop" +
        "yright holder saying it may be distributed\r\nunder the terms of this General Publ" +
        "ic License.  The \"Program\", below,\r\nrefers to any such program or work, and a \"w" +
        "ork based on the Program\"\r\nmeans either the Program or any derivative work under" +
        " copyright law:\r\nthat is to say, a work containing the Program or a portion of i" +
        "t,\r\neither verbatim or with modifications and/or translated into another\r\nlangua" +
        "ge.  (Hereinafter, translation is included without limitation in\r\nthe term \"modi" +
        "fication\".)  Each licensee is addressed as \"you\".\r\n\r\nActivities other than copyi" +
        "ng, distribution and modification are not\r\ncovered by this License; they are out" +
        "side its scope.  The act of\r\nrunning the Program is not restricted, and the outp" +
        "ut from the Program\r\nis covered only if its contents constitute a work based on " +
        "the\r\nProgram (independent of having been made by running the Program).\r\nWhether " +
        "that is true depends on what the Program does.\r\n\r\n  1. You may copy and distribu" +
        "te verbatim copies of the Program\'s\r\nsource code as you receive it, in any mediu" +
        "m, provided that you\r\nconspicuously and appropriately publish on each copy an ap" +
        "propriate\r\ncopyright notice and disclaimer of warranty; keep intact all the\r\nnot" +
        "ices that refer to this License and to the absence of any warranty;\r\nand give an" +
        "y other recipients of the Program a copy of this License\r\nalong with the Program" +
        ".\r\n\r\nYou may charge a fee for the physical act of transferring a copy, and\r\nyou " +
        "may at your option offer warranty protection in exchange for a fee.\r\n\r\n  2. You " +
        "may modify your copy or copies of the Program or any portion\r\nof it, thus formin" +
        "g a work based on the Program, and copy and\r\ndistribute such modifications or wo" +
        "rk under the terms of Section 1\r\nabove, provided that you also meet all of these" +
        " conditions:\r\n\r\n    a) You must cause the modified files to carry prominent noti" +
        "ces\r\n    stating that you changed the files and the date of any change.\r\n\r\n    b" +
        ") You must cause any work that you distribute or publish, that in\r\n    whole or " +
        "in part contains or is derived from the Program or any\r\n    part thereof, to be " +
        "licensed as a whole at no charge to all third\r\n    parties under the terms of th" +
        "is License.\r\n\r\n    c) If the modified program normally reads commands interactiv" +
        "ely\r\n    when run, you must cause it, when started running for such\r\n    interac" +
        "tive use in the most ordinary way, to print or display an\r\n    announcement incl" +
        "uding an appropriate copyright notice and a\r\n    notice that there is no warrant" +
        "y (or else, saying that you provide\r\n    a warranty) and that users may redistri" +
        "bute the program under\r\n    these conditions, and telling the user how to view a" +
        " copy of this\r\n    License.  (Exception: if the Program itself is interactive bu" +
        "t\r\n    does not normally print such an announcement, your work based on\r\n    the" +
        " Program is not required to print an announcement.)\r\n\r\nThese requirements apply " +
        "to the modified work as a whole.  If\r\nidentifiable sections of that work are not" +
        " derived from the Program,\r\nand can be reasonably considered independent and sep" +
        "arate works in\r\nthemselves, then this License, and its terms, do not apply to th" +
        "ose\r\nsections when you distribute them as separate works.  But when you\r\ndistrib" +
        "ute the same sections as part of a whole which is a work based\r\non the Program, " +
        "the distribution of the whole must be on the terms of\r\nthis License, whose permi" +
        "ssions for other licensees extend to the\r\nentire whole, and thus to each and eve" +
        "ry part regardless of who wrote it.\r\n\r\nThus, it is not the intent of this sectio" +
        "n to claim rights or contest\r\nyour rights to work written entirely by you; rathe" +
        "r, the intent is to\r\nexercise the right to control the distribution of derivativ" +
        "e or\r\ncollective works based on the Program.\r\n\r\nIn addition, mere aggregation of" +
        " another work not based on the Program\r\nwith the Program (or with a work based o" +
        "n the Program) on a volume of\r\na storage or distribution medium does not bring t" +
        "he other work under\r\nthe scope of this License.\r\n\r\n  3. You may copy and distrib" +
        "ute the Program (or a work based on it,\r\nunder Section 2) in object code or exec" +
        "utable form under the terms of\r\nSections 1 and 2 above provided that you also do" +
        " one of the following:\r\n\r\n    a) Accompany it with the complete corresponding ma" +
        "chine-readable\r\n    source code, which must be distributed under the terms of Se" +
        "ctions\r\n    1 and 2 above on a medium customarily used for software interchange;" +
        " or,\r\n\r\n    b) Accompany it with a written offer, valid for at least three\r\n    " +
        "years, to give any third party, for a charge no more than your\r\n    cost of phys" +
        "ically performing source distribution, a complete\r\n    machine-readable copy of " +
        "the corresponding source code, to be\r\n    distributed under the terms of Section" +
        "s 1 and 2 above on a medium\r\n    customarily used for software interchange; or,\r" +
        "\n\r\n    c) Accompany it with the information you received as to the offer\r\n    to" +
        " distribute corresponding source code.  (This alternative is\r\n    allowed only f" +
        "or noncommercial distribution and only if you\r\n    received the program in objec" +
        "t code or executable form with such\r\n    an offer, in accord with Subsection b a" +
        "bove.)\r\n\r\nThe source code for a work means the preferred form of the work for\r\nm" +
        "aking modifications to it.  For an executable work, complete source\r\ncode means " +
        "all the source code for all modules it contains, plus any\r\nassociated interface " +
        "definition files, plus the scripts used to\r\ncontrol compilation and installation" +
        " of the executable.  However, as a\r\nspecial exception, the source code distribut" +
        "ed need not include\r\nanything that is normally distributed (in either source or " +
        "binary\r\nform) with the major components (compiler, kernel, and so on) of the\r\nop" +
        "erating system on which the executable runs, unless that component\r\nitself accom" +
        "panies the executable.\r\n\r\nIf distribution of executable or object code is made b" +
        "y offering\r\naccess to copy from a designated place, then offering equivalent\r\nac" +
        "cess to copy the source code from the same place counts as\r\ndistribution of the " +
        "source code, even though third parties are not\r\ncompelled to copy the source alo" +
        "ng with the object code.\r\n\r\n  4. You may not copy, modify, sublicense, or distri" +
        "bute the Program\r\nexcept as expressly provided under this License.  Any attempt\r" +
        "\notherwise to copy, modify, sublicense or distribute the Program is\r\nvoid, and w" +
        "ill automatically terminate your rights under this License.\r\nHowever, parties wh" +
        "o have received copies, or rights, from you under\r\nthis License will not have th" +
        "eir licenses terminated so long as such\r\nparties remain in full compliance.\r\n\r\n " +
        " 5. You are not required to accept this License, since you have not\r\nsigned it. " +
        " However, nothing else grants you permission to modify or\r\ndistribute the Progra" +
        "m or its derivative works.  These actions are\r\nprohibited by law if you do not a" +
        "ccept this License.  Therefore, by\r\nmodifying or distributing the Program (or an" +
        "y work based on the\r\nProgram), you indicate your acceptance of this License to d" +
        "o so, and\r\nall its terms and conditions for copying, distributing or modifying\r\n" +
        "the Program or works based on it.\r\n\r\n  6. Each time you redistribute the Program" +
        " (or any work based on the\r\nProgram), the recipient automatically receives a lic" +
        "ense from the\r\noriginal licensor to copy, distribute or modify the Program subje" +
        "ct to\r\nthese terms and conditions.  You may not impose any further\r\nrestrictions" +
        " on the recipients\' exercise of the rights granted herein.\r\nYou are not responsi" +
        "ble for enforcing compliance by third parties to\r\nthis License.\r\n\r\n  7. If, as a" +
        " consequence of a court judgment or allegation of patent\r\ninfringement or for an" +
        "y other reason (not limited to patent issues),\r\nconditions are imposed on you (w" +
        "hether by court order, agreement or\r\notherwise) that contradict the conditions o" +
        "f this License, they do not\r\nexcuse you from the conditions of this License.  If" +
        " you cannot\r\ndistribute so as to satisfy simultaneously your obligations under t" +
        "his\r\nLicense and any other pertinent obligations, then as a consequence you\r\nmay" +
        " not distribute the Program at all.  For example, if a patent\r\nlicense would not" +
        " permit royalty-free redistribution of the Program by\r\nall those who receive cop" +
        "ies directly or indirectly through you, then\r\nthe only way you could satisfy bot" +
        "h it and this License would be to\r\nrefrain entirely from distribution of the Pro" +
        "gram.\r\n\r\nIf any portion of this section is held invalid or unenforceable under\r\n" +
        "any particular circumstance, the balance of the section is intended to\r\napply an" +
        "d the section as a whole is intended to apply in other\r\ncircumstances.\r\n\r\nIt is " +
        "not the purpose of this section to induce you to infringe any\r\npatents or other " +
        "property right claims or to contest validity of any\r\nsuch claims; this section h" +
        "as the sole purpose of protecting the\r\nintegrity of the free software distributi" +
        "on system, which is\r\nimplemented by public license practices.  Many people have " +
        "made\r\ngenerous contributions to the wide range of software distributed\r\nthrough " +
        "that system in reliance on consistent application of that\r\nsystem; it is up to t" +
        "he author/donor to decide if he or she is willing\r\nto distribute software throug" +
        "h any other system and a licensee cannot\r\nimpose that choice.\r\n\r\nThis section is" +
        " intended to make thoroughly clear what is believed to\r\nbe a consequence of the " +
        "rest of this License.\r\n\r\n  8. If the distribution and/or use of the Program is r" +
        "estricted in\r\ncertain countries either by patents or by copyrighted interfaces, " +
        "the\r\noriginal copyright holder who places the Program under this License\r\nmay ad" +
        "d an explicit geographical distribution limitation excluding\r\nthose countries, s" +
        "o that distribution is permitted only in or among\r\ncountries not thus excluded. " +
        " In such case, this License incorporates\r\nthe limitation as if written in the bo" +
        "dy of this License.\r\n\r\n  9. The Free Software Foundation may publish revised and" +
        "/or new versions\r\nof the General Public License from time to time.  Such new ver" +
        "sions will\r\nbe similar in spirit to the present version, but may differ in detai" +
        "l to\r\naddress new problems or concerns.\r\n\r\nEach version is given a distinguishin" +
        "g version number.  If the Program\r\nspecifies a version number of this License wh" +
        "ich applies to it and \"any\r\nlater version\", you have the option of following the" +
        " terms and conditions\r\neither of that version or of any later version published " +
        "by the Free\r\nSoftware Foundation.  If the Program does not specify a version num" +
        "ber of\r\nthis License, you may choose any version ever published by the Free Soft" +
        "ware\r\nFoundation.\r\n\r\n  10. If you wish to incorporate parts of the Program into " +
        "other free\r\nprograms whose distribution conditions are different, write to the a" +
        "uthor\r\nto ask for permission.  For software which is copyrighted by the Free\r\nSo" +
        "ftware Foundation, write to the Free Software Foundation; we sometimes\r\nmake exc" +
        "eptions for this.  Our decision will be guided by the two goals\r\nof preserving t" +
        "he free status of all derivatives of our free software and\r\nof promoting the sha" +
        "ring and reuse of software generally.\r\n\r\n\t\t\t    NO WARRANTY\r\n\r\n  11. BECAUSE THE" +
        " PROGRAM IS LICENSED FREE OF CHARGE, THERE IS NO WARRANTY\r\nFOR THE PROGRAM, TO T" +
        "HE EXTENT PERMITTED BY APPLICABLE LAW.  EXCEPT WHEN\r\nOTHERWISE STATED IN WRITING" +
        " THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES\r\nPROVIDE THE PROGRAM \"AS IS\" WITHOUT" +
        " WARRANTY OF ANY KIND, EITHER EXPRESSED\r\nOR IMPLIED, INCLUDING, BUT NOT LIMITED " +
        "TO, THE IMPLIED WARRANTIES OF\r\nMERCHANTABILITY AND FITNESS FOR A PARTICULAR PURP" +
        "OSE.  THE ENTIRE RISK AS\r\nTO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH " +
        "YOU.  SHOULD THE\r\nPROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY " +
        "SERVICING,\r\nREPAIR OR CORRECTION.\r\n\r\n  12. IN NO EVENT UNLESS REQUIRED BY APPLIC" +
        "ABLE LAW OR AGREED TO IN WRITING\r\nWILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY " +
        "WHO MAY MODIFY AND/OR\r\nREDISTRIBUTE THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO" +
        " YOU FOR DAMAGES,\r\nINCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL D" +
        "AMAGES ARISING\r\nOUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NO" +
        "T LIMITED\r\nTO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED" +
        " BY\r\nYOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER\r" +
        "\nPROGRAMS), EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE\r\nPOSSIBIL" +
        "ITY OF SUCH DAMAGES.\r\n\r\n\t\t     END OF TERMS AND CONDITIONS\r\n\r\n\t    How to Apply " +
        "These Terms to Your New Programs\r\n\r\n  If you develop a new program, and you want" +
        " it to be of the greatest\r\npossible use to the public, the best way to achieve t" +
        "his is to make it\r\nfree software which everyone can redistribute and change unde" +
        "r these terms.\r\n\r\n  To do so, attach the following notices to the program.  It i" +
        "s safest\r\nto attach them to the start of each source file to most effectively\r\nc" +
        "onvey the exclusion of warranty; and each file should have at least\r\nthe \"copyri" +
        "ght\" line and a pointer to where the full notice is found.\r\n\r\n    <one line to g" +
        "ive the program\'s name and a brief idea of what it does.>\r\n    Copyright (C) <ye" +
        "ar>  <name of author>\r\n\r\n    This program is free software; you can redistribute" +
        " it and/or modify\r\n    it under the terms of the GNU General Public License as p" +
        "ublished by\r\n    the Free Software Foundation; either version 2 of the License, " +
        "or\r\n    (at your option) any later version.\r\n\r\n    This program is distributed i" +
        "n the hope that it will be useful,\r\n    but WITHOUT ANY WARRANTY; without even t" +
        "he implied warranty of\r\n    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE." +
        "  See the\r\n    GNU General Public License for more details.\r\n\r\n    You should ha" +
        "ve received a copy of the GNU General Public License\r\n    along with this progra" +
        "m; if not, write to the Free Software\r\n    Foundation, Inc., 59 Temple Place, Su" +
        "ite 330, Boston, MA  02111-1307  USA\r\n\r\n\r\nAlso add information on how to contact" +
        " you by electronic and paper mail.\r\n\r\nIf the program is interactive, make it out" +
        "put a short notice like this\r\nwhen it starts in an interactive mode:\r\n\r\n    Gnom" +
        "ovision version 69, Copyright (C) year name of author\r\n    Gnomovision comes wit" +
        "h ABSOLUTELY NO WARRANTY; for details type `show w\'.\r\n    This is free software," +
        " and you are welcome to redistribute it\r\n    under certain conditions; type `sho" +
        "w c\' for details.\r\n\r\nThe hypothetical commands `show w\' and `show c\' should show" +
        " the appropriate\r\nparts of the General Public License.  Of course, the commands " +
        "you use may\r\nbe called something other than `show w\' and `show c\'; they could ev" +
        "en be\r\nmouse-clicks or menu items--whatever suits your program.\r\n\r\nYou should al" +
        "so get your employer (if you work as a programmer) or your\r\nschool, if any, to s" +
        "ign a \"copyright disclaimer\" for the program, if\r\nnecessary.  Here is a sample; " +
        "alter the names:\r\n\r\n  Yoyodyne, Inc., hereby disclaims all copyright interest in" +
        " the program\r\n  `Gnomovision\' (which makes passes at compilers) written by James" +
        " Hacker.\r\n\r\n  <signature of Ty Coon>, 1 April 1989\r\n  Ty Coon, President of Vice" +
        "\r\n\r\nThis General Public License does not permit incorporating your program into\r" +
        "\nproprietary programs.  If your program is a subroutine library, you may\r\nconsid" +
        "er it more useful to permit linking proprietary applications with the\r\nlibrary. " +
        " If this is what you want to do, use the GNU Library General\r\nPublic License ins" +
        "tead of this License.\r\n";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.Location = new System.Drawing.Point(312, 16);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(192, 16);
      this.label3.TabIndex = 4;
      this.label3.Text = "(C) 2002-2006 Dr. Dirk Lellinger";
      // 
      // m_LinkLabel
      // 
      this.m_LinkLabel.Location = new System.Drawing.Point(16, 80);
      this.m_LinkLabel.Name = "m_LinkLabel";
      this.m_LinkLabel.Size = new System.Drawing.Size(488, 16);
      this.m_LinkLabel.TabIndex = 5;
      this.m_LinkLabel.TabStop = true;
      this.m_LinkLabel.Text = "You can obtain the latest version of Altaxo from http://sourceforge.net/projects/" +
        "altaxo";
      this.m_LinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_LinkLabel_LinkClicked);
      // 
      // AboutDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(512, 458);
      this.Controls.Add(this.m_LinkLabel);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_btOK);
      this.Name = "AboutDialog";
      this.Text = "About Altaxo";
      this.ResumeLayout(false);

    }
    #endregion

    private void m_LinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
      // Determine which link was clicked within the LinkLabel.
      m_LinkLabel.Links[m_LinkLabel.Links.IndexOf(e.Link)].Visited = true;
      // Display the appropriate link based on the value of the LinkData property of the Link object.
      System.Diagnostics.Process.Start(e.Link.LinkData.ToString());

    }
  }
}
