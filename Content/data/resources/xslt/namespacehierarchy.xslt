<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-16" indent="yes"/>
<xsl:include href="common.xslt"/>

<xsl:param name='namespace'/>

<xsl:template match="/">
	<xsl:apply-templates select="DOC.NET/assembly/module/namespace[@name=$namespace]"/>
</xsl:template>

<xsl:template match="namespace">

<html dir="LTR">
<head>
<title><xsl:value-of select="@name"/> Hierarchy</title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>

<h1><xsl:value-of select="@name"/> Hierarchy</h1>

<br/>

<a href="ms-help://MSDNVS/cpref/html_hh2/frlrfSystemObjectClassTopic.htm">System.Object</a>
<br/>
<xsl:apply-templates select=".//*[(local-name()='class' and not(base)) or (local-name()='base' and not(base))]">
	<xsl:sort select="@name"/>
</xsl:apply-templates>
<br/>

<xsl:if test="interface">
	<h3>Interfaces</h3>
	<p>
	  	<xsl:apply-templates select="interface">
	  		<xsl:sort select="@name"/>
	  	</xsl:apply-templates>
	</p>
</xsl:if>

</body>
</html>
</xsl:template>

<xsl:template match="class">
	<xsl:variable name="class-id" select="class/@id"/>
	<xsl:if test="not(..//base[@id=$class-id])">
		<xsl:call-template name="draw-hierarchy">
			<xsl:with-param name="current" select="."/>
			<xsl:with-param name="level" select="1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template match="base">
	<xsl:call-template name="draw-hierarchy">
		<xsl:with-param name="current" select="."/>
		<xsl:with-param name="level" select="1"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="interface">
	<a>
		<xsl:attribute name="href">
			<xsl:call-template name="get-filename-for-type">
				<xsl:with-param name="id" select="@id"/>
			</xsl:call-template>
		</xsl:attribute>
		<xsl:value-of select="@name"/>
	</a>
	<br/>
</xsl:template>

<xsl:template name="indent">
	<xsl:param name="count"/>
	<xsl:if test="$count &gt; 0">
		<xsl:text>&#160;&#160;</xsl:text>
		<xsl:call-template name="indent">
			<xsl:with-param name="count" select="$count - 1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template name="draw-hierarchy">
	<xsl:param name="current"/>
	<xsl:param name="level"/>
	<xsl:if test="$current[local-name()!='namespace']">
		<xsl:call-template name="indent">
			<xsl:with-param name="count" select="$level"/>
		</xsl:call-template>
		<xsl:text>-</xsl:text>
		<a>
			<xsl:attribute name="href">
				<xsl:choose>
					<xsl:when test="starts-with($current/@id, 'T:System.')">
						<xsl:call-template name="get-filename-for-system-class">
							<xsl:with-param name="class-name" select="substring-after($current/@id, 'T:')"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="$current/@id"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>		
			</xsl:attribute>
			<xsl:call-template name="get-datatype">
				<xsl:with-param name="datatype" select="substring-after($current/@id, 'T:')"/>
				<xsl:with-param name="namespace-name" select="$namespace"/>
			</xsl:call-template>
		</a>
		<br/>
<!-- bug in MSXML that won't allow this recursion to go any deeper.  saxon works ok with it. -->		
<xsl:if test="$level != 7">
		<xsl:call-template name="draw-hierarchy">
			<xsl:with-param name="current" select="$current/.."/>
			<xsl:with-param name="level" select="$level + 1"/>
		</xsl:call-template>
</xsl:if>		
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
  