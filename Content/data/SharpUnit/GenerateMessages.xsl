<?xml version="1.0" encoding="ISO-8859-1"?>
     
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="TestResults">
		<BODY>
			<DIV CLASS="LIGHT">
				<P>Starting tests.</P>
			</DIV>
			<TABLE>
				<xsl:for-each select="TestAssembly">
					<xsl:for-each select="TestSuite">
						<xsl:for-each select="TestCase">
							<TR><TD>
								<DIV CLASS="LIGHT">
									TestCase <xsl:value-of select="@name"/>:
								</DIV>
							</TD></TR>
							<xsl:for-each select="Message">
								<TR><TD><xsl:value-of select="."/></TD></TR>
							</xsl:for-each>
						</xsl:for-each>
					</xsl:for-each>
				</xsl:for-each>
			</TABLE>
			<DIV CLASS="LIGHT">
				<P>Tests completed.</P>
			</DIV>
		</BODY>
	</xsl:template>
</xsl:transform>
