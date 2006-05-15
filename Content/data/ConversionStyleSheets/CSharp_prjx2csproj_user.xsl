<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:Conversion="urn:Conversion">
	<xsl:output method = "xml" indent = "yes" />
	
	<xsl:template match = "/" >
		<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
		
			<!-- Configurations -->
			<xsl:for-each select="/Project/Configurations/Configuration">
				<xsl:element name = "PropertyGroup" >
					<xsl:attribute name = "Condition"> '$(Configuration)|$(Platform)' == '<xsl:value-of select = "@name" />|AnyCPU' </xsl:attribute>
					
					<xsl:element name = "StartProgram"><xsl:value-of select = "Output/@executeScript" /></xsl:element>
					<xsl:element name = "StartArguments"><xsl:value-of select = "Execution/@commandlineparameters" /></xsl:element>
				
				</xsl:element>
			</xsl:for-each>
			
		</Project>
	</xsl:template>
</xsl:stylesheet>
