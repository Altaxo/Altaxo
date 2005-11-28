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

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)


using System;

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Fast Fourier Transform class based on the Fast Hartley Transform.
  /// </summary>
  public class FastHartleyTransform
  {

    /*-----------------------------------------------------------------------------*\
    | Fast Fourier Transform, Fast Hartley Transform                   fhttrigtbl.h |
    |                                                                               |
    | trigonometric tables for the associated FHT routines in the file fft.cc       |
    | This algorithm is apparently patented(!) and the code copyrighted.            |
    | See the comment with the fht routine for more info.                           |
    |                                                                               |
    | Matpack Library Release 1.0                                                   |
    | Copyright (C) 1991-1997 by Berndt M. Gammel                                   |
    |                                                                               |
    | Permission to  use, copy, and  distribute  Matpack  in  its entirety  and its |
    | documentation  for non-commercial purpose and  without fee is hereby granted, |
    | provided that this license information and copyright notice appear unmodified |
    | in all copies.  This software is provided 'as is'  without express or implied |
    | warranty.  In no event will the author be held liable for any damages arising |
    | from the use of this software.            |
    | Note that distributing Matpack 'bundled' in with any product is considered to |
    | be a 'commercial purpose'.              |
    | The software may be modified for your own purposes, but modified versions may |
    | not be distributed without prior consent of the author.     |
    |                                                                               |
    | Read the  COPYRIGHT and  README files in this distribution about registration |
    | and installation of Matpack.              |
    |                                                                               |
    \*-----------------------------------------------------------------------------*/


    //----------------------------------------------------------------------------//
    //
    // Due to the finite tables in the original code (n=20) the fft routines failed 
    // if the size of the vector to be transformed exceeded N = 2^17. 
    //
    // I extended the tables to 60 entries with 50 digits precision and added
    // several changes.
    // Berndt M. Gammel 1994.
    //
    //----------------------------------------------------------------------------//


    //----------------------------------------------------------------------------//
    // Since the original fht routine contained pointer variables
    // I had to change the code for fht and I included the original #defines
    // for TRIG_GOOD
    // Dirk Lellinger 2002
    //----------------------------------------------------------------------------//



    //----------------------------------------------------------------------------//
    // table containing sec[pi/(2^n)]/2, n=3,4,5,...
    // with 50 digits precision generated with Mathematica
    // CForm[Table[N[Sec[Pi/(2^n)]/2,50],{n,3,60}]]
    //----------------------------------------------------------------------------//

    private readonly static double[] halsec=
  {
    0.0,
    0.0,
    0.54119610014619698439972320536638942006107206337802,
    0.50979557910415916894193980398784391368261849190894,
    0.50241928618815570551167011928012092247859337193963,
    0.50060299823519630134550410676638239611758632599592,
    0.5001506360206509882147710127109765849597491301034,
    0.50003765191554772296778139077905492847503165398345,
    0.50000941253588775676512870469186533538523133757983,
    0.50000235310628608051401267171204408939326297376426,
    0.50000058827484117879868526730916804925780637276181,
    0.50000014706860214875463798283871198206179118093251,
    0.50000003676714377807315864400643020315103490883972,
    0.50000000919178552207366560348853455333939112569381,
    0.5000000022979463541156288776790686855899192234892,
    0.50000000057448658687873302235147272458812263401372,
    0.50000000014362164661654736863252589967935073278768,
    0.50000000003590541164769084922906986545517021050714,
    0.5000000000089763529115198377492893545710302607766,
    0.50000000000224408822785477977745008538694053435214,
    0.50000000000056102205696212121562052706092204624429,
    0.5000000000001402555142404319458587574589714979267,
    0.50000000000003506387856010183908679097586112838458,
    0.50000000000000876596964002507556057909474235175031,
    0.50000000000000219149241000624487694985811043883448,
    0.50000000000000054787310250155971841278230418295286,
    0.50000000000000013696827562538983580165293708187947,
    0.50000000000000003424206890634745308781681933523359,
    0.50000000000000000856051722658686290554192890035621,
    0.50000000000000000214012930664671570348471497924829,
    0.50000000000000000053503232666167892443988079194703,
    0.50000000000000000013375808166541973102051407593269,
    0.50000000000000000003343952041635493274953751135479,
    0.50000000000000000000835988010408873318703493986192,
    0.50000000000000000000208997002602218329673689509193,
    0.50000000000000000000052249250650554582418285878089,
    0.50000000000000000000013062312662638645604562938322,
    0.5000000000000000000000326557816565966140114020138,
    0.5000000000000000000000081639454141491535028501702,
    0.50000000000000000000000204098635353728837571252172,
    0.50000000000000000000000051024658838432209392812913,
    0.5000000000000000000000001275616470960805234820322,
    0.50000000000000000000000003189041177402013087050805,
    0.50000000000000000000000000797260294350503271762701,
    0.50000000000000000000000000199315073587625817940675,
    0.50000000000000000000000000049828768396906454485169,
    0.50000000000000000000000000012457192099226613621292,
    0.50000000000000000000000000003114298024806653405323,
    0.50000000000000000000000000000778574506201663351331,
    0.50000000000000000000000000000194643626550415837833,
    0.50000000000000000000000000000048660906637603959458,
    0.50000000000000000000000000000012165226659400989865,
    0.50000000000000000000000000000003041306664850247466,
    0.50000000000000000000000000000000760326666212561867,
    0.50000000000000000000000000000000190081666553140467,
    0.50000000000000000000000000000000047520416638285117,
    0.50000000000000000000000000000000011880104159571279,
    0.5000000000000000000000000000000000297002603989282,
    0.50000000000000000000000000000000000742506509973205,
    0.50000000000000000000000000000000000185626627493301
  };


    //----------------------------------------------------------------------------//
    // table containing cos[pi/(2^n)], n=0,1,2,...
    // with 50 digits precision generated with Mathematica
    // CForm[Table[N[Cos[Pi/(2^n)],50],{n,1,60}]]
    //----------------------------------------------------------------------------//

    private readonly static double[] costab=
  {
    0.0,
    0.70710678118654752440084436210484903928483593768847,
    0.92387953251128675612818318939678828682241662586364,
    0.98078528040323044912618223613423903697393373089334,
    0.99518472667219688624483695310947992157547486872986,
    0.99879545620517239271477160475910069444320361470461,
    0.99969881869620422011576564966617219685006108125773,
    0.99992470183914454092164649119638322435060646880222,
    0.99998117528260114265699043772856771617391725094434,
    0.99999529380957617151158012570011989955298763362219,
    0.99999882345170190992902571017152601904826792288977,
    0.9999997058628822191602282177387656771162638993493,
    0.99999992646571785114473148070738785694820115568892,
    0.99999998161642929380834691540290971450507605124278,
    0.99999999540410731289097193313960614895889430318945,
    0.99999999885102682756267330779455410840053741619428,
    0.99999999971275670684941397221864177608908945791829,
    0.99999999992818917670977509588385049026048033310952,
    0.99999999998204729417728262414778410737963918749628,
    0.99999999999551182354431058417299732444151460146531,
    0.99999999999887795588607701655175253650384922191052,
    0.99999999999971948897151921479471958445201800950003,
    0.99999999999992987224287980123972873675821054314673,
    0.99999999999998246806071995015624773672987531894852,
    0.99999999999999561701517998752945665621623895533496,
    0.99999999999999890425379499688176383418117037102579,
    0.99999999999999972606344874922040343792823700725817,
    0.99999999999999993151586218730509851444349327772074,
    0.99999999999999998287896554682627448204596294604932,
    0.99999999999999999571974138670656861135118383817603,
    0.99999999999999999892993534667664215226527677839799,
    0.99999999999999999973248383666916053803053674577787,
    0.99999999999999999993312095916729013450539778339312,
    0.99999999999999999998328023979182253362620967065757,
    0.99999999999999999999582005994795563340654368171497,
    0.9999999999999999999989550149869889083516353744319,
    0.99999999999999999999973875374674722708790880948317,
    0.99999999999999999999993468843668680677197720023799,
    0.9999999999999999999999836721091717016929942999262,
    0.99999999999999999999999591802729292542324857497322,
    0.99999999999999999999999897950682323135581214374278,
    0.99999999999999999999999974487670580783895303593566,
    0.99999999999999999999999993621917645195973825898391,
    0.99999999999999999999999998405479411298993456474598,
    0.99999999999999999999999999601369852824748364118649,
    0.99999999999999999999999999900342463206187091029662,
    0.99999999999999999999999999975085615801546772757416,
    0.99999999999999999999999999993771403950386693189354,
    0.99999999999999999999999999998442850987596673297338,
    0.99999999999999999999999999999610712746899168324335,
    0.99999999999999999999999999999902678186724792081084,
    0.99999999999999999999999999999975669546681198020271,
    0.99999999999999999999999999999993917386670299505068,
    0.99999999999999999999999999999998479346667574876267,
    0.99999999999999999999999999999999619836666893719067,
    0.99999999999999999999999999999999904959166723429767,
    0.99999999999999999999999999999999976239791680857442,
    0.9999999999999999999999999999999999405994792021436,
    0.9999999999999999999999999999999999851498698005359,
    0.99999999999999999999999999999999999628746745013398
  };


    //----------------------------------------------------------------------------//
    // table containing sin[pi/(2^n)], n=0,1,2,...
    // with 50 digits precision generated with Mathematica
    // CForm[Table[N[Sin[Pi/(2^n)],50],{n,1,60}]]
    //----------------------------------------------------------------------------//

    private readonly static double[] sintab=
  {
    1.0,
    0.70710678118654752440084436210484903928483593768847,
    0.38268343236508977172845998403039886676134456248563,
    0.19509032201612826784828486847702224092769161775195,
    0.098017140329560601994195563888641845861136673167501,
    0.049067674327418014254954976942682658314745363025753,
    0.024541228522912288031734529459282925065466119239451,
    0.0122715382857199260794082619510032121403723195917693,
    0.0061358846491544753596402345903725809170578863173913,
    0.003067956762965976270145365490919842518944610213452,
    0.00153398018628476561230369715026407907995486457523739,
    0.00076699031874270452693856835794857664314091945206328,
    0.00038349518757139558907246168118138126339502603496474,
    0.00019174759731070330743990956198900093346887403385916,
    0.000095873799095977345870517210976476351187065612851145,
    0.000047936899603066884549003990494658872746866687685767,
    0.000023968449808418218729186577165021820094761474895673,
    0.0000119842249050697064215215615969889848047319775383867,
    5.9921124526424278428797118088908617299871778780951e-6,
    2.9960562263346607504548128083570598118251878683408e-6,
    1.49802811316901122885427884615536112069175858615266e-6,
    7.4901405658471572113049856673065563715595930217207e-7,
    3.7450702829238412390316917908463317739740476297248e-7,
    1.8725351414619534486882457659356361712045272098287e-7,
    9.3626757073098082799067286680885620193236507169473e-8,
    4.681337853654909269511551813854009695950362701667e-8,
    2.3406689268274552759505493419034844037886207223779e-8,
    1.17033446341372771812462135032381037980934566399759e-8,
    5.8516723170686386908097901008341396943900085051757e-9,
    2.9258361585343193579282304690689559020175857150074e-9,
    1.4629180792671596805295321618659637103742615227834e-9,
    7.3145903963357984046044319684941757518633453150407e-10,
    3.6572951981678992025468123791426325259552149285834e-10,
    1.8286475990839496013039807389332593442313170468753e-10,
    9.1432379954197480065581218813687255727239618372001e-11,
    4.5716189977098740032838382140221663928079104822215e-11,
    2.2858094988549370016425162661783086472096966705718e-11,
    1.14290474942746850082133277798505750495556602128132e-11,
    5.7145237471373425041067571960451665014662272161861e-12,
    2.8572618735686712520533902612875681228191632468869e-12,
    1.42863093678433562602669658855190717042033782829491e-12,
    7.1431546839216781301334847651446897383651343975396e-13,
    3.5715773419608390650667426103704891049654978557779e-13,
    1.785788670980419532533371333660012581955615260015e-13,
    8.9289433549020976626668567038935229466191592152324e-14,
    4.4644716774510488313334283563959439779147149720109e-14,
    2.2322358387255244156667141787541198020329994065548e-14,
    1.11611791936276220783335708944657837765095494334606e-14,
    5.5805895968138110391667854473197899840478437668161e-15,
    2.7902947984069055195833927236707572539980555146688e-15,
    1.39514739920345275979169636183673640974579446124199e-15,
    6.9757369960172637989584818091853792771624306860945e-16,
    3.4878684980086318994792409045929017921353976405328e-16,
    1.7439342490043159497396204522964774152619716074521e-16,
    8.7196712450215797486981022614824202253026990212426e-17,
    4.359835622510789874349051130741214256275454633619e-17,
    2.1799178112553949371745255653706076460907404571842e-17,
    1.08995890562769746858726278268530388778949687113896e-17,
    5.4497945281384873429363139134265195198776426588784e-18,
    2.7248972640692436714681569567132597700550911173371e-18
  };



    //----------------------------------------------------------------------------//
    // table containing cos[pi/(2^n)], n=0,1,2,...
    // with 50 digits precision generated with Mathematica
    // CForm[Table[N[Cos[Pi/(2^n)],50],{n,1,60}]]
    //----------------------------------------------------------------------------//

    private readonly static double[] coswrk=
  {
    0.0,
    0.70710678118654752440084436210484903928483593768847,
    0.92387953251128675612818318939678828682241662586364,
    0.98078528040323044912618223613423903697393373089334,
    0.99518472667219688624483695310947992157547486872986,
    0.99879545620517239271477160475910069444320361470461,
    0.99969881869620422011576564966617219685006108125773,
    0.99992470183914454092164649119638322435060646880222,
    0.99998117528260114265699043772856771617391725094434,
    0.99999529380957617151158012570011989955298763362219,
    0.99999882345170190992902571017152601904826792288977,
    0.9999997058628822191602282177387656771162638993493,
    0.99999992646571785114473148070738785694820115568892,
    0.99999998161642929380834691540290971450507605124278,
    0.99999999540410731289097193313960614895889430318945,
    0.99999999885102682756267330779455410840053741619428,
    0.99999999971275670684941397221864177608908945791829,
    0.99999999992818917670977509588385049026048033310952,
    0.99999999998204729417728262414778410737963918749628,
    0.99999999999551182354431058417299732444151460146531,
    0.99999999999887795588607701655175253650384922191052,
    0.99999999999971948897151921479471958445201800950003,
    0.99999999999992987224287980123972873675821054314673,
    0.99999999999998246806071995015624773672987531894852,
    0.99999999999999561701517998752945665621623895533496,
    0.99999999999999890425379499688176383418117037102579,
    0.99999999999999972606344874922040343792823700725817,
    0.99999999999999993151586218730509851444349327772074,
    0.99999999999999998287896554682627448204596294604932,
    0.99999999999999999571974138670656861135118383817603,
    0.99999999999999999892993534667664215226527677839799,
    0.99999999999999999973248383666916053803053674577787,
    0.99999999999999999993312095916729013450539778339312,
    0.99999999999999999998328023979182253362620967065757,
    0.99999999999999999999582005994795563340654368171497,
    0.9999999999999999999989550149869889083516353744319,
    0.99999999999999999999973875374674722708790880948317,
    0.99999999999999999999993468843668680677197720023799,
    0.9999999999999999999999836721091717016929942999262,
    0.99999999999999999999999591802729292542324857497322,
    0.99999999999999999999999897950682323135581214374278,
    0.99999999999999999999999974487670580783895303593566,
    0.99999999999999999999999993621917645195973825898391,
    0.99999999999999999999999998405479411298993456474598,
    0.99999999999999999999999999601369852824748364118649,
    0.99999999999999999999999999900342463206187091029662,
    0.99999999999999999999999999975085615801546772757416,
    0.99999999999999999999999999993771403950386693189354,
    0.99999999999999999999999999998442850987596673297338,
    0.99999999999999999999999999999610712746899168324335,
    0.99999999999999999999999999999902678186724792081084,
    0.99999999999999999999999999999975669546681198020271,
    0.99999999999999999999999999999993917386670299505068,
    0.99999999999999999999999999999998479346667574876267,
    0.99999999999999999999999999999999619836666893719067,
    0.99999999999999999999999999999999904959166723429767,
    0.99999999999999999999999999999999976239791680857442,
    0.9999999999999999999999999999999999405994792021436,
    0.9999999999999999999999999999999999851498698005359,
    0.99999999999999999999999999999999999628746745013398
  };

    //----------------------------------------------------------------------------//
    // table containing sin[pi/(2^n)], n=0,1,2,...[
    // with 50 digits precision generated with Mathematica
    // CForm[Table[N[Sin[Pi/(2^n)],50],{n,1,60}]]
    //----------------------------------------------------------------------------//

    private readonly static double[] sinwrk=
  {
    1.0,
    0.70710678118654752440084436210484903928483593768847,
    0.38268343236508977172845998403039886676134456248563,
    0.19509032201612826784828486847702224092769161775195,
    0.098017140329560601994195563888641845861136673167501,
    0.049067674327418014254954976942682658314745363025753,
    0.024541228522912288031734529459282925065466119239451,
    0.0122715382857199260794082619510032121403723195917693,
    0.0061358846491544753596402345903725809170578863173913,
    0.003067956762965976270145365490919842518944610213452,
    0.00153398018628476561230369715026407907995486457523739,
    0.00076699031874270452693856835794857664314091945206328,
    0.00038349518757139558907246168118138126339502603496474,
    0.00019174759731070330743990956198900093346887403385916,
    0.000095873799095977345870517210976476351187065612851145,
    0.000047936899603066884549003990494658872746866687685767,
    0.000023968449808418218729186577165021820094761474895673,
    0.0000119842249050697064215215615969889848047319775383867,
    5.9921124526424278428797118088908617299871778780951e-6,
    2.9960562263346607504548128083570598118251878683408e-6,
    1.49802811316901122885427884615536112069175858615266e-6,
    7.4901405658471572113049856673065563715595930217207e-7,
    3.7450702829238412390316917908463317739740476297248e-7,
    1.8725351414619534486882457659356361712045272098287e-7,
    9.3626757073098082799067286680885620193236507169473e-8,
    4.681337853654909269511551813854009695950362701667e-8,
    2.3406689268274552759505493419034844037886207223779e-8,
    1.17033446341372771812462135032381037980934566399759e-8,
    5.8516723170686386908097901008341396943900085051757e-9,
    2.9258361585343193579282304690689559020175857150074e-9,
    1.4629180792671596805295321618659637103742615227834e-9,
    7.3145903963357984046044319684941757518633453150407e-10,
    3.6572951981678992025468123791426325259552149285834e-10,
    1.8286475990839496013039807389332593442313170468753e-10,
    9.1432379954197480065581218813687255727239618372001e-11,
    4.5716189977098740032838382140221663928079104822215e-11,
    2.2858094988549370016425162661783086472096966705718e-11,
    1.14290474942746850082133277798505750495556602128132e-11,
    5.7145237471373425041067571960451665014662272161861e-12,
    2.8572618735686712520533902612875681228191632468869e-12,
    1.42863093678433562602669658855190717042033782829491e-12,
    7.1431546839216781301334847651446897383651343975396e-13,
    3.5715773419608390650667426103704891049654978557779e-13,
    1.785788670980419532533371333660012581955615260015e-13,
    8.9289433549020976626668567038935229466191592152324e-14,
    4.4644716774510488313334283563959439779147149720109e-14,
    2.2322358387255244156667141787541198020329994065548e-14,
    1.11611791936276220783335708944657837765095494334606e-14,
    5.5805895968138110391667854473197899840478437668161e-15,
    2.7902947984069055195833927236707572539980555146688e-15,
    1.39514739920345275979169636183673640974579446124199e-15,
    6.9757369960172637989584818091853792771624306860945e-16,
    3.4878684980086318994792409045929017921353976405328e-16,
    1.7439342490043159497396204522964774152619716074521e-16,
    8.7196712450215797486981022614824202253026990212426e-17,
    4.359835622510789874349051130741214256275454633619e-17,
    2.1799178112553949371745255653706076460907404571842e-17,
    1.08995890562769746858726278268530388778949687113896e-17,
    5.4497945281384873429363139134265195198776426588784e-18,
    2.7248972640692436714681569567132597700550911173371e-18
  };



    /*-----------------------------------------------------------------------------*\
    | Fast Fourier Transform based on Fast Hartley Transform              fhtfft.cc |
    |                                                                               |
    | Last change: Jun 22, 2001             |
    |                                                                               |
    | Matpack Library Release 1.6.2                                                 |
    | Copyright (C) 1991-2001 by Berndt M. Gammel. All rights reserved.             |
    |                                                                               |
    | Permission to  use, copy, and  distribute  Matpack  in  its entirety  and its |
    | documentation  for non-commercial purpose and  without fee is hereby granted, |
    | provided that this license information and copyright notice appear unmodified |
    | in all copies.  This software is provided 'as is'  without express or implied |
    | warranty.  In no event will the author be held liable for any damages arising |
    | from the use of this software.            |
    | Note that distributing Matpack 'bundled' in with any product is considered to |
    | be a 'commercial purpose'.              |
    | The software may be modified for your own purposes, but modified versions may |
    | not be distributed without prior consent of the author.     |
    |                                                                               |
    | Read the  COPYRIGHT and  README files in this distribution about registration |
    | and installation of Matpack.              |
    |                                                                               |
    \*-----------------------------------------------------------------------------*/

    // #include "fht.h"

    //-----------------------------------------------------------------------------//
    // 
    // The following algorithms for the 'best and fastest implementation' of the
    // FFT in C are due to a public posting in the newsgroups
    //
    //    sci.math.num-analysis,sci.math,comp.lang.c,comp.lang.c++,comp.dsp
    // by 
    //    Ron Mayer, 19 Mar 1993, from  ACUSON, Mountain View, CA
    //    mayer@acuson.com
    //
    // This seems to be a piece of really smart code for doing the fast Fourier
    // transform FFT by means of a fast Hartley transform FHT. It is superior to
    // the code published in the Numerical Recipies and some other published code.
    // That is the reason why I decided to include it into the MatPack C++ library.
    // I left the original code unchanged exept for introducing prototypes for all 
    // functions according to the ANSI standard. All functions now compile with
    // ANSI C and C++. Note that the file "trigtbl.h" is included.
    //
    // Improvements:
    // -------------
    // The FFT routines in their original version competely failed if the vector
    // exceeded a certain size. This was due to the limited size of the trig tables
    // in "fhttrigtbl.h". I (B. Gammel) extended the tables to 60 entries with 50 digits 
    // precision (plus some other changes) which should be sufficient also for the
    // next three computer generations.
    //
    // Important Note:
    // ---------------
    // Ron Mayer notes (below) that the Fast Hartley Trasform code below is 
    // restricted by patents (U.S. Patent No. 4,646,256 (1987)). 
    // As noted in Computer in Pysics, Vol. 9, No. 4,
    // Jul/Aug 1995 pp 373-379 it was placed in the public domain by the
    // Board of Trustees of Stanford University in 1994 and is now freely available.
    //
    // Berndt M. Gammel, 1994.
    //
    //
    // FFT and FHT routines  Copyright 1988, 1993 by Ron Mayer
    // -------------------------------------------------------
    //
    //  void fht (double* fz, int n);
    //      Does a hartley transform of 'n' points in the array 'fz'.
    //
    //  void fht_fft (int n, double* real, double* imag);
    //      Does a fourier transform of 'n' points of the 'real' and
    //      'imag' arrays.
    //
    //  void fht_ifft (int n, double* real, double* imag);
    //      Does an inverse fourier transform of 'n' points of the 'real'
    //      and 'imag' arrays.
    //
    //  void fht_realfft (int n, double* real);
    //      Does a real-valued fourier transform of 'n' points of the
    //      'real' array.  The real part of the transform ends
    //      up in the first half of the array and the imaginary part of the
    //      transform ends up in the second half of the array.

    //
    //  void fht_realifft (int n, double* real);
    //      The inverse of the fht_realfft() routine above.
    // 
    //      
    // NOTE: This routine uses at least 2 patented algorithms, and may be
    //       under the restrictions of a bunch of different organizations.
    //       Although I wrote it completely myself; it is kind of a derivative
    //       of a routine I once authored and released under the GPL, so it
    //       may fall under the free software foundation's restrictions;
    //       it was worked on as a Stanford Univ project, so they claim
    //       some rights to it; it was further optimized at work here, so
    //       I think this company claims parts of it.  The patents are
    //       held by R. Bracewell (the FHT algorithm) and O. Buneman (the
    //       trig generator), both at Stanford Univ.
    //       If it were up to me, I'd say go do whatever you want with it;
    //       but it would be polite to give credit to the following people
    //       if you use this anywhere:
    //           Euler     - probable inventor of the fourier transform.
    //           Gauss     - probable inventor of the FFT.
    //           Hartley   - probable inventor of the hartley transform.
    //           Buneman   - for a really cool trig generator
    //           Mayer     - for authoring this particular version and
    //                       including all the optimizations in one package.
    //       Thanks,
    //       Ron Mayer; mayer@acuson.com
    //
    //
    // The follwing comments from Ron Mayer came along with the code:
    // --------------------------------------------------------------
    // 
    // As I'm sure you realize, 'best' depends quite a bit on what you
    // consider best, and 'fastest' depends largely on the number of points,
    // what compiler, and what architecture you're using.  If it's amazingly
    // critical to have the best and fastest, you're probably best off
    // writing your own to meet your specific application.  Short of
    // specialized hardware, if you can use a real valued FFT this would be
    // your best optimization (should be faster by a factor of 2).
    // 
    // It seems that two of the source code sections in your summary used
    // 'a duhamel-holman split radix fft'.  Perhaps this is considered a
    // 'standard piece of code', but I do know of two different fft's which
    // in my opinion qualify as both 'faster' and 'better'. 
    // 
    // I tried comparing my code to the code by Dave Edelblute that you
    // included in your previous posting; but the times for that code was so
    // much worse (twice as slow) that it's unlikely that such code would
    // have been in a posting labeled best and fastest; so I probably made an
    // error in compiling it.  In the nearly identical test programs I
    // include in my other posting, they both seem to produce the same
    // results; but who knows.  I'll include the code duhamel-holman code I
    // tested in the second part of this posting so someone can point out
    // where I screwed up.  (If anyone tries it, please let me know if you
    // get the same results...)
    // 
    // The best' published FFT routine I have studied recently is based on
    // Singleton's mixed-radix FFT algorithm.  His routine has a number of
    // optimizations which include a decent trig function generator, and
    // doing a quite optimized radix-4 transforms and only 0 or 1 radix-2
    // transforms for any power of 2.  It is also neat in that it works with
    // any sized array, not just powers of 2!  I believe the published
    // version is in fortran; but someone at work translated it to C, and 
    // it seems to be ~25% faster than the duhamel-holman routine you posted
    // in your summary (if I compiled it correctly).  I can probably dig up a
    // reference to this code next time I dig through all my school stuff if
    // anyone really needs it.
    // 
    // The 'fastest' (for typical computers: single processor, non-vector,
    // fast-integer-math, slower-floating-point-math, slow-trig-function) FFT
    // routine I have ever seen is one I wrote myself; trying to incorporate
    // as many optimizations I could find in various IEEE publications while
    // I was at college.  As you can see in the file 'summary.sparc' included
    // below, it is nearly twice as fast as the duhamel-holman routine you
    // posted in your posting.
    // 
    // The routine I came up with includes the following optimizations:
    // 
    //   1) It is a wrapper around a highly optimized FHT (hartley transform)
    //      A FHT better localizes memory accesses for large transforms,
    //      thereby avoiding paging.  Hartley transforms are also much easier
    //      to add optimization tricks too; more than making up for the
    //      overhead of converting the hartley transfrom to a fourier
    //      transform.  Another advantage is that the transformation from
    //          FHT -> real_valued_FFT
    //      is faster than the transformation from
    //          1/2pointFFT-> real_valued_FFT
    //      so my real-valued fft is even better when compared to most
    //      published real valued ffts.
    // 
    //   2) Avoid multiplications by 1 and zero (and sometimes sqrt2).
    //      Many published routines such as Numerical Recipes seem to spend
    //      a lot of time multiplying by cos(0), cos(pi), etc. and almost all
    //      seem to do 1/sqrt_2*x+1/sqrt_2*y instead of 1/sqrt_2*(x+y).
    // 
    //   3) Faster trig generation.
    //      Most algorithms use 1 'sin' library call for each level of the
    //      transform; and 2 real multiplications and 2 real additions for
    //      each needed trig value within it's loop.
    // 
    //      I use a stable algorithm to generate each trig value using 1 real
    //      multiplication and 1 real addition for each value using a small
    //      (log(n)) table of trig values.  The tradeoff is that I require
    //      much more integer arithmetic for this calculation, including a
    //      (n*log(n)) loop; but for multiples of pi/16 or so, my routine
    //      still seems faster.  By taking advantage of the fact that
    //      values required for FFTs are for evenly spaced angles, I avoid
    //      all calls to slow trig library functions which are unnecessarily
    //      complex because they need to work for arbitrary values.
    // 
    //   4) Generate less trig values
    //      I use the identities sin(x)=sin(pi-x)=-sin(pi+x)=-sin(-x),etc. to
    //      avoid excessive trig calculations, and sin(2x) = 2*cos(x)*sin(x)
    //      to allow simpler trig calculations when accuracy permits.  A more
    //      stable than average trig generator mentioned in (3) above allows
    //      me to use the unstable sin(2x) = 2*cos(x)*sin(x) hack for every
    //      other 'level' in the FFT without the usual loss of accuracy.
    // 
    //   5) Mixed 2,4-radix inner loop.
    //      By doing two levels in the inner loop, I gain all the advantages
    //      of a radix-4 algorithm; primarily reducing integer arithmetic and
    //      memory access.  This has a great affect on large arrays when
    //      paging occurs.
    // 
    //   6) Unrolling loops and variable storage to optimize register
    //      allocation.  I try not to require storing too many values in
    //      variables at any one time to ease a compilers register
    //      allocation. 
    // 
    //   7) Remove low levels of the transform out of the loop.  It's
    //      significantly faster to do 8 point transforms explicitly; rather
    //      than using the general loop.
    // 
    // One catch to this routine is that at least two of the algorithms used
    // by it are patented(!) (just about any FHT is patented by R. Bracewell;
    // and the stable trig generator is patented by O. Buneman; both at
    // Stanford Univ.)  Who owns the copyright rights to it is also probably
    // being debated; since it is a derivative work of a GNU-licensed
    // routine, so subject to their restrictions; it was worked on for a
    // Stanford project, so they have a claim on it;  and I optimized it
    // further working for this company, so they probably claim parts of it.
    // 
    // Considering Gauss apparently used the equivalent of real valued FFTs
    // in 1805; and Euler did fourier transforms it in the mid 1700s; I'm
    // amazed that people still want to claim this math.
    // 
    //
    // Here are the test results posted by Ron Mayer
    // ---------------------------------------------
    //
    // This file contains a benchmark results of a number of popular FFT
    // algorithms.  The algorithms compared are:
    // 
    //     FFT-numrec
    //         The FFT from numerical recipies, converted to double precision
    //     FFT-duhamel
    //         A 'duhamel-holman split radix fft' from "electronics letters,
    //         jan. 5, 1994", coded by Dave Edelblute, edelblut@cod.nosc.mil
    //     FFT-wang
    //         Singleton's arbitrary-radix FFT translated to C and coded by
    //         John Wang, wang@acuson.com
    //     FFT-mayer
    //         An original FFT by Ron Mayer (mayer@acuson.com)
    //     real-FFT-numrec
    //         The real valued FFT from numerical recipies, converted to
    //         double precision.
    //     real-FFT-mayer
    //         An original real valued FFT by Ron Mayer (mayer@acuson.com)
    // 
    // I compiled each of the programs using gcc 2.0 with the -O4 flag on a
    // Sun Sparc 1; and timed (using the "clock()" function in SunOS) a
    // number of iterations of forward and reverse transforms of a known data
    // set.  At the end of the iterations of forward and reverse transforms I
    // compared the data with the original to check for accumulated errors.
    // 
    // algorithm                  # of       # of     time           errors
    //   used                   iterations   points
    //
    // n=4
    // FFT-numrec                (16386       4):   4466488 CPU us ;ssq errors 0.0
    // FFT-duhamel               (16386       4):   2016586 CPU us ;ssq errors 0.0
    // FFT-wang                  (16386       4):   3299868 CPU us ;ssq errors 0.0
    // FFT-mayer                 (16386       4):   1333280 CPU us ;ssq errors 0.0
    // real-FFT-numrec           (16386       4):   3133208 CPU us ;ssq errors 0.0
    // real-FFT-mayer            (16386       4):    666640 CPU us ;ssq errors 0.0
    //
    // n=128
    // FFT-numrec                (514       128):   3883178 CPU us ;ssq errors 4.1e-21
    // FFT-duhamel               (514       128):   6349746 CPU us ;ssq errors 8.6e-22
    // FFT-wang                  (514       128):   3866512 CPU us ;ssq errors 1.5e-09
    // FFT-mayer                 (514       128):   2999880 CPU us ;ssq errors 6.9e-22
    // real-FFT-numrec           (514       128):   2333240 CPU us ;ssq errors 4.1e-21
    // real-FFT-mayer            (514       128):   1433276 CPU us ;ssq errors 6.9e-22
    //
    // n=2048
    // FFT-numrec                (34       2048):   5733104 CPU us ;ssq errors 8.6e-19
    // FFT-duhamel               (34       2048):   8849646 CPU us ;ssq errors 3.2e-20
    // FFT-wang                  (34       2048):   5783102 CPU us ;ssq errors 2.2e-08
    // FFT-mayer                 (34       2048):   4649814 CPU us ;ssq errors 9.4e-20
    // real-FFT-numrec           (34       2048):   3116542 CPU us ;ssq errors 1.6e-18
    // real-FFT-mayer            (34       2048):   2183246 CPU us ;ssq errors 9.4e-20
    //
    // n=32768
    // FFT-numrec                (4       32768):  18732584 CPU us ;ssq errors 1.5e-16
    // FFT-duhamel               (4       32768):  22632428 CPU us ;ssq errors 3.7e-18
    // FFT-wang                  (4       32768):  16299348 CPU us ;ssq errors 1.1e-06
    // FFT-mayer                 (4       32768):  13849446 CPU us ;ssq errors 1.2e-17
    // real-FFT-numrec           (4       32768):   9999600 CPU us ;ssq errors 1.9e-16
    // real-FFT-mayer            (4       32768):   6716398 CPU us ;ssq errors 1.2e-17
    //
    //-----------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------//
    //  REAL is usually defined to be double, could also be float,
    //  but all routines execpt fht use double ! So don't change it !
    //-----------------------------------------------------------------------------//

    // #define REAL double
    // #define GOOD_TRIG       //could also use #define FAST_TRIG, but this is worse

    //-----------------------------------------------------------------------------//
    // include trigonometric table generator
    //-----------------------------------------------------------------------------//


    private const double SQRT2_2=0.70710678118654752440084436210484;
    private const double SQRT2  =2*SQRT2_2;


    /// <summary>
    /// Return true if number is 0 (!) or a power of two
    /// </summary>
    /// <param name="x">Argument to test.</param>
    /// <returns>Return true if number is 0 (!) or a power of two.</returns>
    private static bool IsPowerOfTwo(int x)
    {
      return  ((x & -x) == x);
    }
    //----------------------------------------------------------------------------//

    /// <summary>
    ///  Does a hartley transform of 'n' points in the array 'fz'.
    /// </summary>
    /// <param name="fz">The array of points.</param>
    /// <param name="n">The number of points, must be a power of two. This precondition is not checked!</param>
    public static void FHT (double[] fz, int n)
    {
      if(n<4)
        throw new ArgumentException("Invalid n, n is less than 4!");
      if(!IsPowerOfTwo(n))
        throw new ArgumentException("Invalid n, n is not a power of two!");

      int i,k,k1,k2,k3,k4,kx;
      //double *fi,*fn,*gi;
      int fi,fn,gi;

      // TRIG_VARS;
      int t_lam=0;

      for (k1=1,k2=0;k1<n;k1++) 
      {
        double a;
        for (k=n >> 1; (0==(k&(k2=k2^k))); k >>= 1); // the original code is: for (k=n >> 1; (!((k2^=k)&k)); k >>= 1);
        if (k1>k2) 
        {
          a=fz[k1];fz[k1]=fz[k2];fz[k2]=a;
        }
      }

      for ( k=0 ; (1 << k) < n ; k++ );

      k &= 1;

      if (k == 0) 
      {

        for (fi=0,fn=n;fi<fn;fi+=4) 
        {
          double f0,f1,f2,f3;
          f1     = fz[fi]-fz[fi+1];
          f0     = fz[fi]+fz[fi+1];
          f3     = fz[fi+2]-fz[fi+3];
          f2     = fz[fi+2]+fz[fi+3];
          fz[fi+2 ] = (f0-f2);  
          fz[fi ] = (f0+f2);
          fz[fi+3 ] = (f1-f3);  
          fz[fi+1 ] = (f1+f3);
        }

      } 
      else 
      {

        for (fi=0,fn=n,gi=fi+1;fi<fn;fi += 8,gi += 8) 
        {
          double s1,c1,s2,c2,s3,c3,s4,c4,g0,f0,f1,g1,f2,g2,f3,g3;
          c1     = fz[fi] - fz[gi];
          s1     = fz[fi] + fz[gi];
          c2     = fz[fi+2] - fz[gi+2];
          s2     = fz[fi+2] + fz[gi+2];
          c3     = fz[fi+4] - fz[gi+4];
          s3     = fz[fi+4] + fz[gi+4];
          c4     = fz[fi+6] - fz[gi+6];
          s4     = fz[fi+6] + fz[gi+6];
          f1     = (s1 - s2); 
          f0     = (s1 + s2);
          g1     = (c1 - c2); 
          g0     = (c1 + c2);
          f3     = (s3 - s4); 
          f2     = (s3 + s4);
          g3     = SQRT2*c4;    
          g2     = SQRT2*c3;
          fz[fi+4 ] = f0 - f2;
          fz[fi+0 ] = f0 + f2;
          fz[fi+6 ] = f1 - f3;
          fz[fi+2 ] = f1 + f3;
          fz[gi+4 ] = g0 - g2;
          fz[gi+0 ] = g0 + g2;
          fz[gi+6 ] = g1 - g3;
          fz[gi+2 ] = g1 + g3;
        }

      }

      if (n<16) return;

      do 
      {
        double s1,c1;
        k  += 2;
        k1  = 1  << k;
        k2  = k1 << 1;
        k4  = k2 << 1;
        k3  = k2 + k1;
        kx  = k1 >> 1;
        fi  = 0; // fz;
        gi  = fi + kx;
        fn  = n;

        do 
        {
          double g0,f0,f1,g1,f2,g2,f3,g3;
          f1      = fz[fi+0 ] - fz[fi+k1];
          f0      = fz[fi+0 ] + fz[fi+k1];
          f3      = fz[fi+k2] - fz[fi+k3];
          f2      = fz[fi+k2] + fz[fi+k3];
          fz[fi+k2]  = f0   - f2;
          fz[fi+0 ]  = f0   + f2;
          fz[fi+k3]  = f1   - f3;
          fz[fi+k1]  = f1   + f3;
          g1      = fz[gi+0 ] - fz[gi+k1];
          g0      = fz[gi+0 ] + fz[gi+k1];
          g3      = SQRT2  * fz[gi+k3];
          g2      = SQRT2  * fz[gi+k2];
          fz[gi+k2]  = g0   - g2;
          fz[gi+0 ]  = g0   + g2;
          fz[gi+k3]  = g1   - g3;
          fz[gi+k1]  = g1   + g3;
          gi     += k4;
          fi     += k4;
        } while (fi<fn);

        // TRIG_INIT(k,c1,s1);
      {                
        int i1;              
        for (i1=2 ; i1<=k ; i1++)          
        { 
          coswrk[i1]=costab[i1];
          sinwrk[i1]=sintab[i1];}    
        t_lam = 0;                   
        c1 = 1;              
        s1 = 0;              
      }


        for (i=1;i<kx;i++) 
        {
          double c2,s2;

          // TRIG_NEXT(k,c1,s1);
        {                
          int i2,j2;                                           
          (t_lam)++;               
          for (i2=0 ; 0==((1<<i2)&t_lam) ; i2++);      
          i2 = k-i2;             
          s1 = sinwrk[i2];             
          c1 = coswrk[i2];             
          if (i2>1)                
          {                  
            for (j2=k-i2+2 ; 0!=((1<<j2)&t_lam) ; j2++);     
            j2         = k - j2;           
            sinwrk[i2] = halsec[i2] * (sinwrk[i2-1] + sinwrk[j2]);  
            coswrk[i2] = halsec[i2] * (coswrk[i2-1] + coswrk[j2]);  
          }                                                    
        }

          
          
          
          c2 = c1*c1 - s1*s1;
          s2 = 2*(c1*s1);
          fn = n; // fn = fz + n;
          fi = i; // fi = fz +i;
          gi = k1-i; // gi = fz +k1-i;

          do 
          {
            double a,b,g0,f0,f1,g1,f2,g2,f3,g3;
            b       = s2*fz[fi+k1] - c2*fz[gi+k1];
            a       = c2*fz[fi+k1] + s2*fz[gi+k1];
            f1      = fz[fi+0 ]    - a;
            f0      = fz[fi+0 ]    + a;
            g1      = fz[gi+0 ]    - b;
            g0      = fz[gi+0 ]    + b;
            b       = s2*fz[fi+k3] - c2*fz[gi+k3];
            a       = c2*fz[fi+k3] + s2*fz[gi+k3];
            f3      = fz[fi+k2]    - a;
            f2      = fz[fi+k2]    + a;
            g3      = fz[gi+k2]    - b;
            g2      = fz[gi+k2]    + b;
            b       = s1*f2     - c1*g3;
            a       = c1*f2     + s1*g3;
            fz[fi+k2]  = f0        - a;
            fz[fi+0 ]  = f0        + a;
            fz[gi+k3]  = g1        - b;
            fz[gi+k1]  = g1        + b;
            b       = c1*g2     - s1*f3;
            a       = s1*g2     + c1*f3;
            fz[gi+k2]  = g0        - a;
            fz[gi+0 ]  = g0        + a;
            fz[fi+k3]  = f1        - b;
            fz[fi+k1]  = f1        + b;
            gi     += k4;
            fi     += k4;
          } while (fi<fn);
        }

        // TRIG_RESET(k,c1,s1);

      } while (k4<n);
    }

    //----------------------------------------------------------------------------//



    /// <summary>
    ///      Does a fourier transform of 'n' points of the 'real' and
    ///      'imag' arrays.
    /// </summary>
    /// <param name="real">The array holding the real part of the values.</param>
    /// <param name="imag">The array holding the imaginary part of the values.</param>
    /// <param name="n">Number of points to transform. Have to be a power of 2 (unchecked!)</param>
    public static void FFT (double[] real, double[] imag, int n )
    {
      FHT(real,n);
      FHT(imag,n);

      for (int i=1,j=n-1,k=n/2;i<k;i++,j--) 
      {
        double a,b,c,d,q,r,s,t;
        a = real[i]; b = real[j];  q=a+b; r=a-b;
        c = imag[i]; d = imag[j];  s=c+d; t=c-d;
        imag[i] = (s+r)*0.5;  imag[j] = (s-r)*0.5;
        real[i] = (q-t)*0.5;  real[j] = (q+t)*0.5;
      }
    }


    /// <summary>
    ///      Does an in-place inverse fourier transform of 'n' points of the 'real'
    ///      and 'imag' arrays.
    /// </summary>
    /// <param name="n">Number of points to transform. Have to be a power of 2 (unchecked!)</param>
    /// <param name="real">The array holding the real part of the values.</param>
    /// <param name="imag">The array holding the imaginary part of the values.</param>
    public static void IFFT (double[] real, double[] imag, int n )
    {
      for (int i=1,j=n-1,k=n/2;i<k;i++,j--) 
      {
        double a,b,c,d, q,r,s,t;
        a = real[i]; b = real[j];  q=a+b; r=a-b;
        c = imag[i]; d = imag[j];  s=c+d; t=c-d;
        real[i] = (q+t)*.5; real[j] = (q-t)*.5;
        imag[i] = (s-r)*.5; imag[j] = (s+r)*.5;
      }

      FHT(real,n);
      FHT(imag,n);
    }


    /// <summary>
    ///      Does the inverse of a real-valued fourier transform of 'n' points.
    /// </summary>
    /// <param name="n">Number of points to transform. Has to be a power of 2 (unchecked).</param>
    /// <param name="real">The array holding the fourier transform values, which will be transformed back.</param>
    public static void RealIFFT (double[] real, int n )
    {
      for (int i=1,j=n-1,k=n/2;i<k;i++,j--) 
      {
        double a,b;
        a = real[i];
        b = real[j];
        real[j] = (a-b);
        real[i] = (a+b);
      }

      FHT(real,n);
    }

    /// <summary>
    ///      Does a real-valued fourier transform of 'n' points of the
    ///      'real' array.  The real part of the transform ends
    ///      up in the first half of the array and the imaginary part of the
    ///      transform ends up in the second half of the array.
    /// </summary>
    /// <param name="n">The number of points to transform. Has to be a power of 2 (unchecked!).</param>
    /// <param name="real">The array holding the real values to transform.</param>
    public static void RealFFT ( double[] real, int n)
    {
      FHT(real,n);

      for (int i=1,j=n-1,k=n/2;i<k;i++,j--) 
      {
        double a,b;
        a = real[i];
        b = real[j];
        real[j] = (a-b)*0.5;
        real[i] = (a+b)*0.5;
      }
    }


    /// <summary>
    /// Does a fourier transform of 'n' points of the 'real' and 'imag' arrays.
    /// </summary>
    /// <param name="real">The array holding the real part of the values.</param>
    /// <param name="imag">The array holding the imaginary part of the values.</param>
    /// <param name="direction">The direction of the Fourier transformation.</param>
    public static void FFT(double[] real, double[] imag, FourierDirection direction)
    {
      if(real.Length!=imag.Length)
        throw new ArgumentException("Length of real and imag array do not match!");
      
      FFT(real,imag,real.Length,direction);
    }

    /// <summary>
    /// Does a fourier transform of 'n' points of the 'real' and 'imag' arrays.
    /// </summary>
    /// <param name="real">The array holding the real part of the values.</param>
    /// <param name="imag">The array holding the imaginary part of the values.</param>
    /// <param name="n">Number of points to transform. Have to be a power of 2 (unchecked!)</param>
    /// <param name="direction">The direction of the Fourier transformation.</param>
    public static void FFT(double[] real, double[] imag, int n, FourierDirection direction)
    {
      if(direction==FourierDirection.Forward)
        FFT(real,imag,n);
      else
        IFFT(real,imag,n);
    }

    /// <summary>
    /// Does a real-valued fourier transform of 'n' points of the
    /// 'real' array.  On forward transform, the real part of the transform ends
    /// up in the first half of the array and the imaginary part of the
    /// transform ends up in the second half of the array. On backward transform, real and imaginary part
    /// have to be located in the same way like the result of the forward transform.
    /// </summary>
    /// <param name="n">The number of points to transform. Has to be a power of 2 (unchecked!).</param>
    /// <param name="real">The array holding the real values to transform.</param>
    /// <param name="direction">The direction of the Fourier transform.</param>
    public static void RealFFT(double[] real, int n, FourierDirection direction)
    {
      if(direction==FourierDirection.Forward)
        RealFFT(real,n);
      else
        RealIFFT(real,n);
    }

    /// <summary>
    /// Does a real-valued fourier transform of 'n' points of the
    /// 'real' array.  On forward transform, the real part of the transform ends
    /// up in the first half of the array and the imaginary part of the
    /// transform ends up in the second half of the array. On backward transform, real and imaginary part
    /// have to be located in the same way like the result of the forward transform.
    /// </summary>
    /// <param name="real">The array holding the real values to transform.</param>
    /// <param name="direction">The direction of the Fourier transform.</param>
    public static void RealFFT(double[] real, FourierDirection direction)
    {
      RealFFT(real, real.Length, direction);
    }

    /// <summary>
    /// Performs a cyclic convolution of two real valued arrays. The content of the input arrays is destroyed during this operation.
    /// </summary>
    /// <param name="data">The first input array (the data).</param>
    /// <param name="resp">The second input array (the response function).</param>
    /// <param name="result">The result of the convolution.</param>
    /// <param name="n">The convolution size. The provided arrays may be larger than n, but of course not smaller.</param>
    public static void CyclicDestructiveConvolution(double[] data, double[] resp, double[] result, int n)
    {
      FHT(data,n);
      FHT(resp,n);

      double scale = 0.25/n;
      int nh=n/2;
      for (int i=1,j=n-1;i<nh;i++,j--) 
      {
        double a,b,re1,im1,re2,im2,re,im;
        a = data[i];
        b = data[j];
        im1 = (a-b); // this is exactly (a-b)/2, but the /2 is included in the scale
        re1 = (a+b); // this is exactly (a+b)/2, but the /2 is included in the scale

        a = resp[i];
        b = resp[j];
        im2 = (a-b);  // this is exactly (a-b)/2, but the /2 is included in the scale
        re2 = (a+b);  // this is exactly (a+b)/2, but the /2 is included in the scale

        re = (re1*re2 - im1*im2) * scale;
        im = (re1*im2 + im1*re2) * scale;
   
        result[j] = (re-im);
        result[i] = (re+im);
      }

      // handle the zero and the half point
      result[0] = data[0]*resp[0]/n;
      result[nh] = data[nh]*resp[nh]/n;

      FHT(result,n);
    }


    /// <summary>
    /// Performs a cyclic convolution of two real valued arrays. The content of the input arrays is leaved intact.
    /// </summary>
    /// <param name="data">The first input array (the data).</param>
    /// <param name="resp">The second input array (the response function).</param>
    /// <param name="result">The result of the convolution.</param>
    /// <param name="scratch">A helper array of at least size n. If null or a smaller array is provided, a new array will be allocated automatically.</param>
    /// <param name="n">The convolution size. The provided arrays may be larger than n, but of course not smaller.</param>
    public static void CyclicRealConvolution(double[] data, double[] resp, double[] result, int n, double[] scratch)
    {
      if(null==scratch || scratch.Length<n)
        scratch = new double[n];

      Array.Copy(data,result,n);
      Array.Copy(resp,scratch,n);

      FHT(result,n);
      FHT(scratch,n);

      double scale = 0.25/n;
      int nh=n/2;
      for (int i=1,j=n-1;i<nh;i++,j--) 
      {
        double a,b,re1,im1,re2,im2,re,im;
        a = result[i];
        b = result[j];
        im1 = (a-b); // this is exactly (a-b)/2, but the /2 is included in the scale
        re1 = (a+b); // this is exactly (a+b)/2, but the /2 is included in the scale

        a = scratch[i];
        b = scratch[j];
        im2 = (a-b);  // this is exactly (a-b)/2, but the /2 is included in the scale
        re2 = (a+b);  // this is exactly (a+b)/2, but the /2 is included in the scale

        re = (re1*re2 - im1*im2) * scale;
        im = (re1*im2 + im1*re2) * scale;
   
        result[j] = (re-im);
        result[i] = (re+im);
      }

      // handle the zero and the half point
      result[0] = result[0]*scratch[0]/n;
      result[nh] = result[nh]*scratch[nh]/n;

      FHT(result,n);
    }


    /// <summary>
    /// Performs a convolution of two comlex arrays which are in splitted form (i.e. real and imaginary part are separate arrays). Attention: the data into the
    /// input arrays will be destroyed!
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    public static void CyclicDestructiveConvolution(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      FFT( src1real, src1imag, n);
      FFT( src2real, src2imag, n);
      ArrayMath.MultiplySplittedComplexArrays(src1real, src1imag, src2real, src2imag, resultreal, resultimag, n);
      IFFT( resultreal,resultimag, n);
      ArrayMath.NormalizeArrays(resultreal, resultimag, 1.0/n, n);
    }

    /// <summary>
    /// Performs a convolution of two complex arrays which are in splitted form. The input arrays will leave intact.
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="scratchreal">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    /// <param name="scratchimag">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    public static void CyclicConvolution(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      double[] scratchreal, double[] scratchimag,
      int n)
    {
      if ( null==scratchreal || scratchreal.Length<n)
        scratchreal = new  double[n];
      if ( null==scratchimag || scratchimag.Length<n )
        scratchimag = new  double[n];

      // First copy the arrays data and response to result and scratch,
      // respectively, to prevent overwriting of the original data.
      Array.Copy(src1real,resultreal, n);
      Array.Copy(src1imag,resultimag, n);
      Array.Copy(src2real,scratchreal, n);
      Array.Copy(src2imag,scratchimag, n);

      FFT( resultreal, resultimag, n);
      FFT( scratchreal, scratchimag, n);
      ArrayMath.MultiplySplittedComplexArrays(resultreal, resultimag, scratchreal, scratchimag, resultreal, resultimag, n);
      FastHartleyTransform.IFFT( resultreal,resultimag, n);
      ArrayMath.NormalizeArrays(resultreal, resultimag, 1.0/n, n);
    }



    /// <summary>
    /// Performs a correlation of two comlex arrays which are in splitted form (i.e. real and imaginary part are separate arrays). Attention: the data into the
    /// input arrays will be destroyed!
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    public static void CyclicCorrelationDestructive(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      FFT( src1real, src1imag, n);
      FFT( src2real, src2imag, n);
      ArrayMath.MultiplySplittedComplexArraysCrossed(src1real, src1imag, src2real, src2imag, resultreal, resultimag, n, 1.0/n);
      FFT( resultreal,resultimag, n);
    }


    /// <summary>
    /// Performs a cyclic correlation of two complex arrays which are in splitted form. The input arrays will leave intact.
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    /// <remarks>Two helper arrays of length n are automatially allocated and freed during the operation.</remarks>
    public static void CyclicCorrelation(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      double[] help1=null, help2=null;
      CyclicCorrelation(src1real, src1imag,
        src2real,src2imag,
        resultreal,resultimag,
        n,
        ref help1, ref help2);
    }

    /// <summary>
    /// Performs a cyclic correlation of two complex arrays which are in splitted form. The input arrays will leave intact.
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    /// <param name="scratchreal">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    /// <param name="scratchimag">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    public static void CyclicCorrelation(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n,
      ref double[] scratchreal, ref double[] scratchimag
      )
    {
      if ( null==scratchreal || scratchreal.Length<n)
        scratchreal = new  double[n];
      if ( null==scratchimag || scratchimag.Length<n )
        scratchimag = new  double[n];

      // First copy the arrays data and response to result and scratch,
      // respectively, to prevent overwriting of the original data.
      Array.Copy(src1real,resultreal, n);
      Array.Copy(src1imag,resultimag, n);
      Array.Copy(src2real,scratchreal, n);
      Array.Copy(src2imag,scratchimag, n);

      FFT( resultreal, resultimag, n);
      FFT( scratchreal, scratchimag, n);
      ArrayMath.MultiplySplittedComplexArraysCrossed(resultreal, resultimag, scratchreal, scratchimag, resultreal, resultimag, n, 1.0/n);
      FFT( resultreal,resultimag, n);
    }


    /// <summary>
    /// Performes a cyclic correlation between array arr1 and arr2 and stores the result in resultarr. Resultarr must be
    /// different from the other two arrays. 
    /// </summary>
    /// <param name="arr1">First array.</param>
    /// <param name="arr2">Second array.</param>
    /// <param name="resultarr">The array that stores the correleation result.</param>
    /// <param name="n">Number of points to correlate.</param>
    public static  void CyclicCorrelationDestructive(double[] arr1, double[] arr2, double[] resultarr, int n)
    {
      RealFFT(arr1,n);
      RealFFT(arr2,n);
      // multiply the result in arr1 (real part in the first half, imaginary part in the second half)
      // with the complex conjugate of arr2 and store the result in result
      int i, j;
      double re, im;
      double scale = 1.0/n;
      for(i=1,j=n-1;i<j;++i,--j)
      {
        re = arr1[i]*arr2[i] + arr1[j]*arr2[j]; // + because of complex conjugate
        im = arr1[i]*arr2[j] - arr1[j]*arr2[i];
        resultarr[i]=re*scale;
        resultarr[j]=im*scale;
      }
      // special points 0 and n/2
      resultarr[0] = scale*arr1[0]*arr2[0];
      resultarr[n/2] = scale*arr1[n/2]*arr2[n/2];

      RealIFFT(resultarr,n);
    }
  }
}
