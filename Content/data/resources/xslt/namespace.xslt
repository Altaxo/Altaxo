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
<title><xsl:value-of select="@name"/></title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="@name"/> Namespace</h1>

<p class="i1">
	<a>
		<xsl:attribute name="href">
			<xsl:call-template name="get-filename-for-current-namespace-hierarchy"/>
		</xsl:attribute>
		<xsl:text>Hierarchy diagram</xsl:text>
	</a>
</p>

<xsl:call-template name="summary-section"/>

<xsl:if test="class">
<h3>Classes</h3>
<table cellspacing="0">
	<tr valign="top"><th width="50%">Class</th><th width="50%">Description</th></tr>
	<xsl:for-each select="class">
		<xsl:sort select="@name"/>
		<tr valign="top">
			<td width="50%">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="@id"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
		</tr>
	</xsl:for-each>
</table>
</xsl:if>

<xsl:if test="interface">
<h3>Interfaces</h3>
<table cellspacing="0">
	<tr valign="top"><th width="50%">Interface</th>	<th width="50%">Description</th></tr>
	<xsl:for-each select="interface">
		<xsl:sort select="@name"/>
		<tr valign="top">
			<td width="50%">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="@id"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
		</tr>
	</xsl:for-each>
</table>
</xsl:if>

<xsl:if test="structure">
<h3>Structures</h3>
<table cellspacing="0">
	<tr valign="top"><th width="50%">Structure</th><th width="50%">Description</th></tr>
	<xsl:for-each select="structure">
		<xsl:sort select="@name"/>
		<tr valign="top">
			<td width="50%">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="@id"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
		</tr>
	</xsl:for-each>
</table>
</xsl:if>

<xsl:if test="delegate">
<h3>Delegates</h3>
<table cellspacing="0">
	<tr valign="top"><th width="50%">Delegate</th><th width="50%">Description</th></tr>
	<xsl:for-each select="delegate">
		<xsl:sort select="@name"/>
		<tr valign="top">
			<td width="50%">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="@id"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
		</tr>
	</xsl:for-each>
</table>
</xsl:if>

<xsl:if test="enumeration">
<h3>Enumerations</h3>
<table cellspacing="0">
	<tr valign="top"><th width="50%">Enumeration</th><th width="50%">Description</th></tr>
	<xsl:for-each select="enumeration">
		<xsl:sort select="@name"/>
		<tr valign="top">
			<td width="50%">
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="get-filename-for-type">
							<xsl:with-param name="id" select="@id"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
		</tr>
	</xsl:for-each>
</table>
</xsl:if>

</body>
</html>
</xsl:template>

</xsl:stylesheet>
