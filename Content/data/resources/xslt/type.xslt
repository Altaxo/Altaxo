<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-16" indent="yes" />
	<xsl:include href="common.xslt" />
	<xsl:param name='id' />
	<xsl:template match="/">
		<xsl:apply-templates select="DOC.NET/assembly/module/namespace/*[@id=$id]" />
	</xsl:template>
	<xsl:template name="indent">
		<xsl:param name="count" />
		<xsl:if test="$count &gt; 0">
			<xsl:text>&#160;&#160;&#160;</xsl:text>
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$count - 1" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<xsl:template name="draw-hierarchy">
		<xsl:param name="list" />
		<xsl:param name="level" />
		<!-- this is commented out because XslTransform is throwing an InvalidCastException in it. -->
		<!--<xsl:if test="count($list) &gt; 0">
			<xsl:call-template name="indent">
				<xsl:with-param name="count" select="$level" />
			</xsl:call-template>
			<a>
				<xsl:attribute name="href">
					<xsl:choose>
						<xsl:when test="starts-with($list[position() = last()]/@type, 'System.')">
							<xsl:call-template name="get-filename-for-system-class">
								<xsl:with-param name="class-name" select="$list[position() = last()]/@type" />
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="get-filename-for-type">
								<xsl:with-param name="id" select="$list[position() = last()]/@id" />
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:call-template name="get-datatype">
					<xsl:with-param name="datatype" select="$list[position() = last()]/@type" />
					<xsl:with-param name="namespace-name" select="../@name" />
				</xsl:call-template>
			</a>
			<br />
			<xsl:call-template name="draw-hierarchy">
				<xsl:with-param name="list" select="$list[position() != last()]" />
				<xsl:with-param name="level" select="$level + 1" />
			</xsl:call-template>
		</xsl:if>-->
	</xsl:template>
	<xsl:template match="class">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Class</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="interface">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Interface</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="structure">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Structure</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="delegate">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Delegate</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="enumeration">
		<xsl:call-template name="type">
			<xsl:with-param name="type">Enumeration</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template name="type">
		<xsl:param name="type" />
		<html dir="LTR">
			<head>
				<title>
					<xsl:value-of select="concat(@name, ' ', $type)" />
				</title>
			</head>
			<link rel="stylesheet" type="text/css" href="MsdnHelp.css" />
			<body>
				<h1>
					<xsl:value-of select="concat(@name, ' ', $type)" />
				</h1>
				<xsl:call-template name="summary-section" />
				<xsl:if test="local-name() != 'delegate' and local-name() != 'enumeration'">
					<p class="i1">
						<xsl:choose>
							<xsl:when test="self::interface">
								<xsl:if test="base">
									<xsl:call-template name="draw-hierarchy">
										<xsl:with-param name="list" select="descendant::base" />
										<xsl:with-param name="level" select="0" />
									</xsl:call-template>
									<xsl:call-template name="indent">
										<xsl:with-param name="count" select="count(descendant::base)" />
									</xsl:call-template>
									<b>
										<xsl:value-of select="@name" />
									</b>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<a href="ms-help://MSDNVS/cpref/html_hh2/frlrfSystemObjectClassTopic.htm">System.Object</a>
								<br />
								<xsl:call-template name="draw-hierarchy">
									<xsl:with-param name="list" select="descendant::base" />
									<xsl:with-param name="level" select="1" />
								</xsl:call-template>
								<xsl:call-template name="indent">
									<xsl:with-param name="count" select="count(descendant::base) + 1" />
								</xsl:call-template>
								<b>
									<xsl:value-of select="@name" />
								</b>
							</xsl:otherwise>
						</xsl:choose>
					</p>
				</xsl:if>
				<xsl:call-template name="type-syntax" />
				<xsl:if test="local-name() = 'delegate'">
					<xsl:call-template name="parameter-section" />
				</xsl:if>
				<xsl:call-template name="remarks-section" />
				<xsl:call-template name="example-section" />
				<xsl:if test="local-name() = 'enumeration'">
					<xsl:call-template name="members-section" />
				</xsl:if>
				<h3>Requirements</h3>
				<p class="i1">
					<b>Namespace: </b>
					<a>
						<xsl:attribute name="href">
							<xsl:call-template name="get-filename-for-namespace">
								<xsl:with-param name="name" select="../@name" />
							</xsl:call-template>
						</xsl:attribute>
						<xsl:value-of select="../@name" />
						<xsl:text> Namespace</xsl:text>
					</a>
				</p>
				<p class="i1">
					<b>Assembly: </b>
					<xsl:value-of select="../../@name" />
				</p>
				<xsl:variable name="page">
					<xsl:choose>
						<xsl:when test="local-name() = 'enumeration'">enumeration</xsl:when>
						<xsl:when test="local-name() = 'delegate'">delegate</xsl:when>
						<xsl:otherwise>type</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:call-template name="seealso-section">
					<xsl:with-param name="page" select="$page" />
				</xsl:call-template>
				<xsl:if test="local-name() = 'enumeration'">
					<object type="application/x-oleobject" classid="clsid:1e2a7bd0-dab9-11d0-b93a-00c04fc99f9e" viewastext="viewastext">
						<xsl:element name="param">
							<xsl:attribute name="name">Keyword</xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select='@name' /> enumeration</xsl:attribute>
						</xsl:element>
						<xsl:for-each select="field">
							<xsl:element name="param">
								<xsl:attribute name="name">Keyword</xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select='@name' /> enumeration member</xsl:attribute>
							</xsl:element>
						</xsl:for-each>
					</object>
				</xsl:if>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
