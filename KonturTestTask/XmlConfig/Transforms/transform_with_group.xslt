<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

	<!-- Инструкция для преобразователя (формат xml и отформатированный) -->
	<xsl:output method="xml" indent="yes"/>

	<!-- Объединяем все item из корня и вложенных элементов в один ключ (группировка по методу Мюнхена ) -->
	<xsl:key name="employees-key" match="item | */item" use="concat(@name, '|', @surname)"/>

	<xsl:template match="/Pay">
		<Employees>
			<!-- Группировка по уникальным сотрудникам -->
			<xsl:for-each select="(item | */item)[generate-id() = generate-id(key('employees-key', concat(@name, '|', @surname))[1])]">
				<Employee name="{@name}" surname="{@surname}">
					<!-- Обработка всех salary через ключ -->
					<xsl:apply-templates select="key('employees-key', concat(@name, '|', @surname))"/>
				</Employee>
			</xsl:for-each>
		</Employees>
	</xsl:template>

	<!-- Шаблон для salary -->
	<xsl:template match="item">
		<salary amount="{@amount}">
			<xsl:attribute name="mount">
				<xsl:choose>
					<xsl:when test="parent::Pay">
						<xsl:value-of select="@mount"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="name(..)"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</salary>
	</xsl:template>
</xsl:stylesheet>

