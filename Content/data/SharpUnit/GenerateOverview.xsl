<?xml version="1.0" encoding="ISO-8859-1"?>
     
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="TestResults">
		<BODY>
			<H1>Project Overview</H1>
			<TABLE>
				<TR>
					<TD>Start time:</TD>
					<TD><xsl:value-of select="@starttime"/></TD>
				</TR>
				<TR>
					<TD>Milliseconds:</TD>
					<TD><xsl:value-of select="@milliseconds"/></TD>
				</TR>
				<TR>
					<TD>Tests succeeded:</TD>
					<TD><xsl:value-of select="@testssucceeded"/>(<xsl:value-of select="format-number(number(@testssucceeded) * 100 div (number(@testssucceeded) + number(@testsfailed)),'#.#')"/>%)</TD>
				</TR>
				<TR>
					<TD>Tests failed:</TD>
					<TD><xsl:value-of select="@testsfailed"/>(<xsl:value-of select="format-number(number(@testsfailed) * 100 div (number(@testssucceeded) + number(@testsfailed)),'#.#')"/>%)</TD>
				</TR>
			</TABLE>
		</BODY>
	</xsl:template>
</xsl:transform>