<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="WebApplicationAzureDeployment" generation="1" functional="0" release="0" Id="2a0d0176-d565-4f99-8632-66ef8e7e1f43" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WebApplicationAzureDeploymentGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebApplication:dataup2httpsin" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/LB:WebApplication:dataup2httpsin" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="WebApplication:FileDeletionAlertCheckpoints" defaultValue="">
          <maps>
            <mapMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/MapWebApplication:FileDeletionAlertCheckpoints" />
          </maps>
        </aCS>
        <aCS name="WebApplication:FileDeletionAlertDisplayThreshold" defaultValue="">
          <maps>
            <mapMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/MapWebApplication:FileDeletionAlertDisplayThreshold" />
          </maps>
        </aCS>
        <aCS name="WebApplication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/MapWebApplication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebApplicationInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/MapWebApplicationInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebApplication:dataup2httpsin">
          <toPorts>
            <inPortMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplication/dataup2httpsin" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapWebApplication:FileDeletionAlertCheckpoints" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplication/FileDeletionAlertCheckpoints" />
          </setting>
        </map>
        <map name="MapWebApplication:FileDeletionAlertDisplayThreshold" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplication/FileDeletionAlertDisplayThreshold" />
          </setting>
        </map>
        <map name="MapWebApplication:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplication/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebApplicationInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplicationInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WebApplication" generation="1" functional="0" release="0" software="C:\DataOnboarding\Deployment\WebApplicationAzureDeployment\csx\Dev\roles\WebApplication" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="dataup2httpsin" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="FileDeletionAlertCheckpoints" defaultValue="" />
              <aCS name="FileDeletionAlertDisplayThreshold" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebApplication&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebApplication&quot;&gt;&lt;e name=&quot;dataup2httpsin&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplicationInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplicationUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplicationFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebApplicationUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="WebApplicationFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WebApplicationInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="f6c1cade-9b89-4f5e-91bd-35441406ac50" ref="Microsoft.RedDog.Contract\ServiceContract\WebApplicationAzureDeploymentContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="2aa71e71-e774-4875-a76b-25ed8ec96141" ref="Microsoft.RedDog.Contract\Interface\WebApplication:dataup2httpsin@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/WebApplicationAzureDeployment/WebApplicationAzureDeploymentGroup/WebApplication:dataup2httpsin" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>