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
        <title>Virtual host</title>
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
        <xsl:apply-templates select="r:VirtualMachines"/>
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

  
  <xsl:template match="r:VirtualHost">
    <h1>
      <xsl:value-of select="r:HostType"/>-Host <xsl:value-of select="r:Url"/>
    </h1>

    <xsl:apply-templates select="r:VirtualMachines"/>

  </xsl:template>


  <xsl:template match="r:VirtualMachines">

    <table class="wr_table">

      <tr>
        <th>Name of virtual machine</th>
        <th>Hostname guest system</th>
        <th>IP guest system</th>
        <th>Summary</th>
        <th>Details</th>
      </tr>

      <xsl:for-each select="*">
        <xsl:sort select="r:Fullname" data-type="text" order="ascending" />
        <xsl:variable name="css-class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">wr_cell</xsl:when>
            <xsl:otherwise>wr_cell_alt</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <tr>
          <td class="{$css-class}">
            <xsl:value-of select="r:Name"/>
          </td>

          <td class="{$css-class}">
            <xsl:value-of select="r:HostName"/>
          </td>

          <td class="{$css-class}">
            <xsl:value-of select="r:Ip"/>
          </td>

          <td class="{$css-class}">
            <a>
              <xsl:attribute name="href">
                <xsl:value-of select="r:IpAnzeige"/>_summary.htm
              </xsl:attribute>
              <img src="down.png" alt="Go to summary"/>
            </a>
            <xsl:text>  </xsl:text>
          </td>

          <td class="{$css-class}">
            <a>
              <xsl:attribute name="href">
                <xsl:value-of select="r:IpAnzeige"/>.htm
              </xsl:attribute>
              <img src="down.png" alt="Go to details"/>
            </a>
          </td>



        </tr>
      </xsl:for-each>

    </table>


  </xsl:template>


</xsl:stylesheet>
