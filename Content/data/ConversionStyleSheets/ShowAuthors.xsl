<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="AuthorData">
		<HTML><HEAD></HEAD><BODY>
			
			<xsl:for-each select="Authors/Category">
				<H1><xsl:value-of select="@name"/></H1>
				<TABLE>
					<xsl:for-each select="Author">
						<TR>
							<TD align="left" class="copy"><xsl:value-of select="@name"/><BR/>(<xsl:value-of select="@mail"/>)</TD>
							<TD align="left" class="copy"><xsl:value-of select="@achievements"/></TD>
						</TR>
					</xsl:for-each>
				</TABLE>
			</xsl:for-each>
			
			<H1>Included Projects:</H1>
			<TABLE>
				<xsl:for-each select="IncludedProjects/Project">
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@name"/></TD>
						<TD align="left" class="copy"><xsl:value-of select="@url"/></TD>
					</TR>
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@description"/></TD>
					</TR>
				</xsl:for-each>
			</TABLE>
			
			<H1>Greetings fly out to:</H1>
			<TABLE>
				<xsl:for-each select="Greetings/Person">
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@name"/>(<xsl:value-of select="@mail"/>)</TD>
					</TR>
					<TR>
						<TD align="left" class="copy"><xsl:value-of select="@text"/></TD>
					</TR>
				</xsl:for-each>
			</TABLE>
		</BODY></HTML>
	</xsl:template>
</xsl:transform>
