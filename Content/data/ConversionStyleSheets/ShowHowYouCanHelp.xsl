<?xml version="1.0" encoding="ISO-8859-1"?>

<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="HelpWanted">
		<HTML><HEAD></HEAD><BODY>
			<TABLE CLASS="dtTABLE" cellspacing="0">
				<TR>
					<TH align="left" class="copy">Topic</TH>
					<TH align="left" class="copy">Issue</TH>
				</TR>
			
				<xsl:for-each select="Topic">
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@name"/></TD>
						<TD align="left" class="copy"><xsl:value-of select="."/></TD>
					</TR>
				</xsl:for-each>
			</TABLE>

		</BODY></HTML>
	</xsl:template>
</xsl:transform>
