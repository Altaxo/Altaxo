//This file is part of the gNumerator MathML DOM library, a complete 
//implementation of the w3c mathml dom specification
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//andy@epsilon3.net

using System;

namespace MathML
{
	/// <summary>
	/// used to resolve mathml entities to characters or strings, as
	/// specified in (a) mathml dtd. This is essentially a hardcoded 
	/// dtd, and it exists to dramatically speed up document load time.
	/// 
	/// Currently, all entries are hardcoded here, but in the future, it
	/// may be expanded to load the entities from a file.
	/// </summary>
	internal class EntityDictionary
	{
		/// <summary>
		/// static class, nothing done here
		/// </summary>
		private EntityDictionary() {}

        
		/// list of entitiy resolutions.
		/// these were all auto-generated from the w3c provided xhtml-math11-f.dtd on
		/// 12-13-2003		
		private static readonly Entity[] entities = 
		{
			new Entity("aacute",               '\xe1',   '\x0'), // latin small a with acute, U+00E1 ISOlat1 
			new Entity("Aacute",               '\xc1',   '\x0'), // latin capital A with acute, U+00C1 ISOlat1 
			new Entity("abreve",               '\x103',  '\x0'), // =small a, breve 
			new Entity("Abreve",               '\x102',  '\x0'), // =capital A, breve 
			new Entity("ac",                   '\x223e', '\x0'), // most positive 
			new Entity("acd",                  '\x223f', '\x0'), // ac current 
			new Entity("acE",                  '\x223e', '\x333'), // most positive, two lines below 
			new Entity("acirc",                '\xe2',   '\x0'), // latin small a with circumflex, U+00E2 ISOlat1 
			new Entity("Acirc",                '\xc2',   '\x0'), // latin capital A with circumflex, U+00C2 ISOlat1 
			new Entity("acute",                '\xb4',   '\x0'), // acute accent = spacing acute, U+00B4 ISOdia 
			new Entity("acy",                  '\x430',  '\x0'), // =small a, Cyrillic 
			new Entity("Acy",                  '\x410',  '\x0'), // =capital A, Cyrillic 
			new Entity("aelig",                '\xe6',   '\x0'), // latin small ae = latin small ligature ae, U+00E6 ISOlat1 
			new Entity("AElig",                '\xc6',   '\x0'), // latin capital AE = latin capital ligature AE, U+00C6 ISOlat1 
			new Entity("af",                   '\x2061', '\x0'), // character showing function application in presentation tagging 
			// new Entity("afr",                  "%plane1D;51E;"), /frak a, lower case a 
			// new Entity("Afr",                  "%plane1D;504;"), /frak A, upper case a 
			new Entity("agrave",               '\xe0',   '\x0'), // latin small a with grave = latin small a grave, U+00E0 ISOlat1 
			new Entity("Agrave",               '\xc0',   '\x0'), // latin capital A with grave = latin capital A grave, U+00C0 ISOlat1 
			new Entity("alefsym",              '\x2135', '\x0'), // alef symbol = first transfinite cardinal, U+2135 NEW 
			new Entity("aleph",                '\x2135', '\x0'), // /aleph aleph, Hebrew 
			new Entity("alpha",                '\x3b1',  '\x0'), // greek small letter alpha, U+03B1 ISOgrk3 
			new Entity("Alpha",                '\x391',  '\x0'), // greek capital letter alpha, U+0391 
			new Entity("amacr",                '\x101',  '\x0'), // =small a, macron 
			new Entity("Amacr",                '\x100',  '\x0'), // =capital A, macron 
			new Entity("amalg",                '\x2a3f', '\x0'), // /amalg B: amalgamation or coproduct 
			new Entity("amp",                  '\x26',   '\x0'), // ampersand, U+0026 ISOnum 
			new Entity("and",                  '\x2227', '\x0'), // logical and = wedge, U+2227 ISOtech 
			new Entity("And",                  '\x2a53', '\x0'), // dbl logical and 
			new Entity("andand",               '\x2a55', '\x0'), // two logical and 
			new Entity("andd",                 '\x2a5c', '\x0'), // and, horizontal dash 
			new Entity("andslope",             '\x2a58', '\x0'), // sloping large and 
			new Entity("andv",                 '\x2a5a', '\x0'), // and with middle stem 
			new Entity("ang",                  '\x2220', '\x0'), // angle, U+2220 ISOamso 
			new Entity("ange",                 '\x29a4', '\x0'), // angle, equal 
			new Entity("angle",                '\x2220', '\x0'), // alias ISOAMSO ang 
			new Entity("angmsd",               '\x2221', '\x0'), // /measuredangle - angle-measured 
			new Entity("angmsdaa",             '\x29a8', '\x0'), // angle-measured, arrow, up, right 
			new Entity("angmsdab",             '\x29a9', '\x0'), // angle-measured, arrow, up, left 
			new Entity("angmsdac",             '\x29aa', '\x0'), // angle-measured, arrow, down, right 
			new Entity("angmsdad",             '\x29ab', '\x0'), // angle-measured, arrow, down, left 
			new Entity("angmsdae",             '\x29ac', '\x0'), // angle-measured, arrow, right, up 
			new Entity("angmsdaf",             '\x29ad', '\x0'), // angle-measured, arrow, left, up 
			new Entity("angmsdag",             '\x29ae', '\x0'), // angle-measured, arrow, right, down 
			new Entity("angmsdah",             '\x29af', '\x0'), // angle-measured, arrow, left, down 
			new Entity("angrt",                '\x221f', '\x0'), // right (90 degree) angle 
			new Entity("angrtvb",              '\x22be', '\x0'), // right angle-measured 
			new Entity("angrtvbd",             '\x299d', '\x0'), // right angle-measured, dot 
			new Entity("angsph",               '\x2222', '\x0'), // /sphericalangle angle-spherical 
			new Entity("angst",                '\x212b', '\x0'), // Angstrom capital A, ring 
			new Entity("angzarr",              '\x237c', '\x0'), // angle with down zig-zag arrow 
			new Entity("aogon",                '\x105',  '\x0'), // =small a, ogonek 
			new Entity("Aogon",                '\x104',  '\x0'), // =capital A, ogonek 
			// new Entity("aopf",                 "%plane1D;552;"),  
			// new Entity("Aopf",                 "%plane1D;538;"), /Bbb A, open face A 
			new Entity("ap",                   '\x2248', '\x0'), // /approx R: approximate 
			new Entity("apacir",               '\x2a6f', '\x0'), // approximate, circumflex accent 
			new Entity("ape",                  '\x224a', '\x0'), // /approxeq R: approximate, equals 
			new Entity("apE",                  '\x2a70', '\x0'), // approximately equal or equal to 
			new Entity("apid",                 '\x224b', '\x0'), // approximately identical to 
			new Entity("apos",                 '\x27',   '\x0'), // apostrophe, U+0027 ISOnum 
			new Entity("ApplyFunction",        '\x2061', '\x0'), // character showing function application in presentation tagging 
			new Entity("approx",               '\x2248', '\x0'), // alias ISOTECH ap 
			new Entity("approxeq",             '\x224a', '\x0'), // alias ISOAMSR ape 
			new Entity("aring",                '\xe5',   '\x0'), // latin small a with ring above = latin small a ring, U+00E5 ISOlat1 
			new Entity("Aring",                '\xc5',   '\x0'), // latin capital A with ring above = latin capital A ring, U+00C5 ISOlat1 
			// new Entity("ascr",                 "%plane1D;4B6;"), /scr a, script letter a 
			// new Entity("Ascr",                 "%plane1D;49C;"), /scr A, script letter A 
			new Entity("Assign",               '\x2254', '\x0'), // assignment operator, alias ISOAMSR colone 
			new Entity("ast",                  '\x2a',   '\x0'), // /ast B: =asterisk 
			new Entity("asymp",                '\x2248', '\x0'), // almost equal to = asymptotic to, U+2248 ISOamsr 
			new Entity("asympeq",              '\x224d', '\x0'), // Old ISOAMSR asymp (for HTML compatibility) 
			new Entity("atilde",               '\xe3',   '\x0'), // latin small a with tilde, U+00E3 ISOlat1 
			new Entity("Atilde",               '\xc3',   '\x0'), // latin capital A with tilde, U+00C3 ISOlat1 
			new Entity("auml",                 '\xe4',   '\x0'), // latin small a with diaeresis, U+00E4 ISOlat1 
			new Entity("Auml",                 '\xc4',   '\x0'), // latin capital A with diaeresis, U+00C4 ISOlat1 
			new Entity("awconint",             '\x2233', '\x0'), // contour integral, anti-clockwise 
			new Entity("awint",                '\x2a11', '\x0'), // anti clock-wise integration 
			new Entity("backcong",             '\x224c', '\x0'), // alias ISOAMSR bcong 
			new Entity("backepsilon",          '\x3f6',  '\x0'), // alias ISOAMSR bepsi 
			new Entity("backprime",            '\x2035', '\x0'), // alias ISOAMSO bprime 
			new Entity("backsim",              '\x223d', '\x0'), // alias ISOAMSR bsim 
			new Entity("backsimeq",            '\x22cd', '\x0'), // alias ISOAMSR bsime 
			new Entity("Backslash",            '\x2216', '\x0'), // alias ISOAMSB setmn 
			new Entity("Barv",                 '\x2ae7', '\x0'), // vert, dbl bar (over) 
			new Entity("barvee",               '\x22bd', '\x0'), // bar, vee 
			new Entity("barwed",               '\x2305', '\x0'), // /barwedge B: logical and, bar above 
			new Entity("Barwed",               '\x2306', '\x0'), // /doublebarwedge B: log and, dbl bar above 
			new Entity("barwedge",             '\x2305', '\x0'), // alias ISOAMSB barwed 
			new Entity("bbrk",                 '\x23b5', '\x0'), // bottom square bracket 
			new Entity("bbrktbrk",             '\x23b6', '\x0'), // bottom above top square bracket 
			new Entity("bcong",                '\x224c', '\x0'), // /backcong R: reverse congruent 
			new Entity("bcy",                  '\x431',  '\x0'), // =small be, Cyrillic 
			new Entity("Bcy",                  '\x411',  '\x0'), // =capital BE, Cyrillic 
			new Entity("bdquo",                '\x201e', '\x0'), // double low-9 quotation mark, U+201E NEW 
			new Entity("becaus",               '\x2235', '\x0'), // /because R: because 
			new Entity("because",              '\x2235', '\x0'), // alias ISOTECH becaus 
			new Entity("Because",              '\x2235', '\x0'), // alias ISOTECH becaus 
			new Entity("bemptyv",              '\x29b0', '\x0'), // reversed circle, slash 
			new Entity("bepsi",                '\x3f6',  '\x0'), // /backepsilon R: such that 
			new Entity("bernou",               '\x212c', '\x0'), // Bernoulli function (script capital B)  
			new Entity("Bernoullis",           '\x212c', '\x0'), // alias ISOTECH bernou 
			new Entity("beta",                 '\x3b2',  '\x0'), // greek small letter beta, U+03B2 ISOgrk3 
			new Entity("Beta",                 '\x392',  '\x0'), // greek capital letter beta, U+0392 
			new Entity("beth",                 '\x2136', '\x0'), // /beth - beth, Hebrew 
			new Entity("between",              '\x226c', '\x0'), // alias ISOAMSR twixt 
			// new Entity("bfr",                  "%plane1D;51F;"), /frak b, lower case b 
			// new Entity("Bfr",                  "%plane1D;505;"), /frak B, upper case b 
			new Entity("bigcap",               '\x22c2', '\x0'), // alias ISOAMSB xcap 
			new Entity("bigcirc",              '\x25ef', '\x0'), // alias ISOAMSB xcirc 
			new Entity("bigcup",               '\x22c3', '\x0'), // alias ISOAMSB xcup 
			new Entity("bigodot",              '\x2a00', '\x0'), // alias ISOAMSB xodot 
			new Entity("bigoplus",             '\x2a01', '\x0'), // alias ISOAMSB xoplus 
			new Entity("bigotimes",            '\x2a02', '\x0'), // alias ISOAMSB xotime 
			new Entity("bigsqcup",             '\x2a06', '\x0'), // alias ISOAMSB xsqcup 
			new Entity("bigstar",              '\x2605', '\x0'), // ISOPUB    starf  
			new Entity("bigtriangledown",      '\x25bd', '\x0'), // alias ISOAMSB xdtri 
			new Entity("bigtriangleup",        '\x25b3', '\x0'), // alias ISOAMSB xutri 
			new Entity("biguplus",             '\x2a04', '\x0'), // alias ISOAMSB xuplus 
			new Entity("bigvee",               '\x22c1', '\x0'), // alias ISOAMSB xvee 
			new Entity("bigwedge",             '\x22c0', '\x0'), // alias ISOAMSB xwedge 
			new Entity("bkarow",               '\x290d', '\x0'), // alias ISOAMSA rbarr 
			new Entity("blacklozenge",         '\x29eb', '\x0'), // alias ISOPUB lozf 
			new Entity("blacksquare",          '\x25aa', '\x0'), // ISOTECH  squarf  
			new Entity("blacktriangle",        '\x25b4', '\x0'), // alias ISOPUB utrif 
			new Entity("blacktriangledown",    '\x25be', '\x0'), // alias ISOPUB dtrif 
			new Entity("blacktriangleleft",    '\x25c2', '\x0'), // alias ISOPUB ltrif 
			new Entity("blacktriangleright",   '\x25b8', '\x0'), // alias ISOPUB rtrif 
			new Entity("blank",                '\x2423', '\x0'), // =significant blank symbol 
			new Entity("blk12",                '\x2592', '\x0'), // =50% shaded block 
			new Entity("blk14",                '\x2591', '\x0'), // =25% shaded block 
			new Entity("blk34",                '\x2593', '\x0'), // =75% shaded block 
			new Entity("block",                '\x2588', '\x0'), // =full block 
			new Entity("bne",                  '\x3d',   '\x20e5'), // reverse not equal 
			new Entity("bnequiv",              '\x2261', '\x20e5'), // reverse not equivalent 
			new Entity("bnot",                 '\x2310', '\x0'), // reverse not 
			new Entity("bNot",                 '\x2aed', '\x0'), // reverse not with two horizontal strokes 
			// new Entity("bopf",                 "%plane1D;553;"),  
			// new Entity("Bopf",                 "%plane1D;539;"), /Bbb B, open face B 
			new Entity("bot",                  '\x22a5', '\x0'), // alias ISOTECH bottom 
			new Entity("bottom",               '\x22a5', '\x0'), // /bot bottom 
			new Entity("bowtie",               '\x22c8', '\x0'), // /bowtie R: 
			new Entity("boxbox",               '\x29c9', '\x0'), // two joined squares 
			new Entity("boxdl",                '\x2510', '\x0'), // lower left quadrant 
			new Entity("boxdL",                '\x2555', '\x0'), // lower left quadrant 
			new Entity("boxDl",                '\x2556', '\x0'), // lower left quadrant 
			new Entity("boxDL",                '\x2557', '\x0'), // lower left quadrant 
			new Entity("boxdr",                '\x250c', '\x0'), // lower right quadrant 
			new Entity("boxdR",                '\x2552', '\x0'), // lower right quadrant 
			new Entity("boxDr",                '\x2553', '\x0'), // lower right quadrant 
			new Entity("boxDR",                '\x2554', '\x0'), // lower right quadrant 
			new Entity("boxh",                 '\x2500', '\x0'), // horizontal line  
			new Entity("boxH",                 '\x2550', '\x0'), // horizontal line 
			new Entity("boxhd",                '\x252c', '\x0'), // lower left and right quadrants 
			new Entity("boxhD",                '\x2565', '\x0'), // lower left and right quadrants 
			new Entity("boxHd",                '\x2564', '\x0'), // lower left and right quadrants 
			new Entity("boxHD",                '\x2566', '\x0'), // lower left and right quadrants 
			new Entity("boxhu",                '\x2534', '\x0'), // upper left and right quadrants 
			new Entity("boxhU",                '\x2568', '\x0'), // upper left and right quadrants 
			new Entity("boxHu",                '\x2567', '\x0'), // upper left and right quadrants 
			new Entity("boxHU",                '\x2569', '\x0'), // upper left and right quadrants 
			new Entity("boxminus",             '\x229f', '\x0'), // alias ISOAMSB minusb 
			new Entity("boxplus",              '\x229e', '\x0'), // alias ISOAMSB plusb 
			new Entity("boxtimes",             '\x22a0', '\x0'), // alias ISOAMSB timesb 
			new Entity("boxul",                '\x2518', '\x0'), // upper left quadrant 
			new Entity("boxuL",                '\x255b', '\x0'), // upper left quadrant 
			new Entity("boxUl",                '\x255c', '\x0'), // upper left quadrant 
			new Entity("boxUL",                '\x255d', '\x0'), // upper left quadrant 
			new Entity("boxur",                '\x2514', '\x0'), // upper right quadrant 
			new Entity("boxuR",                '\x2558', '\x0'), // upper right quadrant 
			new Entity("boxUr",                '\x2559', '\x0'), // upper right quadrant 
			new Entity("boxUR",                '\x255a', '\x0'), // upper right quadrant 
			new Entity("boxv",                 '\x2502', '\x0'), // vertical line 
			new Entity("boxV",                 '\x2551', '\x0'), // vertical line 
			new Entity("boxvh",                '\x253c', '\x0'), // all four quadrants 
			new Entity("boxvH",                '\x256a', '\x0'), // all four quadrants 
			new Entity("boxVh",                '\x256b', '\x0'), // all four quadrants 
			new Entity("boxVH",                '\x256c', '\x0'), // all four quadrants 
			new Entity("boxvl",                '\x2524', '\x0'), // upper and lower left quadrants 
			new Entity("boxvL",                '\x2561', '\x0'), // upper and lower left quadrants 
			new Entity("boxVl",                '\x2562', '\x0'), // upper and lower left quadrants 
			new Entity("boxVL",                '\x2563', '\x0'), // upper and lower left quadrants 
			new Entity("boxvr",                '\x251c', '\x0'), // upper and lower right quadrants 
			new Entity("boxvR",                '\x255e', '\x0'), // upper and lower right quadrants 
			new Entity("boxVr",                '\x255f', '\x0'), // upper and lower right quadrants 
			new Entity("boxVR",                '\x2560', '\x0'), // upper and lower right quadrants 
			new Entity("bprime",               '\x2035', '\x0'), // /backprime - reverse prime 
			new Entity("breve",                '\x2d8',  '\x0'), // =breve 
			new Entity("Breve",                '\x2d8',  '\x0'), // alias ISODIA breve 
			new Entity("brvbar",               '\xa6',   '\x0'), // broken bar = broken vertical bar, U+00A6 ISOnum 
			// new Entity("bscr",                 "%plane1D;4B7;"), /scr b, script letter b 
			new Entity("Bscr",                 '\x212c', '\x0'), // /scr B, script letter B 
			new Entity("bsemi",                '\x204f', '\x0'), // reverse semi-colon 
			new Entity("bsim",                 '\x223d', '\x0'), // /backsim R: reverse similar 
			new Entity("bsime",                '\x22cd', '\x0'), // /backsimeq R: reverse similar, eq 
			new Entity("bsol",                 '\x5c',   '\x0'), // /backslash =reverse solidus 
			new Entity("bsolb",                '\x29c5', '\x0'), // reverse solidus in square 
			new Entity("bsolhsub",             '\x5c',   '\x2282'), // reverse solidus, subset 
			new Entity("bull",                 '\x2022', '\x0'), // bullet = black small circle, U+2022 ISOpub  
			new Entity("bullet",               '\x2022', '\x0'), // alias ISOPUB bull 
			new Entity("bump",                 '\x224e', '\x0'), // /Bumpeq R: bumpy equals 
			new Entity("bumpe",                '\x224f', '\x0'), // /bumpeq R: bumpy equals, equals 
			new Entity("bumpE",                '\x2aae', '\x0'), // bump, equals 
			new Entity("bumpeq",               '\x224f', '\x0'), // alias ISOAMSR bumpe 
			new Entity("Bumpeq",               '\x224e', '\x0'), // alias ISOAMSR bump 
			new Entity("cacute",               '\x107',  '\x0'), // =small c, acute accent 
			new Entity("Cacute",               '\x106',  '\x0'), // =capital C, acute accent 
			new Entity("cap",                  '\x2229', '\x0'), // intersection = cap, U+2229 ISOtech 
			new Entity("Cap",                  '\x22d2', '\x0'), // /Cap /doublecap B: dbl intersection 
			new Entity("capand",               '\x2a44', '\x0'), // intersection, and 
			new Entity("capbrcup",             '\x2a49', '\x0'), // intersection, bar, union 
			new Entity("capcap",               '\x2a4b', '\x0'), // intersection, intersection, joined 
			new Entity("capcup",               '\x2a47', '\x0'), // intersection above union 
			new Entity("capdot",               '\x2a40', '\x0'), // intersection, with dot 
			new Entity("CapitalDifferentialD", '\x2145', '\x0'), // D for use in differentials, e.g., within integrals 
			new Entity("caps",                 '\x2229', '\xfe00'), // intersection, serifs 
			new Entity("caret",                '\x2041', '\x0'), // =caret (insertion mark) 
			new Entity("caron",                '\x2c7',  '\x0'), // =caron 
			new Entity("Cayleys",              '\x212d', '\x0'), // the non-associative ring of octonions or Cayley numbers 
			new Entity("ccaps",                '\x2a4d', '\x0'), // closed intersection, serifs 
			new Entity("ccaron",               '\x10d',  '\x0'), // =small c, caron 
			new Entity("Ccaron",               '\x10c',  '\x0'), // =capital C, caron 
			new Entity("ccedil",               '\xe7',   '\x0'), // latin small c with cedilla, U+00E7 ISOlat1 
			new Entity("Ccedil",               '\xc7',   '\x0'), // latin capital C with cedilla, U+00C7 ISOlat1 
			new Entity("ccirc",                '\x109',  '\x0'), // =small c, circumflex accent 
			new Entity("Ccirc",                '\x108',  '\x0'), // =capital C, circumflex accent 
			new Entity("Cconint",              '\x2230', '\x0'), // triple contour integral operator 
			new Entity("ccups",                '\x2a4c', '\x0'), // closed union, serifs 
			new Entity("ccupssm",              '\x2a50', '\x0'), // closed union, serifs, smash product 
			new Entity("cdot",                 '\x10b',  '\x0'), // =small c, dot above 
			new Entity("Cdot",                 '\x10a',  '\x0'), // =capital C, dot above 
			new Entity("cedil",                '\xb8',   '\x0'), // cedilla = spacing cedilla, U+00B8 ISOdia 
			new Entity("Cedilla",              '\xb8',   '\x0'), // alias ISODIA cedil 
			new Entity("cemptyv",              '\x29b2', '\x0'), // circle, slash, small circle above 
			new Entity("cent",                 '\xa2',   '\x0'), // cent sign, U+00A2 ISOnum 
			new Entity("centerdot",            '\xb7',   '\x0'), // alias ISONUM middot 
			new Entity("CenterDot",            '\xb7',   '\x0'), // alias ISONUM middot 
			// new Entity("cfr",                  "%plane1D;520;"), /frak c, lower case c 
			new Entity("Cfr",                  '\x212d', '\x0'), // /frak C, upper case c 
			new Entity("chcy",                 '\x447',  '\x0'), // =small che, Cyrillic 
			new Entity("CHcy",                 '\x427',  '\x0'), // =capital CHE, Cyrillic 
			new Entity("check",                '\x2713', '\x0'), // /checkmark =tick, check mark 
			new Entity("checkmark",            '\x2713', '\x0'), // alias ISOPUB check 
			new Entity("chi",                  '\x3c7',  '\x0'), // greek small letter chi, U+03C7 ISOgrk3 
			new Entity("Chi",                  '\x3a7',  '\x0'), // greek capital letter chi, U+03A7 
			new Entity("cir",                  '\x25cb', '\x0'), // /circ B: =circle, open 
			new Entity("circ",                 '\x2c6',  '\x0'), // modifier letter circumflex accent, U+02C6 ISOpub 
			new Entity("circeq",               '\x2257', '\x0'), // alias ISOAMSR cire 
			new Entity("circlearrowleft",      '\x21ba', '\x0'), // alias ISOAMSA olarr 
			new Entity("circlearrowright",     '\x21bb', '\x0'), // alias ISOAMSA orarr 
			new Entity("circledast",           '\x229b', '\x0'), // alias ISOAMSB oast 
			new Entity("circledcirc",          '\x229a', '\x0'), // alias ISOAMSB ocir 
			new Entity("circleddash",          '\x229d', '\x0'), // alias ISOAMSB odash 
			new Entity("CircleDot",            '\x2299', '\x0'), // alias ISOAMSB odot 
			new Entity("circledR",             '\xae',   '\x0'), // alias ISONUM reg 
			new Entity("circledS",             '\x24c8', '\x0'), // alias ISOAMSO oS 
			new Entity("CircleMinus",          '\x2296', '\x0'), // alias ISOAMSB ominus 
			new Entity("CirclePlus",           '\x2295', '\x0'), // alias ISOAMSB oplus 
			new Entity("CircleTimes",          '\x2297', '\x0'), // alias ISOAMSB otimes 
			new Entity("cire",                 '\x2257', '\x0'), // /circeq R: circle, equals 
			new Entity("cirE",                 '\x29c3', '\x0'), // circle, two horizontal stroked to the right 
			new Entity("cirfnint",             '\x2a10', '\x0'), // circulation function 
			new Entity("cirmid",               '\x2aef', '\x0'), // circle, mid below 
			new Entity("cirscir",              '\x29c2', '\x0'), // circle, small circle to the right 
			new Entity("ClockwiseContourIntegral", '\x2232', '\x0'), // alias ISOTECH cwconint 
			new Entity("CloseCurlyDoubleQuote", '\x201d','\x0'), // alias ISONUM rdquo 
			new Entity("CloseCurlyQuote",      '\x2019', '\x0'), // alias ISONUM rsquo 
			new Entity("clubs",                '\x2663', '\x0'), // black club suit = shamrock, U+2663 ISOpub 
			new Entity("clubsuit",             '\x2663', '\x0'), // ISOPUB    clubs  
			new Entity("colon",                '\x3a',   '\x0'), // /colon P: 
			new Entity("Colon",                '\x2237', '\x0'), // /Colon, two colons 
			new Entity("colone",               '\x2254', '\x0'), // /coloneq R: colon, equals 
			new Entity("Colone",               '\x2a74', '\x0'), // double colon, equals 
			new Entity("coloneq",              '\x2254', '\x0'), // alias ISOAMSR colone 
			new Entity("comma",                '\x2c',   '\x0'), // P: =comma 
			new Entity("commat",               '\x40',   '\x0'), // =commercial at 
			new Entity("comp",                 '\x2201', '\x0'), // /complement - complement sign 
			new Entity("compfn",               '\x2218', '\x0'), // /circ B: composite function (small circle) 
			new Entity("complement",           '\x2201', '\x0'), // alias ISOAMSO comp 
			new Entity("complexes",            '\x2102', '\x0'), // the field of complex numbers 
			new Entity("cong",                 '\x2245', '\x0'), // approximately equal to, U+2245 ISOtech 
			new Entity("congdot",              '\x2a6d', '\x0'), // congruent, dot 
			new Entity("Congruent",            '\x2261', '\x0'), // alias ISOTECH equiv 
			new Entity("conint",               '\x222e', '\x0'), // /oint L: contour integral operator 
			new Entity("Conint",               '\x222f', '\x0'), // double contour integral operator 
			new Entity("ContourIntegral",      '\x222e', '\x0'), // alias ISOTECH conint 
			// new Entity("copf",                 "%plane1D;554;"),  
			new Entity("Copf",                 '\x2102', '\x0'), // /Bbb C, open face C 
			new Entity("coprod",               '\x2210', '\x0'), // /coprod L: coproduct operator 
			new Entity("Coproduct",            '\x2210', '\x0'), // alias ISOAMSB coprod 
			new Entity("copy",                 '\xa9',   '\x0'), // copyright sign, U+00A9 ISOnum 
			new Entity("copysr",               '\x2117', '\x0'), // =sound recording copyright sign 
			new Entity("CounterClockwiseContourIntegral", '\x2233', '\x0'), // alias ISOTECH awconint 
			new Entity("crarr",                '\x21b5', '\x0'), // 
			new Entity("cross",                '\x2717', '\x0'), // =ballot cross 
			new Entity("Cross",                '\x2a2f', '\x0'), // cross or vector product 
			// new Entity("cscr",                 "%plane1D;4B8;"), /scr c, script letter c 
			// new Entity("Cscr",                 "%plane1D;49E;"), /scr C, script letter C 
			new Entity("csub",                 '\x2acf', '\x0'), // subset, closed 
			new Entity("csube",                '\x2ad1', '\x0'), // subset, closed, equals 
			new Entity("csup",                 '\x2ad0', '\x0'), // superset, closed 
			new Entity("csupe",                '\x2ad2', '\x0'), // superset, closed, equals 
			new Entity("ctdot",                '\x22ef', '\x0'), // /cdots, three dots, centered 
			new Entity("cudarrl",              '\x2938', '\x0'), // left, curved, down arrow 
			new Entity("cudarrr",              '\x2935', '\x0'), // right, curved, down arrow 
			new Entity("cuepr",                '\x22de', '\x0'), // /curlyeqprec R: curly eq, precedes 
			new Entity("cuesc",                '\x22df', '\x0'), // /curlyeqsucc R: curly eq, succeeds 
			new Entity("cularr",               '\x21b6', '\x0'), // /curvearrowleft A: left curved arrow 
			new Entity("cularrp",              '\x293d', '\x0'), // curved left arrow with plus 
			new Entity("cup",                  '\x222a', '\x0'), // union = cup, U+222A ISOtech 
			new Entity("Cup",                  '\x22d3', '\x0'), // /Cup /doublecup B: dbl union 
			new Entity("cupbrcap",             '\x2a48', '\x0'), // union, bar, intersection 
			new Entity("cupcap",               '\x2a46', '\x0'), // union above intersection 
			new Entity("CupCap",               '\x224d', '\x0'), // alias asympeq 
			new Entity("cupcup",               '\x2a4a', '\x0'), // union, union, joined 
			new Entity("cupdot",               '\x228d', '\x0'), // union, with dot 
			new Entity("cupor",                '\x2a45', '\x0'), // union, or 
			new Entity("cups",                 '\x222a', '\xfe00'), // union, serifs 
			new Entity("curarr",               '\x21b7', '\x0'), // /curvearrowright A: rt curved arrow 
			new Entity("curarrm",              '\x293c', '\x0'), // curved right arrow with minus 
			new Entity("curlyeqprec",          '\x22de', '\x0'), // alias ISOAMSR cuepr 
			new Entity("curlyeqsucc",          '\x22df', '\x0'), // alias ISOAMSR cuesc 
			new Entity("curlyvee",             '\x22ce', '\x0'), // alias ISOAMSB cuvee 
			new Entity("curlywedge",           '\x22cf', '\x0'), // alias ISOAMSB cuwed 
			new Entity("curren",               '\xa4',   '\x0'), // currency sign, U+00A4 ISOnum 
			new Entity("curvearrowleft",       '\x21b6', '\x0'), // alias ISOAMSA cularr 
			new Entity("curvearrowright",      '\x21b7', '\x0'), // alias ISOAMSA curarr 
			new Entity("cuvee",                '\x22ce', '\x0'), // /curlyvee B: curly logical or 
			new Entity("cuwed",                '\x22cf', '\x0'), // /curlywedge B: curly logical and 
			new Entity("cwconint",             '\x2232', '\x0'), // contour integral, clockwise 
			new Entity("cwint",                '\x2231', '\x0'), // clockwise integral 
			new Entity("cylcty",               '\x232d', '\x0'), // cylindricity 
			new Entity("dagger",               '\x2020', '\x0'), // dagger, U+2020 ISOpub 
			new Entity("Dagger",               '\x2021', '\x0'), // double dagger, U+2021 ISOpub 
			new Entity("daleth",               '\x2138', '\x0'), // /daleth - daleth, Hebrew 
			new Entity("darr",                 '\x2193', '\x0'), // downwards arrow, U+2193 ISOnum 
			new Entity("dArr",                 '\x21d3', '\x0'), // downwards double arrow, U+21D3 ISOamsa 
			new Entity("Darr",                 '\x21a1', '\x0'), // down two-headed arrow 
			new Entity("dash",                 '\x2010', '\x0'), // =hyphen (true graphic) 
			new Entity("dashv",                '\x22a3', '\x0'), // /dashv R: dash, vertical 
			new Entity("Dashv",                '\x2ae4', '\x0'), // dbl dash, vertical 
			new Entity("dbkarow",              '\x290f', '\x0'), // alias ISOAMSA rBarr 
			new Entity("dblac",                '\x2dd',  '\x0'), // =double acute accent 
			new Entity("dcaron",               '\x10f',  '\x0'), // =small d, caron 
			new Entity("Dcaron",               '\x10e',  '\x0'), // =capital D, caron 
			new Entity("dcy",                  '\x434',  '\x0'), // =small de, Cyrillic 
			new Entity("Dcy",                  '\x414',  '\x0'), // =capital DE, Cyrillic 
			new Entity("dd",                   '\x2146', '\x0'), // d for use in differentials, e.g., within integrals 
			new Entity("DD",                   '\x2145', '\x0'), // D for use in differentials, e.g., within integrals 
			new Entity("ddagger",              '\x2021', '\x0'), // alias ISOPUB Dagger 
			new Entity("ddarr",                '\x21ca', '\x0'), // /downdownarrows A: two down arrows 
			new Entity("DDotrahd",             '\x2911', '\x0'), // right arrow with dotted stem 
			new Entity("ddotseq",              '\x2a77', '\x0'), // alias ISOAMSR eDDot 
			new Entity("deg",                  '\xb0',   '\x0'), // degree sign, U+00B0 ISOnum 
			new Entity("Del",                  '\x2207', '\x0'), // alias ISOTECH nabla 
			new Entity("delta",                '\x3b4',  '\x0'), // greek small letter delta, U+03B4 ISOgrk3 
			new Entity("Delta",                '\x394',  '\x0'), // greek capital letter delta, U+0394 ISOgrk3 
			new Entity("demptyv",              '\x29b1', '\x0'), // circle, slash, bar above 
			new Entity("dfisht",               '\x297f', '\x0'), // down fish tail 
			// new Entity("dfr",                  "%plane1D;521;"), /frak d, lower case d 
			// new Entity("Dfr",                  "%plane1D;507;"), /frak D, upper case d 
			new Entity("dHar",                 '\x2965', '\x0'), // down harpoon-left, down harpoon-right 
			new Entity("dharl",                '\x21c3', '\x0'), // /downharpoonleft A: dn harpoon-left 
			new Entity("dharr",                '\x21c2', '\x0'), // /downharpoonright A: down harpoon-rt 
			new Entity("DiacriticalAcute",     '\xb4',   '\x0'), // alias ISODIA acute 
			new Entity("DiacriticalDot",       '\x2d9',  '\x0'), // alias ISODIA dot 
			new Entity("DiacriticalDoubleAcute", '\x2dd','\x0'), // alias ISODIA dblac 
			new Entity("DiacriticalGrave",     '\x60',   '\x0'), // alias ISODIA grave 
			new Entity("DiacriticalTilde",     '\x2dc',  '\x0'), // alias ISODIA tilde 
			new Entity("diam",                 '\x22c4', '\x0'), // /diamond B: open diamond 
			new Entity("diamond",              '\x22c4', '\x0'), // alias ISOAMSB diam 
			new Entity("Diamond",              '\x22c4', '\x0'), // alias ISOAMSB diam 
			new Entity("diamondsuit",          '\x2666', '\x0'), // ISOPUB    diams  
			new Entity("diams",                '\x2666', '\x0'), // black diamond suit, U+2666 ISOpub 
			new Entity("die",                  '\xa8',   '\x0'), // =dieresis 
			new Entity("DifferentialD",        '\x2146', '\x0'), // d for use in differentials, e.g., within integrals 
			new Entity("digamma",              '\x3dd',  '\x0'), // alias ISOGRK3 gammad 
			new Entity("disin",                '\x22f2', '\x0'), // set membership, long horizontal stroke 
			new Entity("div",                  '\xf7',   '\x0'), // alias ISONUM divide 
			new Entity("divide",               '\xf7',   '\x0'), // division sign, U+00F7 ISOnum 
			new Entity("divideontimes",        '\x22c7', '\x0'), // alias ISOAMSB divonx 
			new Entity("divonx",               '\x22c7', '\x0'), // /divideontimes B: division on times 
			new Entity("djcy",                 '\x452',  '\x0'), // =small dje, Serbian 
			new Entity("DJcy",                 '\x402',  '\x0'), // =capital DJE, Serbian 
			new Entity("dlcorn",               '\x231e', '\x0'), // /llcorner O: lower left corner 
			new Entity("dlcrop",               '\x230d', '\x0'), // downward left crop mark  
			new Entity("dollar",               '\x24',   '\x0'), // =dollar sign 
			// new Entity("dopf",                 "%plane1D;555;"),  
			// new Entity("Dopf",                 "%plane1D;53B;"), /Bbb D, open face D 
			new Entity("dot",                  '\x2d9',  '\x0'), // =dot above 
			new Entity("Dot",                  '\xa8',   '\x0'), // dieresis or umlaut mark 
			new Entity("DotDot",               '\x20dc', '\x0'), // four dots above 
			new Entity("doteq",                '\x2250', '\x0'), // alias ISOAMSR esdot 
			new Entity("doteqdot",             '\x2251', '\x0'), // alias ISOAMSR eDot 
			new Entity("DotEqual",             '\x2250', '\x0'), // alias ISOAMSR esdot 
			new Entity("dotminus",             '\x2238', '\x0'), // alias ISOAMSB minusd 
			new Entity("dotplus",              '\x2214', '\x0'), // alias ISOAMSB plusdo 
			new Entity("dotsquare",            '\x22a1', '\x0'), // alias ISOAMSB sdotb 
			new Entity("doublebarwedge",       '\x2306', '\x0'), // alias ISOAMSB Barwed 
			new Entity("DoubleContourIntegral", '\x222f','\x0'), // alias ISOTECH Conint 
			new Entity("DoubleDot",            '\xa8',   '\x0'), // alias ISODIA die 
			new Entity("DoubleDownArrow",      '\x21d3', '\x0'), // alias ISOAMSA dArr 
			new Entity("DoubleLeftArrow",      '\x21d0', '\x0'), // alias ISOTECH lArr 
			new Entity("DoubleLeftRightArrow", '\x21d4', '\x0'), // alias ISOAMSA hArr 
			new Entity("DoubleLeftTee",        '\x2ae4', '\x0'), // alias for  &Dashv;  
			new Entity("DoubleLongLeftArrow",  '\x27f8', '\x0'), // alias ISOAMSA xlArr 
			new Entity("DoubleLongLeftRightArrow", '\x27fa', '\x0'), // alias ISOAMSA xhArr 
			new Entity("DoubleLongRightArrow", '\x27f9', '\x0'), // alias ISOAMSA xrArr 
			new Entity("DoubleRightArrow",     '\x21d2', '\x0'), // alias ISOTECH rArr 
			new Entity("DoubleRightTee",       '\x22a8', '\x0'), // alias ISOAMSR vDash 
			new Entity("DoubleUpArrow",        '\x21d1', '\x0'), // alias ISOAMSA uArr 
			new Entity("DoubleUpDownArrow",    '\x21d5', '\x0'), // alias ISOAMSA vArr 
			new Entity("DoubleVerticalBar",    '\x2225', '\x0'), // alias ISOTECH par 
			new Entity("downarrow",            '\x2193', '\x0'), // alias ISONUM darr 
			new Entity("Downarrow",            '\x21d3', '\x0'), // alias ISOAMSA dArr 
			new Entity("DownArrow",            '\x2193', '\x0'), // alias ISONUM darr 
			new Entity("DownArrowBar",         '\x2913', '\x0'), // down arrow to bar 
			new Entity("DownArrowUpArrow",     '\x21f5', '\x0'), // alias ISOAMSA duarr 
			new Entity("DownBreve",            '\x311',  '\x0'), // breve, inverted (non-spacing) 
			new Entity("downdownarrows",       '\x21ca', '\x0'), // alias ISOAMSA ddarr 
			new Entity("downharpoonleft",      '\x21c3', '\x0'), // alias ISOAMSA dharl 
			new Entity("downharpoonright",     '\x21c2', '\x0'), // alias ISOAMSA dharr 
			new Entity("DownLeftRightVector",  '\x2950', '\x0'), // left-down-right-down harpoon 
			new Entity("DownLeftTeeVector",    '\x295e', '\x0'), // left-down harpoon from bar 
			new Entity("DownLeftVector",       '\x21bd', '\x0'), // alias ISOAMSA lhard 
			new Entity("DownLeftVectorBar",    '\x2956', '\x0'), // left-down harpoon to bar 
			new Entity("DownRightTeeVector",   '\x295f', '\x0'), // right-down harpoon from bar 
			new Entity("DownRightVector",      '\x21c1', '\x0'), // alias ISOAMSA rhard 
			new Entity("DownRightVectorBar",   '\x2957', '\x0'), // right-down harpoon to bar 
			new Entity("DownTee",              '\x22a4', '\x0'), // alias ISOTECH top 
			new Entity("DownTeeArrow",         '\x21a7', '\x0'), // alias for mapstodown 
			new Entity("drbkarow",             '\x2910', '\x0'), // alias ISOAMSA RBarr 
			new Entity("drcorn",               '\x231f', '\x0'), // /lrcorner C: lower right corner 
			new Entity("drcrop",               '\x230c', '\x0'), // downward right crop mark  
			// new Entity("dscr",                 "%plane1D;4B9;"), /scr d, script letter d 
			// new Entity("Dscr",                 "%plane1D;49F;"), /scr D, script letter D 
			new Entity("dscy",                 '\x455',  '\x0'), // =small dse, Macedonian 
			new Entity("DScy",                 '\x405',  '\x0'), // =capital DSE, Macedonian 
			new Entity("dsol",                 '\x29f6', '\x0'), // solidus, bar above 
			new Entity("dstrok",               '\x111',  '\x0'), // =small d, stroke 
			new Entity("Dstrok",               '\x110',  '\x0'), // =capital D, stroke 
			new Entity("dtdot",                '\x22f1', '\x0'), // /ddots, three dots, descending 
			new Entity("dtri",                 '\x25bf', '\x0'), // /triangledown =down triangle, open 
			new Entity("dtrif",                '\x25be', '\x0'), // /blacktriangledown =dn tri, filled 
			new Entity("duarr",                '\x21f5', '\x0'), // down arrow, up arrow 
			new Entity("duhar",                '\x296f', '\x0'), // down harp, up harp 
			new Entity("dwangle",              '\x29a6', '\x0'), // large downward pointing angle 
			new Entity("dzcy",                 '\x45f',  '\x0'), // =small dze, Serbian 
			new Entity("DZcy",                 '\x40f',  '\x0'), // =capital dze, Serbian 
			new Entity("dzigrarr",             '\x27ff', '\x0'), // right long zig-zag arrow 
			new Entity("eacute",               '\xe9',   '\x0'), // latin small e with acute, U+00E9 ISOlat1 
			new Entity("Eacute",               '\xc9',   '\x0'), // latin capital E with acute, U+00C9 ISOlat1 
			new Entity("easter",               '\x2a6e', '\x0'), // equal, asterisk above 
			new Entity("ecaron",               '\x11b',  '\x0'), // =small e, caron 
			new Entity("Ecaron",               '\x11a',  '\x0'), // =capital E, caron 
			new Entity("ecir",                 '\x2256', '\x0'), // /eqcirc R: circle on equals sign 
			new Entity("ecirc",                '\xea',   '\x0'), // latin small e with circumflex, U+00EA ISOlat1 
			new Entity("Ecirc",                '\xca',   '\x0'), // latin capital E with circumflex, U+00CA ISOlat1 
			new Entity("ecolon",               '\x2255', '\x0'), // /eqcolon R: equals, colon 
			new Entity("ecy",                  '\x44d',  '\x0'), // =small e, Cyrillic 
			new Entity("Ecy",                  '\x42d',  '\x0'), // =capital E, Cyrillic 
			new Entity("eDDot",                '\x2a77', '\x0'), // /ddotseq R: equal with four dots 
			new Entity("edot",                 '\x117',  '\x0'), // =small e, dot above 
			new Entity("eDot",                 '\x2251', '\x0'), // /doteqdot /Doteq R: eq, even dots 
			new Entity("Edot",                 '\x116',  '\x0'), // =capital E, dot above 
			new Entity("ee",                   '\x2147', '\x0'), // e use for the exponential base of the natural logarithms 
			new Entity("efDot",                '\x2252', '\x0'), // /fallingdotseq R: eq, falling dots 
			// new Entity("efr",                  "%plane1D;522;"), /frak e, lower case e 
			// new Entity("Efr",                  "%plane1D;508;"), /frak E, upper case e 
			new Entity("eg",                   '\x2a9a', '\x0'), // equal-or-greater 
			new Entity("egrave",               '\xe8',   '\x0'), // latin small e with grave, U+00E8 ISOlat1 
			new Entity("Egrave",               '\xc8',   '\x0'), // latin capital E with grave, U+00C8 ISOlat1 
			new Entity("egs",                  '\x2a96', '\x0'), // /eqslantgtr R: equal-or-gtr, slanted 
			new Entity("egsdot",               '\x2a98', '\x0'), // equal-or-greater, slanted, dot inside 
			new Entity("el",                   '\x2a99', '\x0'), // equal-or-less 
			new Entity("Element",              '\x2208', '\x0'), // alias ISOTECH isinv 
			new Entity("elinters",             '\xfffd', '\x0'), // electrical intersection 
			new Entity("ell",                  '\x2113', '\x0'), // /ell - cursive small l 
			new Entity("els",                  '\x2a95', '\x0'), // /eqslantless R: eq-or-less, slanted 
			new Entity("elsdot",               '\x2a97', '\x0'), // equal-or-less, slanted, dot inside 
			new Entity("emacr",                '\x113',  '\x0'), // =small e, macron 
			new Entity("Emacr",                '\x112',  '\x0'), // =capital E, macron 
			new Entity("empty",                '\x2205', '\x0'), // empty set = null set = diameter, U+2205 ISOamso 
			new Entity("emptyset",             '\x2205', '\x0'), // alias ISOAMSO empty 
			new Entity("EmptySmallSquare",     '\x25fb', '\x0'), // empty small square 
			new Entity("emptyv",               '\x2205', '\x0'), // /varnothing - circle, slash 
			new Entity("EmptyVerySmallSquare", '\x25ab', '\x0'), // empty small square 
			new Entity("emsp",                 '\x2003', '\x0'), // em space, U+2003 ISOpub 
			new Entity("emsp13",               '\x2004', '\x0'), // =1/3-em space 
			new Entity("emsp14",               '\x2005', '\x0'), // =1/4-em space 
			new Entity("eng",                  '\x14b',  '\x0'), // =small eng, Lapp 
			new Entity("ENG",                  '\x14a',  '\x0'), // =capital ENG, Lapp 
			new Entity("ensp",                 '\x2002', '\x0'), // en space, U+2002 ISOpub 
			new Entity("eogon",                '\x119',  '\x0'), // =small e, ogonek 
			new Entity("Eogon",                '\x118',  '\x0'), // =capital E, ogonek 
			// new Entity("eopf",                 "%plane1D;556;"),  
			// new Entity("Eopf",                 "%plane1D;53C;"), /Bbb E, open face E 
			new Entity("epar",                 '\x22d5', '\x0'), // parallel, equal; equal or parallel 
			new Entity("eparsl",               '\x29e3', '\x0'), // parallel, slanted, equal; homothetically congruent to 
			new Entity("eplus",                '\x2a71', '\x0'), // equal, plus 
			new Entity("epsi",                 '\x3f5',  '\x0'), // /straightepsilon, small epsilon, Greek 
			new Entity("epsilon",              '\x3b5',  '\x0'), // greek small letter epsilon, U+03B5 ISOgrk3 
			new Entity("Epsilon",              '\x395',  '\x0'), // greek capital letter epsilon, U+0395 
			new Entity("epsiv",                '\x3b5',  '\x0'), // /varepsilon 
			new Entity("eqcirc",               '\x2256', '\x0'), // alias ISOAMSR ecir 
			new Entity("eqcolon",              '\x2255', '\x0'), // alias ISOAMSR ecolon 
			new Entity("eqsim",                '\x2242', '\x0'), // alias ISOAMSR esim 
			new Entity("eqslantgtr",           '\x2a96', '\x0'), // alias ISOAMSR egs 
			new Entity("eqslantless",          '\x2a95', '\x0'), // alias ISOAMSR els 
			new Entity("Equal",                '\x2a75', '\x0'), // two consecutive equal signs 
			new Entity("equals",               '\x3d',   '\x0'), // =equals sign R: 
			new Entity("EqualTilde",           '\x2242', '\x0'), // alias ISOAMSR esim 
			new Entity("equest",               '\x225f', '\x0'), // /questeq R: equal with questionmark 
			new Entity("Equilibrium",          '\x21cc', '\x0'), // alias ISOAMSA rlhar 
			new Entity("equiv",                '\x2261', '\x0'), // identical to, U+2261 ISOtech 
			new Entity("equivDD",              '\x2a78', '\x0'), // equivalent, four dots above 
			new Entity("eqvparsl",             '\x29e5', '\x0'), // equivalent, equal; congruent and parallel 
			new Entity("erarr",                '\x2971', '\x0'), // equal, right arrow below 
			new Entity("erDot",                '\x2253', '\x0'), // /risingdotseq R: eq, rising dots 
			new Entity("escr",                 '\x212f', '\x0'), // /scr e, script letter e 
			new Entity("Escr",                 '\x2130', '\x0'), // /scr E, script letter E 
			new Entity("esdot",                '\x2250', '\x0'), // /doteq R: equals, single dot above 
			new Entity("esim",                 '\x2242', '\x0'), // /esim R: equals, similar 
			new Entity("Esim",                 '\x2a73', '\x0'), // equal, similar 
			new Entity("eta",                  '\x3b7',  '\x0'), // greek small letter eta, U+03B7 ISOgrk3 
			new Entity("Eta",                  '\x397',  '\x0'), // greek capital letter eta, U+0397 
			new Entity("eth",                  '\xf0',   '\x0'), // latin small eth, U+00F0 ISOlat1 
			new Entity("ETH",                  '\xd0',   '\x0'), // latin capital ETH, U+00D0 ISOlat1 
			new Entity("euml",                 '\xeb',   '\x0'), // latin small e with diaeresis, U+00EB ISOlat1 
			new Entity("Euml",                 '\xcb',   '\x0'), // latin capital E with diaeresis, U+00CB ISOlat1 
			new Entity("euro",                 '\x20ac', '\x0'), // euro sign, U+20AC NEW 
			new Entity("excl",                 '\x21',   '\x0'), // =exclamation mark 
			new Entity("exist",                '\x2203', '\x0'), // there exists, U+2203 ISOtech 
			new Entity("Exists",               '\x2203', '\x0'), // alias ISOTECH exist 
			new Entity("expectation",          '\x2130', '\x0'), // expectation (operator) 
			new Entity("exponentiale",         '\x2147', '\x0'), // base of the Napierian logarithms 
			new Entity("ExponentialE",         '\x2147', '\x0'), // e use for the exponential base of the natural logarithms 
			new Entity("fallingdotseq",        '\x2252', '\x0'), // alias ISOAMSR efDot 
			new Entity("fcy",                  '\x444',  '\x0'), // =small ef, Cyrillic 
			new Entity("Fcy",                  '\x424',  '\x0'), // =capital EF, Cyrillic 
			new Entity("female",               '\x2640', '\x0'), // =female symbol 
			new Entity("ffilig",               '\xfb03', '\x0'), // small ffi ligature 
			new Entity("fflig",                '\xfb00', '\x0'), // small ff ligature 
			new Entity("ffllig",               '\xfb04', '\x0'), // small ffl ligature 
			// new Entity("ffr",                  "%plane1D;523;"), /frak f, lower case f 
			// new Entity("Ffr",                  "%plane1D;509;"), /frak F, upper case f 
			new Entity("filig",                '\xfb01', '\x0'), // small fi ligature 
			new Entity("FilledSmallSquare",    '\x25fc', '\x0'), // filled small square 
			new Entity("FilledVerySmallSquare", '\x25aa','\x0'), // filled very small square 
			new Entity("flat",                 '\x266d', '\x0'), // /flat =musical flat 
			new Entity("fllig",                '\xfb02', '\x0'), // small fl ligature 
			new Entity("fltns",                '\x25b1', '\x0'), // flatness 
			new Entity("fnof",                 '\x192',  '\x0'), // 
			// new Entity("fopf",                 "%plane1D;557;"),  
			// new Entity("Fopf",                 "%plane1D;53D;"), /Bbb F, open face F 
			new Entity("forall",               '\x2200', '\x0'), // for all, U+2200 ISOtech 
			new Entity("ForAll",               '\x2200', '\x0'), // alias ISOTECH forall 
			new Entity("fork",                 '\x22d4', '\x0'), // /pitchfork R: pitchfork 
			new Entity("forkv",                '\x2ad9', '\x0'), // fork, variant 
			new Entity("Fouriertrf",           '\x2131', '\x0'), // Fourier transform 
			new Entity("fpartint",             '\x2a0d', '\x0'), // finite part integral 
			new Entity("frac12",               '\xbd',   '\x0'), // vulgar fraction one half = fraction one half, U+00BD ISOnum 
			new Entity("frac13",               '\x2153', '\x0'), // =fraction one-third 
			new Entity("frac14",               '\xbc',   '\x0'), // vulgar fraction one quarter = fraction one quarter, U+00BC ISOnum 
			new Entity("frac15",               '\x2155', '\x0'), // =fraction one-fifth 
			new Entity("frac16",               '\x2159', '\x0'), // =fraction one-sixth 
			new Entity("frac18",               '\x215b', '\x0'), // =fraction one-eighth 
			new Entity("frac23",               '\x2154', '\x0'), // =fraction two-thirds 
			new Entity("frac25",               '\x2156', '\x0'), // =fraction two-fifths 
			new Entity("frac34",               '\xbe',   '\x0'), // vulgar fraction three quarters = fraction three quarters, U+00BE ISOnum 
			new Entity("frac35",               '\x2157', '\x0'), // =fraction three-fifths 
			new Entity("frac38",               '\x215c', '\x0'), // =fraction three-eighths 
			new Entity("frac45",               '\x2158', '\x0'), // =fraction four-fifths 
			new Entity("frac56",               '\x215a', '\x0'), // =fraction five-sixths 
			new Entity("frac58",               '\x215d', '\x0'), // =fraction five-eighths 
			new Entity("frac78",               '\x215e', '\x0'), // =fraction seven-eighths 
			new Entity("frasl",                '\x2044', '\x0'), // fraction slash, U+2044 NEW 
			new Entity("frown",                '\x2322', '\x0'), // /frown R: down curve 
			// new Entity("fscr",                 "%plane1D;4BB;"), /scr f, script letter f 
			new Entity("Fscr",                 '\x2131', '\x0'), // /scr F, script letter F 
			new Entity("gacute",               '\x1f5',  '\x0'), // =small g, acute accent 
			new Entity("gamma",                '\x3b3',  '\x0'), // greek small letter gamma, U+03B3 ISOgrk3 
			new Entity("Gamma",                '\x393',  '\x0'), // greek capital letter gamma, U+0393 ISOgrk3 
			new Entity("gammad",               '\x3dd',  '\x0'), // /digamma 
			new Entity("Gammad",               '\x3dc',  '\x0'), // capital digamma 
			new Entity("gap",                  '\x2a86', '\x0'), // /gtrapprox R: greater, approximate 
			new Entity("gbreve",               '\x11f',  '\x0'), // =small g, breve 
			new Entity("Gbreve",               '\x11e',  '\x0'), // =capital G, breve 
			new Entity("Gcedil",               '\x122',  '\x0'), // =capital G, cedilla 
			new Entity("gcirc",                '\x11d',  '\x0'), // =small g, circumflex accent 
			new Entity("Gcirc",                '\x11c',  '\x0'), // =capital G, circumflex accent 
			new Entity("gcy",                  '\x433',  '\x0'), // =small ghe, Cyrillic 
			new Entity("Gcy",                  '\x413',  '\x0'), // =capital GHE, Cyrillic 
			new Entity("gdot",                 '\x121',  '\x0'), // =small g, dot above 
			new Entity("Gdot",                 '\x120',  '\x0'), // =capital G, dot above 
			new Entity("ge",                   '\x2265', '\x0'), // greater-than or equal to, U+2265 ISOtech 
			new Entity("gE",                   '\x2267', '\x0'), // /geqq R: greater, double equals 
			new Entity("gel",                  '\x22db', '\x0'), // /gtreqless R: greater, equals, less 
			new Entity("gEl",                  '\x2a8c', '\x0'), // /gtreqqless R: gt, dbl equals, less 
			new Entity("geq",                  '\x2265', '\x0'), // alias ISOTECH ge 
			new Entity("geqq",                 '\x2267', '\x0'), // alias ISOAMSR gE 
			new Entity("geqslant",             '\x2a7e', '\x0'), // alias ISOAMSR ges 
			new Entity("ges",                  '\x2a7e', '\x0'), // /geqslant R: gt-or-equal, slanted 
			new Entity("gescc",                '\x2aa9', '\x0'), // greater than, closed by curve, equal, slanted 
			new Entity("gesdot",               '\x2a80', '\x0'), // greater-than-or-equal, slanted, dot inside 
			new Entity("gesdoto",              '\x2a82', '\x0'), // greater-than-or-equal, slanted, dot above 
			new Entity("gesdotol",             '\x2a84', '\x0'), // greater-than-or-equal, slanted, dot above left 
			new Entity("gesl",                 '\x22db', '\xfe00'), // greater, equal, slanted, less 
			new Entity("gesles",               '\x2a94', '\x0'), // greater, equal, slanted, less, equal, slanted 
			// new Entity("gfr",                  "%plane1D;524;"), /frak g, lower case g 
			// new Entity("Gfr",                  "%plane1D;50A;"), /frak G, upper case g 
			new Entity("gg",                   '\x226b', '\x0'), // alias ISOAMSR Gt 
			new Entity("Gg",                   '\x22d9', '\x0'), // /ggg /Gg /gggtr R: triple gtr-than 
			new Entity("ggg",                  '\x22d9', '\x0'), // alias ISOAMSR Gg 
			new Entity("gimel",                '\x2137', '\x0'), // /gimel - gimel, Hebrew 
			new Entity("gjcy",                 '\x453',  '\x0'), // =small gje, Macedonian 
			new Entity("GJcy",                 '\x403',  '\x0'), // =capital GJE Macedonian 
			new Entity("gl",                   '\x2277', '\x0'), // /gtrless R: greater, less 
			new Entity("gla",                  '\x2aa5', '\x0'), // greater, less, apart 
			new Entity("glE",                  '\x2a92', '\x0'), // greater, less, equal 
			new Entity("glj",                  '\x2aa4', '\x0'), // greater, less, overlapping 
			new Entity("gnap",                 '\x2a8a', '\x0'), // /gnapprox N: greater, not approximate 
			new Entity("gnapprox",             '\x2a8a', '\x0'), // alias ISOAMSN gnap 
			new Entity("gne",                  '\x2a88', '\x0'), // /gneq N: greater, not equals 
			new Entity("gnE",                  '\x2269', '\x0'), // /gneqq N: greater, not dbl equals 
			new Entity("gneq",                 '\x2a88', '\x0'), // alias ISOAMSN gne 
			new Entity("gneqq",                '\x2269', '\x0'), // alias ISOAMSN gnE 
			new Entity("gnsim",                '\x22e7', '\x0'), // /gnsim N: greater, not similar 
			// new Entity("gopf",                 "%plane1D;558;"),  
			// new Entity("Gopf",                 "%plane1D;53E;"), /Bbb G, open face G 
			new Entity("grave",                '\x60', '\x0'), // =grave accent 
			new Entity("GreaterEqual",         '\x2265', '\x0'), // alias ISOTECH ge 
			new Entity("GreaterEqualLess",     '\x22db', '\x0'), // alias ISOAMSR gel 
			new Entity("GreaterFullEqual",     '\x2267', '\x0'), // alias ISOAMSR gE 
			new Entity("GreaterGreater",       '\x2aa2', '\x0'), // alias for GT 
			new Entity("GreaterLess",          '\x2277', '\x0'), // alias ISOAMSR gl 
			new Entity("GreaterSlantEqual",    '\x2a7e', '\x0'), // alias ISOAMSR ges 
			new Entity("GreaterTilde",         '\x2273', '\x0'), // alias ISOAMSR gsim 
			new Entity("gscr",                 '\x210a', '\x0'), // /scr g, script letter g 
			// new Entity("Gscr",                 "%plane1D;4A2;"), /scr G, script letter G 
			new Entity("gsim",                 '\x2273', '\x0'), // /gtrsim R: greater, similar 
			new Entity("gsime",                '\x2a8e', '\x0'), // greater, similar, equal 
			new Entity("gsiml",                '\x2a90', '\x0'), // greater, similar, less 
			new Entity("gt",                   '\x3e', '\x0'), // greater-than sign, U+003E ISOnum 
			new Entity("Gt",                   '\x226b', '\x0'), // /gg R: dbl greater-than sign 
			new Entity("gtcc",                 '\x2aa7', '\x0'), // greater than, closed by curve 
			new Entity("gtcir",                '\x2a7a', '\x0'), // greater than, circle inside 
			new Entity("gtdot",                '\x22d7', '\x0'), // /gtrdot R: greater than, with dot 
			new Entity("gtlPar",               '\x2995', '\x0'), // dbl left parenthesis, greater 
			new Entity("gtquest",              '\x2a7c', '\x0'), // greater than, questionmark above 
			new Entity("gtrapprox",            '\x2a86', '\x0'), // alias ISOAMSR gap 
			new Entity("gtrarr",               '\x2978', '\x0'), // greater than, right arrow 
			new Entity("gtrdot",               '\x22d7', '\x0'), // alias ISOAMSR gtdot 
			new Entity("gtreqless",            '\x22db', '\x0'), // alias ISOAMSR gel 
			new Entity("gtreqqless",           '\x2a8c', '\x0'), // alias ISOAMSR gEl 
			new Entity("gtrless",              '\x2277', '\x0'), // alias ISOAMSR gl 
			new Entity("gtrsim",               '\x2273', '\x0'), // alias ISOAMSR gsim 
			new Entity("gvertneqq",            '\x2269', '\xfe00'), // alias ISOAMSN gvnE 
			new Entity("gvnE",                 '\x2269', '\xfe00'), // /gvertneqq N: gt, vert, not dbl eq 
			new Entity("Hacek",                '\x2c7', '\x0'), // alias ISODIA caron 
			new Entity("hairsp",               '\x200a', '\x0'), // =hair space 
			new Entity("half",                 '\xbd', '\x0'), // =fraction one-half 
			new Entity("hamilt",               '\x210b', '\x0'), // Hamiltonian (script capital H)  
			new Entity("hardcy",               '\x44a', '\x0'), // =small hard sign, Cyrillic 
			new Entity("HARDcy",               '\x42a', '\x0'), // =capital HARD sign, Cyrillic 
			new Entity("harr",                 '\x2194', '\x0'), // left right arrow, U+2194 ISOamsa 
			new Entity("hArr",                 '\x21d4', '\x0'), // left right double arrow, U+21D4 ISOamsa 
			new Entity("harrcir",              '\x2948', '\x0'), // left and right arrow with a circle 
			new Entity("harrw",                '\x21ad', '\x0'), // /leftrightsquigarrow A: l&r arr-wavy 
			new Entity("Hat",                  '\x5e', '\x0'), // circumflex accent 
			new Entity("hbar",                 '\x210f', '\x0'), // alias ISOAMSO plank 
			new Entity("hcirc",                '\x125', '\x0'), // =small h, circumflex accent 
			new Entity("Hcirc",                '\x124', '\x0'), // =capital H, circumflex accent 
			new Entity("hearts",               '\x2665', '\x0'), // black heart suit = valentine, U+2665 ISOpub 
			new Entity("heartsuit",            '\x2665', '\x0'), // ISOPUB hearts 
			new Entity("hellip",               '\x2026', '\x0'), // horizontal ellipsis = three dot leader, U+2026 ISOpub  
			new Entity("hercon",               '\x22b9', '\x0'), // hermitian conjugate matrix 
			// new Entity("hfr",                  "%plane1D;525;"), /frak h, lower case h 
			new Entity("Hfr",                  '\x210c', '\x0'), // /frak H, upper case h 
			new Entity("HilbertSpace",         '\x210b', '\x0'), // Hilbert space 
			new Entity("hksearow",             '\x2925', '\x0'), // alias ISOAMSA searhk 
			new Entity("hkswarow",             '\x2926', '\x0'), // alias ISOAMSA swarhk 
			new Entity("hoarr",                '\x21ff', '\x0'), // horizontal open arrow 
			new Entity("homtht",               '\x223b', '\x0'), // homothetic 
			new Entity("hookleftarrow",        '\x21a9', '\x0'), // alias ISOAMSA larrhk 
			new Entity("hookrightarrow",       '\x21aa', '\x0'), // alias ISOAMSA rarrhk 
			// new Entity("hopf",                 "%plane1D;559;"),  
			new Entity("Hopf",                 '\x210d', '\x0'), // /Bbb H, open face H 
			new Entity("horbar",               '\x2015', '\x0'), // =horizontal bar 
			new Entity("HorizontalLine",       '\x2500', '\x0'), // short horizontal line  
			// new Entity("hscr",                 "%plane1D;4BD;"), /scr h, script letter h 
			new Entity("Hscr",                 '\x210b', '\x0'), // /scr H, script letter H 
			new Entity("hslash",               '\x210f', '\x0'), // alias ISOAMSO plankv 
			new Entity("hstrok",               '\x127', '\x0'), // =small h, stroke 
			new Entity("Hstrok",               '\x126', '\x0'), // =capital H, stroke 
			new Entity("HumpDownHump",         '\x224e', '\x0'), // alias ISOAMSR bump 
			new Entity("HumpEqual",            '\x224f', '\x0'), // alias ISOAMSR bumpe 
			new Entity("hybull",               '\x2043', '\x0'), // rectangle, filled (hyphen bullet) 
			new Entity("hyphen",               '\x2010', '\x0'), // =hyphen 
			new Entity("iacute",               '\xed', '\x0'), // latin small i with acute, U+00ED ISOlat1 
			new Entity("Iacute",               '\xcd', '\x0'), // latin capital I with acute, U+00CD ISOlat1 
			new Entity("ic",                   '\x2063', '\x0'), // short form of  &InvisibleComma; 
			new Entity("icirc",                '\xee', '\x0'), // latin small i with circumflex, U+00EE ISOlat1 
			new Entity("Icirc",                '\xce', '\x0'), // latin capital I with circumflex, U+00CE ISOlat1 
			new Entity("icy",                  '\x438', '\x0'), // =small i, Cyrillic 
			new Entity("Icy",                  '\x418', '\x0'), // =capital I, Cyrillic 
			new Entity("Idot",                 '\x130', '\x0'), // =capital I, dot above 
			new Entity("iecy",                 '\x435', '\x0'), // =small ie, Cyrillic 
			new Entity("IEcy",                 '\x415', '\x0'), // =capital IE, Cyrillic 
			new Entity("iexcl",                '\xa1', '\x0'), // inverted exclamation mark, U+00A1 ISOnum 
			new Entity("iff",                  '\x21d4', '\x0'), // /iff if and only if  
			// new Entity("ifr",                  "%plane1D;526;"), /frak i, lower case i 
			new Entity("Ifr",                  '\x2111', '\x0'), // /frak I, upper case i 
			new Entity("igrave",               '\xec', '\x0'), // latin small i with grave, U+00EC ISOlat1 
			new Entity("Igrave",               '\xcc', '\x0'), // latin capital I with grave, U+00CC ISOlat1 
			new Entity("ii",                   '\x2148', '\x0'), // i for use as a square root of -1 
			new Entity("iiiint",               '\x2a0c', '\x0'), // alias ISOTECH qint 
			new Entity("iiint",                '\x222d', '\x0'), // alias ISOTECH tint 
			new Entity("iinfin",               '\x29dc', '\x0'), // infinity sign, incomplete 
			new Entity("iiota",                '\x2129', '\x0'), // inverted iota 
			new Entity("ijlig",                '\x133', '\x0'), // =small ij ligature 
			new Entity("IJlig",                '\x132', '\x0'), // =capital IJ ligature 
			new Entity("Im",                   '\x2111', '\x0'), // alias ISOAMSO image 
			new Entity("imacr",                '\x12b', '\x0'), // =small i, macron 
			new Entity("Imacr",                '\x12a', '\x0'), // =capital I, macron 
			new Entity("image",                '\x2111', '\x0'), // blackletter capital I = imaginary part, U+2111 ISOamso 
			new Entity("ImaginaryI",           '\x2148', '\x0'), // i for use as a square root of -1 
			new Entity("imagline",             '\x2110', '\x0'), // the geometric imaginary line 
			new Entity("imagpart",             '\x2111', '\x0'), // alias ISOAMSO image 
			new Entity("imath",                '\x131', '\x0'), // /imath - small i, no dot 
			new Entity("imof",                 '\x22b7', '\x0'), // image of 
			new Entity("imped",                '\x1b5', '\x0'), // impedance 
			new Entity("Implies",              '\x21d2', '\x0'), // alias ISOTECH rArr 
			new Entity("in",                   '\x2208', '\x0'), // ISOTECH   isin  
			new Entity("incare",               '\x2105', '\x0'), // =in-care-of symbol 
			new Entity("infin",                '\x221e', '\x0'), // infinity, U+221E ISOtech 
			new Entity("infintie",             '\x29dd', '\x0'), // tie, infinity 
			new Entity("inodot",               '\x131', '\x0'), // =small i without dot 
			new Entity("int",                  '\x222b', '\x0'), // integral, U+222B ISOtech 
			new Entity("Int",                  '\x222c', '\x0'), // double integral operator 
			new Entity("intcal",               '\x22ba', '\x0'), // /intercal B: intercal 
			new Entity("integers",             '\x2124', '\x0'), // the ring of integers 
			new Entity("Integral",             '\x222b', '\x0'), // alias ISOTECH int 
			new Entity("intercal",             '\x22ba', '\x0'), // alias ISOAMSB intcal 
			new Entity("Intersection",         '\x22c2', '\x0'), // alias ISOAMSB xcap 
			new Entity("intlarhk",             '\x2a17', '\x0'), // integral, left arrow with hook 
			new Entity("intprod",              '\x2a3c', '\x0'), // alias ISOAMSB iprod 
			new Entity("InvisibleComma",       '\x2063', '\x0'), // used as a separator, e.g., in indices 
			new Entity("InvisibleTimes",       '\x2062', '\x0'), // marks multiplication when it is understood without a mark 
			new Entity("iocy",                 '\x451', '\x0'), // =small io, Russian 
			new Entity("IOcy",                 '\x401', '\x0'), // =capital IO, Russian 
			new Entity("iogon",                '\x12f', '\x0'), // =small i, ogonek 
			new Entity("Iogon",                '\x12e', '\x0'), // =capital I, ogonek 
			// new Entity("iopf",                 "%plane1D;55A;"),  
			// new Entity("Iopf",                 "%plane1D;540;"), /Bbb I, open face I 
			new Entity("iota",                 '\x3b9', '\x0'), // greek small letter iota, U+03B9 ISOgrk3 
			new Entity("Iota",                 '\x399', '\x0'), // greek capital letter iota, U+0399 
			new Entity("iprod",                '\x2a3c', '\x0'), // /intprod 
			new Entity("iquest",               '\xbf', '\x0'), // inverted question mark = turned question mark, U+00BF ISOnum 
			// new Entity("iscr",                 "%plane1D;4BE;"), /scr i, script letter i 
			new Entity("Iscr",                 '\x2110', '\x0'), // /scr I, script letter I 
			new Entity("isin",                 '\x2208', '\x0'), // element of, U+2208 ISOtech 
			new Entity("isindot",              '\x22f5', '\x0'), // set membership, dot above 
			new Entity("isinE",                '\x22f9', '\x0'), // set membership, two horizontal strokes 
			new Entity("isins",                '\x22f4', '\x0'), // set membership, vertical bar on horizontal stroke 
			new Entity("isinsv",               '\x22f3', '\x0'), // large set membership, vertical bar on horizontal stroke 
			new Entity("isinv",                '\x2208', '\x0'), // set membership, variant 
			new Entity("it",                   '\x2062', '\x0'), // marks multiplication when it is understood without a mark 
			new Entity("itilde",               '\x129', '\x0'), // =small i, tilde 
			new Entity("Itilde",               '\x128', '\x0'), // =capital I, tilde 
			new Entity("iukcy",                '\x456', '\x0'), // =small i, Ukrainian 
			new Entity("Iukcy",                '\x406', '\x0'), // =capital I, Ukrainian 
			new Entity("iuml",                 '\xef', '\x0'), // latin small i with diaeresis, U+00EF ISOlat1 
			new Entity("Iuml",                 '\xcf', '\x0'), // latin capital I with diaeresis, U+00CF ISOlat1 
			new Entity("jcirc",                '\x135', '\x0'), // =small j, circumflex accent 
			new Entity("Jcirc",                '\x134', '\x0'), // =capital J, circumflex accent 
			new Entity("jcy",                  '\x439', '\x0'), // =small short i, Cyrillic 
			new Entity("Jcy",                  '\x419', '\x0'), // =capital short I, Cyrillic 
			// new Entity("jfr",                  "%plane1D;527;"), /frak j, lower case j 
			// new Entity("Jfr",                  "%plane1D;50D;"), /frak J, upper case j 
			new Entity("jmath",                '\x6a', '\x0'), // /jmath - small j, no dot 
			// new Entity("jopf",                 "%plane1D;55B;"),  
			// new Entity("Jopf",                 "%plane1D;541;"), /Bbb J, open face J 
			// new Entity("jscr",                 "%plane1D;4BF;"), /scr j, script letter j 
			// new Entity("Jscr",                 "%plane1D;4A5;"), /scr J, script letter J 
			new Entity("jsercy",               '\x458', '\x0'), // =small je, Serbian 
			new Entity("Jsercy",               '\x408', '\x0'), // =capital JE, Serbian 
			new Entity("jukcy",                '\x454', '\x0'), // =small je, Ukrainian 
			new Entity("Jukcy",                '\x404', '\x0'), // =capital JE, Ukrainian 
			new Entity("kappa",                '\x3ba', '\x0'), // greek small letter kappa, U+03BA ISOgrk3 
			new Entity("Kappa",                '\x39a', '\x0'), // greek capital letter kappa, U+039A 
			new Entity("kappav",               '\x3f0', '\x0'), // /varkappa 
			new Entity("kcedil",               '\x137', '\x0'), // =small k, cedilla 
			new Entity("Kcedil",               '\x136', '\x0'), // =capital K, cedilla 
			new Entity("kcy",                  '\x43a', '\x0'), // =small ka, Cyrillic 
			new Entity("Kcy",                  '\x41a', '\x0'), // =capital KA, Cyrillic 
			// new Entity("kfr",                  "%plane1D;528;"), /frak k, lower case k 
			// new Entity("Kfr",                  "%plane1D;50E;"), /frak K, upper case k 
			new Entity("kgreen",               '\x138', '\x0'), // =small k, Greenlandic 
			new Entity("khcy",                 '\x445', '\x0'), // =small ha, Cyrillic 
			new Entity("KHcy",                 '\x425', '\x0'), // =capital HA, Cyrillic 
			new Entity("kjcy",                 '\x45c', '\x0'), // =small kje Macedonian 
			new Entity("KJcy",                 '\x40c', '\x0'), // =capital KJE, Macedonian 
			// new Entity("kopf",                 "%plane1D;55C;"),  
			// new Entity("Kopf",                 "%plane1D;542;"), /Bbb K, open face K  
			// new Entity("kscr",                 "%plane1D;4C0;"), /scr k, script letter k 
			// new Entity("Kscr",                 "%plane1D;4A6;"), /scr K, script letter K 
			new Entity("lAarr",                '\x21da', '\x0'), // /Lleftarrow A: left triple arrow 
			new Entity("lacute",               '\x13a', '\x0'), // =small l, acute accent 
			new Entity("Lacute",               '\x139', '\x0'), // =capital L, acute accent 
			new Entity("laemptyv",             '\x29b4', '\x0'), // circle, slash, left arrow above 
			new Entity("lagran",               '\x2112', '\x0'), // Lagrangian (script capital L)  
			new Entity("lambda",               '\x3bb', '\x0'), // greek small letter lambda, U+03BB ISOgrk3 
			new Entity("Lambda",               '\x39b', '\x0'), // greek capital letter lambda, U+039B ISOgrk3 
			new Entity("lang",                 '\x2329', '\x0'), // left-pointing angle bracket = bra, U+2329 ISOtech 
			new Entity("Lang",                 '\x300a', '\x0'), // left angle bracket, double 
			new Entity("langd",                '\x2991', '\x0'), // left angle, dot 
			new Entity("langle",               '\x2329', '\x0'), // alias ISOTECH lang 
			new Entity("lap",                  '\x2a85', '\x0'), // /lessapprox R: less, approximate 
			new Entity("Laplacetrf",           '\x2112', '\x0'), // Laplace transform 
			new Entity("laquo",                '\xab', '\x0'), // left-pointing double angle quotation mark = left pointing guillemet, U+00AB ISOnum 
			new Entity("larr",                 '\x2190', '\x0'), // leftwards arrow, U+2190 ISOnum 
			new Entity("lArr",                 '\x21d0', '\x0'), // leftwards double arrow, U+21D0 ISOtech 
			new Entity("Larr",                 '\x219e', '\x0'), // /twoheadleftarrow A: 
			new Entity("larrb",                '\x21e4', '\x0'), // leftwards arrow to bar 
			new Entity("larrbfs",              '\x291f', '\x0'), // left arrow-bar, filled square 
			new Entity("larrfs",               '\x291d', '\x0'), // left arrow, filled square 
			new Entity("larrhk",               '\x21a9', '\x0'), // /hookleftarrow A: left arrow-hooked 
			new Entity("larrlp",               '\x21ab', '\x0'), // /looparrowleft A: left arrow-looped 
			new Entity("larrpl",               '\x2939', '\x0'), // left arrow, plus 
			new Entity("larrsim",              '\x2973', '\x0'), // left arrow, similar 
			new Entity("larrtl",               '\x21a2', '\x0'), // /leftarrowtail A: left arrow-tailed 
			new Entity("lat",                  '\x2aab', '\x0'), // larger than 
			new Entity("latail",               '\x2919', '\x0'), // left arrow-tail 
			new Entity("lAtail",               '\x291b', '\x0'), // left double arrow-tail 
			new Entity("late",                 '\x2aad', '\x0'), // larger than or equal 
			new Entity("lates",                '\x2aad', '\xfe00'), // larger than or equal, slanted 
			new Entity("lbarr",                '\x290c', '\x0'), // left broken arrow 
			new Entity("lBarr",                '\x290e', '\x0'), // left doubly broken arrow 
			new Entity("lbbrk",                '\x3014', '\x0'), // left broken bracket 
			new Entity("lbrace",               '\x7b', '\x0'), // alias ISONUM lcub 
			new Entity("lbrack",               '\x5b', '\x0'), // alias ISONUM lsqb 
			new Entity("lbrke",                '\x298b', '\x0'), // left bracket, equal 
			new Entity("lbrksld",              '\x298f', '\x0'), // left bracket, solidus bottom corner 
			new Entity("lbrkslu",              '\x298d', '\x0'), // left bracket, solidus top corner 
			new Entity("lcaron",               '\x13e', '\x0'), // =small l, caron 
			new Entity("Lcaron",               '\x13d', '\x0'), // =capital L, caron 
			new Entity("lcedil",               '\x13c', '\x0'), // =small l, cedilla 
			new Entity("Lcedil",               '\x13b', '\x0'), // =capital L, cedilla 
			new Entity("lceil",                '\x2308', '\x0'), // left ceiling = apl upstile, U+2308 ISOamsc  
			new Entity("lcub",                 '\x7b', '\x0'), // /lbrace O: =left curly bracket 
			new Entity("lcy",                  '\x43b', '\x0'), // =small el, Cyrillic 
			new Entity("Lcy",                  '\x41b', '\x0'), // =capital EL, Cyrillic 
			new Entity("ldca",                 '\x2936', '\x0'), // left down curved arrow 
			new Entity("ldquo",                '\x201c', '\x0'), // left double quotation mark, U+201C ISOnum 
			new Entity("ldquor",               '\x201e', '\x0'), // =rising dbl quote, left (low) 
			new Entity("ldrdhar",              '\x2967', '\x0'), // left harpoon-down over right harpoon-down 
			new Entity("ldrushar",             '\x294b', '\x0'), // left-down-right-up harpoon 
			new Entity("ldsh",                 '\x21b2', '\x0'), // left down angled arrow 
			new Entity("le",                   '\x2264', '\x0'), // less-than or equal to, U+2264 ISOtech 
			new Entity("lE",                   '\x2266', '\x0'), // /leqq R: less, double equals 
			new Entity("LeftAngleBracket",     '\x2329', '\x0'), // alias ISOTECH lang 
			new Entity("leftarrow",            '\x2190', '\x0'), // alias ISONUM larr 
			new Entity("Leftarrow",            '\x21d0', '\x0'), // alias ISOTECH lArr 
			new Entity("LeftArrow",            '\x2190', '\x0'), // alias ISONUM larr 
			new Entity("LeftArrowBar",         '\x21e4', '\x0'), // alias for larrb 
			new Entity("LeftArrowRightArrow",  '\x21c6', '\x0'), // alias ISOAMSA lrarr 
			new Entity("leftarrowtail",        '\x21a2', '\x0'), // alias ISOAMSA larrtl 
			new Entity("LeftCeiling",          '\x2308', '\x0'), // alias ISOAMSC lceil 
			new Entity("LeftDoubleBracket",    '\x301a', '\x0'), // left double bracket delimiter 
			new Entity("LeftDownTeeVector",    '\x2961', '\x0'), // down-left harpoon from bar 
			new Entity("LeftDownVector",       '\x21c3', '\x0'), // alias ISOAMSA dharl 
			new Entity("LeftDownVectorBar",    '\x2959', '\x0'), // down-left harpoon to bar 
			new Entity("LeftFloor",            '\x230a', '\x0'), // alias ISOAMSC lfloor 
			new Entity("leftharpoondown",      '\x21bd', '\x0'), // alias ISOAMSA lhard 
			new Entity("leftharpoonup",        '\x21bc', '\x0'), // alias ISOAMSA lharu 
			new Entity("leftleftarrows",       '\x21c7', '\x0'), // alias ISOAMSA llarr 
			new Entity("leftrightarrow",       '\x2194', '\x0'), // alias ISOAMSA harr 
			new Entity("Leftrightarrow",       '\x21d4', '\x0'), // alias ISOAMSA hArr 
			new Entity("LeftRightArrow",       '\x2194', '\x0'), // alias ISOAMSA harr 
			new Entity("leftrightarrows",      '\x21c6', '\x0'), // alias ISOAMSA lrarr 
			new Entity("leftrightharpoons",    '\x21cb', '\x0'), // alias ISOAMSA lrhar 
			new Entity("leftrightsquigarrow",  '\x21ad', '\x0'), // alias ISOAMSA harrw 
			new Entity("LeftRightVector",      '\x294e', '\x0'), // left-up-right-up harpoon 
			new Entity("LeftTee",              '\x22a3', '\x0'), // alias ISOAMSR dashv 
			new Entity("LeftTeeArrow",         '\x21a4', '\x0'), // alias for mapstoleft 
			new Entity("LeftTeeVector",        '\x295a', '\x0'), // left-up harpoon from bar 
			new Entity("leftthreetimes",       '\x22cb', '\x0'), // alias ISOAMSB lthree 
			new Entity("LeftTriangle",         '\x22b2', '\x0'), // alias ISOAMSR vltri 
			new Entity("LeftTriangleBar",      '\x29cf', '\x0'), // left triangle, vertical bar 
			new Entity("LeftTriangleEqual",    '\x22b4', '\x0'), // alias ISOAMSR ltrie 
			new Entity("LeftUpDownVector",     '\x2951', '\x0'), // up-left-down-left harpoon 
			new Entity("LeftUpTeeVector",      '\x2960', '\x0'), // up-left harpoon from bar 
			new Entity("LeftUpVector",         '\x21bf', '\x0'), // alias ISOAMSA uharl 
			new Entity("LeftUpVectorBar",      '\x2958', '\x0'), // up-left harpoon to bar 
			new Entity("LeftVector",           '\x21bc', '\x0'), // alias ISOAMSA lharu 
			new Entity("LeftVectorBar",        '\x2952', '\x0'), // left-up harpoon to bar 
			new Entity("leg",                  '\x22da', '\x0'), // /lesseqgtr R: less, eq, greater 
			new Entity("lEg",                  '\x2a8b', '\x0'), // /lesseqqgtr R: less, dbl eq, greater 
			new Entity("leq",                  '\x2264', '\x0'), // alias ISOTECH le 
			new Entity("leqq",                 '\x2266', '\x0'), // alias ISOAMSR lE 
			new Entity("leqslant",             '\x2a7d', '\x0'), // alias ISOAMSR les 
			new Entity("les",                  '\x2a7d', '\x0'), // /leqslant R: less-than-or-eq, slant 
			new Entity("lescc",                '\x2aa8', '\x0'), // less than, closed by curve, equal, slanted 
			new Entity("lesdot",               '\x2a7f', '\x0'), // less-than-or-equal, slanted, dot inside 
			new Entity("lesdoto",              '\x2a81', '\x0'), // less-than-or-equal, slanted, dot above 
			new Entity("lesdotor",             '\x2a83', '\x0'), // less-than-or-equal, slanted, dot above right 
			new Entity("lesg",                 '\x22da', '\xfe00'), // less, equal, slanted, greater 
			new Entity("lesges",               '\x2a93', '\x0'), // less, equal, slanted, greater, equal, slanted 
			new Entity("lessapprox",           '\x2a85', '\x0'), // alias ISOAMSR lap 
			new Entity("lessdot",              '\x22d6', '\x0'), // alias ISOAMSR ltdot 
			new Entity("lesseqgtr",            '\x22da', '\x0'), // alias ISOAMSR leg 
			new Entity("lesseqqgtr",           '\x2a8b', '\x0'), // alias ISOAMSR lEg 
			new Entity("LessEqualGreater",     '\x22da', '\x0'), // alias ISOAMSR leg 
			new Entity("LessFullEqual",        '\x2266', '\x0'), // alias ISOAMSR lE 
			new Entity("LessGreater",          '\x2276', '\x0'), // alias ISOAMSR lg 
			new Entity("lessgtr",              '\x2276', '\x0'), // alias ISOAMSR lg 
			new Entity("LessLess",             '\x2aa1', '\x0'), // alias for Lt 
			new Entity("lesssim",              '\x2272', '\x0'), // alias ISOAMSR lsim 
			new Entity("LessSlantEqual",       '\x2a7d', '\x0'), // alias ISOAMSR les 
			new Entity("LessTilde",            '\x2272', '\x0'), // alias ISOAMSR lsim 
			new Entity("lfisht",               '\x297c', '\x0'), // left fish tail 
			new Entity("lfloor",               '\x230a', '\x0'), // left floor = apl downstile, U+230A ISOamsc  
			// new Entity("lfr",                  "%plane1D;529;"), /frak l, lower case l 
			// new Entity("Lfr",                  "%plane1D;50F;"), /frak L, upper case l 
			new Entity("lg",                   '\x2276', '\x0'), // /lessgtr R: less, greater 
			new Entity("lgE",                  '\x2a91', '\x0'), // less, greater, equal 
			new Entity("lHar",                 '\x2962', '\x0'), // left harpoon-up over left harpoon-down 
			new Entity("lhard",                '\x21bd', '\x0'), // /leftharpoondown A: l harpoon-down 
			new Entity("lharu",                '\x21bc', '\x0'), // /leftharpoonup A: left harpoon-up 
			new Entity("lharul",               '\x296a', '\x0'), // left harpoon-up over long dash 
			new Entity("lhblk",                '\x2584', '\x0'), // =lower half block 
			new Entity("ljcy",                 '\x459', '\x0'), // =small lje, Serbian 
			new Entity("LJcy",                 '\x409', '\x0'), // =capital LJE, Serbian 
			new Entity("ll",                   '\x226a', '\x0'), // alias ISOAMSR Lt 
			new Entity("Ll",                   '\x22d8', '\x0'), // /Ll /lll /llless R: triple less-than 
			new Entity("llarr",                '\x21c7', '\x0'), // /leftleftarrows A: two left arrows 
			new Entity("llcorner",             '\x231e', '\x0'), // alias ISOAMSC dlcorn 
			new Entity("Lleftarrow",           '\x21da', '\x0'), // alias ISOAMSA lAarr 
			new Entity("llhard",               '\x296b', '\x0'), // left harpoon-down below long dash 
			new Entity("lltri",                '\x25fa', '\x0'), // lower left triangle 
			new Entity("lmidot",               '\x140', '\x0'), // =small l, middle dot 
			new Entity("Lmidot",               '\x13f', '\x0'), // =capital L, middle dot 
			new Entity("lmoust",               '\x23b0', '\x0'), // /lmoustache 
			new Entity("lmoustache",           '\x23b0', '\x0'), // alias ISOAMSC lmoust 
			new Entity("lnap",                 '\x2a89', '\x0'), // /lnapprox N: less, not approximate 
			new Entity("lnapprox",             '\x2a89', '\x0'), // alias ISOAMSN lnap 
			new Entity("lne",                  '\x2a87', '\x0'), // /lneq N: less, not equals 
			new Entity("lnE",                  '\x2268', '\x0'), // /lneqq N: less, not double equals 
			new Entity("lneq",                 '\x2a87', '\x0'), // alias ISOAMSN lne 
			new Entity("lneqq",                '\x2268', '\x0'), // alias ISOAMSN lnE 
			new Entity("lnsim",                '\x22e6', '\x0'), // /lnsim N: less, not similar 
			new Entity("loang",                '\x3018', '\x0'), // left open angular bracket 
			new Entity("loarr",                '\x21fd', '\x0'), // left open arrow 
			new Entity("lobrk",                '\x301a', '\x0'), // left open bracket 
			new Entity("longleftarrow",        '\x27f5', '\x0'), // alias ISOAMSA xlarr 
			new Entity("Longleftarrow",        '\x27f8', '\x0'), // alias ISOAMSA xlArr 
			new Entity("LongLeftArrow",        '\x27f5', '\x0'), // alias ISOAMSA xlarr 
			new Entity("longleftrightarrow",   '\x27f7', '\x0'), // alias ISOAMSA xharr 
			new Entity("Longleftrightarrow",   '\x27fa', '\x0'), // alias ISOAMSA xhArr 
			new Entity("LongLeftRightArrow",   '\x27f7', '\x0'), // alias ISOAMSA xharr 
			new Entity("longmapsto",           '\x27fc', '\x0'), // alias ISOAMSA xmap 
			new Entity("longrightarrow",       '\x27f6', '\x0'), // alias ISOAMSA xrarr 
			new Entity("Longrightarrow",       '\x27f9', '\x0'), // alias ISOAMSA xrArr 
			new Entity("LongRightArrow",       '\x27f6', '\x0'), // alias ISOAMSA xrarr 
			new Entity("looparrowleft",        '\x21ab', '\x0'), // alias ISOAMSA larrlp 
			new Entity("looparrowright",       '\x21ac', '\x0'), // alias ISOAMSA rarrlp 
			new Entity("lopar",                '\x2985', '\x0'), // left open parenthesis 
			// new Entity("lopf",                 "%plane1D;55D;"),  
			// new Entity("Lopf",                 "%plane1D;543;"), /Bbb L, open face L  
			new Entity("loplus",               '\x2a2d', '\x0'), // plus sign in left half circle 
			new Entity("lotimes",              '\x2a34', '\x0'), // multiply sign in left half circle  
			new Entity("lowast",               '\x2217', '\x0'), // asterisk operator, U+2217 ISOtech 
			new Entity("lowbar",               '\x5f', '\x0'), // =low line 
			new Entity("LowerLeftArrow",       '\x2199', '\x0'), // alias ISOAMSA swarr 
			new Entity("LowerRightArrow",      '\x2198', '\x0'), // alias ISOAMSA searr 
			new Entity("loz",                  '\x25ca', '\x0'), // lozenge, U+25CA ISOpub 
			new Entity("lozenge",              '\x25ca', '\x0'), // alias ISOPUB loz 
			new Entity("lozf",                 '\x29eb', '\x0'), // /blacklozenge - lozenge, filled 
			new Entity("lpar",                 '\x28', '\x0'), // O: =left parenthesis 
			new Entity("lparlt",               '\x2993', '\x0'), // O: left parenthesis, lt 
			new Entity("lrarr",                '\x21c6', '\x0'), // /leftrightarrows A: l arr over r arr 
			new Entity("lrcorner",             '\x231f', '\x0'), // alias ISOAMSC drcorn 
			new Entity("lrhar",                '\x21cb', '\x0'), // /leftrightharpoons A: l harp over r 
			new Entity("lrhard",               '\x296d', '\x0'), // right harpoon-down below long dash 
			new Entity("lrm",                  '\x200e', '\x0'), // left-to-right mark, U+200E NEW RFC 2070 
			new Entity("lrtri",                '\x22bf', '\x0'), // lower right triangle 
			new Entity("lsaquo",               '\x2039', '\x0'), // single left-pointing angle quotation mark, U+2039 ISO proposed 
			// new Entity("lscr",                 "%plane1D;4C1;"), /scr l, script letter l 
			new Entity("Lscr",                 '\x2112', '\x0'), // /scr L, script letter L 
			new Entity("lsh",                  '\x21b0', '\x0'), // /Lsh A: 
			new Entity("Lsh",                  '\x21b0', '\x0'), // alias ISOAMSA lsh 
			new Entity("lsim",                 '\x2272', '\x0'), // /lesssim R: less, similar 
			new Entity("lsime",                '\x2a8d', '\x0'), // less, similar, equal 
			new Entity("lsimg",                '\x2a8f', '\x0'), // less, similar, greater 
			new Entity("lsqb",                 '\x5b', '\x0'), // /lbrack O: =left square bracket 
			new Entity("lsquo",                '\x2018', '\x0'), // left single quotation mark, U+2018 ISOnum 
			new Entity("lsquor",               '\x201a', '\x0'), // =rising single quote, left (low) 
			new Entity("lstrok",               '\x142', '\x0'), // =small l, stroke 
			new Entity("Lstrok",               '\x141', '\x0'), // =capital L, stroke 
			new Entity("lt",                   '\x26', '\x26'), // less-than sign, U+003C ISOnum 
			new Entity("Lt",                   '\x226a', '\x0'), // /ll R: double less-than sign 
			new Entity("ltcc",                 '\x2aa6', '\x0'), // less than, closed by curve 
			new Entity("ltcir",                '\x2a79', '\x0'), // less than, circle inside 
			new Entity("ltdot",                '\x22d6', '\x0'), // /lessdot R: less than, with dot 
			new Entity("lthree",               '\x22cb', '\x0'), // /leftthreetimes B: 
			new Entity("ltimes",               '\x22c9', '\x0'), // /ltimes B: times sign, left closed 
			new Entity("ltlarr",               '\x2976', '\x0'), // less than, left arrow 
			new Entity("ltquest",              '\x2a7b', '\x0'), // less than, questionmark above 
			new Entity("ltri",                 '\x25c3', '\x0'), // /triangleleft B: l triangle, open 
			new Entity("ltrie",                '\x22b4', '\x0'), // /trianglelefteq R: left triangle, eq 
			new Entity("ltrif",                '\x25c2', '\x0'), // /blacktriangleleft R: =l tri, filled 
			new Entity("ltrPar",               '\x2996', '\x0'), // dbl right parenthesis, less 
			new Entity("lurdshar",             '\x294a', '\x0'), // left-up-right-down harpoon 
			new Entity("luruhar",              '\x2966', '\x0'), // left harpoon-up over right harpoon-up 
			new Entity("lvertneqq",            '\x2268', '\xfe00'), // alias ISOAMSN lvnE 
			new Entity("lvnE",                 '\x2268', '\xfe00'), // /lvertneqq N: less, vert, not dbl eq 
			new Entity("macr",                 '\xaf', '\x0'), // macron = spacing macron = overline = APL overbar, U+00AF ISOdia 
			new Entity("male",                 '\x2642', '\x0'), // =male symbol 
			new Entity("malt",                 '\x2720', '\x0'), // /maltese =maltese cross 
			new Entity("maltese",              '\x2720', '\x0'), // alias ISOPUB malt 
			new Entity("map",                  '\x21a6', '\x0'), // /mapsto A: 
			new Entity("Map",                  '\x2905', '\x0'), // twoheaded mapsto 
			new Entity("mapsto",               '\x21a6', '\x0'), // alias ISOAMSA map 
			new Entity("mapstodown",           '\x21a7', '\x0'), // downwards arrow from bar 
			new Entity("mapstoleft",           '\x21a4', '\x0'), // leftwards arrow from bar 
			new Entity("mapstoup",             '\x21a5', '\x0'), // upwards arrow from bar 
			new Entity("marker",               '\x25ae', '\x0'), // =histogram marker 
			new Entity("mcomma",               '\x2a29', '\x0'), // minus, comma above 
			new Entity("mcy",                  '\x43c', '\x0'), // =small em, Cyrillic 
			new Entity("Mcy",                  '\x41c', '\x0'), // =capital EM, Cyrillic 
			new Entity("mdash",                '\x2014', '\x0'), // em dash, U+2014 ISOpub 
			new Entity("mDDot",                '\x223a', '\x0'), // minus with four dots, geometric properties 
			new Entity("measuredangle",        '\x2221', '\x0'), // alias ISOAMSO angmsd 
			new Entity("MediumSpace",          '\x205f', '\x0'), // space of width 4/18 em 
			new Entity("Mellintrf",            '\x2133', '\x0'), // Mellin transform 
			// new Entity("mfr",                  "%plane1D;52A;"), /frak m, lower case m 
			// new Entity("Mfr",                  "%plane1D;510;"), /frak M, upper case m 
			new Entity("mho",                  '\x2127', '\x0'), // /mho - conductance 
			new Entity("micro",                '\xb5', '\x0'), // micro sign, U+00B5 ISOnum 
			new Entity("mid",                  '\x2223', '\x0'), // /mid R: 
			new Entity("midast",               '\x2a', '\x0'), // /ast B: asterisk 
			new Entity("midcir",               '\x2af0', '\x0'), // mid, circle below  
			new Entity("middot",               '\xb7', '\x0'), // middle dot = Georgian comma = Greek middle dot, U+00B7 ISOnum 
			new Entity("minus",                '\x2212', '\x0'), // minus sign, U+2212 ISOtech 
			new Entity("minusb",               '\x229f', '\x0'), // /boxminus B: minus sign in box 
			new Entity("minusd",               '\x2238', '\x0'), // /dotminus B: minus sign, dot above 
			new Entity("minusdu",              '\x2a2a', '\x0'), // minus sign, dot below 
			new Entity("MinusPlus",            '\x2213', '\x0'), // alias ISOTECH mnplus 
			new Entity("mlcp",                 '\x2adb', '\x0'), // /mlcp 
			new Entity("mldr",                 '\x2026', '\x0'), // em leader 
			new Entity("mnplus",               '\x2213', '\x0'), // /mp B: minus-or-plus sign 
			new Entity("models",               '\x22a7', '\x0'), // /models R: 
			// new Entity("mopf",                 "%plane1D;55E;"),  
			// new Entity("Mopf",                 "%plane1D;544;"), /Bbb M, open face M  
			new Entity("mp",                   '\x2213', '\x0'), // alias ISOTECH mnplus 
			// new Entity("mscr",                 "%plane1D;4C2;"), /scr m, script letter m 
			new Entity("Mscr",                 '\x2133', '\x0'), // /scr M, script letter M 
			new Entity("mstpos",               '\x223e', '\x0'), // most positive 
			new Entity("mu",                   '\x3bc', '\x0'), // greek small letter mu, U+03BC ISOgrk3 
			new Entity("Mu",                   '\x39c', '\x0'), // greek capital letter mu, U+039C 
			new Entity("multimap",             '\x22b8', '\x0'), // alias ISOAMSA mumap 
			new Entity("mumap",                '\x22b8', '\x0'), // /multimap A: 
			new Entity("nabla",                '\x2207', '\x0'), // nabla = backward difference, U+2207 ISOtech 
			new Entity("nacute",               '\x144', '\x0'), // =small n, acute accent 
			new Entity("Nacute",               '\x143', '\x0'), // =capital N, acute accent 
			new Entity("nang",                 '\x2220', '\x20d2'), // not, vert, angle 
			new Entity("nap",                  '\x2249', '\x0'), // /napprox N: not approximate 
			new Entity("napE",                 '\x2a70', '\x338'), // not approximately equal or equal to 
			new Entity("napid",                '\x224b', '\x338'), // not approximately identical to 
			new Entity("napos",                '\x149', '\x0'), // =small n, apostrophe 
			new Entity("napprox",              '\x2249', '\x0'), // alias ISOAMSN nap 
			new Entity("natur",                '\x266e', '\x0'), // /natural - music natural 
			new Entity("natural",              '\x266e', '\x0'), // alias ISOPUB natur 
			new Entity("naturals",             '\x2115', '\x0'), // the semi-ring of natural numbers 
			new Entity("nbsp",                 '\xa0', '\x0'), // no-break space = non-breaking space, U+00A0 ISOnum 
			new Entity("nbump",                '\x224e', '\x338'), // not bumpy equals 
			new Entity("nbumpe",               '\x224f', '\x338'), // not bumpy single equals 
			new Entity("ncap",                 '\x2a43', '\x0'), // bar, intersection 
			new Entity("ncaron",               '\x148', '\x0'), // =small n, caron 
			new Entity("Ncaron",               '\x147', '\x0'), // =capital N, caron 
			new Entity("ncedil",               '\x146', '\x0'), // =small n, cedilla 
			new Entity("Ncedil",               '\x145', '\x0'), // =capital N, cedilla 
			new Entity("ncong",                '\x2247', '\x0'), // /ncong N: not congruent with 
			new Entity("ncongdot",             '\x2a6d', '\x338'), // not congruent, dot 
			new Entity("ncup",                 '\x2a42', '\x0'), // bar, union 
			new Entity("ncy",                  '\x43d', '\x0'), // =small en, Cyrillic 
			new Entity("Ncy",                  '\x41d', '\x0'), // =capital EN, Cyrillic 
			new Entity("ndash",                '\x2013', '\x0'), // en dash, U+2013 ISOpub 
			new Entity("ne",                   '\x2260', '\x0'), // not equal to, U+2260 ISOtech 
			new Entity("nearhk",               '\x2924', '\x0'), // NE arrow-hooked 
			new Entity("nearr",                '\x2197', '\x0'), // /nearrow A: NE pointing arrow 
			new Entity("neArr",                '\x21d7', '\x0'), // NE pointing dbl arrow 
			new Entity("nearrow",              '\x2197', '\x0'), // alias ISOAMSA nearr 
			new Entity("nedot",                '\x2250', '\x338'), // not equal, dot 
			new Entity("NegativeMediumSpace",  '\x200b', '\x0'), // space of width -4/18 em 
			new Entity("NegativeThickSpace",   '\x200b', '\x0'), // space of width -5/18 em 
			new Entity("NegativeThinSpace",    '\x200b', '\x0'), // space of width -3/18 em 
			new Entity("NegativeVeryThinSpace", '\x200b', '\x0'), // space of width -1/18 em 
			new Entity("nequiv",               '\x2262', '\x0'), // /nequiv N: not identical with 
			new Entity("nesear",               '\x2928', '\x0'), // /toea A: NE & SE arrows 
			new Entity("nesim",                '\x2242', '\x338'), // not equal or similar 
			new Entity("NestedGreaterGreater", '\x226b', '\x0'), // alias ISOAMSR Gt 
			new Entity("NestedLessLess",       '\x226a', '\x0'), // alias ISOAMSR Lt 
			new Entity("NewLine",              '\xa', '\x0'), // force a line break; line feed 
			new Entity("nexist",               '\x2204', '\x0'), // /nexists - negated exists 
			new Entity("nexists",              '\x2204', '\x0'), // alias ISOAMSO nexist 
			// new Entity("nfr",                  "%plane1D;52B;"), /frak n, lower case n 
			// new Entity("Nfr",                  "%plane1D;511;"), /frak N, upper case n 
			new Entity("nge",                  '\x2271', '\x0'), // /ngeq N: not greater-than-or-equal 
			new Entity("ngE",                  '\x2267', '\x338'), // /ngeqq N: not greater, dbl equals 
			new Entity("ngeq",                 '\x2271', '\x0'), // alias ISOAMSN nge 
			new Entity("ngeqq",                '\x2267', '\x338'), // alias ISOAMSN ngE 
			new Entity("ngeqslant",            '\x2a7e', '\x338'), // alias ISOAMSN nges 
			new Entity("nges",                 '\x2a7e', '\x338'), // /ngeqslant N: not gt-or-eq, slanted 
			new Entity("nGg",                  '\x22d9', '\x338'), // not triple greater than 
			new Entity("ngsim",                '\x2275', '\x0'), // not greater, similar 
			new Entity("ngt",                  '\x226f', '\x0'), // /ngtr N: not greater-than 
			new Entity("nGt",                  '\x226b', '\x20d2'), // not, vert, much greater than 
			new Entity("ngtr",                 '\x226f', '\x0'), // alias ISOAMSN ngt 
			new Entity("nGtv",                 '\x226b', '\x338'), // not much greater than, variant 
			new Entity("nharr",                '\x21ae', '\x0'), // /nleftrightarrow A: not l&r arrow 
			new Entity("nhArr",                '\x21ce', '\x0'), // /nLeftrightarrow A: not l&r dbl arr 
			new Entity("nhpar",                '\x2af2', '\x0'), // not, horizontal, parallel 
			new Entity("ni",                   '\x220b', '\x0'), // contains as member, U+220B ISOtech 
			new Entity("nis",                  '\x22fc', '\x0'), // contains, vertical bar on horizontal stroke 
			new Entity("nisd",                 '\x22fa', '\x0'), // contains, long horizontal stroke 
			new Entity("niv",                  '\x220b', '\x0'), // contains, variant 
			new Entity("njcy",                 '\x45a', '\x0'), // =small nje, Serbian 
			new Entity("NJcy",                 '\x40a', '\x0'), // =capital NJE, Serbian 
			new Entity("nlarr",                '\x219a', '\x0'), // /nleftarrow A: not left arrow 
			new Entity("nlArr",                '\x21cd', '\x0'), // /nLeftarrow A: not implied by 
			new Entity("nldr",                 '\x2025', '\x0'), // =double baseline dot (en leader) 
			new Entity("nle",                  '\x2270', '\x0'), // /nleq N: not less-than-or-equal 
			new Entity("nlE",                  '\x2266', '\x338'), // /nleqq N: not less, dbl equals 
			new Entity("nleftarrow",           '\x219a', '\x0'), // alias ISOAMSA nlarr 
			new Entity("nLeftarrow",           '\x21cd', '\x0'), // alias ISOAMSA nlArr 
			new Entity("nleftrightarrow",      '\x21ae', '\x0'), // alias ISOAMSA nharr 
			new Entity("nLeftrightarrow",      '\x21ce', '\x0'), // alias ISOAMSA nhArr 
			new Entity("nleq",                 '\x2270', '\x0'), // alias ISOAMSN nle 
			new Entity("nleqq",                '\x2266', '\x338'), // alias ISOAMSN nlE 
			new Entity("nleqslant",            '\x2a7d', '\x338'), // alias ISOAMSN nles 
			new Entity("nles",                 '\x2a7d', '\x338'), // /nleqslant N: not less-or-eq, slant 
			new Entity("nless",                '\x226e', '\x0'), // alias ISOAMSN nlt 
			new Entity("nLl",                  '\x22d8', '\x338'), // not triple less than 
			new Entity("nlsim",                '\x2274', '\x0'), // not less, similar 
			new Entity("nlt",                  '\x226e', '\x0'), // /nless N: not less-than 
			new Entity("nLt",                  '\x226a', '\x20d2'), // not, vert, much less than 
			new Entity("nltri",                '\x22ea', '\x0'), // /ntriangleleft N: not left triangle 
			new Entity("nltrie",               '\x22ec', '\x0'), // /ntrianglelefteq N: not l tri, eq 
			new Entity("nLtv",                 '\x226a', '\x338'), // not much less than, variant 
			new Entity("nmid",                 '\x2224', '\x0'), // /nmid 
			new Entity("NoBreak",              '\x2060', '\x0'), // never break line here 
			new Entity("NonBreakingSpace",     '\xa0', '\x0'), // alias ISONUM nbsp 
			// new Entity("nopf",                 "%plane1D;55F;"),  
			new Entity("Nopf",                 '\x2115', '\x0'), // /Bbb N, open face N 
			new Entity("not",                  '\xac', '\x0'), // not sign, U+00AC ISOnum 
			new Entity("Not",                  '\x2aec', '\x0'), // not with two horizontal strokes 
			new Entity("NotCongruent",         '\x2262', '\x0'), // alias ISOAMSN nequiv 
			new Entity("NotCupCap",            '\x226d', '\x0'), // alias for &nasymp; 
			new Entity("NotDoubleVerticalBar", '\x2226', '\x0'), // alias ISOAMSN npar 
			new Entity("NotElement",           '\x2209', '\x0'), // alias ISOTECH notin 
			new Entity("NotEqual",             '\x2260', '\x0'), // alias ISOTECH ne 
			new Entity("NotEqualTilde",        '\x2242', '\x338'), // alias for  &nesim; 
			new Entity("NotExists",            '\x2204', '\x0'), // alias ISOAMSO nexist 
			new Entity("NotGreater",           '\x226f', '\x0'), // alias ISOAMSN ngt 
			new Entity("NotGreaterEqual",      '\x2271', '\x0'), // alias ISOAMSN nge 
			new Entity("NotGreaterFullEqual",  '\x2266', '\x338'), // alias ISOAMSN nlE 
			new Entity("NotGreaterGreater",    '\x226b', '\x338'), // alias ISOAMSN nGtv 
			new Entity("NotGreaterLess",       '\x2279', '\x0'), // alias ISOAMSN ntvgl 
			new Entity("NotGreaterSlantEqual", '\x2a7e', '\x338'), // alias ISOAMSN nges 
			new Entity("NotGreaterTilde",      '\x2275', '\x0'), // alias ISOAMSN ngsim 
			new Entity("NotHumpDownHump",      '\x224e', '\x338'), // alias for &nbump; 
			new Entity("NotHumpEqual",         '\x224f', '\x338'), // alias for &nbumpe; 
			new Entity("notin",                '\x2209', '\x0'), // not an element of, U+2209 ISOtech 
			new Entity("notindot",             '\x22f5', '\x338'), // negated set membership, dot above 
			new Entity("notinE",               '\x22f9', '\x338'), // negated set membership, two horizontal strokes 
			new Entity("notinva",              '\x2209', '\x0'), // negated set membership, variant 
			new Entity("notinvb",              '\x22f7', '\x0'), // negated set membership, variant 
			new Entity("notinvc",              '\x22f6', '\x0'), // negated set membership, variant 
			new Entity("NotLeftTriangle",      '\x22ea', '\x0'), // alias ISOAMSN nltri 
			new Entity("NotLeftTriangleBar",   '\x29cf', '\x338'), // not left triangle, vertical bar 
			new Entity("NotLeftTriangleEqual", '\x22ec', '\x0'), // alias ISOAMSN nltrie 
			new Entity("NotLess",              '\x226e', '\x0'), // alias ISOAMSN nlt 
			new Entity("NotLessEqual",         '\x2270', '\x0'), // alias ISOAMSN nle 
			new Entity("NotLessGreater",       '\x2278', '\x0'), // alias ISOAMSN ntvlg 
			new Entity("NotLessLess",          '\x226a', '\x338'), // alias ISOAMSN nLtv 
			new Entity("NotLessSlantEqual",    '\x2a7d', '\x338'), // alias ISOAMSN nles 
			new Entity("NotLessTilde",         '\x2274', '\x0'), // alias ISOAMSN nlsim 
			new Entity("NotNestedGreaterGreater", '\x2aa2', '\x338'), // not double greater-than sign 
			new Entity("NotNestedLessLess",    '\x2aa1', '\x338'), // not double less-than sign 
			new Entity("notni",                '\x220c', '\x0'), // negated contains 
			new Entity("notniva",              '\x220c', '\x0'), // negated contains, variant 
			new Entity("notnivb",              '\x22fe', '\x0'), // contains, variant 
			new Entity("notnivc",              '\x22fd', '\x0'), // contains, variant 
			new Entity("NotPrecedes",          '\x2280', '\x0'), // alias ISOAMSN npr 
			new Entity("NotPrecedesEqual",     '\x2aaf', '\x338'), // alias ISOAMSN npre 
			new Entity("NotPrecedesSlantEqual", '\x22e0', '\x0'), // alias ISOAMSN nprcue 
			new Entity("NotReverseElement",    '\x220c', '\x0'), // alias ISOTECH notniva 
			new Entity("NotRightTriangle",     '\x22eb', '\x0'), // alias ISOAMSN nrtri 
			new Entity("NotRightTriangleBar",  '\x29d0', '\x338'), // not vertical bar, right triangle 
			new Entity("NotRightTriangleEqual", '\x22ed', '\x0'), // alias ISOAMSN nrtrie 
			new Entity("NotSquareSubset",      '\x228f', '\x338'), // square not subset 
			new Entity("NotSquareSubsetEqual", '\x22e2', '\x0'), // alias ISOAMSN nsqsube 
			new Entity("NotSquareSuperset",    '\x2290', '\x338'), // negated set-like partial order operator 
			new Entity("NotSquareSupersetEqual", '\x22e3', '\x0'), // alias ISOAMSN nsqsupe 
			new Entity("NotSubset",            '\x2282', '\x20d2'), // alias ISOAMSN vnsub 
			new Entity("NotSubsetEqual",       '\x2288', '\x0'), // alias ISOAMSN nsube 
			new Entity("NotSucceeds",          '\x2281', '\x0'), // alias ISOAMSN nsc 
			new Entity("NotSucceedsEqual",     '\x2ab0', '\x338'), // alias ISOAMSN nsce 
			new Entity("NotSucceedsSlantEqual", '\x22e1', '\x0'), // alias ISOAMSN nsccue 
			new Entity("NotSucceedsTilde",     '\x227f', '\x338'), // not succeeds or similar 
			new Entity("NotSuperset",          '\x2283', '\x20d2'), // alias ISOAMSN vnsup 
			new Entity("NotSupersetEqual",     '\x2289', '\x0'), // alias ISOAMSN nsupe 
			new Entity("NotTilde",             '\x2241', '\x0'), // alias ISOAMSN nsim 
			new Entity("NotTildeEqual",        '\x2244', '\x0'), // alias ISOAMSN nsime 
			new Entity("NotTildeFullEqual",    '\x2247', '\x0'), // alias ISOAMSN ncong 
			new Entity("NotTildeTilde",        '\x2249', '\x0'), // alias ISOAMSN nap 
			new Entity("NotVerticalBar",       '\x2224', '\x0'), // alias ISOAMSN nmid 
			new Entity("npar",                 '\x2226', '\x0'), // /nparallel N: not parallel 
			new Entity("nparallel",            '\x2226', '\x0'), // alias ISOAMSN npar 
			new Entity("nparsl",               '\x2afd', '\x20e5'), // not parallel, slanted 
			new Entity("npart",                '\x2202', '\x338'), // not partial differential 
			new Entity("npolint",              '\x2a14', '\x0'), // line integration, not including the pole 
			new Entity("npr",                  '\x2280', '\x0'), // /nprec N: not precedes 
			new Entity("nprcue",               '\x22e0', '\x0'), // not curly precedes, eq 
			new Entity("npre",                 '\x2aaf', '\x338'), // /npreceq N: not precedes, equals 
			new Entity("nprec",                '\x2280', '\x0'), // alias ISOAMSN npr 
			new Entity("npreceq",              '\x2aaf', '\x338'), // alias ISOAMSN npre 
			new Entity("nrarr",                '\x219b', '\x0'), // /nrightarrow A: not right arrow 
			new Entity("nrArr",                '\x21cf', '\x0'), // /nRightarrow A: not implies 
			new Entity("nrarrc",               '\x2933', '\x338'), // not right arrow-curved 
			new Entity("nrarrw",               '\x219d', '\x338'), // not right arrow-wavy 
			new Entity("nrightarrow",          '\x219b', '\x0'), // alias ISOAMSA nrarr 
			new Entity("nRightarrow",          '\x21cf', '\x0'), // alias ISOAMSA nrArr 
			new Entity("nrtri",                '\x22eb', '\x0'), // /ntriangleright N: not rt triangle 
			new Entity("nrtrie",               '\x22ed', '\x0'), // /ntrianglerighteq N: not r tri, eq 
			new Entity("nsc",                  '\x2281', '\x0'), // /nsucc N: not succeeds 
			new Entity("nsccue",               '\x22e1', '\x0'), // not succeeds, curly eq 
			new Entity("nsce",                 '\x2ab0', '\x338'), // /nsucceq N: not succeeds, equals 
			// new Entity("nscr",                 "%plane1D;4C3;"), /scr n, script letter n 
			// new Entity("Nscr",                 "%plane1D;4A9;"), /scr N, script letter N 
			new Entity("nshortmid",            '\x2224', '\x0'), // alias ISOAMSN nsmid 
			new Entity("nshortparallel",       '\x2226', '\x0'), // alias ISOAMSN nspar 
			new Entity("nsim",                 '\x2241', '\x0'), // /nsim N: not similar 
			new Entity("nsime",                '\x2244', '\x0'), // /nsimeq N: not similar, equals 
			new Entity("nsimeq",               '\x2244', '\x0'), // alias ISOAMSN nsime 
			new Entity("nsmid",                '\x2224', '\x0'), // /nshortmid 
			new Entity("nspar",                '\x2226', '\x0'), // /nshortparallel N: not short par 
			new Entity("nsqsube",              '\x22e2', '\x0'), // not, square subset, equals 
			new Entity("nsqsupe",              '\x22e3', '\x0'), // not, square superset, equals 
			new Entity("nsub",                 '\x2284', '\x0'), // not a subset of, U+2284 ISOamsn 
			new Entity("nsube",                '\x2288', '\x0'), // /nsubseteq N: not subset, equals 
			new Entity("nsubE",                '\x2ac5', '\x338'), // /nsubseteqq N: not subset, dbl eq 
			new Entity("nsubset",              '\x2282', '\x20d2'), // alias ISOAMSN vnsub 
			new Entity("nsubseteq",            '\x2288', '\x0'), // alias ISOAMSN nsube 
			new Entity("nsubseteqq",           '\x2ac5', '\x338'), // alias ISOAMSN nsubE 
			new Entity("nsucc",                '\x2281', '\x0'), // alias ISOAMSN nsc 
			new Entity("nsucceq",              '\x2ab0', '\x338'), // alias ISOAMSN nsce 
			new Entity("nsup",                 '\x2285', '\x0'), // not superset 
			new Entity("nsupe",                '\x2289', '\x0'), // /nsupseteq N: not superset, equals 
			new Entity("nsupE",                '\x2ac6', '\x338'), // /nsupseteqq N: not superset, dbl eq 
			new Entity("nsupset",              '\x2283', '\x20d2'), // alias ISOAMSN vnsup 
			new Entity("nsupseteq",            '\x2289', '\x0'), // alias ISOAMSN nsupe 
			new Entity("nsupseteqq",           '\x2ac6', '\x338'), // alias ISOAMSN nsupE 
			new Entity("ntgl",                 '\x2279', '\x0'), // not greater, less 
			new Entity("ntilde",               '\xf1', '\x0'), // latin small n with tilde, U+00F1 ISOlat1 
			new Entity("Ntilde",               '\xd1', '\x0'), // latin capital N with tilde, U+00D1 ISOlat1 
			new Entity("ntlg",                 '\x2278', '\x0'), // not less, greater 
			new Entity("ntriangleleft",        '\x22ea', '\x0'), // alias ISOAMSN nltri 
			new Entity("ntrianglelefteq",      '\x22ec', '\x0'), // alias ISOAMSN nltrie 
			new Entity("ntriangleright",       '\x22eb', '\x0'), // alias ISOAMSN nrtri 
			new Entity("ntrianglerighteq",     '\x22ed', '\x0'), // alias ISOAMSN nrtrie 
			new Entity("nu",                   '\x3bd', '\x0'), // greek small letter nu, U+03BD ISOgrk3 
			new Entity("Nu",                   '\x39d', '\x0'), // greek capital letter nu, U+039D 
			new Entity("num",                  '\x23', '\x0'), // =number sign 
			new Entity("numero",               '\x2116', '\x0'), // =numero sign 
			new Entity("numsp",                '\x2007', '\x0'), // =digit space (width of a number) 
			new Entity("nvap",                 '\x224d', '\x20d2'), // not, vert, approximate 
			new Entity("nvdash",               '\x22ac', '\x0'), // /nvdash N: not vertical, dash 
			new Entity("nvDash",               '\x22ad', '\x0'), // /nvDash N: not vertical, dbl dash 
			new Entity("nVdash",               '\x22ae', '\x0'), // /nVdash N: not dbl vertical, dash 
			new Entity("nVDash",               '\x22af', '\x0'), // /nVDash N: not dbl vert, dbl dash 
			new Entity("nvge",                 '\x2265', '\x20d2'), // not, vert, greater-than-or-equal 
			new Entity("nvgt",                 '\x3e', '\x20d2'), // not, vert, greater-than 
			new Entity("nvHarr",               '\x2904', '\x0'), // not, vert, left and right double arrow  
			new Entity("nvinfin",              '\x29de', '\x0'), // not, vert, infinity 
			new Entity("nvlArr",               '\x2902', '\x0'), // not, vert, left double arrow 
			new Entity("nvle",                 '\x2264', '\x20d2'), // not, vert, less-than-or-equal 
			new Entity("nvlt",                 '\x26', '\x0'), // not, vert, less-than 
			new Entity("nvltrie",              '\x22b4', '\x20d2'), // not, vert, left triangle, equals 
			new Entity("nvrArr",               '\x2903', '\x0'), // not, vert, right double arrow 
			new Entity("nvrtrie",              '\x22b5', '\x20d2'), // not, vert, right triangle, equals 
			new Entity("nvsim",                '\x223c', '\x20d2'), // not, vert, similar 
			new Entity("nwarhk",               '\x2923', '\x0'), // NW arrow-hooked 
			new Entity("nwarr",                '\x2196', '\x0'), // /nwarrow A: NW pointing arrow 
			new Entity("nwArr",                '\x21d6', '\x0'), // NW pointing dbl arrow 
			new Entity("nwarrow",              '\x2196', '\x0'), // alias ISOAMSA nwarr 
			new Entity("nwnear",               '\x2927', '\x0'), // NW & NE arrows 
			new Entity("oacute",               '\xf3', '\x0'), // latin small o with acute, U+00F3 ISOlat1 
			new Entity("Oacute",               '\xd3', '\x0'), // latin capital O with acute, U+00D3 ISOlat1 
			new Entity("oast",                 '\x229b', '\x0'), // /circledast B: asterisk in circle 
			new Entity("ocir",                 '\x229a', '\x0'), // /circledcirc B: small circle in circle 
			new Entity("ocirc",                '\xf4', '\x0'), // latin small o with circumflex, U+00F4 ISOlat1 
			new Entity("Ocirc",                '\xd4', '\x0'), // latin capital O with circumflex, U+00D4 ISOlat1 
			new Entity("ocy",                  '\x43e', '\x0'), // =small o, Cyrillic 
			new Entity("Ocy",                  '\x41e', '\x0'), // =capital O, Cyrillic 
			new Entity("odash",                '\x229d', '\x0'), // /circleddash B: hyphen in circle 
			new Entity("odblac",               '\x151', '\x0'), // =small o, double acute accent 
			new Entity("Odblac",               '\x150', '\x0'), // =capital O, double acute accent 
			new Entity("odiv",                 '\x2a38', '\x0'), // divide in circle 
			new Entity("odot",                 '\x2299', '\x0'), // /odot B: middle dot in circle 
			new Entity("odsold",               '\x29bc', '\x0'), // dot, solidus, dot in circle 
			new Entity("oelig",                '\x153', '\x0'), // latin small ligature oe, U+0153 ISOlat2 
			new Entity("OElig",                '\x152', '\x0'), // latin capital ligature OE, U+0152 ISOlat2 
			new Entity("ofcir",                '\x29bf', '\x0'), // filled circle in circle 
			// new Entity("ofr",                  "%plane1D;52C;"), /frak o, lower case o 
			// new Entity("Ofr",                  "%plane1D;512;"), /frak O, upper case o 
			new Entity("ogon",                 '\x2db', '\x0'), // =ogonek 
			new Entity("ograve",               '\xf2', '\x0'), // latin small o with grave, U+00F2 ISOlat1 
			new Entity("Ograve",               '\xd2', '\x0'), // latin capital O with grave, U+00D2 ISOlat1 
			new Entity("ogt",                  '\x29c1', '\x0'), // greater-than in circle 
			new Entity("ohbar",                '\x29b5', '\x0'), // circle with horizontal bar 
			new Entity("ohm",                  '\x2126', '\x0'), // =ohm sign 
			new Entity("oint",                 '\x222e', '\x0'), // alias ISOTECH conint 
			new Entity("olarr",                '\x21ba', '\x0'), // /circlearrowleft A: l arr in circle 
			new Entity("olcir",                '\x29be', '\x0'), // large circle in circle 
			new Entity("olcross",              '\x29bb', '\x0'), // circle, cross 
			new Entity("oline",                '\x203e', '\x0'), // overline = spacing overscore, U+203E NEW 
			new Entity("olt",                  '\x29c0', '\x0'), // less-than in circle 
			new Entity("omacr",                '\x14d', '\x0'), // =small o, macron 
			new Entity("Omacr",                '\x14c', '\x0'), // =capital O, macron 
			new Entity("omega",                '\x3c9', '\x0'), // greek small letter omega, U+03C9 ISOgrk3 
			new Entity("Omega",                '\x3a9', '\x0'), // greek capital letter omega, U+03A9 ISOgrk3 
			new Entity("omicron",              '\x3bf', '\x0'), // greek small letter omicron, U+03BF NEW 
			new Entity("Omicron",              '\x39f', '\x0'), // greek capital letter omicron, U+039F 
			new Entity("omid",                 '\x29b6', '\x0'), // vertical bar in circle 
			new Entity("ominus",               '\x2296', '\x0'), // /ominus B: minus sign in circle 
			// new Entity("oopf",                 "%plane1D;560;"),  
			// new Entity("Oopf",                 "%plane1D;546;"), /Bbb O, open face O 
			new Entity("opar",                 '\x29b7', '\x0'), // parallel in circle 
			new Entity("OpenCurlyDoubleQuote", '\x201c', '\x0'), // alias ISONUM ldquo 
			new Entity("OpenCurlyQuote",       '\x2018', '\x0'), // alias ISONUM lsquo 
			new Entity("operp",                '\x29b9', '\x0'), // perpendicular in circle 
			new Entity("oplus",                '\x2295', '\x0'), // circled plus = direct sum, U+2295 ISOamsb 
			new Entity("or",                   '\x2228', '\x0'), // logical or = vee, U+2228 ISOtech 
			new Entity("Or",                   '\x2a54', '\x0'), // dbl logical or 
			new Entity("orarr",                '\x21bb', '\x0'), // /circlearrowright A: r arr in circle 
			new Entity("ord",                  '\x2a5d', '\x0'), // or, horizontal dash 
			new Entity("order",                '\x2134', '\x0'), // order of (script small o)  
			new Entity("orderof",              '\x2134', '\x0'), // alias ISOTECH order 
			new Entity("ordf",                 '\xaa', '\x0'), // feminine ordinal indicator, U+00AA ISOnum 
			new Entity("ordm",                 '\xba', '\x0'), // masculine ordinal indicator, U+00BA ISOnum 
			new Entity("origof",               '\x22b6', '\x0'), // original of 
			new Entity("oror",                 '\x2a56', '\x0'), // two logical or 
			new Entity("orslope",              '\x2a57', '\x0'), // sloping large or 
			new Entity("orv",                  '\x2a5b', '\x0'), // or with middle stem 
			new Entity("oS",                   '\x24c8', '\x0'), // /circledS - capital S in circle 
			new Entity("oscr",                 '\x2134', '\x0'), // /scr o, script letter o 
			// new Entity("Oscr",                 "%plane1D;4AA;"), /scr O, script letter O 
			new Entity("oslash",               '\xf8', '\x0'), // latin small o with stroke, = latin small o slash, U+00F8 ISOlat1 
			new Entity("Oslash",               '\xd8', '\x0'), // latin capital O with stroke = latin capital O slash, U+00D8 ISOlat1 
			new Entity("osol",                 '\x2298', '\x0'), // /oslash B: solidus in circle 
			new Entity("otilde",               '\xf5', '\x0'), // latin small o with tilde, U+00F5 ISOlat1 
			new Entity("Otilde",               '\xd5', '\x0'), // latin capital O with tilde, U+00D5 ISOlat1 
			new Entity("otimes",               '\x2297', '\x0'), // circled times = vector product, U+2297 ISOamsb 
			new Entity("Otimes",               '\x2a37', '\x0'), // multiply sign in double circle 
			new Entity("otimesas",             '\x2a36', '\x0'), // multiply sign in circle, circumflex accent 
			new Entity("ouml",                 '\xf6', '\x0'), // latin small o with diaeresis, U+00F6 ISOlat1 
			new Entity("Ouml",                 '\xd6', '\x0'), // latin capital O with diaeresis, U+00D6 ISOlat1 
			new Entity("ovbar",                '\x233d', '\x0'), // circle with vertical bar 
			new Entity("OverBar",              '\xaf', '\x0'), // over bar 
			new Entity("OverBrace",            '\xfe37', '\x0'), // over brace  
			new Entity("OverBracket",          '\x23b4', '\x0'), // over bracket 
			new Entity("OverParenthesis",      '\xfe35', '\x0'), // over parenthesis 
			new Entity("par",                  '\x2225', '\x0'), // /parallel R: parallel 
			new Entity("para",                 '\xb6', '\x0'), // pilcrow sign = paragraph sign, U+00B6 ISOnum 
			new Entity("parallel",             '\x2225', '\x0'), // alias ISOTECH par 
			new Entity("parsim",               '\x2af3', '\x0'), // parallel, similar 
			new Entity("parsl",                '\x2afd', '\x0'), // parallel, slanted 
			new Entity("part",                 '\x2202', '\x0'), // partial differential, U+2202 ISOtech  
			new Entity("PartialD",             '\x2202', '\x0'), // alias ISOTECH part 
			new Entity("pcy",                  '\x43f', '\x0'), // =small pe, Cyrillic 
			new Entity("Pcy",                  '\x41f', '\x0'), // =capital PE, Cyrillic 
			new Entity("percnt",               '\x25', '\x0'), // =percent sign 
			new Entity("period",               '\x2e', '\x0'), // =full stop, period 
			new Entity("permil",               '\x2030', '\x0'), // per mille sign, U+2030 ISOtech 
			new Entity("perp",                 '\x22a5', '\x0'), // up tack = orthogonal to = perpendicular, U+22A5 ISOtech 
			new Entity("pertenk",              '\x2031', '\x0'), // per 10 thousand 
			// new Entity("pfr",                  "%plane1D;52D;"), /frak p, lower case p 
			// new Entity("Pfr",                  "%plane1D;513;"), /frak P, upper case p 
			new Entity("phi",                  '\x3c6', '\x0'), // greek small letter phi, U+03C6 ISOgrk3 
			new Entity("Phi",                  '\x3a6', '\x0'), // greek capital letter phi, U+03A6 ISOgrk3 
			new Entity("phiv",                 '\x3c6', '\x0'), // /varphi - curly or open phi 
			new Entity("phmmat",               '\x2133', '\x0'), // physics M-matrix (script capital M)  
			new Entity("phone",                '\x260e', '\x0'), // =telephone symbol  
			new Entity("pi",                   '\x3c0', '\x0'), // greek small letter pi, U+03C0 ISOgrk3 
			new Entity("Pi",                   '\x3a0', '\x0'), // greek capital letter pi, U+03A0 ISOgrk3 
			new Entity("pitchfork",            '\x22d4', '\x0'), // alias ISOAMSR fork 
			new Entity("piv",                  '\x3d6', '\x0'), // greek pi symbol, U+03D6 ISOgrk3 
			new Entity("planck",               '\x210f', '\x0'), // /hbar - Planck's over 2pi 
			new Entity("planckh",              '\x210e', '\x0'), // the ring (skew field) of quaternions 
			new Entity("plankv",               '\x210f', '\x0'), // /hslash - variant Planck's over 2pi 
			new Entity("plus",                 '\x2b', '\x0'), // =plus sign B: 
			new Entity("plusacir",             '\x2a23', '\x0'), // plus, circumflex accent above 
			new Entity("plusb",                '\x229e', '\x0'), // /boxplus B: plus sign in box 
			new Entity("pluscir",              '\x2a22', '\x0'), // plus, small circle above 
			new Entity("plusdo",               '\x2214', '\x0'), // /dotplus B: plus sign, dot above 
			new Entity("plusdu",               '\x2a25', '\x0'), // plus sign, dot below 
			new Entity("pluse",                '\x2a72', '\x0'), // plus, equals 
			new Entity("PlusMinus",            '\xb1', '\x0'), // alias ISONUM plusmn 
			new Entity("plusmn",               '\xb1', '\x0'), // plus-minus sign = plus-or-minus sign, U+00B1 ISOnum 
			new Entity("plussim",              '\x2a26', '\x0'), // plus, similar below 
			new Entity("plustwo",              '\x2a27', '\x0'), // plus, two; Nim-addition 
			new Entity("pm",                   '\xb1', '\x0'), // alias ISONUM plusmn 
			new Entity("Poincareplane",        '\x210c', '\x0'), // the Poincare upper half-plane 
			new Entity("pointint",             '\x2a15', '\x0'), // integral around a point operator 
			// new Entity("popf",                 "%plane1D;561;"),  
			new Entity("Popf",                 '\x2119', '\x0'), // /Bbb P, open face P 
			new Entity("pound",                '\xa3', '\x0'), // pound sign, U+00A3 ISOnum 
			new Entity("pr",                   '\x227a', '\x0'), // /prec R: precedes 
			new Entity("Pr",                   '\x2abb', '\x0'), // dbl precedes 
			new Entity("prap",                 '\x2ab7', '\x0'), // /precapprox R: precedes, approximate 
			new Entity("prcue",                '\x227c', '\x0'), // /preccurlyeq R: precedes, curly eq 
			new Entity("pre",                  '\x2aaf', '\x0'), // /preceq R: precedes, equals 
			new Entity("prE",                  '\x2ab3', '\x0'), // precedes, dbl equals 
			new Entity("prec",                 '\x227a', '\x0'), // alias ISOAMSR pr 
			new Entity("precapprox",           '\x2ab7', '\x0'), // alias ISOAMSR prap 
			new Entity("preccurlyeq",          '\x227c', '\x0'), // alias ISOAMSR prcue 
			new Entity("Precedes",             '\x227a', '\x0'), // alias ISOAMSR pr 
			new Entity("PrecedesEqual",        '\x2aaf', '\x0'), // alias ISOAMSR pre 
			new Entity("PrecedesSlantEqual",   '\x227c', '\x0'), // alias ISOAMSR prcue 
			new Entity("PrecedesTilde",        '\x227e', '\x0'), // alias ISOAMSR prsim 
			new Entity("preceq",               '\x2aaf', '\x0'), // alias ISOAMSR pre 
			new Entity("precnapprox",          '\x2ab9', '\x0'), // alias ISOAMSN prnap 
			new Entity("precneqq",             '\x2ab5', '\x0'), // alias ISOAMSN prnE 
			new Entity("precnsim",             '\x22e8', '\x0'), // alias ISOAMSN prnsim 
			new Entity("precsim",              '\x227e', '\x0'), // alias ISOAMSR prsim 
			new Entity("prime",                '\x2032', '\x0'), // prime = minutes = feet, U+2032 ISOtech 
			new Entity("Prime",                '\x2033', '\x0'), // double prime = seconds = inches, U+2033 ISOtech 
			new Entity("primes",               '\x2119', '\x0'), // the prime natural numbers 
			new Entity("prnap",                '\x2ab9', '\x0'), // /precnapprox N: precedes, not approx 
			new Entity("prnE",                 '\x2ab5', '\x0'), // /precneqq N: precedes, not dbl eq 
			new Entity("prnsim",               '\x22e8', '\x0'), // /precnsim N: precedes, not similar 
			new Entity("prod",                 '\x220f', '\x0'), // n-ary product = product sign, U+220F ISOamsb 
			new Entity("Product",              '\x220f', '\x0'), // alias for &prod; 
			new Entity("profalar",             '\x232e', '\x0'), // all-around profile 
			new Entity("profline",             '\x2312', '\x0'), // profile of a line 
			new Entity("profsurf",             '\x2313', '\x0'), // profile of a surface 
			new Entity("prop",                 '\x221d', '\x0'), // proportional to, U+221D ISOtech 
			new Entity("Proportion",           '\x2237', '\x0'), // alias ISOAMSR Colon 
			new Entity("Proportional",         '\x221d', '\x0'), // alias ISOTECH prop 
			new Entity("propto",               '\x221d', '\x0'), // alias ISOTECH prop 
			new Entity("prsim",                '\x227e', '\x0'), // /precsim R: precedes, similar 
			new Entity("prurel",               '\x22b0', '\x0'), // element precedes under relation 
			// new Entity("pscr",                 "%plane1D;4C5;"), /scr p, script letter p 
			// new Entity("Pscr",                 "%plane1D;4AB;"), /scr P, script letter P 
			new Entity("psi",                  '\x3c8', '\x0'), // greek small letter psi, U+03C8 ISOgrk3 
			new Entity("Psi",                  '\x3a8', '\x0'), // greek capital letter psi, U+03A8 ISOgrk3 
			new Entity("puncsp",               '\x2008', '\x0'), // =punctuation space (width of comma) 
			// new Entity("qfr",                  "%plane1D;52E;"), /frak q, lower case q 
			// new Entity("Qfr",                  "%plane1D;514;"), /frak Q, upper case q 
			new Entity("qint",                 '\x2a0c', '\x0'), // /iiiint quadruple integral operator 
			// new Entity("qopf",                 "%plane1D;562;"),  
			new Entity("Qopf",                 '\x211a', '\x0'), // /Bbb Q, open face Q 
			new Entity("qprime",               '\x2057', '\x0'), // quadruple prime 
			// new Entity("qscr",                 "%plane1D;4C6;"), /scr q, script letter q 
			// new Entity("Qscr",                 "%plane1D;4AC;"), /scr Q, script letter Q 
			new Entity("quaternions",          '\x210d', '\x0'), // the ring (skew field) of quaternions 
			new Entity("quatint",              '\x2a16', '\x0'), // quaternion integral operator 
			new Entity("quest",                '\x3f', '\x0'), // =question mark 
			new Entity("questeq",              '\x225f', '\x0'), // alias ISOAMSR equest 
			new Entity("quot",                 '\x22', '\x0'), // quotation mark = APL quote, U+0022 ISOnum 
			new Entity("rAarr",                '\x21db', '\x0'), // /Rrightarrow A: right triple arrow 
			new Entity("race",                 '\x29da', '\x0'), // reverse most positive, line below 
			new Entity("racute",               '\x155', '\x0'), // =small r, acute accent 
			new Entity("Racute",               '\x154', '\x0'), // =capital R, acute accent 
			new Entity("radic",                '\x221a', '\x0'), // square root = radical sign, U+221A ISOtech 
			new Entity("raemptyv",             '\x29b3', '\x0'), // circle, slash, right arrow above 
			new Entity("rang",                 '\x232a', '\x0'), // right-pointing angle bracket = ket, U+232A ISOtech 
			new Entity("Rang",                 '\x300b', '\x0'), // right angle bracket, double 
			new Entity("rangd",                '\x2992', '\x0'), // right angle, dot 
			new Entity("range",                '\x29a5', '\x0'), // reverse angle, equal 
			new Entity("rangle",               '\x232a', '\x0'), // alias ISOTECH rang 
			new Entity("raquo",                '\xbb', '\x0'), // right-pointing double angle quotation mark = right pointing guillemet, U+00BB ISOnum 
			new Entity("rarr",                 '\x2192', '\x0'), // rightwards arrow, U+2192 ISOnum 
			new Entity("rArr",                 '\x21d2', '\x0'), // rightwards double arrow, U+21D2 ISOtech 
			new Entity("Rarr",                 '\x21a0', '\x0'), // /twoheadrightarrow A: 
			new Entity("rarrap",               '\x2975', '\x0'), // approximate, right arrow above 
			new Entity("rarrb",                '\x21e5', '\x0'), // leftwards arrow to bar 
			new Entity("rarrbfs",              '\x2920', '\x0'), // right arrow-bar, filled square 
			new Entity("rarrc",                '\x2933', '\x0'), // right arrow-curved 
			new Entity("rarrfs",               '\x291e', '\x0'), // right arrow, filled square 
			new Entity("rarrhk",               '\x21aa', '\x0'), // /hookrightarrow A: rt arrow-hooked 
			new Entity("rarrlp",               '\x21ac', '\x0'), // /looparrowright A: rt arrow-looped 
			new Entity("rarrpl",               '\x2945', '\x0'), // right arrow, plus 
			new Entity("rarrsim",              '\x2974', '\x0'), // right arrow, similar 
			new Entity("rarrtl",               '\x21a3', '\x0'), // /rightarrowtail A: rt arrow-tailed 
			new Entity("Rarrtl",               '\x2916', '\x0'), // right two-headed arrow with tail 
			new Entity("rarrw",                '\x219d', '\x0'), // /rightsquigarrow A: rt arrow-wavy 
			new Entity("ratail",               '\x291a', '\x0'), // right arrow-tail 
			new Entity("rAtail",               '\x291c', '\x0'), // right double arrow-tail 
			new Entity("ratio",                '\x2236', '\x0'), // /ratio 
			new Entity("rationals",            '\x211a', '\x0'), // the field of rational numbers 
			new Entity("rbarr",                '\x290d', '\x0'), // /bkarow A: right broken arrow 
			new Entity("rBarr",                '\x290f', '\x0'), // /dbkarow A: right doubly broken arrow 
			new Entity("RBarr",                '\x2910', '\x0'), // /drbkarow A: twoheaded right broken arrow 
			new Entity("rbbrk",                '\x3015', '\x0'), // right broken bracket 
			new Entity("rbrace",               '\x7d', '\x0'), // alias ISONUM rcub 
			new Entity("rbrack",               '\x5d', '\x0'), // alias ISONUM rsqb 
			new Entity("rbrke",                '\x298c', '\x0'), // right bracket, equal 
			new Entity("rbrksld",              '\x298e', '\x0'), // right bracket, solidus bottom corner 
			new Entity("rbrkslu",              '\x2990', '\x0'), // right bracket, solidus top corner 
			new Entity("rcaron",               '\x159', '\x0'), // =small r, caron 
			new Entity("Rcaron",               '\x158', '\x0'), // =capital R, caron 
			new Entity("rcedil",               '\x157', '\x0'), // =small r, cedilla 
			new Entity("Rcedil",               '\x156', '\x0'), // =capital R, cedilla 
			new Entity("rceil",                '\x2309', '\x0'), // right ceiling, U+2309 ISOamsc  
			new Entity("rcub",                 '\x7d', '\x0'), // /rbrace C: =right curly bracket 
			new Entity("rcy",                  '\x440', '\x0'), // =small er, Cyrillic 
			new Entity("Rcy",                  '\x420', '\x0'), // =capital ER, Cyrillic 
			new Entity("rdca",                 '\x2937', '\x0'), // right down curved arrow 
			new Entity("rdldhar",              '\x2969', '\x0'), // right harpoon-down over left harpoon-down 
			new Entity("rdquo",                '\x201d', '\x0'), // right double quotation mark, U+201D ISOnum 
			new Entity("rdquor",               '\x201d', '\x0'), // rising dbl quote, right (high) 
			new Entity("rdsh",                 '\x21b3', '\x0'), // right down angled arrow 
			new Entity("Re",                   '\x211c', '\x0'), // alias ISOAMSO real 
			new Entity("real",                 '\x211c', '\x0'), // blackletter capital R = real part symbol, U+211C ISOamso 
			new Entity("realine",              '\x211b', '\x0'), // the geometric real line 
			new Entity("realpart",             '\x211c', '\x0'), // alias ISOAMSO real 
			new Entity("reals",                '\x211d', '\x0'), // the field of real numbers 
			new Entity("rect",                 '\x25ad', '\x0'), // =rectangle, open 
			new Entity("reg",                  '\xae', '\x0'), // registered sign = registered trade mark sign, U+00AE ISOnum 
			new Entity("ReverseElement",       '\x220b', '\x0'), // alias ISOTECH niv 
			new Entity("ReverseEquilibrium",   '\x21cb', '\x0'), // alias ISOAMSA lrhar 
			new Entity("ReverseUpEquilibrium", '\x296f', '\x0'), // alias ISOAMSA duhar 
			new Entity("rfisht",               '\x297d', '\x0'), // right fish tail 
			new Entity("rfloor",               '\x230b', '\x0'), // right floor, U+230B ISOamsc  
			// new Entity("rfr",                  "%plane1D;52F;"), /frak r, lower case r 
			new Entity("Rfr",                  '\x211c', '\x0'), // /frak R, upper case r 
			new Entity("rHar",                 '\x2964', '\x0'), // right harpoon-up over right harpoon-down 
			new Entity("rhard",                '\x21c1', '\x0'), // /rightharpoondown A: rt harpoon-down 
			new Entity("rharu",                '\x21c0', '\x0'), // /rightharpoonup A: rt harpoon-up 
			new Entity("rharul",               '\x296c', '\x0'), // right harpoon-up over long dash 
			new Entity("rho",                  '\x3c1', '\x0'), // greek small letter rho, U+03C1 ISOgrk3 
			new Entity("Rho",                  '\x3a1', '\x0'), // greek capital letter rho, U+03A1 
			new Entity("rhov",                 '\x3f1', '\x0'), // /varrho 
			new Entity("RightAngleBracket",    '\x232a', '\x0'), // alias ISOTECH rang 
			new Entity("rightarrow",           '\x2192', '\x0'), // alias ISONUM rarr 
			new Entity("Rightarrow",           '\x21d2', '\x0'), // alias ISOTECH rArr 
			new Entity("RightArrow",           '\x2192', '\x0'), // alias ISONUM rarr 
			new Entity("RightArrowBar",        '\x21e5', '\x0'), // alias for rarrb 
			new Entity("RightArrowLeftArrow",  '\x21c4', '\x0'), // alias ISOAMSA rlarr 
			new Entity("rightarrowtail",       '\x21a3', '\x0'), // alias ISOAMSA rarrtl 
			new Entity("RightCeiling",         '\x2309', '\x0'), // alias ISOAMSC rceil 
			new Entity("RightDoubleBracket",   '\x301b', '\x0'), // right double bracket delimiter 
			new Entity("RightDownTeeVector",   '\x295d', '\x0'), // down-right harpoon from bar 
			new Entity("RightDownVector",      '\x21c2', '\x0'), // alias ISOAMSA dharr 
			new Entity("RightDownVectorBar",   '\x2955', '\x0'), // down-right harpoon to bar 
			new Entity("RightFloor",           '\x230b', '\x0'), // alias ISOAMSC rfloor 
			new Entity("rightharpoondown",     '\x21c1', '\x0'), // alias ISOAMSA rhard 
			new Entity("rightharpoonup",       '\x21c0', '\x0'), // alias ISOAMSA rharu 
			new Entity("rightleftarrows",      '\x21c4', '\x0'), // alias ISOAMSA rlarr 
			new Entity("rightleftharpoons",    '\x21cc', '\x0'), // alias ISOAMSA rlhar 
			new Entity("rightrightarrows",     '\x21c9', '\x0'), // alias ISOAMSA rrarr 
			new Entity("rightsquigarrow",      '\x219d', '\x0'), // alias ISOAMSA rarrw 
			new Entity("RightTee",             '\x22a2', '\x0'), // alias ISOAMSR vdash 
			new Entity("RightTeeArrow",        '\x21a6', '\x0'), // alias ISOAMSA map 
			new Entity("RightTeeVector",       '\x295b', '\x0'), // right-up harpoon from bar 
			new Entity("rightthreetimes",      '\x22cc', '\x0'), // alias ISOAMSB rthree 
			new Entity("RightTriangle",        '\x22b3', '\x0'), // alias ISOAMSR vrtri 
			new Entity("RightTriangleBar",     '\x29d0', '\x0'), // vertical bar, right triangle 
			new Entity("RightTriangleEqual",   '\x22b5', '\x0'), // alias ISOAMSR rtrie 
			new Entity("RightUpDownVector",    '\x294f', '\x0'), // up-right-down-right harpoon 
			new Entity("RightUpTeeVector",     '\x295c', '\x0'), // up-right harpoon from bar 
			new Entity("RightUpVector",        '\x21be', '\x0'), // alias ISOAMSA uharr 
			new Entity("RightUpVectorBar",     '\x2954', '\x0'), // up-right harpoon to bar 
			new Entity("RightVector",          '\x21c0', '\x0'), // alias ISOAMSA rharu 
			new Entity("RightVectorBar",       '\x2953', '\x0'), // up-right harpoon to bar 
			new Entity("ring",                 '\x2da', '\x0'), // =ring 
			new Entity("risingdotseq",         '\x2253', '\x0'), // alias ISOAMSR erDot 
			new Entity("rlarr",                '\x21c4', '\x0'), // /rightleftarrows A: r arr over l arr 
			new Entity("rlhar",                '\x21cc', '\x0'), // /rightleftharpoons A: r harp over l 
			new Entity("rlm",                  '\x200f', '\x0'), // right-to-left mark, U+200F NEW RFC 2070 
			new Entity("rmoust",               '\x23b1', '\x0'), // /rmoustache 
			new Entity("rmoustache",           '\x23b1', '\x0'), // alias ISOAMSC rmoust 
			new Entity("rnmid",                '\x2aee', '\x0'), // reverse /nmid 
			new Entity("roang",                '\x3019', '\x0'), // right open angular bracket 
			new Entity("roarr",                '\x21fe', '\x0'), // right open arrow 
			new Entity("robrk",                '\x301b', '\x0'), // right open bracket 
			new Entity("ropar",                '\x2986', '\x0'), // right open parenthesis 
			// new Entity("ropf",                 "%plane1D;563;"),  
			new Entity("Ropf",                 '\x211d', '\x0'), // /Bbb R, open face R 
			new Entity("roplus",               '\x2a2e', '\x0'), // plus sign in right half circle 
			new Entity("rotimes",              '\x2a35', '\x0'), // multiply sign in right half circle 
			new Entity("RoundImplies",         '\x2970', '\x0'), // round implies 
			new Entity("rpar",                 '\x29', '\x0'), // C: =right parenthesis 
			new Entity("rpargt",               '\x2994', '\x0'), // C: right paren, gt 
			new Entity("rppolint",             '\x2a12', '\x0'), // line integration, rectangular path around pole 
			new Entity("rrarr",                '\x21c9', '\x0'), // /rightrightarrows A: two rt arrows 
			new Entity("Rrightarrow",          '\x21db', '\x0'), // alias ISOAMSA rAarr 
			new Entity("rsaquo",               '\x203a', '\x0'), // single right-pointing angle quotation mark, U+203A ISO proposed 
			// new Entity("rscr",                 "%plane1D;4C7;"), /scr r, script letter r 
			new Entity("Rscr",                 '\x211b', '\x0'), // /scr R, script letter R 
			new Entity("rsh",                  '\x21b1', '\x0'), // /Rsh A: 
			new Entity("Rsh",                  '\x21b1', '\x0'), // alias ISOAMSA rsh 
			new Entity("rsqb",                 '\x5d', '\x0'), // /rbrack C: =right square bracket 
			new Entity("rsquo",                '\x2019', '\x0'), // right single quotation mark, U+2019 ISOnum 
			new Entity("rsquor",               '\x2019', '\x0'), // rising single quote, right (high) 
			new Entity("rthree",               '\x22cc', '\x0'), // /rightthreetimes B: 
			new Entity("rtimes",               '\x22ca', '\x0'), // /rtimes B: times sign, right closed 
			new Entity("rtri",                 '\x25b9', '\x0'), // /triangleright B: r triangle, open 
			new Entity("rtrie",                '\x22b5', '\x0'), // /trianglerighteq R: right tri, eq 
			new Entity("rtrif",                '\x25b8', '\x0'), // /blacktriangleright R: =r tri, filled 
			new Entity("rtriltri",             '\x29ce', '\x0'), // right triangle above left triangle 
			new Entity("RuleDelayed",          '\x29f4', '\x0'), // rule-delayed (colon right arrow) 
			new Entity("ruluhar",              '\x2968', '\x0'), // right harpoon-up over left harpoon-up 
			new Entity("rx",                   '\x211e', '\x0'), // pharmaceutical prescription (Rx) 
			new Entity("sacute",               '\x15b', '\x0'), // =small s, acute accent 
			new Entity("Sacute",               '\x15a', '\x0'), // =capital S, acute accent 
			new Entity("sbquo",                '\x201a', '\x0'), // single low-9 quotation mark, U+201A NEW 
			new Entity("sc",                   '\x227b', '\x0'), // /succ R: succeeds 
			new Entity("Sc",                   '\x2abc', '\x0'), // dbl succeeds 
			new Entity("scap",                 '\x2ab8', '\x0'), // /succapprox R: succeeds, approximate 
			new Entity("scaron",               '\x161', '\x0'), // latin small letter s with caron, U+0161 ISOlat2 
			new Entity("Scaron",               '\x160', '\x0'), // latin capital letter S with caron, U+0160 ISOlat2 
			new Entity("sccue",                '\x227d', '\x0'), // /succcurlyeq R: succeeds, curly eq 
			new Entity("sce",                  '\x2ab0', '\x0'), // /succeq R: succeeds, equals 
			new Entity("scE",                  '\x2ab4', '\x0'), // succeeds, dbl equals 
			new Entity("scedil",               '\x15f', '\x0'), // =small s, cedilla 
			new Entity("Scedil",               '\x15e', '\x0'), // =capital S, cedilla 
			new Entity("scirc",                '\x15d', '\x0'), // =small s, circumflex accent 
			new Entity("Scirc",                '\x15c', '\x0'), // =capital S, circumflex accent 
			new Entity("scnap",                '\x2aba', '\x0'), // /succnapprox N: succeeds, not approx 
			new Entity("scnE",                 '\x2ab6', '\x0'), // /succneqq N: succeeds, not dbl eq 
			new Entity("scnsim",               '\x22e9', '\x0'), // /succnsim N: succeeds, not similar 
			new Entity("scpolint",             '\x2a13', '\x0'), // line integration, semi-circular path around pole 
			new Entity("scsim",                '\x227f', '\x0'), // /succsim R: succeeds, similar 
			new Entity("scy",                  '\x441', '\x0'), // =small es, Cyrillic 
			new Entity("Scy",                  '\x421', '\x0'), // =capital ES, Cyrillic 
			new Entity("sdot",                 '\x22c5', '\x0'), // dot operator, U+22C5 ISOamsb 
			new Entity("sdotb",                '\x22a1', '\x0'), // /dotsquare /boxdot B: small dot in box 
			new Entity("sdote",                '\x2a66', '\x0'), // equal, dot below 
			new Entity("searhk",               '\x2925', '\x0'), // /hksearow A: SE arrow-hooken 
			new Entity("searr",                '\x2198', '\x0'), // /searrow A: SE pointing arrow 
			new Entity("seArr",                '\x21d8', '\x0'), // SE pointing dbl arrow 
			new Entity("searrow",              '\x2198', '\x0'), // alias ISOAMSA searr 
			new Entity("sect",                 '\xa7', '\x0'), // section sign, U+00A7 ISOnum 
			new Entity("semi",                 '\x3b', '\x0'), // =semicolon P: 
			new Entity("seswar",               '\x2929', '\x0'), // /tosa A: SE & SW arrows 
			new Entity("setminus",             '\x2216', '\x0'), // alias ISOAMSB setmn 
			new Entity("setmn",                '\x2216', '\x0'), // /setminus B: reverse solidus 
			new Entity("sext",                 '\x2736', '\x0'), // sextile (6-pointed star) 
			// new Entity("sfr",                  "%plane1D;530;"), /frak s, lower case s 
			// new Entity("Sfr",                  "%plane1D;516;"), /frak S, upper case s 
			new Entity("sfrown",               '\x2322', '\x0'), // /smallfrown R: small down curve 
			new Entity("sharp",                '\x266f', '\x0'), // /sharp =musical sharp 
			new Entity("shchcy",               '\x449', '\x0'), // =small shcha, Cyrillic 
			new Entity("SHCHcy",               '\x429', '\x0'), // =capital SHCHA, Cyrillic 
			new Entity("shcy",                 '\x448', '\x0'), // =small sha, Cyrillic 
			new Entity("SHcy",                 '\x428', '\x0'), // =capital SHA, Cyrillic 
			new Entity("ShortDownArrow",       '\x2193', '\x0'), // short down arrow 
			new Entity("ShortLeftArrow",       '\x2190', '\x0'), // alias ISOAMSA slarr 
			new Entity("shortmid",             '\x2223', '\x0'), // alias ISOAMSR smid 
			new Entity("shortparallel",        '\x2225', '\x0'), // alias ISOAMSR spar 
			new Entity("ShortRightArrow",      '\x2192', '\x0'), // alias ISOAMSA srarr 
			new Entity("ShortUpArrow",         '\x2191', '\x0'), // short up arrow  
			new Entity("shy",                  '\xad', '\x0'), // soft hyphen = discretionary hyphen, U+00AD ISOnum 
			new Entity("sigma",                '\x3c3', '\x0'), // greek small letter sigma, U+03C3 ISOgrk3 
			new Entity("Sigma",                '\x3a3', '\x0'), // greek capital letter sigma, U+03A3 ISOgrk3 
			new Entity("sigmaf",               '\x3c2', '\x0'), // greek small letter final sigma, U+03C2 ISOgrk3 
			new Entity("sigmav",               '\x3c2', '\x0'), // /varsigma 
			new Entity("sim",                  '\x223c', '\x0'), // tilde operator = varies with = similar to, U+223C ISOtech 
			new Entity("simdot",               '\x2a6a', '\x0'), // similar, dot 
			new Entity("sime",                 '\x2243', '\x0'), // /simeq R: similar, equals 
			new Entity("simeq",                '\x2243', '\x0'), // alias ISOTECH sime 
			new Entity("simg",                 '\x2a9e', '\x0'), // similar, greater 
			new Entity("simgE",                '\x2aa0', '\x0'), // similar, greater, equal 
			new Entity("siml",                 '\x2a9d', '\x0'), // similar, less 
			new Entity("simlE",                '\x2a9f', '\x0'), // similar, less, equal 
			new Entity("simne",                '\x2246', '\x0'), // similar, not equals 
			new Entity("simplus",              '\x2a24', '\x0'), // plus, similar above 
			new Entity("simrarr",              '\x2972', '\x0'), // similar, right arrow below 
			new Entity("slarr",                '\x2190', '\x0'), // short left arrow 
			new Entity("SmallCircle",          '\x2218', '\x0'), // alias ISOTECH compfn 
			new Entity("smallsetminus",        '\x2216', '\x0'), // alias ISOAMSB ssetmn 
			new Entity("smashp",               '\x2a33', '\x0'), // smash product 
			new Entity("smeparsl",             '\x29e4', '\x0'), // similar, parallel, slanted, equal 
			new Entity("smid",                 '\x2223', '\x0'), // /shortmid R: 
			new Entity("smile",                '\x2323', '\x0'), // /smile R: up curve 
			new Entity("smt",                  '\x2aaa', '\x0'), // smaller than 
			new Entity("smte",                 '\x2aac', '\x0'), // smaller than or equal 
			new Entity("smtes",                '\x2aac', '\xfe00'), // smaller than or equal, slanted 
			new Entity("softcy",               '\x44c', '\x0'), // =small soft sign, Cyrillic 
			new Entity("SOFTcy",               '\x42c', '\x0'), // =capital SOFT sign, Cyrillic 
			new Entity("sol",                  '\x2f', '\x0'), // =solidus 
			new Entity("solb",                 '\x29c4', '\x0'), // solidus in square 
			new Entity("solbar",               '\x233f', '\x0'), // solidus, bar through 
			// new Entity("sopf",                 "%plane1D;564;"),  
			// new Entity("Sopf",                 "%plane1D;54A;"), /Bbb S, open face S 
			new Entity("spades",               '\x2660', '\x0'), // black spade suit, U+2660 ISOpub 
			new Entity("spadesuit",            '\x2660', '\x0'), // ISOPUB    spades  
			new Entity("spar",                 '\x2225', '\x0'), // /shortparallel R: short parallel 
			new Entity("sqcap",                '\x2293', '\x0'), // /sqcap B: square intersection 
			new Entity("sqcaps",               '\x2293', '\xfe00'), // square intersection, serifs 
			new Entity("sqcup",                '\x2294', '\x0'), // /sqcup B: square union 
			new Entity("sqcups",               '\x2294', '\xfe00'), // square union, serifs 
			new Entity("Sqrt",                 '\x221a', '\x0'), // alias ISOTECH radic 
			new Entity("sqsub",                '\x228f', '\x0'), // /sqsubset R: square subset 
			new Entity("sqsube",               '\x2291', '\x0'), // /sqsubseteq R: square subset, equals 
			new Entity("sqsubset",             '\x228f', '\x0'), // alias ISOAMSR sqsub 
			new Entity("sqsubseteq",           '\x2291', '\x0'), // alias ISOAMSR sqsube 
			new Entity("sqsup",                '\x2290', '\x0'), // /sqsupset R: square superset 
			new Entity("sqsupe",               '\x2292', '\x0'), // /sqsupseteq R: square superset, eq 
			new Entity("sqsupset",             '\x2290', '\x0'), // alias ISOAMSR sqsup 
			new Entity("sqsupseteq",           '\x2292', '\x0'), // alias ISOAMSR sqsupe 
			new Entity("squ",                  '\x25a1', '\x0'), // =square, open 
			new Entity("square",               '\x25a1', '\x0'), // /square, square 
			new Entity("Square",               '\x25a1', '\x0'), // alias for square 
			new Entity("SquareIntersection",   '\x2293', '\x0'), // alias ISOAMSB sqcap 
			new Entity("SquareSubset",         '\x228f', '\x0'), // alias ISOAMSR sqsub 
			new Entity("SquareSubsetEqual",    '\x2291', '\x0'), // alias ISOAMSR sqsube 
			new Entity("SquareSuperset",       '\x2290', '\x0'), // alias ISOAMSR sqsup 
			new Entity("SquareSupersetEqual",  '\x2292', '\x0'), // alias ISOAMSR sqsupe 
			new Entity("SquareUnion",          '\x2294', '\x0'), // alias ISOAMSB sqcup 
			new Entity("squarf",               '\x25aa', '\x0'), // /blacksquare, square, filled  
			new Entity("squf",                 '\x25aa', '\x0'), // /blacksquare =sq bullet, filled 
			new Entity("srarr",                '\x2192', '\x0'), // short right arrow 
			// new Entity("sscr",                 "%plane1D;4C8;"), /scr s, script letter s 
			// new Entity("Sscr",                 "%plane1D;4AE;"), /scr S, script letter S 
			new Entity("ssetmn",               '\x2216', '\x0'), // /smallsetminus B: sm reverse solidus 
			new Entity("ssmile",               '\x2323', '\x0'), // /smallsmile R: small up curve 
			new Entity("sstarf",               '\x22c6', '\x0'), // /star B: small star, filled 
			new Entity("star",                 '\x2606', '\x0'), // =star, open 
			new Entity("Star",                 '\x22c6', '\x0'), // alias ISOAMSB sstarf 
			new Entity("starf",                '\x2605', '\x0'), // /bigstar - star, filled  
			new Entity("straightepsilon",      '\x3f5', '\x0'), // alias ISOGRK3 epsi 
			new Entity("straightphi",          '\x3d5', '\x0'), // alias ISOGRK3 phi 
			new Entity("strns",                '\xaf', '\x0'), // straightness 
			new Entity("sub",                  '\x2282', '\x0'), // subset of, U+2282 ISOtech 
			new Entity("Sub",                  '\x22d0', '\x0'), // /Subset R: double subset 
			new Entity("subdot",               '\x2abd', '\x0'), // subset, with dot 
			new Entity("sube",                 '\x2286', '\x0'), // subset of or equal to, U+2286 ISOtech 
			new Entity("subE",                 '\x2ac5', '\x0'), // /subseteqq R: subset, dbl equals 
			new Entity("subedot",              '\x2ac3', '\x0'), // subset, equals, dot 
			new Entity("submult",              '\x2ac1', '\x0'), // subset, multiply 
			new Entity("subne",                '\x228a', '\x0'), // /subsetneq N: subset, not equals 
			new Entity("subnE",                '\x2acb', '\x0'), // /subsetneqq N: subset, not dbl eq 
			new Entity("subplus",              '\x2abf', '\x0'), // subset, plus 
			new Entity("subrarr",              '\x2979', '\x0'), // subset, right arrow 
			new Entity("subset",               '\x2282', '\x0'), // alias ISOTECH sub 
			new Entity("Subset",               '\x22d0', '\x0'), // alias ISOAMSR Sub 
			new Entity("subseteq",             '\x2286', '\x0'), // alias ISOTECH sube 
			new Entity("subseteqq",            '\x2ac5', '\x0'), // alias ISOAMSR subE 
			new Entity("SubsetEqual",          '\x2286', '\x0'), // alias ISOTECH sube 
			new Entity("subsetneq",            '\x228a', '\x0'), // alias ISOAMSN subne 
			new Entity("subsetneqq",           '\x2acb', '\x0'), // alias ISOAMSN subnE 
			new Entity("subsim",               '\x2ac7', '\x0'), // subset, similar 
			new Entity("subsub",               '\x2ad5', '\x0'), // subset above subset 
			new Entity("subsup",               '\x2ad3', '\x0'), // subset above superset 
			new Entity("succ",                 '\x227b', '\x0'), // alias ISOAMSR sc 
			new Entity("succapprox",           '\x2ab8', '\x0'), // alias ISOAMSR scap 
			new Entity("succcurlyeq",          '\x227d', '\x0'), // alias ISOAMSR sccue 
			new Entity("Succeeds",             '\x227b', '\x0'), // alias ISOAMSR sc 
			new Entity("SucceedsEqual",        '\x2ab0', '\x0'), // alias ISOAMSR sce 
			new Entity("SucceedsSlantEqual",   '\x227d', '\x0'), // alias ISOAMSR sccue 
			new Entity("SucceedsTilde",        '\x227f', '\x0'), // alias ISOAMSR scsim 
			new Entity("succeq",               '\x2ab0', '\x0'), // alias ISOAMSR sce 
			new Entity("succnapprox",          '\x2aba', '\x0'), // alias ISOAMSN scnap 
			new Entity("succneqq",             '\x2ab6', '\x0'), // alias ISOAMSN scnE 
			new Entity("succnsim",             '\x22e9', '\x0'), // alias ISOAMSN scnsim 
			new Entity("succsim",              '\x227f', '\x0'), // alias ISOAMSR scsim 
			new Entity("SuchThat",             '\x220b', '\x0'), // ISOTECH  ni 
			new Entity("sum",                  '\x2211', '\x0'), // n-ary sumation, U+2211 ISOamsb 
			new Entity("Sum",                  '\x2211', '\x0'), // alias ISOAMSB sum 
			new Entity("sung",                 '\x266a', '\x0'), // =music note (sung text sign) 
			new Entity("sup",                  '\x2283', '\x0'), // superset of, U+2283 ISOtech 
			new Entity("Sup",                  '\x22d1', '\x0'), // /Supset R: dbl superset 
			new Entity("sup1",                 '\xb9', '\x0'), // superscript one = superscript digit one, U+00B9 ISOnum 
			new Entity("sup2",                 '\xb2', '\x0'), // superscript two = superscript digit two = squared, U+00B2 ISOnum 
			new Entity("sup3",                 '\xb3', '\x0'), // superscript three = superscript digit three = cubed, U+00B3 ISOnum 
			new Entity("supdot",               '\x2abe', '\x0'), // superset, with dot 
			new Entity("supdsub",              '\x2ad8', '\x0'), // superset, subset, dash joining them 
			new Entity("supe",                 '\x2287', '\x0'), // superset of or equal to, U+2287 ISOtech 
			new Entity("supE",                 '\x2ac6', '\x0'), // /supseteqq R: superset, dbl equals 
			new Entity("supedot",              '\x2ac4', '\x0'), // superset, equals, dot 
			new Entity("Superset",             '\x2283', '\x0'), // alias ISOTECH sup 
			new Entity("SupersetEqual",        '\x2287', '\x0'), // alias ISOTECH supe 
			new Entity("suphsol",              '\x2283', '\x2f'), // superset, solidus 
			new Entity("suphsub",              '\x2ad7', '\x0'), // superset, subset 
			new Entity("suplarr",              '\x297b', '\x0'), // superset, left arrow 
			new Entity("supmult",              '\x2ac2', '\x0'), // superset, multiply 
			new Entity("supne",                '\x228b', '\x0'), // /supsetneq N: superset, not equals 
			new Entity("supnE",                '\x2acc', '\x0'), // /supsetneqq N: superset, not dbl eq 
			new Entity("supplus",              '\x2ac0', '\x0'), // superset, plus 
			new Entity("supset",               '\x2283', '\x0'), // alias ISOTECH sup 
			new Entity("Supset",               '\x22d1', '\x0'), // alias ISOAMSR Sup 
			new Entity("supseteq",             '\x2287', '\x0'), // alias ISOTECH supe 
			new Entity("supseteqq",            '\x2ac6', '\x0'), // alias ISOAMSR supE 
			new Entity("supsetneq",            '\x228b', '\x0'), // alias ISOAMSN supne 
			new Entity("supsetneqq",           '\x2acc', '\x0'), // alias ISOAMSN supnE 
			new Entity("supsim",               '\x2ac8', '\x0'), // superset, similar 
			new Entity("supsub",               '\x2ad4', '\x0'), // superset above subset 
			new Entity("supsup",               '\x2ad6', '\x0'), // superset above superset 
			new Entity("swarhk",               '\x2926', '\x0'), // /hkswarow A: SW arrow-hooked 
			new Entity("swarr",                '\x2199', '\x0'), // /swarrow A: SW pointing arrow 
			new Entity("swArr",                '\x21d9', '\x0'), // SW pointing dbl arrow 
			new Entity("swarrow",              '\x2199', '\x0'), // alias ISOAMSA swarr 
			new Entity("swnwar",               '\x292a', '\x0'), // SW & NW arrows 
			new Entity("szlig",                '\xdf', '\x0'), // latin small sharp s = ess-zed, U+00DF ISOlat1 
			new Entity("Tab",                  '\x9', '\x0'), // tabulator stop; horizontal tabulation 
			new Entity("target",               '\x2316', '\x0'), // register mark or target 
			new Entity("tau",                  '\x3c4', '\x0'), // greek small letter tau, U+03C4 ISOgrk3 
			new Entity("Tau",                  '\x3a4', '\x0'), // greek capital letter tau, U+03A4 
			new Entity("tbrk",                 '\x23b4', '\x0'), // top square bracket 
			new Entity("tcaron",               '\x165', '\x0'), // =small t, caron 
			new Entity("Tcaron",               '\x164', '\x0'), // =capital T, caron 
			new Entity("tcedil",               '\x163', '\x0'), // =small t, cedilla 
			new Entity("Tcedil",               '\x162', '\x0'), // =capital T, cedilla 
			new Entity("tcy",                  '\x442', '\x0'), // =small te, Cyrillic 
			new Entity("Tcy",                  '\x422', '\x0'), // =capital TE, Cyrillic 
			new Entity("tdot",                 '\x20db', '\x0'), // three dots above 
			new Entity("telrec",               '\x2315', '\x0'), // =telephone recorder symbol 
			// new Entity("tfr",                  "%plane1D;531;"), /frak t, lower case t 
			// new Entity("Tfr",                  "%plane1D;517;"), /frak T, upper case t 
			new Entity("there4",               '\x2234', '\x0'), // therefore, U+2234 ISOtech 
			new Entity("therefore",            '\x2234', '\x0'), // alias ISOTECH there4 
			new Entity("Therefore",            '\x2234', '\x0'), // alias ISOTECH there4 
			new Entity("theta",                '\x3b8', '\x0'), // greek small letter theta, U+03B8 ISOgrk3 
			new Entity("Theta",                '\x398', '\x0'), // greek capital letter theta, U+0398 ISOgrk3 
			new Entity("thetasym",             '\x3d1', '\x0'), // greek small letter theta symbol, U+03D1 NEW 
			new Entity("thetav",               '\x3d1', '\x0'), // /vartheta - curly or open theta 
			new Entity("thickapprox",          '\x2248', '\x0'), // ISOAMSR   thkap  
			new Entity("thicksim",             '\x223c', '\x0'), // ISOAMSR   thksim 
			new Entity("ThickSpace",           '\x2009', '\x0'), // space of width 5/18 em 
			new Entity("thinsp",               '\x2009', '\x0'), // thin space, U+2009 ISOpub 
			new Entity("ThinSpace",            '\x2009', '\x0'), // space of width 3/18 em alias ISOPUB thinsp 
			new Entity("thkap",                '\x2248', '\x0'), // /thickapprox R: thick approximate 
			new Entity("thksim",               '\x223c', '\x0'), // /thicksim R: thick similar 
			new Entity("thorn",                '\xfe', '\x0'), // latin small thorn with, U+00FE ISOlat1 
			new Entity("THORN",                '\xde', '\x0'), // latin capital THORN, U+00DE ISOlat1 
			new Entity("tilde",                '\x2dc', '\x0'), // small tilde, U+02DC ISOdia 
			new Entity("Tilde",                '\x223c', '\x0'), // alias ISOTECH sim 
			new Entity("TildeEqual",           '\x2243', '\x0'), // alias ISOTECH sime 
			new Entity("TildeFullEqual",       '\x2245', '\x0'), // alias ISOTECH cong 
			new Entity("TildeTilde",           '\x2248', '\x0'), // alias ISOTECH ap 
			new Entity("times",                '\xd7', '\x0'), // multiplication sign, U+00D7 ISOnum 
			new Entity("timesb",               '\x22a0', '\x0'), // /boxtimes B: multiply sign in box 
			new Entity("timesbar",             '\x2a31', '\x0'), // multiply sign, bar below 
			new Entity("timesd",               '\x2a30', '\x0'), // times, dot 
			new Entity("tint",                 '\x222d', '\x0'), // /iiint triple integral operator 
			new Entity("toea",                 '\x2928', '\x0'), // alias ISOAMSA nesear 
			new Entity("top",                  '\x22a4', '\x0'), // /top top 
			new Entity("topbot",               '\x2336', '\x0'), // top and bottom 
			new Entity("topcir",               '\x2af1', '\x0'), // top, circle below 
			// new Entity("topf",                 "%plane1D;565;"),  
			// new Entity("Topf",                 "%plane1D;54B;"), /Bbb T, open face T 
			new Entity("topfork",              '\x2ada', '\x0'), // fork with top 
			new Entity("tosa",                 '\x2929', '\x0'), // alias ISOAMSA seswar 
			new Entity("tprime",               '\x2034', '\x0'), // triple prime 
			new Entity("trade",                '\x2122', '\x0'), // trade mark sign, U+2122 ISOnum 
			new Entity("triangle",             '\x25b5', '\x0'), // alias ISOPUB utri 
			new Entity("triangledown",         '\x25bf', '\x0'), // alias ISOPUB dtri 
			new Entity("triangleleft",         '\x25c3', '\x0'), // alias ISOPUB ltri 
			new Entity("trianglelefteq",       '\x22b4', '\x0'), // alias ISOAMSR ltrie 
			new Entity("triangleq",            '\x225c', '\x0'), // alias ISOAMSR trie 
			new Entity("triangleright",        '\x25b9', '\x0'), // alias ISOPUB rtri 
			new Entity("trianglerighteq",      '\x22b5', '\x0'), // alias ISOAMSR rtrie 
			new Entity("tridot",               '\x25ec', '\x0'), // dot in triangle 
			new Entity("trie",                 '\x225c', '\x0'), // /triangleq R: triangle, equals 
			new Entity("triminus",             '\x2a3a', '\x0'), // minus in triangle 
			new Entity("TripleDot",            '\x20db', '\x0'), // alias ISOTECH tdot 
			new Entity("triplus",              '\x2a39', '\x0'), // plus in triangle 
			new Entity("trisb",                '\x29cd', '\x0'), // triangle, serifs at bottom 
			new Entity("tritime",              '\x2a3b', '\x0'), // multiply in triangle 
			new Entity("trpezium",             '\xfffd', '\x0'), // trapezium 
			// new Entity("tscr",                 "%plane1D;4C9;"), /scr t, script letter t 
			// new Entity("Tscr",                 "%plane1D;4AF;"), /scr T, script letter T 
			new Entity("tscy",                 '\x446', '\x0'), // =small tse, Cyrillic 
			new Entity("TScy",                 '\x426', '\x0'), // =capital TSE, Cyrillic 
			new Entity("tshcy",                '\x45b', '\x0'), // =small tshe, Serbian 
			new Entity("TSHcy",                '\x40b', '\x0'), // =capital TSHE, Serbian 
			new Entity("tstrok",               '\x167', '\x0'), // =small t, stroke 
			new Entity("Tstrok",               '\x166', '\x0'), // =capital T, stroke 
			new Entity("twixt",                '\x226c', '\x0'), // /between R: between 
			new Entity("twoheadleftarrow",     '\x219e', '\x0'), // alias ISOAMSA Larr 
			new Entity("twoheadrightarrow",    '\x21a0', '\x0'), // alias ISOAMSA Rarr 
			new Entity("uacute",               '\xfa', '\x0'), // latin small u with acute, U+00FA ISOlat1 
			new Entity("Uacute",               '\xda', '\x0'), // latin capital U with acute, U+00DA ISOlat1 
			new Entity("uarr",                 '\x2191', '\x0'), // upwards arrow, U+2191 ISOnum
			new Entity("uArr",                 '\x21d1', '\x0'), // upwards double arrow, U+21D1 ISOamsa 
			new Entity("Uarr",                 '\x219f', '\x0'), // up two-headed arrow 
			new Entity("Uarrocir",             '\x2949', '\x0'), // up two-headed arrow above circle 
			new Entity("ubrcy",                '\x45e', '\x0'), // =small u, Byelorussian 
			new Entity("Ubrcy",                '\x40e', '\x0'), // =capital U, Byelorussian 
			new Entity("ubreve",               '\x16d', '\x0'), // =small u, breve 
			new Entity("Ubreve",               '\x16c', '\x0'), // =capital U, breve 
			new Entity("ucirc",                '\xfb', '\x0'), // latin small u with circumflex, U+00FB ISOlat1 
			new Entity("Ucirc",                '\xdb', '\x0'), // latin capital U with circumflex, U+00DB ISOlat1 
			new Entity("ucy",                  '\x443', '\x0'), // =small u, Cyrillic 
			new Entity("Ucy",                  '\x423', '\x0'), // =capital U, Cyrillic 
			new Entity("udarr",                '\x21c5', '\x0'), // up arrow, down arrow 
			new Entity("udblac",               '\x171', '\x0'), // =small u, double acute accent 
			new Entity("Udblac",               '\x170', '\x0'), // =capital U, double acute accent 
			new Entity("udhar",                '\x296e', '\x0'), // up harp, down harp 
			new Entity("ufisht",               '\x297e', '\x0'), // up fish tail 
			// new Entity("ufr",                  "%plane1D;532;"), /frak u, lower case u 
			// new Entity("Ufr",                  "%plane1D;518;"), /frak U, upper case u 
			new Entity("ugrave",               '\xf9', '\x0'), // latin small u with grave, U+00F9 ISOlat1 
			new Entity("Ugrave",               '\xd9', '\x0'), // latin capital U with grave, U+00D9 ISOlat1 
			new Entity("uHar",                 '\x2963', '\x0'), // up harpoon-left, up harpoon-right 
			new Entity("uharl",                '\x21bf', '\x0'), // /upharpoonleft A: up harpoon-left 
			new Entity("uharr",                '\x21be', '\x0'), // /upharpoonright /restriction A: up harp-r 
			new Entity("uhblk",                '\x2580', '\x0'), // =upper half block 
			new Entity("ulcorn",               '\x231c', '\x0'), // /ulcorner O: upper left corner 
			new Entity("ulcorner",             '\x231c', '\x0'), // alias ISOAMSC ulcorn 
			new Entity("ulcrop",               '\x230f', '\x0'), // upward left crop mark  
			new Entity("ultri",                '\x25f8', '\x0'), // upper left triangle 
			new Entity("umacr",                '\x16b', '\x0'), // =small u, macron 
			new Entity("Umacr",                '\x16a', '\x0'), // =capital U, macron 
			new Entity("uml",                  '\xa8', '\x0'), // diaeresis = spacing diaeresis, U+00A8 ISOdia 
			new Entity("UnderBar",             '\x332', '\x0'), // combining low line 
			new Entity("UnderBrace",           '\xfe38', '\x0'), // under brace  
			new Entity("UnderBracket",         '\x23b5', '\x0'), // under bracket 
			new Entity("UnderParenthesis",     '\xfe36', '\x0'), // under parenthesis 
			new Entity("Union",                '\x22c3', '\x0'), // alias ISOAMSB xcup 
			new Entity("UnionPlus",            '\x228e', '\x0'), // alias ISOAMSB uplus 
			new Entity("uogon",                '\x173', '\x0'), // =small u, ogonek 
			new Entity("Uogon",                '\x172', '\x0'), // =capital U, ogonek 
			// new Entity("uopf",                 "%plane1D;566;"),  
			// new Entity("Uopf",                 "%plane1D;54C;"), /Bbb U, open face U 
			new Entity("uparrow",              '\x2191', '\x0'), // alias ISONUM uarr 
			new Entity("Uparrow",              '\x21d1', '\x0'), // alias ISOAMSA uArr 
			new Entity("UpArrow",              '\x2191', '\x0'), // alias ISONUM uarr 
			new Entity("UpArrowBar",           '\x2912', '\x0'), // up arrow to bar 
			new Entity("UpArrowDownArrow",     '\x21c5', '\x0'), // alias ISOAMSA udarr 
			new Entity("updownarrow",          '\x2195', '\x0'), // alias ISOAMSA varr 
			new Entity("Updownarrow",          '\x21d5', '\x0'), // alias ISOAMSA vArr 
			new Entity("UpDownArrow",          '\x2195', '\x0'), // alias ISOAMSA varr 
			new Entity("UpEquilibrium",        '\x296e', '\x0'), // alias ISOAMSA udhar 
			new Entity("upharpoonleft",        '\x21bf', '\x0'), // alias ISOAMSA uharl 
			new Entity("upharpoonright",       '\x21be', '\x0'), // alias ISOAMSA uharr 
			new Entity("uplus",                '\x228e', '\x0'), // /uplus B: plus sign in union 
			new Entity("UpperLeftArrow",       '\x2196', '\x0'), // alias ISOAMSA nwarr 
			new Entity("UpperRightArrow",      '\x2197', '\x0'), // alias ISOAMSA nearr 
			new Entity("upsi",                 '\x3c5',  '\x0'), // /upsilon small upsilon, Greek 
			new Entity("Upsi",                 '\x3d2',  '\x0'), // /Upsilon capital Upsilon, Greek 
			new Entity("upsih",                '\x3d2',  '\x0'), // greek upsilon with hook symbol, U+03D2 NEW 
			new Entity("upsilon",              '\x3c5',  '\x0'), // greek small letter upsilon, U+03C5 ISOgrk3 
			new Entity("Upsilon",              '\x3a5',  '\x0'), // 
			new Entity("UpTee",                '\x22a5', '\x0'), // alias ISOTECH perp 
			new Entity("UpTeeArrow",           '\x21a5', '\x0'), // Alias mapstoup 
			new Entity("upuparrows",           '\x21c8', '\x0'), // alias ISOAMSA uuarr 
			new Entity("urcorn",               '\x231d', '\x0'), // /urcorner C: upper right corner 
			new Entity("urcorner",             '\x231d', '\x0'), // alias ISOAMSC urcorn 
			new Entity("urcrop",               '\x230e', '\x0'), // upward right crop mark  
			new Entity("uring",                '\x16f',  '\x0'), // =small u, ring 
			new Entity("Uring",                '\x16e',  '\x0'), // =capital U, ring 
			new Entity("urtri",                '\x25f9', '\x0'), // upper right triangle 
			// new Entity("uscr",                 "%plane1D;4CA;"), /scr u, script letter u 
			// new Entity("Uscr",                 "%plane1D;4B0;"), /scr U, script letter U 
			new Entity("utdot",                '\x22f0', '\x0'), // three dots, ascending 
			new Entity("utilde",               '\x169',  '\x0'), // =small u, tilde 
			new Entity("Utilde",               '\x168',  '\x0'), // =capital U, tilde 
			new Entity("utri",                 '\x25b5', '\x0'), // /triangle =up triangle, open 
			new Entity("utrif",                '\x25b4', '\x0'), // /blacktriangle =up tri, filled 
			new Entity("uuarr",                '\x21c8', '\x0'), // /upuparrows A: two up arrows 
			new Entity("uuml",                 '\xfc',   '\x0'), // latin small u with diaeresis, U+00FC ISOlat1 
			new Entity("Uuml",                 '\xdc',   '\x0'), // latin capital U with diaeresis, U+00DC ISOlat1 
			new Entity("uwangle",              '\x29a7', '\x0'), // large upward pointing angle 
			new Entity("vangrt",               '\x299c', '\x0'), // right angle, variant 
			new Entity("varepsilon",           '\x3b5',  '\x0'), // alias ISOGRK3 epsiv 
			new Entity("varkappa",             '\x3f0',  '\x0'), // alias ISOGRK3 kappav 
			new Entity("varnothing",           '\x2205', '\x0'), // alias ISOAMSO emptyv 
			new Entity("varphi",               '\x3c6',  '\x0'), // alias ISOGRK3 phiv 
			new Entity("varpi",                '\x3d6',  '\x0'), // alias ISOGRK3 piv 
			new Entity("varpropto",            '\x221d', '\x0'), // alias ISOAMSR vprop 
			new Entity("varr",                 '\x2195', '\x0'), // /updownarrow A: up&down arrow 
			new Entity("vArr",                 '\x21d5', '\x0'), // /Updownarrow A: up&down dbl arrow 
			new Entity("varrho",               '\x3f1',  '\x0'), // alias ISOGRK3 rhov 
			new Entity("varsigma",             '\x3c2',  '\x0'), // alias ISOGRK3 sigmav 
			new Entity("varsubsetneq",         '\x228a', '\xfe00'), // alias ISOAMSN vsubne 
			new Entity("varsubsetneqq",        '\x2acb', '\xfe00'), // alias ISOAMSN vsubnE 
			new Entity("varsupsetneq",         '\x228b', '\xfe00'), // alias ISOAMSN vsupne 
			new Entity("varsupsetneqq",        '\x2acc', '\xfe00'), // alias ISOAMSN vsupnE 
			new Entity("vartheta",             '\x3d1',  '\x0'), // alias ISOGRK3 thetav 
			new Entity("vartriangleleft",      '\x22b2', '\x0'), // alias ISOAMSR vltri 
			new Entity("vartriangleright",     '\x22b3', '\x0'), // alias ISOAMSR vrtri 
			new Entity("vBar",                 '\x2ae8', '\x0'), // vert, dbl bar (under) 
			new Entity("Vbar",                 '\x2aeb', '\x0'), // dbl vert, bar (under) 
			new Entity("vBarv",                '\x2ae9', '\x0'), // dbl bar, vert over and under 
			new Entity("vcy",                  '\x432',  '\x0'), // =small ve, Cyrillic 
			new Entity("Vcy",                  '\x412',  '\x0'), // =capital VE, Cyrillic 
			new Entity("vdash",                '\x22a2', '\x0'), // /vdash R: vertical, dash 
			new Entity("vDash",                '\x22a8', '\x0'), // /vDash R: vertical, dbl dash 
			new Entity("Vdash",                '\x22a9', '\x0'), // /Vdash R: dbl vertical, dash 
			new Entity("VDash",                '\x22ab', '\x0'), // dbl vert, dbl dash 
			new Entity("Vdashl",               '\x2ae6', '\x0'), // vertical, dash (long) 
			new Entity("vee",                  '\x2228', '\x0'), // alias ISOTECH or 
			new Entity("Vee",                  '\x22c1', '\x0'), // alias ISOAMSB xvee 
			new Entity("veebar",               '\x22bb', '\x0'), // /veebar B: logical or, bar below 
			new Entity("veeeq",                '\x225a', '\x0'), // logical or, equals 
			new Entity("vellip",               '\x22ee', '\x0'), // vertical ellipsis 
			new Entity("verbar",               '\x7c',   '\x0'), // /vert =vertical bar 
			new Entity("Verbar",               '\x2016', '\x0'), // /Vert dbl vertical bar 
			new Entity("vert",                 '\x7c',   '\x0'), // alias ISONUM verbar 
			new Entity("Vert",                 '\x2016', '\x0'), // alias ISOTECH Verbar 
			new Entity("VerticalBar",          '\x2223', '\x0'), // alias ISOAMSR mid 
			new Entity("VerticalLine",         '\x7c',   '\x0'), // alias ISONUM verbar 
			new Entity("VerticalSeparator",    '\x2758', '\x0'), // vertical separating operator 
			new Entity("VerticalTilde",        '\x2240', '\x0'), // alias ISOAMSB wreath 
			new Entity("VeryThinSpace",        '\x200a', '\x0'), // space of width 1/18 em alias ISOPUB hairsp 
			// new Entity("vfr",                  "%plane1D;533;"), /frak v, lower case v 
			// new Entity("Vfr",                  "%plane1D;519;"), /frak V, upper case v 
			new Entity("vltri",                '\x22b2', '\x0'), // /vartriangleleft R: l tri, open, var 
			new Entity("vnsub",                '\x2282', '\x20d2'), // /nsubset N: not subset, var 
			new Entity("vnsup",                '\x2283', '\x20d2'), // /nsupset N: not superset, var 
			// new Entity("vopf",                 "%plane1D;567;"),  
			// new Entity("Vopf",                 "%plane1D;54D;"), /Bbb V, open face V 
			new Entity("vprop",                '\x221d', '\x0'), // /varpropto R: proportional, variant 
			new Entity("vrtri",                '\x22b3', '\x0'), // /vartriangleright R: r tri, open, var 
			// new Entity("vscr",                 "%plane1D;4CB;"), /scr v, script letter v 
			// new Entity("Vscr",                 "%plane1D;4B1;"), /scr V, script letter V 
			new Entity("vsubne",               '\x228a', '\xfe00'), // /varsubsetneq N: subset, not eq, var 
			new Entity("vsubnE",               '\x2acb', '\xfe00'), // /varsubsetneqq N: subset not dbl eq, var 
			new Entity("vsupne",               '\x228b', '\xfe00'), // /varsupsetneq N: superset, not eq, var 
			new Entity("vsupnE",               '\x2acc', '\xfe00'), // /varsupsetneqq N: super not dbl eq, var 
			new Entity("Vvdash",               '\x22aa', '\x0'), // /Vvdash R: triple vertical, dash 
			new Entity("vzigzag",              '\x299a', '\x0'), // vertical zig-zag line 
			new Entity("wcirc",                '\x175',  '\x0'), // =small w, circumflex accent 
			new Entity("Wcirc",                '\x174',  '\x0'), // =capital W, circumflex accent 
			new Entity("wedbar",               '\x2a5f', '\x0'), // wedge, bar below 
			new Entity("wedge",                '\x2227', '\x0'), // alias ISOTECH and 
			new Entity("Wedge",                '\x22c0', '\x0'), // alias ISOAMSB xwedge 
			new Entity("wedgeq",               '\x2259', '\x0'), // /wedgeq R: corresponds to (wedge, equals) 
			new Entity("weierp",               '\x2118', '\x0'), // script capital P = power set = Weierstrass p, U+2118 ISOamso 
			// new Entity("wfr",                  "%plane1D;534;"), /frak w, lower case w 
			// new Entity("Wfr",                  "%plane1D;51A;"), /frak W, upper case w 
			// new Entity("wopf",                 "%plane1D;568;"),  
			// new Entity("Wopf",                 "%plane1D;54E;"), /Bbb W, open face W 
			new Entity("wp",                   '\x2118', '\x0'), // alias ISOAMSO weierp 
			new Entity("wr",                   '\x2240', '\x0'), // alias ISOAMSB wreath 
			new Entity("wreath",               '\x2240', '\x0'), // /wr B: wreath product 
			// new Entity("wscr",                 "%plane1D;4CC;"), /scr w, script letter w 
			// new Entity("Wscr",                 "%plane1D;4B2;"), /scr W, script letter W 
			new Entity("xcap",                 '\x22c2', '\x0'), // /bigcap L: intersection operator 
			new Entity("xcirc",                '\x25ef', '\x0'), // /bigcirc B: large circle 
			new Entity("xcup",                 '\x22c3', '\x0'), // /bigcup L: union operator 
			new Entity("xdtri",                '\x25bd', '\x0'), // /bigtriangledown B: big dn tri, open 
			// new Entity("xfr",                  "%plane1D;535;"), /frak x, lower case x 
			// new Entity("Xfr",                  "%plane1D;51B;"), /frak X, upper case x 
			new Entity("xharr",                '\x27f7', '\x0'), // /longleftrightarrow A: long l&r arr 
			new Entity("xhArr",                '\x27fa', '\x0'), // /Longleftrightarrow A: long l&r dbl arr 
			new Entity("xi",                   '\x3be', '\x0'), // greek small letter xi, U+03BE ISOgrk3 
			new Entity("Xi",                   '\x39e', '\x0'), // greek capital letter xi, U+039E ISOgrk3 
			new Entity("xlarr",                '\x27f5', '\x0'), // /longleftarrow A: long left arrow 
			new Entity("xlArr",                '\x27f8', '\x0'), // /Longleftarrow A: long l dbl arrow 
			new Entity("xmap",                 '\x27fc', '\x0'), // /longmapsto A: 
			new Entity("xnis",                 '\x22fb', '\x0'), // large contains, vertical bar on horizontal stroke 
			new Entity("xodot",                '\x2a00', '\x0'), // /bigodot L: circle dot operator 
			// new Entity("xopf",                 "%plane1D;569;"),  
			// new Entity("Xopf",                 "%plane1D;54F;"), /Bbb X, open face X 
			new Entity("xoplus",               '\x2a01', '\x0'), // /bigoplus L: circle plus operator 
			new Entity("xotime",               '\x2a02', '\x0'), // /bigotimes L: circle times operator 
			new Entity("xrarr",                '\x27f6', '\x0'), // /longrightarrow A: long right arrow 
			new Entity("xrArr",                '\x27f9', '\x0'), // /Longrightarrow A: long rt dbl arr 
			// new Entity("xscr",                 "%plane1D;4CD;"), /scr x, script letter x 
			// new Entity("Xscr",                 "%plane1D;4B3;"), /scr X, script letter X 
			new Entity("xsqcup",               '\x2a06', '\x0'), // /bigsqcup L: square union operator 
			new Entity("xuplus",               '\x2a04', '\x0'), // /biguplus L: 
			new Entity("xutri",                '\x25b3', '\x0'), // /bigtriangleup B: big up tri, open 
			new Entity("xvee",                 '\x22c1', '\x0'), // /bigvee L: logical and operator 
			new Entity("xwedge",               '\x22c0', '\x0'), // /bigwedge L: logical or operator 
			new Entity("yacute",               '\xfd', '\x0'), // latin small y with acute, U+00FD ISOlat1 
			new Entity("Yacute",               '\xdd', '\x0'), // latin capital Y with acute, U+00DD ISOlat1 
			new Entity("yacy",                 '\x44f', '\x0'), // =small ya, Cyrillic 
			new Entity("YAcy",                 '\x42f', '\x0'), // =capital YA, Cyrillic 
			new Entity("ycirc",                '\x177', '\x0'), // =small y, circumflex accent 
			new Entity("Ycirc",                '\x176', '\x0'), // =capital Y, circumflex accent 
			new Entity("ycy",                  '\x44b', '\x0'), // =small yeru, Cyrillic 
			new Entity("Ycy",                  '\x42b', '\x0'), // =capital YERU, Cyrillic 
			new Entity("yen",                  '\xa5', '\x0'), // yen sign = yuan sign, U+00A5 ISOnum 
			// new Entity("yfr",                  "%plane1D;536;"), /frak y, lower case y 
			// new Entity("Yfr",                  "%plane1D;51C;"), /frak Y, upper case y 
			new Entity("yicy",                 '\x457', '\x0'), // =small yi, Ukrainian 
			new Entity("YIcy",                 '\x407', '\x0'), // =capital YI, Ukrainian 
			// new Entity("yopf",                 "%plane1D;56A;"),  
			// new Entity("Yopf",                 "%plane1D;550;"), /Bbb Y, open face Y 
			// new Entity("yscr",                 "%plane1D;4CE;"), /scr y, script letter y 
			// new Entity("Yscr",                 "%plane1D;4B4;"), /scr Y, script letter Y 
			new Entity("yucy",                 '\x44e', '\x0'), // =small yu, Cyrillic 
			new Entity("YUcy",                 '\x42e', '\x0'), // =capital YU, Cyrillic 
			new Entity("yuml",                 '\xff', '\x0'), // latin small y with diaeresis, U+00FF ISOlat1 
			new Entity("Yuml",                 '\x178', '\x0'), // latin capital letter Y with diaeresis, U+0178 ISOlat2 
			new Entity("zacute",               '\x17a', '\x0'), // =small z, acute accent 
			new Entity("Zacute",               '\x179', '\x0'), // =capital Z, acute accent 
			new Entity("zcaron",               '\x17e', '\x0'), // =small z, caron 
			new Entity("Zcaron",               '\x17d', '\x0'), // =capital Z, caron 
			new Entity("zcy",                  '\x437', '\x0'), // =small ze, Cyrillic 
			new Entity("Zcy",                  '\x417', '\x0'), // =capital ZE, Cyrillic 
			new Entity("zdot",                 '\x17c', '\x0'), // =small z, dot above 
			new Entity("Zdot",                 '\x17b', '\x0'), // =capital Z, dot above 
			new Entity("zeetrf",               '\x2128', '\x0'), // zee transform 
			new Entity("ZeroWidthSpace",       '\x200b', '\x0'), // zero width space 
			new Entity("zeta",                 '\x3b6', '\x0'), // greek small letter zeta, U+03B6 ISOgrk3 
			new Entity("Zeta",                 '\x396', '\x0'), // greek capital letter zeta, U+0396 
			// new Entity("zfr",                  "%plane1D;537;"), /frak z, lower case z 
			new Entity("Zfr",                  '\x2128', '\x0'), // /frak Z, upper case z  
			new Entity("zhcy",                 '\x436', '\x0'), // =small zhe, Cyrillic 
			new Entity("ZHcy",                 '\x416', '\x0'), // =capital ZHE, Cyrillic 
			new Entity("zigrarr",              '\x21dd', '\x0'), // right zig-zag arrow 
			// new Entity("zopf",                 "%plane1D;56B;"),  
			new Entity("Zopf",                 '\x2124', '\x0'), // /Bbb Z, open face Z 
			// new Entity("zscr",                 "%plane1D;4CF;"), /scr z, script letter z 
			// new Entity("Zscr",                 "%plane1D;4B5;"), /scr Z, script letter Z 
			new Entity("zwj",                  '\x200d', '\x0'), // zero width joiner, U+200D NEW RFC 2070 
			new Entity("zwnj",                 '\x200c', '\x0') // zero width non-joiner, U+200C NEW RFC 2070 
		};

		/// <summary>
		/// get a value for an enttity name. If there is no matching
		/// entry, return a empty string 
		/// 
		/// TODO the list is sorted, do a binary search
		/// </summary>
		public static string GetValue(string name)
		{
			for(int i=0; i<entities.Length; i++)
			{
				if(name == entities[i].Name) 
				{
					return entities[i].Value;
				}
			}
			return "";
		}
	}
}
