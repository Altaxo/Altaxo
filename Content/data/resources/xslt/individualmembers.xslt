<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
<xsl:include href="common.xslt"/>
<xsl:include href="memberscommon.xslt"/>

<xsl:param name='id'/>
<xsl:param name='member'/>

<xsl:template name="type-members">
	<xsl:param name="type"/>
	<xsl:variable name="Members">
		<xsl:call-template name="get-big-member-plural">
			<xsl:with-param name="member" select="$member"/>
		</xsl:call-template>												
	</xsl:variable>
	<xsl:variable name="members">
		<xsl:call-template name="get-small-member-plural">
			<xsl:with-param name="member" select="$member"/>
		</xsl:call-template>												
	</xsl:variable>

<html dir="LTR">
<head>
	<title><xsl:value-of select="@name"/>&#160;<xsl:value-of select="$Members"/></title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="@name"/>&#160;<xsl:value-of select="$Members"/></h1>

<p class="i1">
	<xsl:text>The </xsl:text>
	<xsl:value-of select="$members"/>
	<xsl:text> of the </xsl:text>
	<b><xsl:value-of select="@name"/></b>
	<xsl:text> class are listed below. For a complete list of </xsl:text>
	<b><xsl:value-of select="@name"/></b>
	<xsl:text> class members, see the </xsl:text>
	<a>
		<xsl:attribute name="href">
			<xsl:call-template name="get-filename-for-type-members">
				<xsl:with-param name="id" select="@id"/>
			</xsl:call-template>
		</xsl:attribute>
		<xsl:value-of select="@name"/>
		<xsl:text> Members</xsl:text>
	</a>
	<xsl:text> topic.</xsl:text>
</p>

<!-- static members -->

<xsl:call-template name="public-static-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="protected-static-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-static-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="internal-static-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="private-static-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<!-- instance members -->

<xsl:call-template name="public-instance-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="protected-instance-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-instance-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="internal-instance-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="private-instance-section">
	<xsl:with-param name="member" select="$member"/>
</xsl:call-template>												

<xsl:call-template name="seealso-section">
    <xsl:with-param name="page"><xsl:value-of select="$members"/></xsl:with-param>
</xsl:call-template>

</body>
</html>
</xsl:template>

</xsl:stylesheet>
