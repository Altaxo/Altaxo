<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="get-filename-for-current-namespace-hierarchy">
	<xsl:value-of select="concat(translate(@name, '.', ''), 'Hierarchy.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-namespace">
	<xsl:param name="name"/>
	<xsl:value-of select="concat(translate($name, '.', ''), '.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-type">
	<xsl:param name="id"/>
	<xsl:value-of select="concat(translate(substring-after($id, 'T:'), '.', ''), '.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-constructor-overloads">
	<xsl:variable name="type-part" select="translate(substring-after(../@id, 'T:'), '.', '')"/>
	<xsl:value-of select="concat($type-part, 'ConstructorOverloads.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-constructor">
	<!-- .#ctor or .#cctor -->
	<xsl:value-of select="concat(translate(substring-after(substring-before(@id, '.#c'), 'M:'), '.', ''), 'Constructor', @overload, '.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-type-members">
	<xsl:param name="id"/>
	<xsl:value-of select="concat(translate(substring-after($id, 'T:'), '.', ''), 'Members.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-field">
	<xsl:value-of select="concat(translate(substring-after(@id, 'F:'), '.', ''), 'Field.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-event">
	<xsl:value-of select="concat(translate(substring-after(@id, 'E:'), '.', ''), 'Event.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-property-overloads">
	<xsl:variable name="type-part" select="translate(substring-after(../@id, 'T:'), '.', '')"/>
	<xsl:value-of select="concat($type-part, @name, 'Overloads.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-current-property">
	<xsl:choose>
		<xsl:when test="contains(@id, '(')">
			<xsl:value-of select="concat(translate(substring-after(substring-before(@id, '('), 'P:'), '.', ''), 'Property', @overload, '.html')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="concat(translate(substring-after(@id, 'P:'), '.', ''), 'Property', @overload, '.html')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="get-filename-for-current-method-overloads">
	<xsl:variable name="type-part" select="translate(substring-after(../@id, 'T:'), '.', '')"/>
	<xsl:value-of select="concat($type-part, @name, 'Overloads.html')"/>
</xsl:template>

<xsl:template name="get-filename-for-method">
	<xsl:param name="method" select="."/>
	<xsl:choose>
		<xsl:when test="contains($method/@id, '(')">
			<xsl:value-of select="concat(translate(substring-after(substring-before($method/@id, '('), 'M:'), '.', ''), 'Method', $method/@overload, '.html')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="concat(translate(substring-after($method/@id, 'M:'), '.', ''), 'Method', $method/@overload, '.html')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="get-filename-for-system-method">
	<!-- EXAMPLE:  ms-help://MSDNVS/cpref/html_hh2/frlrfSystemObjectClassEqualsTopic.htm -->
	<xsl:value-of select="concat('ms-help://MSDNVS/cpref/html_hh2/frlrf', translate(@declaringType, '.', ''), 'Class', @name, 'Topic.htm')"/>
</xsl:template>

<xsl:template name="get-filename-for-system-class">
	<xsl:param name="class-name"/>
	<xsl:value-of select="concat('ms-help://MSDNVS/cpref/html_hh2/frlrf', translate($class-name, '.', ''), 'ClassTopic.htm')"/>
</xsl:template>

<xsl:template name="get-filename-for-individual-member">
	<xsl:param name="member"/>
	<xsl:choose>
		<xsl:when test="$member = 'field'">
			<xsl:call-template name="get-filename-for-current-field"/>
		</xsl:when>
		<xsl:when test="$member = 'property'">
			<xsl:call-template name="get-filename-for-current-property"/>
		</xsl:when>
		<xsl:when test="$member = 'event'">
			<xsl:call-template name="get-filename-for-current-event"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="get-filename-for-method"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="get-filename-for-individual-member-overloads">
	<xsl:param name="member"/>
	<xsl:choose>
		<xsl:when test="$member = 'property'">
			<xsl:call-template name="get-filename-for-current-property-overloads"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="get-filename-for-current-method-overloads"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
