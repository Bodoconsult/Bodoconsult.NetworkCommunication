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
        <title>Summary</title>
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
        <xsl:apply-templates select="r:DomainItem"/>
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

  <xsl:template match="r:DomainItem">
    <h1>
      Domain <xsl:value-of select="r:Name"/>
    </h1>
    <p>
      <a href="#Users">User accounts</a>&#160;
      <a href="#Groups">Group accounts</a>&#160;
      <a href="#Computers">Computer accounts</a>
    </p>

    <xsl:apply-templates select="r:Users"/>

    <xsl:apply-templates select="r:Groups"/>

    <xsl:apply-templates select="r:Computers"/>


  </xsl:template>


  <xsl:template match="r:Users">
    <h2>
      <a name="Users">User accounts</a>
    </h2>

    <table class="wr_table">


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
            <a>
              <xsl:attribute name="name">
                <xsl:value-of select="r:Fullname"/>
              </xsl:attribute>
            </a>
            <xsl:value-of select="r:Fullname"/>
          </td>
          <td class="{$css-class}">
            <p>
              Logon: <xsl:value-of select="r:PrincipalName"/>
              <br />
              Surname:  <xsl:value-of select="r:Surname"/>, First name: <xsl:value-of select="r:FirstName"/>
              <xsl:if test="string-length(r:MailAddress)>0">
                <br />eMail: <xsl:value-of select="r:MailAddress"/>
              </xsl:if>
              <br />
              ID: <xsl:value-of select="r:DistinguishedName"/>
              <br />
              Last logon date: <xsl:value-of select="r:LastLogon"/>


              <br />
              Disabled: <xsl:value-of select="r:Disabled"/>

              <br />
              Pwd not required: <xsl:value-of select="r:PasswordNotRequired"/>

              <br />
              User can't change pwd: <xsl:value-of select="r:PasswordCantChange"/>

              <br />
              Don't expire pwd: <xsl:value-of select="r:DontExpirePassword"/>

              <xsl:if test="string-length(r:ProfilePath)>0">
                <br />Profile path: <xsl:value-of select="r:ProfilePath"/>
              </xsl:if>

              <xsl:if test="string-length(r:ScriptPath)>0">
                <br />Script path: <xsl:value-of select="r:ScriptPath"/>
              </xsl:if>
            </p>

            <xsl:for-each select="r:Groups">
              <xsl:choose>
                <xsl:when test="count(child::*)>0">
                  <p>Group membership:</p>
                  <p class="Einzug">
                    <xsl:for-each select="*">
                      <xsl:sort select="." data-type="text" order="ascending" />

                      <a>
                        <xsl:attribute name="href">
                          <xsl:text>#</xsl:text>
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                        <xsl:value-of select="."/>
                      </a>
                      <br/>
                    </xsl:for-each>
                  </p>
                </xsl:when>
                <xsl:otherwise>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>

          </td>

        </tr>

      </xsl:for-each>

    </table>

  </xsl:template>



  <xsl:template match="r:Groups">
    <h2>
      <a name="Groups">Group accounts</a>
    </h2>

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
              <xsl:attribute name="name">
                <xsl:value-of select="r:Name"/>
              </xsl:attribute>
            </a>
            <xsl:value-of select="r:Name"/>
          </td>
          <td class="{$css-class}">
            <p>
              ID: <xsl:value-of select="r:DistinguishedName"/>
            </p>

            <xsl:call-template name="GroupUsers" />


            <xsl:for-each select="r:GroupMembers">
              <xsl:sort select="." data-type="text" order="ascending" />
              <xsl:choose>
                <xsl:when test="count(child::*)>0">
                  <p>Groups as members:</p>
                  <p class="Einzug">

                    <xsl:for-each select="*">
                      <a>
                        <xsl:attribute name="href">
                          <xsl:text>#</xsl:text>
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                        <xsl:value-of select="."/>
                      </a>
                      <br/>
                    </xsl:for-each>
                  </p>
                </xsl:when>
                <xsl:otherwise>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>


            <xsl:for-each select="r:MemberOfGroups">
              <xsl:sort select="." data-type="text" order="ascending" />

              <xsl:choose>
                <xsl:when test="count(child::*)>0">
                  <p>Member of groups:</p>
                  <p class="Einzug">

                    <xsl:for-each select="*">
                      <a>
                        <xsl:attribute name="href">
                          <xsl:text>#</xsl:text>
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                        <xsl:value-of select="."/>
                      </a>
                      <br/>
                    </xsl:for-each>
                  </p>
                </xsl:when>
                <xsl:otherwise>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>



          </td>


        </tr>

      </xsl:for-each>

    </table>

  </xsl:template>


  <xsl:template  name="GroupUsers" match="r:DomainItem/r:Groups/r:GroupItem/r:Users">
    <xsl:for-each select="*">

      <xsl:choose>
        <xsl:when test="count(child::*)>0">
          <p>Users as member:</p>
          <p class="Einzug">


            <xsl:for-each select="*">
              <xsl:sort select="r:Fullname" data-type="text" order="ascending" />
              <a>
                <xsl:attribute name="href">
                  <xsl:text>#</xsl:text>
                  <xsl:value-of select="r:Fullname"/>
                </xsl:attribute>
                <xsl:value-of select="r:Fullname"/>
              </a>
              <br/>

            </xsl:for-each>
          </p>
        </xsl:when>
        <xsl:otherwise>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>



  </xsl:template>





  <xsl:template match="r:Computers">

    <h2>
      <a name="Computers">Computer accounts</a>
    </h2>

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
              Logon: <xsl:value-of select="r:AdsPath"/>
              <br />
              Operating system: <xsl:value-of select="r:OperatingSystem"/>
              <br />
              Service pack: <xsl:value-of select="r:ServicePack"/>
              <br />
              Last logon date: <xsl:value-of select="r:LastLogon"/>
            </p>
          </td>

        </tr>

      </xsl:for-each>

    </table>

  </xsl:template>

</xsl:stylesheet>
