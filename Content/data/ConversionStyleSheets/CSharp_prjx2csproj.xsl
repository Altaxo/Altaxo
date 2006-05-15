<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:Conversion="urn:Conversion">
	<xsl:output method = "xml" indent = "yes" />
	
	<xsl:template match = "/" >
		<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
		<!-- <xsl:element name = "Project" namespace="http://schemas.microsoft.com/developer/msbuild/2003" >-->
			<!-- <xsl:attribute name = "DefaultTargets">Build</xsl:attribute> -->
			
			<!-- Global project options -->
			<xsl:element name = "PropertyGroup" >
				<xsl:element name = "Configuration" >
					<xsl:attribute name = "Condition"> '$(Configuration)' == '' </xsl:attribute>
					<xsl:value-of select = "/Project/Configurations/@active" />
				</xsl:element>
				<xsl:element name = "Platform" ><xsl:attribute name = "Condition"> '$(Platform)' == '' </xsl:attribute>AnyCPU</xsl:element>
				<!--<xsl:element name = "ProductVersion">8.0.40607</xsl:element>-->
				<xsl:element name = "SchemaVersion">2.0</xsl:element>
				<xsl:element name = "ProjectGuid"><xsl:value-of select = "Conversion:GetGuid(/Project/@name)" /></xsl:element>
				<xsl:element name = "RootNamespace"><xsl:value-of select = "Conversion:SetRootNamespace(/Project/@standardNamespace)" /></xsl:element>
				
				<!-- Copy global object from 'Debug' node -->
				<xsl:for-each select="/Project/Configurations/Configuration[@name='Debug']">
					<xsl:element name = "AssemblyName"><xsl:value-of select = "Output/@assembly" /></xsl:element>
					<xsl:element name = "OutputType"><xsl:value-of select = "CodeGeneration/@target" /></xsl:element>
					
					<xsl:element name = "ApplicationIcon"><xsl:value-of select = "CodeGeneration/@win32Icon" /></xsl:element>
					
					<xsl:element name = "WarningLevel"><xsl:value-of select = "CodeGeneration/@warninglevel" /></xsl:element>
					<xsl:element name = "NoWarn"><xsl:value-of select = "CodeGeneration/@nowarn" /></xsl:element>
					<xsl:element name = "StartupObject"><xsl:value-of select = "CodeGeneration/@mainclass" /></xsl:element>
					<xsl:element name = "NoStdLib"><xsl:value-of select = "CodeGeneration/@nostdlib" /></xsl:element>
					<xsl:element name = "NoConfig"><xsl:value-of select = "CodeGeneration/@noconfig" /></xsl:element>
					<xsl:element name = "RunPostBuildEvent">OnSuccessfulBuild</xsl:element>
					<xsl:element name = "PreBuildEvent"><xsl:value-of select = "Conversion:ConvertBuildEvent(Output/@executeBeforeBuild, Output/@executeBeforeBuildArguments)" /></xsl:element>
					<xsl:element name = "PostBuildEvent"><xsl:value-of select = "Conversion:ConvertBuildEvent(Output/@executeAfterBuild, Output/@executeAfterBuildArguments)" /></xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<!-- Configurations -->
			<xsl:for-each select="/Project/Configurations/Configuration">
				<xsl:element name = "PropertyGroup" >
					<xsl:attribute name = "Condition"> '$(Configuration)|$(Platform)' == '<xsl:value-of select = "@name" />|AnyCPU' </xsl:attribute>
					
					<xsl:element name = "DebugSymbols"><xsl:value-of select = "CodeGeneration/@includedebuginformation" /></xsl:element>
					<xsl:element name = "Optimize"><xsl:value-of select = "CodeGeneration/@optimize" /></xsl:element>
					<xsl:element name = "AllowUnsafeBlocks"><xsl:value-of select = "CodeGeneration/@unsafecodeallowed" /></xsl:element>
					<xsl:element name = "CheckForOverflowUnderflow"><xsl:value-of select = "CodeGeneration/@generateoverflowchecks" /></xsl:element>
					<xsl:element name = "DefineConstants"><xsl:value-of select = "CodeGeneration/@definesymbols" /></xsl:element>
					<xsl:element name = "OutputPath"><xsl:value-of select = "Conversion:CanocializePath(Output/@directory)" /></xsl:element>
					<xsl:element name = "TreatWarningsAsErrors"><xsl:value-of select = "Conversion:Negate(@runwithwarnings)" /></xsl:element>
				</xsl:element>
			</xsl:for-each>
			
			<xsl:element name = "ItemGroup">
				<xsl:element name = "Reference" ><xsl:attribute name = "Include">System</xsl:attribute></xsl:element>
				<xsl:element name = "Reference" ><xsl:attribute name = "Include">System.Data</xsl:attribute></xsl:element>
				<xsl:element name = "Reference" ><xsl:attribute name = "Include">System.Drawing</xsl:attribute></xsl:element>
				<xsl:element name = "Reference" ><xsl:attribute name = "Include">System.Windows.Forms</xsl:attribute></xsl:element>
				<xsl:element name = "Reference" ><xsl:attribute name = "Include">System.Xml</xsl:attribute></xsl:element>
				
				<xsl:for-each select="/Project/References/Reference[@type='Gac']">
					<xsl:element name = "Reference" >
						<xsl:attribute name = "Include"><xsl:value-of select = "@refto" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
				
				<xsl:for-each select="/Project/References/Reference[@type='Assembly']">
					<xsl:element name = "Reference" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:GetFileNameWithoutExtension(@refto)" /></xsl:attribute>
						<xsl:element name = "HintPath" ><xsl:value-of select = "Conversion:CanocializeFileName(@refto)" /></xsl:element>
						<xsl:element name = "Private" ><xsl:value-of select = "@localcopy" /></xsl:element>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="/Project/Contents/File[@buildaction='Compile' and @subtype='Code']">
					<xsl:element name = "Compile" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:CanocializeFileName(@name)" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
				<xsl:for-each select="/Project/Contents/File[@buildaction='EmbedAsResource']">
					<xsl:element name = "EmbeddedResource" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:ConvertResource(@name)" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
				
				<xsl:for-each select="/Project/Contents/File[@buildaction='Nothing' and @subtype='Code']">
					<xsl:element name = "None" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:CanocializeFileName(@name)" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="/Project/Contents/File[@subtype='Directory']">
					<xsl:element name = "Folder" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:CanocializePath(@name)" /></xsl:attribute>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "ItemGroup">
				<xsl:for-each select="/Project/References/Reference[@type='Project']">
					<xsl:element name = "ProjectReference" >
						<xsl:attribute name = "Include"><xsl:value-of select = "Conversion:GetRelativeProjectPath(@refto)" /></xsl:attribute>
						
						<xsl:element name = "Project"><xsl:value-of select = "Conversion:GetGuid(@refto)" /></xsl:element>
						<xsl:element name = "Name"><xsl:value-of select = "@refto" /></xsl:element>
					</xsl:element>
				</xsl:for-each>
			</xsl:element>
			
			<xsl:element name = "Import" >
				<xsl:attribute name = "Project">$(MSBuildBinPath)\Microsoft.<xsl:value-of select = "Conversion:GetLanguageName()" />.Targets</xsl:attribute>
			</xsl:element>
		
		
		</Project>
<!--		</xsl:element>-->
	</xsl:template>
</xsl:stylesheet>
