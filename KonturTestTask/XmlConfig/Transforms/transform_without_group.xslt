<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<!-- Инструкция для преобразователя (формат xml и отформатированный) -->
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="/Pay">
		<Employees>
			<!-- Если Data1.xml -> обработка элементов из корня Pay -->
			<xsl:for-each select="item[not(@name = preceding::item/@name and @surname = preceding::item/@surname)]">
				<xsl:call-template name="EmployeeTemplate">
					<xsl:with-param name="name" select="@name"/>
					<xsl:with-param name="surname" select="@surname"/>
				</xsl:call-template>
			</xsl:for-each>

			<!-- Если Data2.xml -> обработка элементов из вложенных месяцев -->
			<xsl:for-each select="*/item[not(@name = preceding::item/@name and @surname = preceding::item/@surname)]">
				<xsl:call-template name="EmployeeTemplate">
					<xsl:with-param name="name" select="@name"/>
					<xsl:with-param name="surname" select="@surname"/>
				</xsl:call-template>
			</xsl:for-each>
		</Employees>
	</xsl:template>

	<!-- Сборка всех salary -->
	<xsl:template name="EmployeeTemplate">
		<xsl:param name="name"/>
		<xsl:param name="surname"/>

		<Employee name="{$name}" surname="{$surname}">
			<!-- Если Data1.xml -->
			<xsl:for-each select="/Pay/item[@name = $name and @surname = $surname]">
				<salary amount="{@amount}" mount="{@mount}"/>
			</xsl:for-each>

			<!-- Если Data2.xml -->
			<xsl:for-each select="/Pay/*/item[@name = $name and @surname = $surname]">
				<!-- берем имя родителя -> нужный месяц -->
				<salary amount="{@amount}" mount="{name(..)}"/>
			</xsl:for-each>
		</Employee>
	</xsl:template>
</xsl:stylesheet>

