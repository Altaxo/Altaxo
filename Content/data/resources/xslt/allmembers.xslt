<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
<xsl:include href="common.xslt"/>
<xsl:include href="memberscommon.xslt"/>

<xsl:param name='id'/>

<xsl:template name="type-members">
	<xsl:param name="type"/>

<html dir="LTR">
<head>
	<title><xsl:value-of select="@name"/> Members</title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="@name"/> Members</h1>

<!-- public static members -->
<xsl:call-template name="public-static-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="public-static-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="public-static-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- protected static members -->
<xsl:call-template name="protected-static-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="protected-static-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="protected-static-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- protected internal static members -->
<xsl:call-template name="protected-internal-static-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-static-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-static-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- internal static members -->
<xsl:call-template name="internal-static-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="internal-static-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="internal-static-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- private static members -->
<xsl:call-template name="private-static-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="private-static-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="private-static-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- public instance members -->
<xsl:if test="constructor[@access='Public']">
<h3>Public Instance Constructors</h3>
<table cellspacing="0">
  	<xsl:apply-templates select="constructor[@access='Public']"/>
</table>
</xsl:if>

<xsl:call-template name="public-instance-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="public-instance-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="public-instance-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- protected instance members -->
<xsl:if test="constructor[@access='Family']">
<h3>Protected Instance Constructors</h3>
<table cellspacing="0">
  	<xsl:apply-templates select="constructor[@access='Family']"/>
</table>
</xsl:if>

<xsl:call-template name="protected-instance-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="protected-instance-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="protected-instance-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- protected internal instance members -->
<xsl:if test="constructor[@access='FamilyOrAssembly']">
<h3>Protected Internal Instance Constructors</h3>
<table cellspacing="0">
  	<xsl:apply-templates select="constructor[@access='FamilyOrAssembly']"/>
</table>
</xsl:if>

<xsl:call-template name="protected-internal-instance-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-instance-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="protected-internal-instance-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- internal instance members -->
<xsl:if test="constructor[@access='Assembly']">
<h3>Internal Instance Constructors</h3>
<table cellspacing="0">
  	<xsl:apply-templates select="constructor[@access='Assembly']"/>
</table>
</xsl:if>

<xsl:call-template name="internal-instance-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="internal-instance-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="internal-instance-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<!-- private instance members -->
<xsl:if test="constructor[@access='Private']">
<h3>Private Instance Constructors</h3>
<table cellspacing="0">
  	<xsl:apply-templates select="constructor[@access='Private']"/>
</table>
</xsl:if>

<xsl:call-template name="private-instance-section">
	<xsl:with-param name="member" select="'field'"/>
</xsl:call-template>												

<xsl:call-template name="private-instance-section">
	<xsl:with-param name="member" select="'property'"/>
</xsl:call-template>												

<xsl:call-template name="private-instance-section">
	<xsl:with-param name="member" select="'method'"/>
</xsl:call-template>												

<xsl:call-template name="seealso-section">
    <xsl:with-param name="page">members</xsl:with-param>
</xsl:call-template>

</body>
</html>

</xsl:template>

<xsl:template match="constructor">
	<xsl:variable name="access" select="@access"/>
	<xsl:if test="not(preceding-sibling::constructor[@access=$access])">
		<tr VALIGN="top">
			<xsl:choose>
				<xsl:when test="count(../constructor) &gt; 1">
					<td width="50%">
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-constructor-overloads"/>
							</xsl:attribute>
							<xsl:value-of select="../@name"/>
						</a>
					</td>
					<td width="50%">Overloaded. Initialize a new instance of the <xsl:value-of select="../@name"/> class.</td>
				</xsl:when>
				<xsl:otherwise>
					<td width="50%">
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-constructor"/>
							</xsl:attribute>
							<xsl:value-of select="../@name"/>
							<xsl:text> Constructor</xsl:text>
						</a>
					</td>
					<td width="50%">
						<xsl:apply-templates select="summary/node()" mode="slashdoc"/>
					</td>
				</xsl:otherwise>
			</xsl:choose>
		</tr>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
