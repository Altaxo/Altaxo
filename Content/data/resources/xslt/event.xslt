<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
<xsl:include href="common.xslt"/>

<xsl:param name='id'/>

<xsl:template match="/">
  	<xsl:apply-templates select="DOC.NET/assembly/module/namespace/*/event[@id=$id]"/>
</xsl:template>

<xsl:template match="event">

<html dir="LTR">
<head>
<title>
	<xsl:value-of select="../@name"/>.<xsl:value-of select="@name"/>
	<xsl:text> Event</xsl:text>
</title>
</head>

<link rel="stylesheet" type="text/css" href="MsdnHelp.css"/>

<body>
<h1><xsl:value-of select="../@name"/>.<xsl:value-of select="@name"/> Event</h1>

<xsl:call-template name="summary-section"/>
<xsl:call-template name="field-or-event-syntax"/>
<p/>

<xsl:variable name="type" select="@type"/>
<xsl:variable name="eventargs-id" select="concat('T:', //delegate[@id=concat('T:', $type)]/parameter[contains(@type, 'EventArgs')][1]/@type)"/>
<xsl:if test="//class[@id=$eventargs-id]/property[@access='Public' and not(@static)]">
	<h3>Event Data</h3>
	<p class="i1">
		<xsl:text>The event handler receives a </xsl:text>
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="get-filename-for-type-members">
					<xsl:with-param name="id" select="$eventargs-id"/>
				</xsl:call-template>
			</xsl:attribute>
			<xsl:value-of select="//class[@id=$eventargs-id]/@name"/> 
		</a>
		<xsl:text> containing data related to the </xsl:text>
		<B><xsl:value-of select="@name"/></B>
		<xsl:text> event. The following </xsl:text>
		<B><xsl:value-of select="//class[@id=$eventargs-id]/@name"/></B>
		<xsl:text> properties provide information specific to this event.</xsl:text>
	</p>
	<table cellspacing="0">
		<tr valign="top"><th width="50%">Property</th><th width="50%">Description</th></tr>
	  	<xsl:apply-templates select="//class[@id=$eventargs-id]/property[@access='Public' and not(@static)]">
	  		<xsl:sort select="@name"/>
	  	</xsl:apply-templates>
	</table>
</xsl:if>


<xsl:call-template name="remarks-section"/>
<xsl:call-template name="example-section"/>
<xsl:call-template name="seealso-section">
    <xsl:with-param name="page">event</xsl:with-param>
</xsl:call-template>

<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="viewastext">
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='@name'/> event</xsl:attribute>
	</xsl:element>
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='@name'/> event, <xsl:value-of select='../@name'/> class</xsl:attribute>
	</xsl:element>
	<xsl:element name="param">
		<xsl:attribute name="name">Keyword</xsl:attribute>
		<xsl:attribute name="value"><xsl:value-of select='../@name'/>.<xsl:value-of select='@name'/> event</xsl:attribute>
	</xsl:element>
</object>

</body>
</html>

</xsl:template>

<xsl:template match="property">
	<xsl:variable name="name" select="@name"/>
	<xsl:if test="not(preceding-sibling::property[@name=$name])">
		<tr VALIGN="top">
			<xsl:choose>
				<xsl:when test="following-sibling::property[@name=$name]">
					<td width="50%">
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-property-overloads"/>
							</xsl:attribute>
							<xsl:value-of select="@name"/>
						</a>
					</td>
					<td width="50%">Overloaded. <xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
				</xsl:when>
				<xsl:otherwise>
					<td width="50%">
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-property"/>
							</xsl:attribute>
							<xsl:value-of select="@name"/>
						</a>
					</td>
					<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
				</xsl:otherwise>
			</xsl:choose>
		</tr>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
