<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
<xsl:include href="common.xslt"/>

<xsl:param name='id'/>

<xsl:template match="/">
  	<xsl:apply-templates select="DOC.NET/assembly/module/namespace/*/field[@id=$id]"/>
</xsl:template>

<xsl:template match="field">

<html dir="LTR">
<head>
<title>
	<xsl:value-of select="../@name"/>.<xsl:value-of select="@name"/>
	<xsl:text> Field</xsl:text>
</title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="../@name"/>.<xsl:value-of select="@name"/> Field</h1>

<xsl:call-template name="summary-section"/>
<xsl:call-template name="field-or-event-syntax"/>
<p/>
<xsl:call-template name="remarks-section"/>
<xsl:call-template name="example-section"/>
<xsl:call-template name="seealso-section">
    <xsl:with-param name="page">field</xsl:with-param>
</xsl:call-template>

<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="viewastext">
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='@name'/> field</xsl:attribute>
	</xsl:element>
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='@name'/> field, <xsl:value-of select='../@name'/> class</xsl:attribute>
	</xsl:element>
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='../@name'/>.<xsl:value-of select='@name'/> field</xsl:attribute>
	</xsl:element>
</object>

</body>
</html>

</xsl:template>

</xsl:stylesheet>
