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
        <title>All shares</title>
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
        <xsl:apply-templates select="r:AllShares"/>
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

  <xsl:template match="r:AllShares">
    <p>
      <a href="index.htm">Homepage</a>
    </p>
    <h1>
      All shares on windows computers in the network
    </h1>

    <xsl:apply-templates select="r:Shares"/>

  </xsl:template>


  <xsl:template match="r:Shares">
    <xsl:choose>
      <xsl:when test="count(child::*)>0">


        <table class="wr_table">
          <xsl:for-each select="*">
            <xsl:sort select="r:Name" data-type="text" order="ascending" />
            <xsl:variable name="css-class">
              <xsl:choose>
                <xsl:when test="position() mod 2 = 0">wr_cell</xsl:when>
                <xsl:otherwise>wr_cell_alt</xsl:otherwise>
              </xsl:choose>
            </xsl:variable>

            <tr>
              <td class="{$css-class}">
                <a>
                  <xsl:attribute name="href">
                    <xsl:value-of select="r:Name"/>
                  </xsl:attribute>
                  <xsl:value-of select="r:Name"/>
                </a>
              </td>
              <td class="{$css-class}">
                <p>
                  <xsl:value-of select="r:Description"/>
                </p>
                <xsl:apply-templates select="r:AccessControlList"/>
              </td>

            </tr>
          </xsl:for-each>

        </table>

      </xsl:when>
      <xsl:otherwise>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <xsl:template match="r:AccessControlList">
    <xsl:choose>
      <xsl:when test="count(child::*)>0">
        <p>
          ACL:
        </p>

        <table class="wr2_table">

          <xsl:for-each select="*">
            <xsl:sort select="r:Name" data-type="text" order="ascending" />
            <xsl:variable name="css-class">
              <xsl:choose>
                <xsl:when test="position() mod 2 = 0">wr2_cell</xsl:when>
                <xsl:otherwise>wr2_cell_alt</xsl:otherwise>
              </xsl:choose>
            </xsl:variable>

            <tr>
              <td class="{$css-class}">
                <xsl:value-of select="r:Identifier"/>
              </td>
              <td class="{$css-class}">
                <p>
                  <xsl:value-of select="r:Rights"/>
                </p>
              </td>
              <!--<td class="{$css-class}">
            <p>
            </p>
          </td>-->
            </tr>
          </xsl:for-each>

        </table>
      </xsl:when>
      <xsl:otherwise>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>



</xsl:stylesheet>
