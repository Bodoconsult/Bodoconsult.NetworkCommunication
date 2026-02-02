<?xml version='1.0'?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:d2p1="http://schemas.microsoft.com/2003/10/Serialization/Arrays"
                xmlns:r="http://bodoconsult/inventory"
                exclude-result-prefixes ="r d2p1">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:decimal-format name="de" decimal-separator="," grouping-separator="."/>


  <xsl:variable name="BreiteSpalte1" select="100"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>
          Details for <xsl:value-of select="/NetworkItemRoot/NetworkItemSection[@name='Metadata']/NetworkItemSubSection/NetworkItemItem/NetworkItemValue[@name='Hostname']"/>
          (IP: <xsl:value-of select="/NetworkItemRoot/NetworkItemSection[@name='Metadata']/NetworkItemSubSection/NetworkItemItem/NetworkItemValue[@name='IpAddress']"/>)
        </title>
        <link href="layout.css" rel="stylesheet" type="text/css" />
      </head>
      <body>
        <div id="LeftColumn">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
        </div>
        <div id="MiddleColumn">
          <div class="page">
            <p>
              <a href="index.htm" class="TopLink">
                <img src="logo.jpg" alt="Firmenlogo" class="logo"/>
              </a>
            </p>

            <h1>
              All details for <xsl:value-of select="/NetworkItemRoot/NetworkItemSection[@name='Metadata']/NetworkItemSubSection/NetworkItemItem/NetworkItemValue[@name='Hostname']"/>
              (IP: <xsl:value-of select="/NetworkItemRoot/NetworkItemSection[@name='Metadata']/NetworkItemSubSection/NetworkItemItem/NetworkItemValue[@name='IpAddress']"/>)
            </h1>

            <xsl:apply-templates select="NetworkItemRoot"/>
          </div>
        </div>
        <div id="RightColumn">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
        </div>
        <p>
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
        </p>
      </body>

    </html>

  </xsl:template>





  <xsl:template match="NetworkItemRoot">

    <xsl:apply-templates select="NetworkItemSection"/>

  </xsl:template>


  <xsl:template match="NetworkItemSection">

    <h2>
      <xsl:value-of select="@name"/>
    </h2>


    <xsl:apply-templates select="NetworkItemSubSection"/>

  </xsl:template>

  <xsl:template match="NetworkItemSubSection">
    <h3>
      <xsl:value-of select="@name"/>
    </h3>
    <xsl:apply-templates select="NetworkItemItem"/>

  </xsl:template>

  <xsl:template match="NetworkItemItem">
    <xsl:if test="count(../*)>1">
      <h4>
        <xsl:value-of select="@name"/>
      </h4>
    </xsl:if>
    <table class="wr_table">
      <xsl:apply-templates select="*"/>
    </table>


  </xsl:template>

  <xsl:template match="NetworkItemValue">
    <xsl:variable name="css-class">
      <xsl:choose>
        <xsl:when test="position() mod 2 = 0">wr_cell</xsl:when>
        <xsl:otherwise>wr_cell_alt</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:if test="string-length(.)>0">
      <tr>
        <td class="{$css-class}">
          <xsl:value-of select="@name"/>
        </td>
        <td class="{$css-class}">

          <xsl:value-of select="."/>
        </td>

      </tr>
    </xsl:if>

  </xsl:template>

  <xsl:template match="NetworkItemSubValue">
    <xsl:variable name="css-class">
      <xsl:choose>
        <xsl:when test="position() mod 2 = 0">wr_cell</xsl:when>
        <xsl:otherwise>wr_cell_alt</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <tr>
      <td class="{$css-class}">
        <xsl:value-of select="@name"/>
      </td>
      <td class="{$css-class}">
        <xsl:if test="count(./*)=0">
          <xsl:value-of select="."/>
        </xsl:if>
        <xsl:if test="count(./*)>0">
          <xsl:apply-templates select="NetworkItemSubItem"/>
        </xsl:if>
      </td>

    </tr>

  </xsl:template>


  <xsl:template match="NetworkItemSubItem">

    <p class="subHeader">
      <xsl:value-of select="@name"/>
    </p>

    <xsl:if test="count(./*)=0">
      <xsl:value-of select="."/>
    </xsl:if>
    <xsl:if test="count(./*)>0">
      <xsl:apply-templates select="NetworkItemSubItemValue"/>
    </xsl:if>

  </xsl:template>

  <xsl:template match="NetworkItemSubItemValue">
    <xsl:if test="string-length(.)>0">
      <p class="Einzug">
        <xsl:value-of select="@name"/>: <xsl:value-of select="."/>
      </p>
    </xsl:if>

  </xsl:template>


</xsl:stylesheet>
