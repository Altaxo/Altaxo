<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
<xsl:include href="common.xslt"/>

<xsl:param name='id'/>
<xsl:template match="/">
  	<xsl:apply-templates select="DOC.NET/assembly/module/namespace/*/*[@id=$id]"/>
</xsl:template>

<xsl:template match="method | constructor | property">
	<xsl:variable name="type">
		<xsl:choose>
			<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
			<xsl:otherwise>Class</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="childType">
		<xsl:choose>
			<xsl:when test="local-name()='method'">Method</xsl:when>
			<xsl:when test="local-name()='constructor'">Constructor</xsl:when>
			<xsl:otherwise>Property</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="memberName" select="@name"/>

<html dir="LTR">
<head>
<title>
	<xsl:choose>
		<xsl:when test="local-name()!='constructor'"><xsl:value-of select="@name"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="../@name"/></xsl:otherwise>
	</xsl:choose>&#160;<xsl:value-of select="childType"/>
</title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="../@name"/><xsl:if test="local-name()!='constructor'">.<xsl:value-of select="@name"/></xsl:if>&#160;<xsl:value-of select="childType"/></h1>

<xsl:call-template name="summary-section"/>

<h3>Overload List</h3>
<xsl:for-each select="parent::node()/*[@name=$memberName]">
	<p class="i1"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></p>
	<p class="i2">
		<a>
			<xsl:attribute name="href">
				<xsl:choose>
					<xsl:when test="local-name()='constructor'">
						<xsl:call-template name="get-filename-for-current-constructor"/>
					</xsl:when>
					<xsl:when test="local-name()='method'">
						<xsl:call-template name="get-filename-for-method"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="get-filename-for-current-property"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:apply-templates select="self::node()" mode="syntax"/>
		</a>
	</p>
</xsl:for-each>

<xsl:call-template name="seealso-section">
    <xsl:with-param name="page">memberoverload</xsl:with-param>
</xsl:call-template>

</body>
</html>

</xsl:template>

<xsl:template match="constructor | method" mode="syntax">
	<xsl:call-template name="member-syntax2"/>
</xsl:template>

<xsl:template match="property" mode="syntax">
	<xsl:call-template name="property-syntax">
		<xsl:with-param name="indent" select="false()"/>
		<xsl:with-param name="display-names" select="false()"/>
	</xsl:call-template>
</xsl:template>

</xsl:stylesheet>
