<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="syntax.xslt"/>
<xsl:include href="filenames.xslt"/>

<xsl:template name="csharp-type">
	<xsl:param name="runtime-type"/>
	
	<xsl:variable name="old-type">
		<xsl:choose>
			<xsl:when test="contains($runtime-type, '[]')">
				<xsl:value-of select="substring-before($runtime-type, '[]')"/>
			</xsl:when>
			<xsl:when test="contains($runtime-type, '&amp;')">
				<xsl:value-of select="substring-before($runtime-type, '&amp;')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$runtime-type"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="new-type">
		<xsl:choose>
			<xsl:when test="$old-type='System.Byte'">byte</xsl:when>
			<xsl:when test="$old-type='Byte'">byte</xsl:when>
			<xsl:when test="$old-type='System.SByte'">sbyte</xsl:when>
			<xsl:when test="$old-type='SByte'">sbyte</xsl:when>
			<xsl:when test="$old-type='System.Int16'">short</xsl:when>
			<xsl:when test="$old-type='Int16'">short</xsl:when>
			<xsl:when test="$old-type='System.UInt16'">ushort</xsl:when>
			<xsl:when test="$old-type='UInt16'">ushort</xsl:when>
			<xsl:when test="$old-type='System.Int32'">int</xsl:when>
			<xsl:when test="$old-type='Int32'">int</xsl:when>
			<xsl:when test="$old-type='System.UInt32'">uint</xsl:when>
			<xsl:when test="$old-type='UInt32'">uint</xsl:when>
			<xsl:when test="$old-type='System.Int64'">long</xsl:when>
			<xsl:when test="$old-type='Int64'">long</xsl:when>
			<xsl:when test="$old-type='System.UInt64'">ulong</xsl:when>
			<xsl:when test="$old-type='UInt64'">ulong</xsl:when>
			<xsl:when test="$old-type='System.Single'">float</xsl:when>
			<xsl:when test="$old-type='Single'">float</xsl:when>
			<xsl:when test="$old-type='System.Double'">double</xsl:when>
			<xsl:when test="$old-type='Double'">double</xsl:when>
			<xsl:when test="$old-type='System.Decimal'">decimal</xsl:when>
			<xsl:when test="$old-type='Decimal'">decimal</xsl:when>
			<xsl:when test="$old-type='System.String'">string</xsl:when>
			<xsl:when test="$old-type='String'">string</xsl:when>
			<xsl:when test="$old-type='System.Char'">char</xsl:when>
			<xsl:when test="$old-type='Char'">char</xsl:when>
			<xsl:when test="$old-type='System.Boolean'">bool</xsl:when>
			<xsl:when test="$old-type='Boolean'">bool</xsl:when>
			<xsl:when test="$old-type='System.Void'">void</xsl:when>
			<xsl:when test="$old-type='Void'">void</xsl:when>
			<xsl:when test="$old-type='System.Object'">object</xsl:when>
			<xsl:when test="$old-type='Object'">object</xsl:when>
			<xsl:otherwise><xsl:value-of select="$old-type"/></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:choose>
		<xsl:when test="contains($runtime-type, '[]')">
			<xsl:value-of select="concat($new-type, '[]')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$new-type"/>
		</xsl:otherwise>
	</xsl:choose>
	
</xsl:template>

<xsl:template name="type-access">
	<xsl:param name="access"/>
	<xsl:choose>
		<xsl:when test="$access='Public'">public</xsl:when>
		<xsl:when test="$access='NotPublic'"></xsl:when>
		<xsl:when test="$access='NestedPublic'">public</xsl:when>
		<xsl:when test="$access='NestedFamily'">protected</xsl:when>
		<xsl:when test="$access='NestedFamilyOrAssembly'">protected internal</xsl:when>
		<xsl:when test="$access='NestedAssembly'">internal</xsl:when>
		<xsl:when test="$access='NestedPrivate'">private</xsl:when>		
		<xsl:otherwise>/* unknown */</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="method-access">
	<xsl:param name="access"/>
	<xsl:choose>
		<xsl:when test="$access='Public'">public</xsl:when>
		<xsl:when test="$access='Family'">protected</xsl:when>
		<xsl:when test="$access='FamilyOrAssembly'">protected internal</xsl:when>
		<xsl:when test="$access='Assembly'">internal</xsl:when>
		<xsl:when test="$access='Private'">private</xsl:when>
		<xsl:otherwise>/* unknown */</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="contract">
	<xsl:param name="contract"/>
	<xsl:choose>
		<xsl:when test="$contract='Static'">static</xsl:when>
		<xsl:when test="$contract='Abstract'">abstract</xsl:when>
		<xsl:when test="$contract='Final'">final</xsl:when>
		<xsl:when test="$contract='Virtual'">virtual</xsl:when>
		<xsl:when test="$contract='Override'">override</xsl:when>
		<xsl:when test="$contract='Normal'"></xsl:when>
		<xsl:otherwise>/* unknown */</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="parameter-topic">
	<xsl:for-each select="parameter">
		<xsl:variable name="name" select="@name"/>
		<p class="i1"><i><xsl:value-of select="@name"/></i></p>
		<p class="i2"><xsl:apply-templates select="parent::node()/param[@name=$name]/node()" mode="slashdoc"/></p>
	</xsl:for-each>
</xsl:template>

<xsl:template name="type-mixed">
	<xsl:choose>
		<xsl:when test="local-name()='constructor' or local-name()='property' or local-name()='method' or local-name()='event'">
			<xsl:choose>
				<xsl:when test="local-name(..)='interface'">Interface</xsl:when>
				<xsl:otherwise>Class</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:choose>
				<xsl:when test="local-name()='interface'">Interface</xsl:when>
				<xsl:otherwise>Class</xsl:otherwise>
			</xsl:choose>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="type-element">
	<xsl:choose>
		<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event'"><xsl:value-of select="local-name(..)"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="type-name">
	<xsl:choose>
		<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event'"><xsl:value-of select="../@name"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="@name"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="type-id">
	<xsl:choose>
		<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event'"><xsl:value-of select="../@id"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="@id"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="namespace-name">
	<xsl:choose>
		<xsl:when test="local-name()='constructor' or local-name()='field' or local-name()='property' or local-name()='method' or local-name()='event'"><xsl:value-of select="../../@name"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="../@name"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="seealso-section">
	<xsl:param name="page"/>

	<xsl:variable name="typeMixed"><xsl:call-template name="type-mixed"/></xsl:variable>
	<xsl:variable name="typeElement"><xsl:call-template name="type-element"/></xsl:variable>
	<xsl:variable name="typeName"><xsl:call-template name="type-name"/></xsl:variable>
	<xsl:variable name="typeID"><xsl:call-template name="type-id"/></xsl:variable>
	<xsl:variable name="namespaceName"><xsl:call-template name="namespace-name"/></xsl:variable>

	<h3>See Also</h3>
	<p class="i1">
		<xsl:if test="$page!='type' and $page!='enumeration' and $page!='delegate'">
			<xsl:variable name="type-filename">
				<xsl:call-template name="get-filename-for-type">
					<xsl:with-param name="id" select="$typeID"/>
				</xsl:call-template>
			</xsl:variable>
			<a href="{$type-filename}"><xsl:value-of select="concat($typeName, ' ', $typeMixed)"/></a>
			<xsl:text> | </xsl:text>
		</xsl:if>
		<xsl:if test="$page!='member' and $page!='members' and $page!='enumeration' and $page!='delegate' and $page!='methods' and $page!='properties' and $page!='fields' and $page!='events'">
			<a>
				<xsl:attribute name="href">
					<xsl:call-template name="get-filename-for-type-members">
						<xsl:with-param name="id" select="$typeID"/>
					</xsl:call-template>
				</xsl:attribute>
				<xsl:value-of select="$typeName"/> 
				<xsl:text> Members</xsl:text>
			</a>
			<xsl:text> | </xsl:text>
		</xsl:if>
		<a>
			<xsl:attribute name="href">
				<xsl:call-template name="get-filename-for-namespace">
					<xsl:with-param name="name" select="$namespaceName"/>
				</xsl:call-template>
			</xsl:attribute>
			<xsl:value-of select="$namespaceName"/>
			<xsl:text> Namespace</xsl:text>
		</a>
		<xsl:if test="$page='member' or $page='property'">
			<xsl:variable name="memberName" select="@name"/>
			<xsl:if test="count(parent::node()/*[@name=$memberName]) &gt; 1">
				<xsl:text> | </xsl:text>
				<xsl:choose>
					<xsl:when test="local-name()!='constructor'">
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-method-overloads"/>
							</xsl:attribute>
							<xsl:value-of select="concat($typeName, '.', @name)"/>
							<xsl:text> Overload List</xsl:text>
						</a>
					</xsl:when>
					<xsl:otherwise>
						<a>
							<xsl:attribute name="href">
								<xsl:call-template name="get-filename-for-current-constructor-overloads"/>
							</xsl:attribute>
							<xsl:value-of select="$typeName"/> 
							<xsl:text> Constructor Overload List</xsl:text>
						</a>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:if>
		<xsl:if test="seealso">
			<xsl:for-each select="seealso">
				<xsl:variable name="cref" select="@cref"/>
				<xsl:text> | </xsl:text>
				<xsl:choose>
					<xsl:when test="//*[@id=$cref]">
						<a href="{concat(local-name(//*[@id=$cref]), translate($cref, ':#', '!$'))}.html">
							<xsl:value-of select="//*[@id=$cref]/@name"/>
						</a>
					</xsl:when>
					<xsl:otherwise><xsl:value-of select="substring(@cref, 3)"/></xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</xsl:if>
	</p>
</xsl:template>

<xsl:template match="see" mode="slashdoc">
	<xsl:variable name="cref" select="@cref"/>
	<xsl:choose>
		<xsl:when test="@langword"><b><xsl:value-of select="@langword"/></b></xsl:when>
		<xsl:when test="//*[@id=$cref]"><a href="{concat(local-name(//*[@id=$cref]), translate($cref, ':#', '!$'))}.html"><xsl:value-of select="//*[@id=$cref]/@name"/></a></xsl:when>
		<xsl:otherwise><xsl:value-of select="substring(@cref, 3)"/></xsl:otherwise>
	</xsl:choose>	
</xsl:template>

<xsl:template name="summary-section">
	<p class="i1"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></p>
</xsl:template>

<xsl:template name="parameter-section">
	<xsl:if test="parameter">
		<h3>Parameters</h3>
		<xsl:call-template name="parameter-topic"/>
	</xsl:if>
</xsl:template>


<xsl:template name="returnvalue-section">
	<xsl:if test="returns">
		<h3>Return Value</h3>
		<p class="i1"><xsl:apply-templates select="returns/node()" mode="slashdoc"/></p>
	</xsl:if>
</xsl:template>

<xsl:template name="remarks-section">
	<xsl:if test="remarks">
		<h3>Remarks</h3>
		<p class="i1"><xsl:apply-templates select="remarks/node()" mode="slashdoc"/></p>
	</xsl:if>
</xsl:template>

<xsl:template name="value-section">
	<xsl:if test="value">
		<h3>Property Value</h3>
		<p class="i1"><xsl:apply-templates select="value/node()" mode="slashdoc"/></p>
	</xsl:if>
</xsl:template>

<xsl:template name="exceptions-section">
	<xsl:if test="exception">
		<h3>Exceptions</h3>
		<table cellspacing="0">
			<tr valign="top"><th width="50%">Exception Type</th><th width="50%">Condition</th></tr>
			<xsl:for-each select="exception">
				<xsl:sort select="@name"/>
				<xsl:variable name="cref" select="@cref"/>
				<tr valign="top">
					<td width="50%">
						<xsl:variable name="type-filename">
							<xsl:call-template name="get-filename-for-type">
								<xsl:with-param name="id" select="$cref"/>
							</xsl:call-template>
						</xsl:variable>
						<a href="{$type-filename}"><xsl:value-of select="//class[@id=$cref]/@name"/></a>
					</td>
					<td width="50%"><xsl:apply-templates select="./node()" mode="slashdoc"/></td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:if>
</xsl:template>

<xsl:template name="example-section">
	<xsl:if test="example">
		<h3>Example</h3>
		<p class="i1"><xsl:apply-templates select="example/node()" mode="slashdoc"/></p>
	</xsl:if>
</xsl:template>

<xsl:template name="members-section">
	<xsl:if test="field">
		<h3>Members</h3>
		<table cellspacing="0">
			<tr valign="top"><th width="50%">Member Name</th><th width="50%">Description</th></tr>
			<xsl:for-each select="field">
				<tr valign="top">
					<td width="50%"><b><xsl:value-of select="@name"/></b></td>
					<td width="50%"><xsl:apply-templates select="summary/node()" mode="slashdoc"/></td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:if>
</xsl:template>

<xsl:template match="node()|@*" mode="slashdoc">
	<xsl:copy>
		<xsl:apply-templates select="node()|@*" mode="slashdoc"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="code" mode="slashdoc">
	<pre class="code">
		<xsl:apply-templates/>
	</pre>
</xsl:template>

<xsl:template match="note" mode="slashdoc">
	<xsl:choose>
		<xsl:when test="@type='note'"><B>Note: </B><xsl:apply-templates select="./node()" mode="slashdoc"/></xsl:when>
		<xsl:when test="@type='inheritinfo'"><B>Notes to Inheritors: </B><xsl:apply-templates select="./node()" mode="slashdoc"/></xsl:when>
		<xsl:when test="@type='inotes'"><B>Notes to Implementers: </B><xsl:apply-templates select="./node()" mode="slashdoc"/></xsl:when>		
		<xsl:otherwise><xsl:apply-templates select="./node()"/></xsl:otherwise>		
	</xsl:choose>	
</xsl:template>

<xsl:template match="list" mode="slashdoc">
	<xsl:choose>
		<xsl:when test="@type='bullet'">
			<ul type="disc">
				<xsl:apply-templates select="item" mode="slashdoc"/>
			</ul>
		</xsl:when>
		<xsl:when test="@type='number'">
			<ol>
				<xsl:apply-templates select="item" mode="slashdoc"/>
			</ol>
		</xsl:when>
		<xsl:otherwise><!-- table not implemented yet. -->
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="item" mode="slashdoc">
	<li>
		<xsl:apply-templates select="./node()" mode="slashdoc"/>
	</li>
</xsl:template>

<xsl:template match="term" mode="slashdoc">
	<b><xsl:apply-templates select="./node()" mode="slashdoc"/> - </b>
</xsl:template>

<xsl:template match="description" mode="slashdoc">
	<xsl:apply-templates select="./node()" mode="slashdoc"/>
</xsl:template>

<xsl:template match="para" mode="slashdoc">
	<p class="i1">
		<xsl:apply-templates select="./node()" mode="slashdoc"/>
	</p>
</xsl:template>

<xsl:template name="value">
	<xsl:param name="type"/>
	<xsl:variable name="namespace"><xsl:value-of select="concat(../../@name, '.')"/></xsl:variable>
	<xsl:choose>
		<xsl:when test="contains($type, $namespace)"><xsl:value-of select="substring-after($type, $namespace)"/></xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="csharp-type">
				<xsl:with-param name="runtime-type" select="$type"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
