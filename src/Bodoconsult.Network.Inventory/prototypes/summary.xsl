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
          Summary <xsl:value-of select="//r:HostName"/> (IP: <xsl:value-of select="//r:Ip"/>)
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
            <xsl:apply-templates select="r:NetworkItem"/>
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

  <xsl:template match="r:NetworkItem">



    <h1>
      Summary <xsl:value-of select="r:HostName"/> (IP: <xsl:value-of select="r:Ip"/>)
    </h1>
    <h2>General information</h2>

    <table class="wr_table">
      <tr>
        <td class="wr_cell">
          IP Address

        </td>
        <td class="wr_cell">
          <xsl:for-each select="r:IpAddresses">
            <xsl:for-each select="*">
              <xsl:sort select="." data-type="text" order="ascending" />
              <xsl:value-of select="."/>

              <br/>
            </xsl:for-each>
          </xsl:for-each>

        </td>

      </tr>
      <tr>
        <td class="wr_cell_alt">
          Host name

        </td>
        <td class="wr_cell_alt">
          <xsl:value-of select="r:HostName"/>

        </td>
      </tr>
      <tr>
        <td class="wr_cell">
          Operating system

        </td>
        <td class="wr_cell">
          <xsl:value-of select="r:OperatingSystem"/>

        </td>
      </tr>

      <!--<tr>
        <td class="wr_cell_alt">
          N.N.

        </td>
        <td class="wr_cell_alt">


        </td>
      </tr>-->

      <tr>
        <td class="wr_cell">
          Domain role

        </td>
        <td class="wr_cell">
          <xsl:value-of select="r:DomainRole"/>

        </td>
      </tr>

      <tr>
        <td class="wr_cell_alt">
          Install date

        </td>
        <td class="wr_cell_alt">
          <xsl:value-of select="r:InstallDate"/>

        </td>
      </tr>

      <!--<tr>
      <td>
        Operating system

      </td>
      <td>
        <xsl:value-of select="r:OperatingSystem"/>

      </td>
    </tr>-->


      <!--<tr>
        <td>
          Host name

        </td>
        <td>
          <xsl:value-of select="r:HostName"/>

        </td>
      </tr>-->
      <tr>
      </tr>



    </table>

    <h2>Roles</h2>

    <xsl:apply-templates select="r:Roles"/>


    <h2>Hardware</h2>

    <table class="wr_table">
      <tr>
        <td class="wr_cell">
          Virtual machine
        </td>
        <td class="wr_cell">
          <xsl:value-of select="r:VirtualMachine"/>
          Host: <xsl:value-of select="r:VmHost"/>
        </td>
      </tr>

      <xsl:if test="r:Ram!='0'">
        <tr>
          <td class="wr_cell_alt">
            Physical RAM
          </td>
          <td class="wr_cell_alt">
            <xsl:value-of select="r:Ram"/> MB
          </td>

        </tr>
      </xsl:if>
      <xsl:if test="r:FreeRam!='0'">
        <tr>
          <td class="wr_cell">
            Free physical RAM
          </td>
          <td class="wr_cell">
            <xsl:value-of select="r:FreeRam"/> MB

          </td>

        </tr>
      </xsl:if>

      <xsl:if test="string-length(r:Drives)>0">
      <tr>
        <td class="wr_cell_alt">
          Hard drives
        </td>
        <td class="wr_cell_alt">
          <xsl:apply-templates select="r:Drives"/>
        </td>

      </tr>
      </xsl:if>

        <xsl:if test="string-length(r:LogicalDrives)>0">
      <tr>
        <td class="wr_cell">
          Logical drives
        </td>
        <td class="wr_cell">
          <xsl:apply-templates select="r:LogicalDrives"/>
        </td>

      </tr>
        </xsl:if>

      <xsl:if test="string-length(r:NetworkAdapters)>0">
      <tr>
        <td class="wr_cell_alt">
          Network adapters
        </td>
        <td class="wr_cell_alt">
          <xsl:apply-templates select="r:NetworkAdapters"/>
        </td>

      </tr>
      </xsl:if>
    </table>

    <h2>Installed software</h2>

    <xsl:apply-templates select="r:Software"/>

    <h2>Shares</h2>
    <xsl:apply-templates select="r:Shares"/>

  </xsl:template>

  <xsl:template match="r:Drives">
    <xsl:for-each select="*">
      <p>
        <xsl:value-of select="r:Name"/> (Id <xsl:value-of select="r:DriveId"/>, Size <xsl:value-of select="r:Size"/> GB, Used space: <xsl:value-of select="r:SizeUsed"/> GB)
      </p>



    </xsl:for-each>

  </xsl:template>

  <xsl:template match="r:Roles">
    <xsl:for-each select="*">
      <p>
        <xsl:value-of select="r:Name"/>
      </p>
    </xsl:for-each>

  </xsl:template>


  <xsl:template match="r:LogicalDrives">
    <xsl:for-each select="*">
      <p>
        <xsl:value-of select="r:Name"/> (<xsl:value-of select="r:FileSystem"/>, Size: <xsl:value-of select="r:Size"/> GB; free: <xsl:value-of select="r:FreeSpace"/> GB)
      </p>
    </xsl:for-each>

  </xsl:template>


  <xsl:template match="r:NetworkAdapters">
    <xsl:for-each select="*">
      <p>
        <xsl:value-of select="r:Name"/> (Id <xsl:value-of select="r:Id"/>,
        Ip: <xsl:value-of select="r:IpAddress"/>,
        DhcpEnabled:  <xsl:value-of select="r:DhcpEnabled"/>,
        Dns: <xsl:value-of select="r:DnsServer"/>,
        DefaultIpGateway: <xsl:value-of select="r:DefaultIpGateway"/>
        Speed: <xsl:value-of select="r:Speed"/> MBit/s,
        MAC-Address: <xsl:value-of select="r:MacAddress"/>)
      </p>
    </xsl:for-each>
  </xsl:template>



  <xsl:template match="r:Software">


    <table class="wr_table">


      <xsl:for-each select="*">
        <xsl:sort select="r:Name" data-type="text" order="ascending" />
        <xsl:sort select="r:Version" data-type="text" order="ascending" />
        <xsl:variable name="css-class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">wr_cell</xsl:when>
            <xsl:otherwise>wr_cell_alt</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <tr>
          <td class="{$css-class}">
            <xsl:value-of select="r:Name"/>
            <xsl:value-of select="r:Version"/>
          </td>
          <td class="{$css-class}">
            <p>
              <xsl:value-of select="r:Vendor"/>
              <br />
              Version: <xsl:value-of select="r:Version"/>
              <br />
              ID: <xsl:value-of select="r:IdentifyingNumber"/>
            </p>

          </td>

        </tr>

      </xsl:for-each>

    </table>

  </xsl:template>



  <xsl:template match="r:Shares">

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
            <xsl:value-of select="r:Name"/>
          </td>
          <td class="{$css-class}">
            <p>
              Path: <xsl:value-of select="r:Path"/>
              <br />
              Descr.: <xsl:value-of select="r:Description"/>
              <br />
              Type: <xsl:value-of select="r:Type"/>
            </p>

          </td>

        </tr>

      </xsl:for-each>

    </table>

  </xsl:template>



</xsl:stylesheet>
