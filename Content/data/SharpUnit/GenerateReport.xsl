<?xml version="1.0" encoding="ISO-8859-1"?>
     
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="TestResults">
		<BODY>
			<H1>Project Report</H1>
			
			<xsl:for-each select="TestAssembly">
				<H2>Executed tests in assembly <xsl:value-of select="@name"/></H2>
				<xsl:for-each select="TestSuite">
						<DIV CLASS="LOCALHEADER">
							Processed test suite <xsl:value-of select="@name"/>
						</DIV>
					<TABLE>
						<xsl:for-each select="TestCase">
							<xsl:choose>
								<xsl:when test="@succeeded='True'">
									<TR><TD>
										Passed test <xsl:value-of select="@name"/> <xsl:if test="@description"> : <xsl:value-of select="@description"/></xsl:if>
									</TD></TR>
								</xsl:when>
								<xsl:otherwise>
									<TR><TD><DIV CLASS="FAILED">
										Failed test <xsl:value-of select="@name"/> <xsl:if test="@description"> : <xsl:value-of select="@description"/></xsl:if>
									</DIV></TD></TR>
									
									<TR><TD><DIV CLASS="FAILED">
										Exception got:
									</DIV></TD></TR>
									
									<TR><TD><DIV CLASS="FAILED">
										<pre><xsl:value-of select="Exception"/></pre>
									</DIV></TD></TR>
								</xsl:otherwise>
							</xsl:choose>
							<TR><TD>
								<DIV CLASS="LIGHT">
									Test case execution time : <xsl:value-of select="number(@milliseconds)"/> milliseconds.
								</DIV>
							</TD></TR>
						</xsl:for-each>
					</TABLE>
					<P>
						<DIV CLASS="LIGHT">
							Test suite execution time : <xsl:value-of select="number(@milliseconds)"/> milliseconds.
						</DIV>					
					</P>
					<P>
						<xsl:choose>
							<xsl:when test="number(@testssucceeded)>0 and number(@testsfailed)=0">
								<DIV CLASS="SUCCEEDEDREPORT">
									All tests in <xsl:value-of select="@name"/> succeeded
								</DIV>
							</xsl:when>
							<xsl:when test="number(@testssucceeded)=0 and number(@testsfailed)>0">
								<DIV CLASS="FAILEDREPORT">
									All tests in <xsl:value-of select="@name"/> failed
								</DIV>
							</xsl:when>
							<xsl:otherwise>
								<DIV CLASS="FAILEDREPORT">
									Some tests in <xsl:value-of select="@name"/> failed
								</DIV>
							</xsl:otherwise>
						</xsl:choose>
					</P>
				</xsl:for-each>
				Summary: <xsl:value-of select="number(@testssucceeded)+number(@testsfailed)"/> test cases executed, <xsl:value-of select="number(@testssucceeded)"/> passed, <xsl:value-of select="number(@testsfailed)"/> failed.
				Total Execution time : <xsl:value-of select="number(@milliseconds)"/> milliseconds.
			</xsl:for-each>
		</BODY>
	</xsl:template>
</xsl:transform>
